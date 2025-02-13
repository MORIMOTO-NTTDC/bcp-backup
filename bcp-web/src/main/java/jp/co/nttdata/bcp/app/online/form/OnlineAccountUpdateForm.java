package jp.co.nttdata.bcp.app.online.form;

import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import org.hibernate.validator.constraints.Range;

import jp.co.nttdata.bcp.app.common.validation.HalfNumericString;
import lombok.Data;

/**
 * アカウント更新用入力フォーム
 */
@Data
public class OnlineAccountUpdateForm {

	/** アカウントID */
	@NotNull
	@HalfNumericString
	@Size(max = 10)
	private String accountId;

	/** アップロード実行可否 */
	@NotNull
	private Boolean uploadEnable;

	/** アップロード頻度 */
	@NotNull
	@Range(min = 0, max = 999)
	private Integer uploadTiming;

	/** ダウンロード実行可否 */
	@NotNull
	private Boolean downloadEnable;

	/** ベンダID */
	@NotNull
	@HalfNumericString
	@Size(max = 4)
	private String vendorId;

	/** 更新回数 */
	@Range(min = 0)
	@NotNull
	private Integer version;
}
