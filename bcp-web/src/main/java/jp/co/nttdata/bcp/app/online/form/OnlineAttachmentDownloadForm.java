package jp.co.nttdata.bcp.app.online.form;

import jakarta.validation.constraints.NotNull;

import org.hibernate.validator.constraints.Range;

import lombok.Data;

/**
 * 添付ファイルダウンロード用入力フォーム
 */
@Data
public class OnlineAttachmentDownloadForm {

	/** 添付ファイルキー */
	@Range(min = 0)
	@NotNull
	private Long key; 

}
