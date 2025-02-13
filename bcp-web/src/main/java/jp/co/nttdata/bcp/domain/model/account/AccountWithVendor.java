package jp.co.nttdata.bcp.domain.model.account;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.JsonView;

import lombok.Data;

/**
 * アカウント情報（ベンダ情報付き）
 */
@Data
public class AccountWithVendor {

	/** WebAPIのアカウント情報取得でレスポンスに出力する項目定義用インタフェース */
	public interface ApiDetail {
	}

	/** オンラインのアカウント情報取得でレスポンスに出力する項目定義用インタフェース */
	public interface OnlineDetail {
	}

	@JsonView({ApiDetail.class,OnlineDetail.class})
	private String accountId;

	@JsonView({OnlineDetail.class})
	private String accountName;

	@JsonView({OnlineDetail.class})
	private String accountAddress;

	@JsonView({OnlineDetail.class})
	private String vendorId;

	private String parentId;

	@JsonView({ApiDetail.class,OnlineDetail.class})
	private Boolean uploadEnable;

	@JsonView({ApiDetail.class,OnlineDetail.class})
	private Integer uploadTiming;

	@JsonView({ApiDetail.class,OnlineDetail.class})
	private Integer backupCapa;

	@JsonView({ApiDetail.class,OnlineDetail.class})
	private Integer usedSize;

	@JsonView({ApiDetail.class,OnlineDetail.class})
	private Boolean downloadEnable;

	@JsonView({ApiDetail.class,OnlineDetail.class})
	@JsonFormat(pattern = "yyyy/MM/dd HH:mm:ss", timezone = "Asia/Tokyo")
	private Date lastdate;

	@JsonView({ApiDetail.class,OnlineDetail.class})
	private String status;

	@JsonView({ApiDetail.class,OnlineDetail.class})
	private String errorInfo;

	@JsonView({OnlineDetail.class})
	private String softVersion;

	@JsonView({OnlineDetail.class})
	private String localDir;

	private Boolean mailEnable;

	@JsonView({ApiDetail.class,OnlineDetail.class})
	private Integer version;

	private Date updateDate;

	private String vendorName;

	private String mailAddress;

	@JsonView({ApiDetail.class,OnlineDetail.class})
	private String s3backet;

	private String cognitoUserId;

	private String cognitoAccessToken;

	@JsonView(ApiDetail.class)
	private String permitPassword;

	private Date vendorUpdateDate;
	
	private String disable_flg;

	private String authKey;

	@JsonView(ApiDetail.class)
	private String s3Endpoint;

}
