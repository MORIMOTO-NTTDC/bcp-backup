package jp.co.nttdata.bcp.app.online.form;

import jakarta.validation.constraints.AssertTrue;
import jakarta.validation.constraints.Email;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import org.hibernate.validator.constraints.Range;

import jp.co.nttdata.bcp.app.common.code.ModeType;
import jp.co.nttdata.bcp.app.common.validation.CodeOfEnum;
import jp.co.nttdata.bcp.app.common.validation.HalfAlphaNumericSymble;
import jp.co.nttdata.bcp.app.common.validation.HalfFullWidthString;
import jp.co.nttdata.bcp.app.common.validation.HalfNumericString;
import lombok.Data;

/**
 * ユーザ編集用入力フォーム
 */
@Data
public class OnlineUserModifyForm {

	/** 処理区分（0:更新 1:削除） */
	@NotNull
	@HalfNumericString
	@Size(min = 1, max = 1)
	@CodeOfEnum(enumClass = ModeType.class)
	private String mode;

	/** ユーザキー */
	@Range(min = 0)
	@NotNull
	private Long key;

	/** ユーザID */
	@HalfAlphaNumericSymble
	@Size(max = 20)
	private String userId;

	/** ユーザ名 */
	@HalfFullWidthString
	@Size(max = 100)
	private String userName;

	/** ベンダID */
	@HalfNumericString
	@Size(min = 4, max = 4)
	private String vendorId;

	/** メールアドレス */
	@Email
	@Size(max = 100)
	private String mailAddress;

	/** 管理者権限有無（true:あり） */
	private Boolean admin;

	/** 更新回数 */
	@Range(min = 0)
	@NotNull
	private Integer version;

	/**
	 * 処理区分が更新時の必須チェック
	 * @return チェックに引っかかる場合false
	 */
	@AssertTrue
	public boolean isValiedUpdateItem() {
		if (ModeType.UPDATE.getCodeValue().equals(mode)) {
			if (userId == null) {
				return false;
			}
			if (userName == null) {
				return false;
			}
			if (vendorId == null) {
				return false;
			}
			if (mailAddress == null) {
				return false;
			}
			if (admin == null) {
				return false;
			}
		}
		return true;
	}

}
