package jp.co.nttdata.bcp.app.online.form;

import java.util.Date;
import java.util.List;

import jakarta.validation.Valid;
import jakarta.validation.constraints.AssertTrue;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.OptBoolean;

import jp.co.nttdata.bcp.app.common.validation.HalfFullWidthString;
import jp.co.nttdata.bcp.app.online.form.info.AtacchedFileForm;
import lombok.Data;

/**
 * お知らせ登録用フォーム
 */
@Data
public class OnlineInfoRegisForm {

	/** 掲載開始日（YYYY/MM/DD形式） */
	@NotNull
	@JsonFormat(pattern = "yyyy/MM/dd", lenient = OptBoolean.FALSE, timezone = "Asia/Tokyo")
	private Date startDate;

	/** 掲載終了日（YYYY/MM/DD形式） */
	@JsonFormat(pattern = "yyyy/MM/dd", lenient = OptBoolean.FALSE, timezone = "Asia/Tokyo")
	private Date endDate;

	/** タイトル */
	@NotNull
	@HalfFullWidthString
	@Size(max = 50)
	private String title;

	/** 記事 */
	@NotNull
	@HalfFullWidthString
	@Size(max = 500)
	private String article;

	/** 添付ファイルリスト */
	@Valid
	private List<AtacchedFileForm> files;
	
	/**
	 * 掲載開始日、掲載終了日のチェック
	 * @return 掲載開始日＞掲載終了日の場合false
	 */
	@AssertTrue
	public boolean isValiedStartDateAndEndDate() {

		if (endDate==null) {
			return true;
		}
		
		return !startDate.after(endDate);
	}


}
