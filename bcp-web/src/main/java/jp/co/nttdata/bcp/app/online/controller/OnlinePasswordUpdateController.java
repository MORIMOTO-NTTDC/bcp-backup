package jp.co.nttdata.bcp.app.online.controller;

import jakarta.inject.Inject;
import jakarta.servlet.http.HttpServletRequest;

import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.online.form.OnlinePasswordUpdateForm;
import jp.co.nttdata.bcp.domain.model.Users;
import jp.co.nttdata.bcp.domain.model.login.BcpUserDetails;
import jp.co.nttdata.bcp.domain.repository.UsersMapper;

/**
 * パスワード変更
 */
@RestController
@Transactional
public class OnlinePasswordUpdateController {

	@Inject
	private UsersMapper usersMapper;

	@Inject
	private PasswordEncoder passwordEncoder;

	/**
	 * パスワード変更.<br>
	 * パスワードの変更を行う。
	 * @param form 入力値
	 * @param userDetails 認証情報
	 * @param request HTTPリクエスト
	 */
	@PostMapping("/password_update.do")
	public void passwordUpdate(@Validated @RequestBody OnlinePasswordUpdateForm form,
			@AuthenticationPrincipal BcpUserDetails userDetails, HttpServletRequest request) {

		// ユーザIDと入力されたパスワードが一致する場合
		if (form.getPassword().equals(userDetails.getUsername())) {
			throw new BcpBuisinessException(400, "入力チェックエラー");
		}
		
		Users user = new Users();
		user.setUserId(userDetails.getUsername());
		user.setPassword(passwordEncoder.encode(form.getPassword()));

		usersMapper.updatePasswordByUserId(user);

		// セッションIDの変更
		request.changeSessionId();

	}
}
