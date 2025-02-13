package jp.co.nttdata.bcp.domain.model.login;

import com.fasterxml.jackson.annotation.JsonIgnore;

import lombok.Data;

/**
 * ログインユーザのアカウント情報
 */
@Data
public class UserAccount {
	
	/** ベンダID */
	private String vendorId;
	
	/** 親ベンダID */
	private String parentId;
	
	/** ベンダ名 */
	private String vendorName;
	
	/** ユーザID */
	private String userId;
	
	/** ユーザ名 */
	private String userName;
	
	/** ユーザ種別 */
	private String userType;
	
	/** パスワード */
	@JsonIgnore
	private String password;

}
