package jp.co.nttdata.bcp.app.online.form;

import jakarta.validation.constraints.AssertTrue;
import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import jp.co.nttdata.bcp.app.common.validation.HalfAlphaNumericSymble;
import jp.co.nttdata.bcp.app.common.validation.HalfFullWidthString;
import jp.co.nttdata.bcp.app.common.validation.HalfNumericString;
import jp.co.nttdata.bcp.app.common.validation.Password;
import lombok.Data;

/**
 * ユーザ登録用フォーム
 */
@Data
public class OnlineUserRegistForm {

	/** ユーザID */
	@NotNull
	@HalfAlphaNumericSymble
	@Size(max = 20)
	private String userId;

	/** ユーザ名 */
	@NotNull
	@HalfFullWidthString
	@Size(max = 100)
	private String userName;

	/** パスワード */
	@NotNull
	@Password
	private String password;

	/** ベンダID*/
	@NotNull
	@HalfNumericString
	@Size(min = 4, max = 4)
	private String vendorId;

	/** メールアドレス */
	@NotNull
	@Email
	@Size(max = 100)
	private String mailAddress;

	/** 管理者権限有無（true:あり） */
	@NotNull
	private Boolean admin;

	/**
	 * パスワードがユーザIDと同じの場合エラー
	 * @return チェックNGの場合false
	 */
	@AssertTrue
	public boolean isValiedPassword() {

		if (userId == null || password == null) {
			return true;
		}

		return !userId.equals(password);
	}

}
