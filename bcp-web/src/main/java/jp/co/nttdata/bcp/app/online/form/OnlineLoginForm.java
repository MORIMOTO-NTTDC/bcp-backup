package jp.co.nttdata.bcp.app.online.form;

import jakarta.validation.constraints.NotEmpty;
import jakarta.validation.constraints.Size;

import jp.co.nttdata.bcp.app.common.validation.Password;
import lombok.Data;

/**
 * ログイン用入力フォーム
 */
@Data
public class OnlineLoginForm {

	/** ユーザID（メールアドレスの場合あり） */
	@NotEmpty
	@Size(max = 100)
	private String userId;

	/** パスワード（平文） */
	@NotEmpty
	@Password
	private String password;
}
