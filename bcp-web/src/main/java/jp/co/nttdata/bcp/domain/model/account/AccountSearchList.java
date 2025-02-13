package jp.co.nttdata.bcp.domain.model.account;

import lombok.Data;

/**
 * アカウント一覧取得用入力値
 */
@Data
public class AccountSearchList {
	
	/** ベンダID（絞り込条件） */
	private String vendorId;
	
	/** アカウントID（絞り込条件：部分一致） */
	private String accountId;
	
	/** アカウント名（絞り込条件：部分一致） */
	private String accountName;
	
	/** アカウント住所（絞り込条件：部分一致） */
	private String accountAddress;
	
	/** 同期失敗（'1'）（絞り込条件） */
	private String syncError;
	
	/** 使用領域90％以上（'1'）（絞り込条件） */
	private String diskAlert;
	
	/** 旧バージョン利用（'1'）（絞り込条件） */
	private String versionAlert;
}
