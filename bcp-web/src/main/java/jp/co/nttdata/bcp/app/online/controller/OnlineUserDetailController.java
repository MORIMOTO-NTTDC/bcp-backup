package jp.co.nttdata.bcp.app.online.controller;

import jakarta.inject.Inject;

import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import com.fasterxml.jackson.annotation.JsonView;

import jp.co.nttdata.bcp.app.common.code.UserType;
import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.online.form.OnlineUserDetailForm;
import jp.co.nttdata.bcp.domain.model.login.BcpUserDetails;
import jp.co.nttdata.bcp.domain.model.login.UserAccount;
import jp.co.nttdata.bcp.domain.model.user.UserWithVendor;
import jp.co.nttdata.bcp.domain.model.user.UserWithVendor.UserDetail;
import jp.co.nttdata.bcp.domain.repository.UsersMapper;

/**
 * ユーザ詳細取得
 */
@RestController
@Transactional
public class OnlineUserDetailController {

	@Inject
	private UsersMapper usersMapper;

	/**
	 * ユーザ詳細取得.<br>
	 * 指定された登録済みのユーザ情報を取得する。
	 * 
	 * @param form 入力値
	 * @param userDetails 認証情報
	 * @return ユーザ情報
	 */
	@PreAuthorize("hasAnyRole('SYSTEM_ADMIN', 'VENDOR_ADMIN')")
	@PostMapping("/sc13_detail.do")
	@JsonView(UserDetail.class)
	public UserWithVendor userDetail(@Validated @RequestBody OnlineUserDetailForm form,
			@AuthenticationPrincipal BcpUserDetails userDetails) {

		UserWithVendor user = usersMapper.selectByPrimaryKeyWithVendor(form.getKey());

		if (user == null) {
			throw new BcpBuisinessException(404, "該当のユーザは存在しません。");
		}

		UserAccount userAccount = userDetails.getUserAccount();
		//ベンダ管理者の場合
		if (UserType.VENDOR_ADMIN.getCodeValue().equals(userAccount.getUserType())) {
			if (userAccount.getVendorId().equals(userAccount.getParentId())) {
				// 親ベンダの場合
				if (!userAccount.getParentId().equals(user.getParentId())) {
					// 配下のベンダのユーザでない場合
					throw new BcpBuisinessException(403, "権限なし");
				}

			} else {
				// 代理店の場合
				if (!userAccount.getVendorId().equals(user.getVendorId())) {
					// 入力されたベンダIDと取得したベンダIDが異なる場合
					throw new BcpBuisinessException(403, "権限なし");
				}
			}
		}

		return user;
	}
}
