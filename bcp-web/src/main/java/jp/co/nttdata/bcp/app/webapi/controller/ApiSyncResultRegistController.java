package jp.co.nttdata.bcp.app.webapi.controller;

import jakarta.inject.Inject;

import org.apache.commons.lang3.StringUtils;
import org.springframework.beans.BeanUtils;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import jp.co.nttdata.bcp.app.common.code.SyncStatus;
import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.webapi.form.ApiSyncResultRegistForm;
import jp.co.nttdata.bcp.domain.model.Accounts;
import jp.co.nttdata.bcp.domain.repository.AccountsMapper;

/**
 * 同期結果登録
 */
@RestController
@Transactional
public class ApiSyncResultRegistController {

	@Inject
	private AccountsMapper accountsMapper;

	/**
	 * 同期結果登録.<br>
	 * S3へのアップロード（バックアップ）、S3からのダウンロード（リストア）の同期結果や使用容量を登録する。
	 * 
	 * @param form 入力値
	 */
	@PostMapping("/api01_sync.do")
	public void sync(@Validated @RequestBody ApiSyncResultRegistForm form) {

		// 値の詰め替え
		Accounts account = new Accounts();
		BeanUtils.copyProperties(form, account);

		// 『同期ステータス』が"2"（DL成功）の場合、false
		if (SyncStatus.SUCCESS_DOWNLOAD.getCodeValue().equals(form.getStatus())) {
			account.setDownloadEnable(false);
		}
		
		// エラー情報は更新対象のため、Nullを空文字に変換して更新されるようにする
		account.setErrorInfo(StringUtils.defaultString(account.getErrorInfo()));

		account.setMailEnable(false);

		// アカウント管理更新
		int updateCount = accountsMapper.updateActiveByPrimaryKeySelective(account);

		if (updateCount != 1) {
			throw new BcpBuisinessException(404, "アカウントID不一致");
		}
	}
}
