package jp.co.nttdata.bcp.domain.model.mail;

import java.util.List;

import lombok.Data;

/**
 * メール送信のための各種情報を保持するクラス
 */
@Data
public class MailInfo {

	/** メールの件名 */
	private String subject;

	/** 送信先 */
	private List<String> tos;

	/** Cc(複数) */
	private List<String> ccs;

	/** BCc(複数) */
	private List<String> bccs;

	/** メール本文 */
	private String body;
}
