package jp.co.nttdata.bcp.app.online.form;

import jakarta.validation.constraints.NotNull;

import lombok.Data;

/**
 * お知らせ編集用フォーム
 */
@Data
public class OnlineLinkListForm {
	
	/** お知らせキー */
	@NotNull
	private Long key; 

}
