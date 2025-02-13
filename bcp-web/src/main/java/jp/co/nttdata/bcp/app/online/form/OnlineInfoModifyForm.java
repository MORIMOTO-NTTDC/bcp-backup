package jp.co.nttdata.bcp.app.online.form;

import java.util.Date;
import java.util.List;

import jakarta.validation.Valid;
import jakarta.validation.constraints.AssertTrue;
import jakarta.validation.constraints.NotNull;
import jakarta.validation.constraints.Size;

import org.hibernate.validator.constraints.Range;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.OptBoolean;

import jp.co.nttdata.bcp.app.common.code.ModeType;
import jp.co.nttdata.bcp.app.common.validation.CodeOfEnum;
import jp.co.nttdata.bcp.app.common.validation.HalfFullWidthString;
import jp.co.nttdata.bcp.app.online.form.info.AtacchedFileForm;
import lombok.Data;

/**
 * お知らせ編集用フォーム
 */
@Data
public class OnlineInfoModifyForm {
	
	/** 処理区分（0:更新 1:削除） */
	@NotNull
	@CodeOfEnum(enumClass = ModeType.class)
	private String mode;
	
	/** お知らせキー */
	@Range(min = 0)
	@NotNull
	private Long key;
	
	/** 掲載開始日（YYYY/MM/DD形式） */
	@JsonFormat(pattern = "yyyy/MM/dd", lenient = OptBoolean.FALSE, timezone = "Asia/Tokyo")
	private Date startDate;
	
	/** 掲載終了日（YYYY/MM/DD形式） */
	@JsonFormat(pattern = "yyyy/MM/dd", lenient = OptBoolean.FALSE, timezone = "Asia/Tokyo")
	private Date endDate;
	
	/** タイトル */
	@HalfFullWidthString
	@Size(max = 50)
	private String title;
	
	/** 記事 */
	@HalfFullWidthString
	@Size(max = 500)
	private String article;

	/** 添付ファイルリスト */
	@Valid
	private List<AtacchedFileForm> files;
	
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
			// 掲載開始日の必須チェック
			if (startDate==null) {
				return false;
			}
		}
		return true;
	}
	
	/**
	 * 掲載開始日、掲載終了日のチェック</br>
	 * ※処理区分が削除の場合はチェックしない
	 * @return 掲載開始日＞掲載終了日の場合false
	 */
	@AssertTrue
	public boolean isValiedStartDateAndEndDate() {

		if (ModeType.DELETE.getCodeValue().equals(mode)) {
			return true;
		}
		
		if (startDate==null || endDate==null) {
			return true;
		}
		
		return !startDate.after(endDate);
	}

}
