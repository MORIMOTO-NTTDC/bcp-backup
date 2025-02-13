package jp.co.nttdata.bcp.app.online.controller;

import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.stereotype.Controller;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.ResponseBody;

import jp.co.nttdata.bcp.domain.model.login.BcpUserDetails;
import jp.co.nttdata.bcp.domain.model.login.UserAccount;

/**
 * ログインユーザ情報取得
 */
@Controller
@Transactional
public class OnlineUserinfoController {

	/**
	 * ログインユーザ情報取得.<br>
	 * セッション内のログインユーザ情報を取得する。
	 * 
	 * @param userDetails 認証情報
	 * @return ログインユーザのアカウント情報
	 */
	@PostMapping("/userinfo.do")
	@ResponseBody
	public UserAccount userinfo(@AuthenticationPrincipal BcpUserDetails userDetails) {

		return userDetails.getUserAccount();
	}

}
