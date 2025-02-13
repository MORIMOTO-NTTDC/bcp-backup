package jp.co.nttdata.bcp.app.online.form.info;

import jakarta.validation.constraints.AssertTrue;
import jakarta.validation.constraints.Pattern;
import jakarta.validation.constraints.Size;

import org.springframework.util.StringUtils;

import jp.co.nttdata.bcp.app.common.validation.FileName;
import lombok.Data;

/**
 * 添付ファイルの入力フォーム
 */
@Data
public class AtacchedFileForm {

	/** 添付ファイル名 */
	@FileName
	@Size(max = 260)
	private String attachedfileName;

	/** 添付ファイルデータ（base64形式） */
	@Pattern(regexp = "^data\\:\\w+\\/\\w+;base64,([0-9A-Za-z\\+\\/]{4})*([0-9A-Za-z\\+\\/]{3}\\=|[0-9A-Za-z\\+\\/]{2}\\={2})?$")
	private String attachedfileData;

	/**
	 * 添付ファイル名、添付ファイルデータのチェック
	 * @return 添付ファイル名、添付ファイルデータのいずれかが存在しない場合false
	 */
	@AssertTrue
	public boolean isValiedAttachedFileNameAndAttachedFileData() {

		//添付ファイル名、添付ファイルデータのいずれも存在しない場合
		if (!StringUtils.hasLength(attachedfileName) && !StringUtils.hasLength(attachedfileData)) {
			//添付ファイル名、添付ファイルデータのいずれも存在しない場合
			return true;
		}
		if (StringUtils.hasLength(attachedfileName) && StringUtils.hasLength(attachedfileData)) {
			//添付ファイル名、添付ファイルデータのいずれも存在する場合
			return true;
		}

		//添付ファイル名、添付ファイルデータのいずれかが存在しない場合
		return false;
	}

}
