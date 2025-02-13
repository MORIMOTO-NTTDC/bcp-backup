package jp.co.nttdata.bcp.app.online.form;

import jakarta.validation.constraints.NotNull;

import org.hibernate.validator.constraints.Range;

import lombok.Data;

@Data
public class OnlineInfoDetailForm {
	
	/** お知らせキー */
	@NotNull
	@Range(min = 0)
	private Long key; 

}
