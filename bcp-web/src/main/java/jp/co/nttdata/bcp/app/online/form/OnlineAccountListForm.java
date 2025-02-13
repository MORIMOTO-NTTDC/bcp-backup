package jp.co.nttdata.bcp.app.online.form;

import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import org.hibernate.validator.constraints.Range;

import jp.co.nttdata.bcp.app.common.code.IsChecked;
import jp.co.nttdata.bcp.app.common.validation.CodeOfEnum;
import jp.co.nttdata.bcp.app.common.validation.HalfFullWidthString;
import jp.co.nttdata.bcp.app.common.validation.HalfNumericString;
import lombok.Data;

/**
 * アカウント一覧取得用入力フォーム
 */
@Data
public class OnlineAccountListForm {

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
	
	/** アカウントID（絞り込条件：部分一致） */
	@HalfNumericString
	@Size(max = 10)
	private String accountId;
	
	/** アカウント名（絞り込条件：部分一致） */
	@HalfFullWidthString
	@Size(max = 100)
	private String accountName;
	
	/** アカウント住所（絞り込条件：部分一致） */
	@HalfFullWidthString
	@Size(max = 200)
	private String accountAddress;
	
	/** 同期失敗（'1'）（絞り込条件） */
	@HalfNumericString
	@Size(max = 1)
	@CodeOfEnum(enumClass = IsChecked.class)
	private String syncError;
	
	/** 使用領域90％以上（'1'）（絞り込条件） */
	@HalfNumericString
	@Size(max = 1)
	@CodeOfEnum(enumClass = IsChecked.class)
	private String diskAlert;
	
	/** 旧バージョン利用（'1'）（絞り込条件） */
	@HalfNumericString
	@Size(max = 1)
	@CodeOfEnum(enumClass = IsChecked.class)
	private String versionAlert;
}
