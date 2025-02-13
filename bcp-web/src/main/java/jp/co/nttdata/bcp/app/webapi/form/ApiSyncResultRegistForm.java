package jp.co.nttdata.bcp.app.webapi.form;

import java.util.Date;

import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Pattern;
import jakarta.validation.constraints.Size;

import org.hibernate.validator.constraints.Range;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.OptBoolean;

import jp.co.nttdata.bcp.app.common.validation.HalfFullWidthString;
import jp.co.nttdata.bcp.app.common.validation.HalfNumericString;
import lombok.Data;

/**
 * 同期結果登録用入力フォーム
 */
@Data
public class ApiSyncResultRegistForm {

	/** アカウントID */
	@NotNull
	@HalfNumericString
	@Size(min = 10, max = 10)
	private String accountId;

	/** 最終同期日時 */
	@NotNull
	@JsonFormat(pattern = "yyyy/MM/dd HH:mm:ss", lenient = OptBoolean.FALSE, timezone = "Asia/Tokyo")
	private Date lastdate;

	/** 同期ステータス */
	@NotNull
	@Pattern(regexp = "^(1|2|3|4|5)$")
	private String status;

	/** エラー情報 */
	@HalfFullWidthString
	@Size(max = 200)
	private String errorInfo;

	/** BCP端末同期ディレクトリ使用容量 */
	@NotNull
	@Range(min = 0, max = 999)
	private Integer usedSize;

}
