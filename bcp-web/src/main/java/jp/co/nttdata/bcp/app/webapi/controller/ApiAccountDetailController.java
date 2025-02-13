package jp.co.nttdata.bcp.app.webapi.controller;

import jakarta.inject.Inject;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import com.fasterxml.jackson.annotation.JsonView;

import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.webapi.form.ApiAccountDetailForm;
import jp.co.nttdata.bcp.domain.model.account.AccountWithVendor;
import jp.co.nttdata.bcp.domain.model.account.AccountWithVendor.ApiDetail;
import jp.co.nttdata.bcp.domain.repository.AccountsMapper;

/**
 * アカウント設定情報取得
 */
@RestController
@Transactional
public class ApiAccountDetailController {

	@Inject
	private AccountsMapper accountsMapper;

	@Value("${aws.s3.endpoint-url}")
	private String s3Endpoint;

	/**
	 * アカウント設定情報取得.<br>
	 * アカウント設定情報を取得する。
	 * 
	 * @param form 入力値
	 * @return アカウント設定情報
	 */
	@PostMapping("/api01_detail.do")
	@JsonView(ApiDetail.class)
	public AccountWithVendor accountDetail(@Validated @RequestBody ApiAccountDetailForm form) {

		AccountWithVendor account = accountsMapper.selectActiveAccountWithParentVendorByPrimaryKey(form.getAccountId());

		if (account == null) {
			throw new BcpBuisinessException(404, "アカウントID不一致");
		}

		account.setS3Endpoint(s3Endpoint);
		return account;
	}
}
