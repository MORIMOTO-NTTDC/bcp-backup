package jp.co.nttdata.bcp.app.online.controller;

import org.springframework.http.HttpStatus;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.ResponseStatus;
import org.springframework.web.bind.annotation.RestController;

import lombok.extern.slf4j.Slf4j;

/**
 * ログアウト
 */
@RestController
@Transactional
@Slf4j
public class OnlineLogoutController {

	/**
	 * ログアウト成功後.<br>
	 * 当処理はフレームワークにって認証処理が正常に行われた後に呼ばれる。
	 */
	@RequestMapping("/logoutSuccess")
	@ResponseStatus(HttpStatus.OK)
	public void logoutSuccess() {
		log.debug("ログアウトOK");
	}
}
