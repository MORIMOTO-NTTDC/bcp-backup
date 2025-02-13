package jp.co.nttdata.bcp.app.online.form;

import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import lombok.Data;

/**
 * パスワード初期化用入力フォーム.
 */
@Data
public class OnlinePasswordReset {

	/** メールアドレス */
	@NotNull
	@Email
	@Size(max = 100)
	private String mailAddress;
}
