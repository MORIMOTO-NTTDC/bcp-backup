package jp.co.nttdata.bcp.app.online.controller;

import jakarta.inject.Inject;

import org.springframework.beans.BeanUtils;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import jp.co.nttdata.bcp.app.common.code.UserType;
import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.online.form.OnlineAccountUpdateForm;
import jp.co.nttdata.bcp.domain.model.Accounts;
import jp.co.nttdata.bcp.domain.model.Vendors;
import jp.co.nttdata.bcp.domain.model.login.BcpUserDetails;
import jp.co.nttdata.bcp.domain.model.login.UserAccount;
import jp.co.nttdata.bcp.domain.repository.AccountsMapper;
import jp.co.nttdata.bcp.domain.repository.VendorsMapper;

/**
 * アカウント更新
 */
@RestController
@Transactional
public class OnlineAccountUpdateController {

	@Inject
	private AccountsMapper accountsMapper;

	@Inject
	private VendorsMapper vendorssMapper;

	/**
	 * アカウント更新.<br>
	 * 指定された登録済みのアカウント情報を変更する。
	 * 
	 * @param form 入力値
	 */
	@PreAuthorize("hasAnyRole('SYSTEM_ADMIN', 'VENDOR_ADMIN')")
	@PostMapping("/sc14_update.do")
	public void accountUpdate(@Validated @RequestBody OnlineAccountUpdateForm form,
			@AuthenticationPrincipal BcpUserDetails userDetails) {

		UserAccount userAccount = userDetails.getUserAccount();

		//ベンダ管理者の場合
		if (UserType.VENDOR_ADMIN.getCodeValue().equals(userAccount.getUserType())) {
			if (userAccount.getVendorId().equals(userAccount.getParentId())) {
				// 親ベンダの場合
				Vendors vendor = vendorssMapper.selectByVendorIdAndParentId(form.getVendorId(),
						userAccount.getParentId());

				if (vendor == null) {
					// 配下のベンダでない場合
					throw new BcpBuisinessException(403, "権限なし");
				}

			} else {
				// 代理店の場合
				if (!userAccount.getVendorId().equals(form.getVendorId())) {
					// 入力されたベンダIDとセッションのベンダIDが異なる場合
					throw new BcpBuisinessException(403, "権限なし");
				}
			}
		}

		// 入力内容を移し替える
		Accounts account = new Accounts();
		BeanUtils.copyProperties(form, account);

		// アカウント管理の楽観的ロック
		Accounts accountRecord = accountsMapper.selectActiveByPrimaryKey(account.getAccountId(), true);

		// ユーザ情報を取得できない場合
		if (accountRecord == null) {
			throw new BcpBuisinessException(404, "該当のアカウントは存在しません。");
		}
		// 取得した『更新回数』とform『更新回数』が不一致
		if (account.getVersion().intValue() != accountRecord.getVersion().intValue()) {
			throw new BcpBuisinessException(412, "該当のアカウントは他で更新されております。");
		}

		//システム管理者以外の場合
		if (!UserType.SYSTEM_ADMIN.getCodeValue().equals(userAccount.getUserType())) {
			if (userAccount.getVendorId().equals(userAccount.getParentId())) {
				// 親ベンダの場合
				if (!userAccount.getParentId().equals(accountRecord.getParentId())) {
					// 配下のベンダのユーザでない場合
					throw new BcpBuisinessException(403, "権限なし");
				}

			} else {
				// 代理店の場合
				if (!userAccount.getVendorId().equals(accountRecord.getVendorId())) {
					// 入力されたベンダIDと取得したベンダIDが異なる場合
					throw new BcpBuisinessException(403, "権限なし");
				}
			}
		}

		// アカウント管理を更新
		account.setVersion(form.getVersion() + 1);
		accountsMapper.updateActiveByPrimaryKeySelective(account);
	}
}
