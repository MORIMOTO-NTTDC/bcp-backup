package jp.co.nttdata.bcp.app.webapi.controller;

import jakarta.inject.Inject;

import org.springframework.beans.BeanUtils;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.webapi.form.ApiAccountUpdateForm;
import jp.co.nttdata.bcp.domain.model.Accounts;
import jp.co.nttdata.bcp.domain.repository.AccountsMapper;

/**
 * アカウント設定情報更新
 */
@RestController
@Transactional
public class ApiAccountUpdateController {

	@Inject
	private AccountsMapper accountsMapper;

	/**
	 * アカウント設定情報更新.<br>
	 * アカウント設定情報を更新する
	 * 
	 * @param form 入力値
	 */
	@PostMapping("/api01_update.do")
	public void accountUpdate(@Validated @RequestBody ApiAccountUpdateForm form) {

		// 値の詰め替え
		Accounts account = new Accounts();
		BeanUtils.copyProperties(form, account);
		// 更新
		int updateCount = accountsMapper.updateAndIncrementCountByVersion(account);

		// 更新件数が1件以外
		if (updateCount != 1) {
			throw new BcpBuisinessException(412, "アカウントID不一致又は更新済み");
		}
	}
}
