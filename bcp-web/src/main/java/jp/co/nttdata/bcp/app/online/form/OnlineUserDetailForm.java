package jp.co.nttdata.bcp.app.online.form;

import jakarta.validation.constraints.NotNull;

import org.hibernate.validator.constraints.Range;

import lombok.Data;

/**
 * ユーザ詳細取得用入力フォーム
 */
@Data
public class OnlineUserDetailForm {

	/** ユーザキー */
	@NotNull
	@Range(min = 0)
	private Long key;

}
