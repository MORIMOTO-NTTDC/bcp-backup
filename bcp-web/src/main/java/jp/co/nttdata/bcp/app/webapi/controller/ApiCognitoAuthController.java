package jp.co.nttdata.bcp.app.webapi.controller;

import jakarta.inject.Inject;

import org.springframework.beans.BeanUtils;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.webapi.form.ApiCognitoAuthForm;
import jp.co.nttdata.bcp.domain.model.Accounts;
import jp.co.nttdata.bcp.domain.model.Vendors;
import jp.co.nttdata.bcp.domain.model.cognito.TokenInfo;
import jp.co.nttdata.bcp.domain.repository.AccountsMapper;
import jp.co.nttdata.bcp.domain.repository.VendorsMapper;
import jp.co.nttdata.bcp.domain.service.CognitoService;

/**
 * アカウント認証
 */
@RestController
@Transactional
public class ApiCognitoAuthController {

	@Inject
	private CognitoService cognitoService;

	@Inject
	private AccountsMapper accountsMapper;

	@Inject
	private VendorsMapper vendorsMapper;

	/**
	 * アカウント認証.<br>
	 * Cognito認証情報（ID、パスワード）でCognito認証を行いトークンを取得する。
	 * 
	 * @param form 入力値
	 * @return アカウント設定情報
	 */
	@PostMapping("/api01_auth.do")
	public TokenInfo auth(@Validated @RequestBody ApiCognitoAuthForm form) {

		// アカウント情報取得
		Accounts account = accountsMapper.selectActiveByPrimaryKey(form.getAccountId());
		if (account == null || !account.getAuthKey().equals(form.getAuthKey())) {
			throw new BcpBuisinessException(404, "アカウントID不一致");
		}

		// ベンダ情報取得
		Vendors vendor = vendorsMapper.selectParentVendorByVendorId(account.getVendorId());
		if (vendor == null) {
			throw new BcpBuisinessException(404, "アカウントID不一致");
		}

		// Cognitoにログイン
		String idToken;
		try {
			idToken = cognitoService.login(vendor.getCognitoUserId(), vendor.getCognitoAccessToken());
		} catch (Exception e) {
			// エラーの場合
			throw new BcpBuisinessException(480, "Cognito認証エラー", e);
		}

		TokenInfo tokenInfo;
		try {
			// アイデンティティIDを取得
			String identityId = cognitoService.getId(idToken);

			//認証情報を取得
			tokenInfo = cognitoService.getCredentialsForIdentity(identityId, idToken);
		} catch (Exception e) {
			// エラーの場合
			throw new BcpBuisinessException(412, "認証情報等取得エラー", e);
		}

		// バージョン等更新
		Accounts updateAccount = new Accounts();
		BeanUtils.copyProperties(form, updateAccount);
		int updateCount = accountsMapper.updateActiveByPrimaryKeySelective(updateAccount);
		if (updateCount == 0) {
			throw new BcpBuisinessException(404, "アカウントID不一致");
		}

		// アカウント名をレスポンスに設定する
		tokenInfo.setAccountName(account.getAccountName());
		// 同期ステータスをレスポンスに設定する
		tokenInfo.setStatus(account.getStatus());
		return tokenInfo;

	}
}
