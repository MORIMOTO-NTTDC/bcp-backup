package jp.co.nttdata.bcp.app.webapi.controller;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;

import jakarta.inject.Inject;

import org.springframework.beans.BeanUtils;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import jp.co.nttdata.bcp.app.common.code.SyncStatus;
import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.webapi.form.ApiCapaOverForm;
import jp.co.nttdata.bcp.domain.model.Accounts;
import jp.co.nttdata.bcp.domain.model.account.AccountWithVendor;
import jp.co.nttdata.bcp.domain.model.mail.MailInfo;
import jp.co.nttdata.bcp.domain.repository.AccountsMapper;
import jp.co.nttdata.bcp.domain.service.MailSendService;

/**
 * 容量オーバー通知
 */
@RestController
@Transactional
public class ApiCapaOverController {

	@Inject
	private AccountsMapper accountsMapper;

	@Inject
	private MailSendService mailSendService;

	@Value("${aws.ses.email.subject.insufficient}")
	private String subject;

	@Value("${aws.ses.email.insufficient}")
	private String templatePath;

	/**
	 * 容量オーバー通知.<br>
	 * バックアップ容量オーバーをアカウント管理に登録し、当該アカウントのベンダ担当者へメール通知する
	 * 
	 * @param form 入力値
	 */
	@PostMapping("/api01_capa_over.do")
	public void capaOver(@Validated @RequestBody ApiCapaOverForm form) {

		// 値の詰め替え
		Accounts account = new Accounts();
		BeanUtils.copyProperties(form, account);

		// メール通知状態などを取得
		AccountWithVendor accountWithVendor = accountsMapper.selectActiveAccountWithVendorByPrimaryKey(form.getAccountId());

		// 該当レコードが存在しない場合
		if (accountWithVendor == null) {
			throw new BcpBuisinessException(404, "アカウントID不一致");
		}

		// メール通知状態が”通知あり（true）”
		if (accountWithVendor.getMailEnable() == true) {
			return;
		}

		// エラー情報更新
		account.setStatus(SyncStatus.SYNC_IMPOSSIBLE.getCodeValue());
		account.setErrorInfo("バックアップ容量オーバー");
		account.setMailEnable(true);
		int updateCount = accountsMapper.updateActiveByPrimaryKeySelective(account);

		// 更新件数が1件以外
		if (updateCount != 1) {
			throw new BcpBuisinessException(404, "アカウントID不一致");
		}

		//メール本文生成
		//埋め込みパラメータ
		Map<String, Object> params = new HashMap<String, Object>();
		params.put("vendorName", accountWithVendor.getVendorName());
		params.put("accountId", accountWithVendor.getAccountId());
		params.put("accountName", accountWithVendor.getAccountName());
		params.put("backupCapa", accountWithVendor.getBackupCapa());
		params.put("usedSize", form.getUsedSize());
		String body = mailSendService.createBody(templatePath, params);

		//メール送信情報
		MailInfo mailInfo = new MailInfo();
		//To
		mailInfo.setTos(new ArrayList<String>(Arrays.asList(accountWithVendor.getMailAddress())));
		//Subject
		mailInfo.setSubject(subject);
		//本文
		mailInfo.setBody(body);

		//メール送信
		mailSendService.sendEmail(mailInfo);

	}
}
