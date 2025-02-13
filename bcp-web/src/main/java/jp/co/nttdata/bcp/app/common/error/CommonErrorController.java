package jp.co.nttdata.bcp.app.common.error;

import org.springframework.http.HttpStatus;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseStatus;
import org.springframework.web.bind.annotation.RestController;

import jp.co.nttdata.bcp.app.common.exceition.ErrorObect;
import lombok.extern.slf4j.Slf4j;

/**
 * エラー処理用コントローラー
 */
@RestController
@RequestMapping("/common/error")
@Slf4j
public class CommonErrorController {

	@RequestMapping("/accessDeniedError")
	public ErrorObect accessDeniedError() {
		ErrorObect errorObect = new ErrorObect(403, "access denied error.");
		return errorObect;
	}

	/**
	 * 正しいCSRFトークンが送信されなかった場合
	 * 当処理は、クライアントから送信されたCSRFトークンが無効（もしくはクライアントからトークンが送信されなかった場合）
	 * フレームワークによって呼び出される。
	 * @return エラーオブジェクト
	 */
	@RequestMapping("/invalidCsrfTokenError")
	public ErrorObect invalidCsrfTokenError() {
		ErrorObect errorObect = new ErrorObect(403, "invalid csrf token error.");
		return errorObect;
	}

	/**
	 * CSRFトークンが未設定の場合
	 * 当処理は、CSRFトークンがセッションになかった場合にフレームワークによって呼び出される。
	 * @return エラーオブジェクト
	 */
	@RequestMapping("/missingCsrfTokenError")
	@ResponseStatus(HttpStatus.UNAUTHORIZED)
	public ErrorObect missingCsrfTokenError() {
		log.warn("未ログイン");
		ErrorObect errorObect = new ErrorObect(401, "未ログイン");
		return errorObect;
	}

	@RequestMapping("/resourceNotFoundError")
	public ErrorObect resourceNotFoundError() {
		ErrorObect errorObect = new ErrorObect(404, "resource not found error.");
		return errorObect;
	}

	/**
	 * 予期せぬエラー<br>
	 * 当処理は、Controlerに入る前に予期せぬ例外が発生した場合に、web.xmlの定義にしたがって呼び出される。
	 * @return エラーオブジェクト
	 */
	@RequestMapping("/systemError")
	@ResponseStatus(HttpStatus.INTERNAL_SERVER_ERROR)
	public ErrorObect systemError() {
		ErrorObect errorObect = new ErrorObect(500, "予期せぬエラー");
		return errorObect;
	}

	/**
	 * 未ログイン.<br>
	 * 当処理は未ログイン状態でログイン後画面が呼ばれた場合にフレームワークによって呼び出される。
	 * @return エラーオブジェクト
	 */
	@RequestMapping("/unauthenticated")
	@ResponseStatus(HttpStatus.UNAUTHORIZED)
	public ErrorObect unauthenticated() {
		log.warn("未ログイン");
		ErrorObect errorObect = new ErrorObect(401, "未ログイン");
		return errorObect;
	}

}
