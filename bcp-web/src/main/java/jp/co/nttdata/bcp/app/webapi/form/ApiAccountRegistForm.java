package jp.co.nttdata.bcp.app.webapi.form;

import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import org.hibernate.validator.constraints.Range;

import jp.co.nttdata.bcp.app.common.validation.HalfAlphaNumericSymble;
import jp.co.nttdata.bcp.app.common.validation.HalfFullWidthString;
import jp.co.nttdata.bcp.app.common.validation.HalfNumericString;
import lombok.Data;

/**
 * アカウント登録用入力フォーム
 */
@Data
public class ApiAccountRegistForm {

	/** アカウントID */
	@NotNull
	@HalfNumericString
	@Size(min = 10, max = 10)
	private String accountId;

	/** 医療機関名 */
	@NotNull
	@HalfFullWidthString
	@Size(max = 100)
	private String accountName;

	/** 住所 */
	@NotNull
	@HalfFullWidthString
	@Size(max = 200)
	private String accountAddress;

	/** バックアップ容量（GB） */
	@NotNull
	@Range(min = 0, max = 999)
	private Integer backupCapa;

	/** ベンダ番号 */
	@NotNull
	@Size(min = 4, max = 4)
	@HalfNumericString
	private String vendorId;

	/** 親ベンダ番号 */
	@NotNull
	@Size(min = 4, max = 4)
	@HalfNumericString
	private String parentId;

	/** 認証キー */
	@NotNull
	@Size(min = 8, max = 8)
	@HalfAlphaNumericSymble
	private String authKey;
}
