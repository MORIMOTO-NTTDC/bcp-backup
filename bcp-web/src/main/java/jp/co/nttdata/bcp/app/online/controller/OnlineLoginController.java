package jp.co.nttdata.bcp.app.online.controller;

import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;
import jakarta.validation.Valid;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.UsernamePasswordAuthenticationToken;
import org.springframework.security.core.Authentication;
import org.springframework.security.core.AuthenticationException;
import org.springframework.security.core.context.SecurityContext;
import org.springframework.security.core.context.SecurityContextHolder;
import org.springframework.security.core.context.SecurityContextHolderStrategy;
import org.springframework.security.web.context.HttpSessionSecurityContextRepository;
import org.springframework.security.web.csrf.CsrfToken;
import org.springframework.validation.BindingResult;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.online.form.OnlineLoginForm;
import jp.co.nttdata.bcp.domain.model.login.BcpUserDetails;
import lombok.extern.slf4j.Slf4j;

/**
 * ログイン
 */
@Slf4j
@RestController
public class OnlineLoginController {

	@Autowired
	private AuthenticationManager authenticationManager;

	private final SecurityContextHolderStrategy securityContextHolderStrategy = SecurityContextHolder
			.getContextHolderStrategy();

	/**
	 * CSRFトークンを取得する
	 * 
	 * @param token CSRFトークン
	 * @return CSRFトークン
	 */
	@GetMapping(path = "/csrf")
	public CsrfToken csrf(CsrfToken token) {
		return token;
	}

	/**
	 * ログイン.<br>?
	 * ユーザID、パスワードをもとにログイン認証を行う。
	 * @param form 入力値
	 * @param result Formにおける入力チェック結果
	 * @param request HTTPリクエスト
	 * @param response HTTPレスポンス
	 * @return ログインユーザのアカウント情報
	 */
	@PostMapping("login.do")
	public Object login(@Valid @RequestBody OnlineLoginForm form, BindingResult result, HttpServletRequest request,
			HttpServletResponse response) {

		// 入力チェックエラー
		if (result.hasErrors()) {
			log.warn(result.toString());
			throw new BcpBuisinessException(404, "ユーザID、メールアドレスまたはパスワードが誤っています。");
		}

		// 認証用の値を設定
		UsernamePasswordAuthenticationToken token = UsernamePasswordAuthenticationToken.unauthenticated(
				form.getUserId(), form.getPassword());
		try {
			// 認証
			Authentication authentication = authenticationManager.authenticate(token);

			// セッションを無効にする
			request.getSession().invalidate();

			// セッションへの認証情報の保存などを行う
			SecurityContext context = securityContextHolderStrategy.createEmptyContext();
			context.setAuthentication(authentication);
			securityContextHolderStrategy.setContext(context);
			new HttpSessionSecurityContextRepository().saveContext(context, request, response);

			//　ログインユーザのアカウント情報を返却
			return ((BcpUserDetails) authentication.getPrincipal()).getUserAccount();

		} catch (AuthenticationException e) {
			throw new BcpBuisinessException(404, "ユーザID、メールアドレスまたはパスワードが誤っています。", e);
		}
	}

}
