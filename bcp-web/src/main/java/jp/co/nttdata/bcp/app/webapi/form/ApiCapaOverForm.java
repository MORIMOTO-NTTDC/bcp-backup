package jp.co.nttdata.bcp.app.webapi.form;

import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import org.hibernate.validator.constraints.Range;

import jp.co.nttdata.bcp.app.common.validation.HalfNumericString;
import lombok.Data;

/**
 * 容量オーバー通知用入力フォーム
 */
@Data
public class ApiCapaOverForm {

	/** アカウントID */
	@NotNull
	@HalfNumericString
	@Size(min = 10, max = 10)
	private String accountId;

	/** BCP端末同期ディレクトリ使用容量 */
	@NotNull
	@Range(min = 0, max = 999)
	private Integer usedSize;
}
