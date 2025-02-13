package jp.co.nttdata.bcp.app.online.form;

import jakarta.validation.constraints.NotEmpty;

import jp.co.nttdata.bcp.app.common.validation.Password;
import lombok.Data;

/**
 * パスワード変更用入力フォーム
 */
@Data
public class OnlinePasswordUpdateForm {

	/** パスワード */
	@NotEmpty
	@Password
	private String password;
	
}
