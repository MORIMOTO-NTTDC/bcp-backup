package jp.co.nttdata.bcp.domain.model.user;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.JsonView;

import lombok.Data;

/**
 * ユーザ情報（ベンダ情報付き）
 */
@Data
public class UserWithVendor {

	/** ユーザ情報取得でレスポンスに出力する項目定義用インタフェース */
	public interface UserDetail {

	}

	@JsonView({UserDetail.class})
	private Long key;

	@JsonView({UserDetail.class})
	private String userId;

	@JsonView({UserDetail.class})
	private String userName;

	@JsonView({UserDetail.class})
	private String vendorId;

	private String parentId;

	private String password;

	@JsonView({UserDetail.class})
	private String mailAddress;

	@JsonView({UserDetail.class})
	private Boolean admin;

	@JsonView({UserDetail.class})
	@JsonFormat(pattern = "yyyy/MM/dd HH:mm:ss", timezone = "Asia/Tokyo")
	private Date updateDate;

	@JsonView({UserDetail.class})
	private Integer version;

	@JsonView({UserDetail.class})
	private String vendorName;

	private String vendorMailAddress;

	private String s3backet;

	private String cognitoUserId;

	private String cognitoAccessToken;

	private String permitPassword;

	@JsonFormat(pattern = "yyyy/MM/dd HH:mm:ss", timezone = "Asia/Tokyo")
	private Date vendorUpdateDate;

}
