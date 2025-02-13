package jp.co.nttdata.bcp.app.online.form;

import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import org.hibernate.validator.constraints.Range;

import jp.co.nttdata.bcp.app.common.validation.HalfAlphaNumericSymble;
import jp.co.nttdata.bcp.app.common.validation.HalfFullWidthString;
import jp.co.nttdata.bcp.app.common.validation.HalfNumericString;
import lombok.Data;

/**
 * ユーザ一覧取得用入力フォーム
 */
@Data
public class OnlineUserListForm {
	
	/** 検索結果のオフセット位置（0相対） */
	@NotNull
	@Range(min = 0, max = 9999)
	private Integer offset;
	
	/** 取得件数 */
	@NotNull
	@Range(min = 0, max = 9999)
	private Integer limit;
	
	/** ベンダID（絞り込条件） */
	@HalfNumericString
	@Size(max = 4)
	private String vendorId;
	
	/** ユーザID（絞り込条件：部分一致） */
	@HalfAlphaNumericSymble
	@Size(max = 20)
	private String userId;
	
	/** ユーザ名（絞り込条件：部分一致） */
	@HalfFullWidthString
	@Size(max = 100)
	private String userName;

}
