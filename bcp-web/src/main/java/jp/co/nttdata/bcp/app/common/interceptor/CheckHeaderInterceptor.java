package jp.co.nttdata.bcp.app.common.interceptor;

import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

import org.springframework.web.servlet.HandlerInterceptor;

import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;

/**
 * リクエストヘッダ のチェック
 */
public class CheckHeaderInterceptor implements HandlerInterceptor {

	/**
	 * X-Requested-With ヘッダがXMLHttpRequestであることを確認する
	 */
	@Override
	public boolean preHandle(HttpServletRequest request, HttpServletResponse response, Object handler)
			throws Exception {

		if (!"XMLHttpRequest".equalsIgnoreCase(request.getHeader("X-Requested-With"))) {
			throw new BcpBuisinessException(403, "CSRFチェックエラー");
		}
		return true;
	}

}
