package jp.co.nttdata.bcp.app.webapi.form;

import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import jp.co.nttdata.bcp.app.common.validation.HalfAlphaNumericSymble;
import lombok.Data;

/**
 * 端末ソフトバージョンチェック用入力フォーム
 */
@Data
public class ApiBcpSoftCheckForm {

	/** BCP端末ソフトバージョン */
	@NotNull
	@HalfAlphaNumericSymble
	@Size(max = 10)
	private String softVersion;


}
