package jp.co.nttdata.bcp.app.common.exceition;

import lombok.AllArgsConstructor;
import lombok.Data;

/**
 * クライアントにJSONで返却するためのエラーオブジェクト
 */
@Data
@AllArgsConstructor
public class ErrorObect {

	/** NG時のHTTPステータスコード */
	private int errorCode;
	
	/** エラーメッセージ */
	private String messages;
	
	
}
