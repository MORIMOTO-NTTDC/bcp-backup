package jp.co.nttdata.bcp.app.online.form;

import jakarta.validation.constraints.NotNull;

import org.hibernate.validator.constraints.Range;

import lombok.Data;

/**
 * お知らせ一覧取得用フォーム
 */
@Data
public class OnlineInfoListForm {

	/** 検索結果のオフセット位置（0相対） */
	@NotNull
	@Range(min = 0, max = 9999)
	private Integer offset;

	/** 取得件数 */
	@NotNull
	@Range(min = 0, max = 9999)
	private Integer limit;

}
