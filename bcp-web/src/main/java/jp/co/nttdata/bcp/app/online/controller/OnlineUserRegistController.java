package jp.co.nttdata.bcp.app.online.controller;

import jakarta.inject.Inject;

import org.springframework.beans.BeanUtils;
import org.springframework.dao.DuplicateKeyException;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import jp.co.nttdata.bcp.app.common.code.UserType;
import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.online.form.OnlineUserRegistForm;
import jp.co.nttdata.bcp.domain.model.Users;
import jp.co.nttdata.bcp.domain.model.Vendors;
import jp.co.nttdata.bcp.domain.model.login.BcpUserDetails;
import jp.co.nttdata.bcp.domain.model.login.UserAccount;
import jp.co.nttdata.bcp.domain.repository.UsersMapper;
import jp.co.nttdata.bcp.domain.repository.VendorsMapper;

/**
 * ユーザ登録
 */
@RestController
@Transactional
public class OnlineUserRegistController {

	@Inject
	private VendorsMapper vendorssMapper;

	@Inject
	private UsersMapper usersMapper;

	@Inject
	private PasswordEncoder passwordEncoder;

	/**
	 * ユーザ登録.<br>
	 * ユーザ情報を新規登録する。
	 * @param form 入力値
	 * @param userDetails 認証情報
	 */
	@PreAuthorize("hasAnyRole('SYSTEM_ADMIN', 'VENDOR_ADMIN')")
	@PostMapping("/sc13_regist.do")
	public void userRegist(@Validated @RequestBody OnlineUserRegistForm form,
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
		Users user = new Users();
		BeanUtils.copyProperties(form, user);

		Users result = usersMapper.selectIserByUserIdOrEmail(user);

		if (result == null) {
			//『ユーザID』 =クエリ文字列パラメータ『ユーザID』場合
		} else if (user.getUserId().equals(result.getUserId())) {
			throw new BcpBuisinessException(406, "既に使用されているユーザIDです。");
		} else {
			//『メールアドレス』 =クエリ文字列パラメータ『メールアドレス』場合
			throw new BcpBuisinessException(409, "既に使用されているメールアドレスです。");
		}

		//パスワードはハッシュ化したパスワードに置き換える
		user.setPassword(passwordEncoder.encode(form.getPassword()));

		// ユーザ登録
		try {
			usersMapper.insertWithSelect(user);
		} catch (DuplicateKeyException e) {
			throw new BcpBuisinessException(406, "既に使用されているユーザIDです。", e);
		}

	}

}
