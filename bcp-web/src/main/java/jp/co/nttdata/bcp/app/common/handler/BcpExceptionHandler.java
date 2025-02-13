package jp.co.nttdata.bcp.app.common.handler;

import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.HttpStatusCode;
import org.springframework.http.ResponseEntity;
import org.springframework.http.converter.HttpMessageNotReadableException;
import org.springframework.lang.Nullable;
import org.springframework.security.access.AccessDeniedException;
import org.springframework.validation.BindException;
import org.springframework.web.bind.MethodArgumentNotValidException;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RestControllerAdvice;
import org.springframework.web.context.request.WebRequest;
import org.springframework.web.servlet.mvc.method.annotation.ResponseEntityExceptionHandler;

import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.common.exceition.ErrorObect;

/**
 * エラーハンドラー
 */
@RestControllerAdvice
public class BcpExceptionHandler extends ResponseEntityExceptionHandler {

	/**
	 * 業務例外
	 * @param ex 業務例外
	 * @param request HTTPリクエスト
	 * @return レスポンス
	 */
	@ExceptionHandler(BcpBuisinessException.class)
	public ResponseEntity<Object> handlBcpBuisinessException(BcpBuisinessException ex, WebRequest request) {
		return new ResponseEntity<>(ex.getErrorObject(), null, ex.getErrorObject().getErrorCode());
	}

	/**
	 * 入力エラー
	 * {@inheritDoc}
	 */
	@Override
	protected ResponseEntity<Object> handleHttpMessageNotReadable(HttpMessageNotReadableException ex,
			HttpHeaders headers, HttpStatusCode status, WebRequest request) {
		return handleExceptionInternal(ex, new ErrorObect(400, "入力チェックエラー"), null,
				HttpStatus.BAD_REQUEST, request);
	}

	/**
	 * 入力エラー
	 * {@inheritDoc}
	 */
	@Override
	protected ResponseEntity<Object> handleMethodArgumentNotValid(MethodArgumentNotValidException ex,
			HttpHeaders headers, HttpStatusCode status, WebRequest request) {
		return handleBindException(ex, headers, status, request);
	}

	/**
	 * 入力エラー
	 * {@inheritDoc}
	 */
	@Override
	protected ResponseEntity<Object> handleBindException(BindException ex, HttpHeaders headers, HttpStatusCode status,
			WebRequest request) {
		return handleExceptionInternal(ex, new ErrorObect(400, "入力チェックエラー"), null,
				HttpStatus.BAD_REQUEST, request);
	}

	/**
	 * 権限エラー
	 * @param ex 業務例外
	 * @param request HTTPリクエスト
	 * @return レスポンス
	 */
	@ExceptionHandler(AccessDeniedException.class)
	public ResponseEntity<Object> handleAccessDeniedException(AccessDeniedException ex, WebRequest request) {
		return handleExceptionInternal(ex, new ErrorObect(403, "利用権限なし"), new HttpHeaders(),
				HttpStatus.FORBIDDEN, request);
	}

	/**
	 * 予期せぬ例外
	 * @param ex 業務例外
	 * @param request HTTPリクエスト
	 * @return レスポンス
	 */
	@ExceptionHandler(Exception.class)
	public ResponseEntity<Object> handleAllException(Exception ex, WebRequest request) {
		return handleExceptionInternal(ex, new ErrorObect(500, "予期せぬエラー"), new HttpHeaders(),
				HttpStatus.INTERNAL_SERVER_ERROR, request);
	}

	/**
	 * {@inheritDoc}
	 */
	@Override
	protected ResponseEntity<Object> handleExceptionInternal(
			Exception ex, @Nullable Object body, HttpHeaders headers, HttpStatusCode statusCode, WebRequest request) {

		if (body == null) {
			return super.handleExceptionInternal(ex, new ErrorObect(500, "予期せぬエラー"), headers, statusCode, request);
		} else if (body instanceof ErrorObect) {
			return super.handleExceptionInternal(ex, body, headers, statusCode, request);
		} else {
			return super.handleExceptionInternal(ex, new ErrorObect(500, "予期せぬエラー"), headers,
					HttpStatus.INTERNAL_SERVER_ERROR, request);
		}
	}

}
