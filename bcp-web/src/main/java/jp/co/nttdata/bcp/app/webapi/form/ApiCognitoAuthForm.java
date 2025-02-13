package jp.co.nttdata.bcp.app.webapi.form;

import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Pattern;
import jakarta.validation.constraints.Size;

import jp.co.nttdata.bcp.app.common.validation.HalfAlphaNumericSymble;
import jp.co.nttdata.bcp.app.common.validation.HalfNumericString;
import lombok.Data;

/**
 * アカウント設定情報取得用入力フォーム
 */
@Data
public class ApiCognitoAuthForm {

	/** アカウントID */
	@NotNull
	@HalfNumericString
	@Size(min = 10, max = 10)
	private String accountId;

	/** BCP端末ソフトバージョン */
	@NotNull
	@HalfAlphaNumericSymble
	@Size(max = 10)
	private String softVersion;

	/** BCP端末同期ディレクトリ */
	@NotNull
	@HalfAlphaNumericSymble
	@Size(max = 260)
	@Pattern(regexp = "^[a-zA-Z]\\:\\\\(([^\\\\\\:]+\\\\)*[^\\\\\\:]*)?$")
	private String localDir;

	/** 認証キー */
	@NotNull
	@Size(min = 8, max = 8)
	@HalfAlphaNumericSymble
	private String authKey;

}
