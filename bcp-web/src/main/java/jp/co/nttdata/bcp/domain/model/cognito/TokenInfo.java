package jp.co.nttdata.bcp.domain.model.cognito;

import lombok.Data;

/**
 * Cognito認証で取得したトークン情報
 */
@Data
public class TokenInfo {

	/** アカウント名 */
	private String accountName;

	/** アクセスキーID */
	private String accessKeyId;

	/** シークレットアクセスキー */
	private String secretAccessKey;

	/** セッショントークン */
	private String sessionToken;
	
	/** 同期ステータス */
	private String status;

}
