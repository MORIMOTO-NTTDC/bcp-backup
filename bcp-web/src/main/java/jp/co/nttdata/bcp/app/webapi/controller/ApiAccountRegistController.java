package jp.co.nttdata.bcp.app.webapi.controller;

import jakarta.inject.Inject;

import org.springframework.beans.BeanUtils;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import jp.co.nttdata.bcp.app.common.code.SyncStatus;
import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.webapi.form.ApiAccountRegistForm;
import jp.co.nttdata.bcp.domain.model.Accounts;
import jp.co.nttdata.bcp.domain.model.Vendors;
import jp.co.nttdata.bcp.domain.repository.AccountsMapper;
import jp.co.nttdata.bcp.domain.repository.VendorsMapper;

/**
 * アカウント登録
 */
@RestController
@Transactional
public class ApiAccountRegistController {

	@Inject
	private AccountsMapper accountsMapper;

	@Inject
	private VendorsMapper vendorsMapper;

	/**
	 * アカウント登録.<br>
	 * 申請Webから連携された申請情報をアカウントとして登録する。
	 * 
	 * @param form 入力値
	 * @return アカウント設定情報
	 */
	@PostMapping("/account_regist.do")
	public void accountRegist(@Validated @RequestBody ApiAccountRegistForm form) {

		// アカウント情報取得
		Accounts result = accountsMapper.selectByPrimaryKeyRecordLock(form.getAccountId());

		if (result == null) {
			// アカウント情報が存在しない場合

			// ベンダ管理取得
			Vendors vendor = vendorsMapper.selectByPrimaryKey(form.getVendorId());
			if (vendor == null) {
				throw new BcpBuisinessException(400, "不正なリクエスト（パラメータ誤り）");
			}

			if (!vendor.getParentId().equals(form.getParentId())) {
				throw new BcpBuisinessException(400, "不正なリクエスト（パラメータ誤り）");
			}

			// 入力値の移し替え
			Accounts account = new Accounts();
			BeanUtils.copyProperties(form, account);
			// その他の値を設定
			account.setParentId(vendor.getParentId());
			account.setUploadEnable(true);
			account.setUploadTiming(60);
			account.setUsedSize(0);
			account.setDownloadEnable(false);
			account.setStatus(SyncStatus.NOT_SYNC.getCodeValue());
			account.setMailEnable(false);
			// 他の項目はデフォルト値のため設定の必要なし

			// アカウント登録
			accountsMapper.insertSelective(account);

		} else {
			// アカウント情報が存在する場合

			// 入力値の移し替え
			Accounts account = new Accounts();
			account.setAccountId(form.getAccountId());
			account.setAccountName(form.getAccountName());
			account.setAccountAddress(form.getAccountAddress());
			account.setBackupCapa(form.getBackupCapa());
			// 更新回数+1
			account.setVersion(result.getVersion() + 1);
			if (form.getBackupCapa() == 0) {
				account.setDisableFlg(true);
			} else {
				account.setDisableFlg(false);
			}

			// アカウント更新
			accountsMapper.updateByPrimaryKeySelective(account);
		}
	}
}
