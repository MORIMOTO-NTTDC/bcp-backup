package jp.co.nttdata.bcp.domain.model.info;

import java.util.Date;
import java.util.List;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.JsonView;

import lombok.Data;

/**
 * お知らせ情報（添付フィル付き、ユーザ名つき）
 */
@Data
public class InfoWithFiles {

	/** オンラインのトップページ情報取得でレスポンスに出力する項目定義用インタフェース */
	public interface OnlineTop {
	}

	/** オンラインのお知らせ一覧取得でレスポンスに出力する項目定義用インタフェース */
	public interface OnlineInfoList {
	}

	/** オンラインのお知らせ取得でレスポンスに出力する項目定義用インタフェース */
	public interface OnlineInfoDetail {
	}

	@JsonView({ OnlineTop.class, OnlineInfoList.class, OnlineInfoDetail.class })
	private Long key;

	@JsonView({ OnlineTop.class, OnlineInfoList.class, OnlineInfoDetail.class })
	@JsonFormat(pattern = "yyyy/MM/dd", timezone = "Asia/Tokyo")
	private Date startDate;

	@JsonView({ OnlineTop.class, OnlineInfoList.class, OnlineInfoDetail.class })
	@JsonFormat(pattern = "yyyy/MM/dd", timezone = "Asia/Tokyo")
	private Date endDate;

	@JsonView({ OnlineTop.class, OnlineInfoList.class, OnlineInfoDetail.class })
	private String title;

	@JsonView({ OnlineTop.class, OnlineInfoList.class, OnlineInfoDetail.class })
	private String article;

	@JsonView({ OnlineTop.class, OnlineInfoList.class, OnlineInfoDetail.class })
	private String userId;

	@JsonView({ OnlineInfoList.class, OnlineInfoDetail.class })
	private String userName;

	@JsonView({ OnlineInfoDetail.class })
	private Integer version;

	private Date updateDate;

	@JsonView({ OnlineTop.class, OnlineInfoList.class, OnlineInfoDetail.class })
	private List<FileInInfo> files;

}
