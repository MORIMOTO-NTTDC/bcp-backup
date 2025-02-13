package jp.co.nttdata.bcp.app.online.controller;

import jakarta.inject.Inject;

import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import com.fasterxml.jackson.annotation.JsonView;

import jp.co.nttdata.bcp.app.common.code.UserType;
import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.online.form.OnlineAccountDetailForm;
import jp.co.nttdata.bcp.domain.model.account.AccountWithVendor;
import jp.co.nttdata.bcp.domain.model.account.AccountWithVendor.OnlineDetail;
import jp.co.nttdata.bcp.domain.model.login.BcpUserDetails;
import jp.co.nttdata.bcp.domain.model.login.UserAccount;
import jp.co.nttdata.bcp.domain.repository.AccountsMapper;

/**
 * アカウント詳細取得
 */
@RestController
@Transactional
public class OnlineAccountDetailController {

	@Inject
	private AccountsMapper accountsMapper;

	/**
	 * アカウント詳細取得.<br>
	 * 指定された登録済みのアカウント情報を取得する。
	 * 
	 * @param form 入力値
	 * @param userDetails 認証情報
	 * @return アカウント設定情報
	 */
	@PostMapping("/sc14_detail.do")
	@JsonView(OnlineDetail.class)
	public AccountWithVendor accountDetail(@Validated @RequestBody OnlineAccountDetailForm form,
			@AuthenticationPrincipal BcpUserDetails userDetails) {
		
		AccountWithVendor account = accountsMapper.selectActiveAccountWithParentVendorByPrimaryKey(form.getAccountId());

		if (account == null) {
			throw new BcpBuisinessException(404, "該当のアカウントは存在しません。");
		}

		UserAccount userAccount = userDetails.getUserAccount();
		//システム管理者以外の場合
		if (!UserType.SYSTEM_ADMIN.getCodeValue().equals(userAccount.getUserType())) {
			if (userAccount.getVendorId().equals(userAccount.getParentId())) {
				// 親ベンダの場合
				if (!userAccount.getParentId().equals(account.getParentId())) {
					// 配下のベンダのユーザでない場合
					throw new BcpBuisinessException(403, "権限なし");
				}

			} else {
				// 代理店の場合
				if (!userAccount.getVendorId().equals(account.getVendorId())) {
					// 入力されたベンダIDと取得したベンダIDが異なる場合
					throw new BcpBuisinessException(403, "権限なし");
				}
			}
		}

		return account;
	}
}
