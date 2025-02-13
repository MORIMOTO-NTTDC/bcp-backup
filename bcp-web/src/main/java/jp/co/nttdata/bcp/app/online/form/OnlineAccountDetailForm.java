package jp.co.nttdata.bcp.app.online.form;

import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import jp.co.nttdata.bcp.app.common.validation.HalfNumericString;
import lombok.Data;

/**
 * アカウント設定情報取得用入力フォーム
 */
@Data
public class OnlineAccountDetailForm {

	/** アカウントID */
	@NotNull
	@HalfNumericString
	@Size(min = 10, max = 10)
	private String accountId;
}
