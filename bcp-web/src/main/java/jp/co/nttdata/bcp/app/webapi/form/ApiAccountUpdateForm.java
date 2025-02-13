package jp.co.nttdata.bcp.app.webapi.form;

import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Pattern;
import jakarta.validation.constraints.Size;

import org.hibernate.validator.constraints.Range;

import jp.co.nttdata.bcp.app.common.validation.HalfAlphaNumericSymble;
import jp.co.nttdata.bcp.app.common.validation.HalfNumericString;
import lombok.Data;

/**
 * アカウント設定情報取得用入力フォーム
 */
@Data
public class ApiAccountUpdateForm {

	/** アカウントID */
	@NotNull
	@HalfNumericString
	@Size(min = 10, max = 10)
	private String accountId;

	/** アップロード実行可否 */
	@NotNull
	private Boolean uploadEnable;
	
	/** アップロード頻度 */
	@NotNull
	@Range(min = 0, max = 999)
	private Integer uploadTiming;
	
	/** BCP端末同期ディレクトリ */
	@NotNull
	@HalfAlphaNumericSymble
	@Size(max = 260)
	@Pattern(regexp = "^[a-zA-Z]\\:\\\\(([^\\\\\\:]+\\\\)*[^\\\\\\:]*)?$")
	private String localDir;
	
	/** 更新回数 */
	@NotNull
	@Range(min = 0)
    private Integer version;

}
