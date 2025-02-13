package jp.co.nttdata.bcp.app.common.exceition;

import lombok.Getter;

/**
 * 業務例外クラス
 */
public class BcpBuisinessException extends RuntimeException {

	/** クライアントにJSONで返却するためのエラーオブジェクト */
	@Getter
	private ErrorObect errorObject;

	/**
	 * コンストラクタ
	 * @param httpStatus NG時のHTTPステータスコード
	 * @param messages エラーメッセージ
	 */
	public BcpBuisinessException(int httpStatus, String messages) {
		super(messages);
		errorObject = new ErrorObect(httpStatus, messages);
	}

	/**
	 * コンストラクタ
	 * @param httpStatus NG時のHTTPステータスコード
	 * @param messages エラーメッセージ
	 * @param e 発生した例外
	 */
	public BcpBuisinessException(int httpStatus, String messages, Exception e) {
		super(messages, e);
		errorObject = new ErrorObect(httpStatus, messages);
	}

}
