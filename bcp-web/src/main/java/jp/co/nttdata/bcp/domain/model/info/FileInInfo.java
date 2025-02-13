package jp.co.nttdata.bcp.domain.model.info;

import java.util.Date;

import com.fasterxml.jackson.annotation.JsonProperty;
import com.fasterxml.jackson.annotation.JsonView;

import jp.co.nttdata.bcp.domain.model.info.InfoWithFiles.OnlineInfoDetail;
import jp.co.nttdata.bcp.domain.model.info.InfoWithFiles.OnlineInfoList;
import jp.co.nttdata.bcp.domain.model.info.InfoWithFiles.OnlineTop;
import lombok.Data;

/**
 * お知らせ情報の中に含むための添付ファイル情報
 */
@Data
public class FileInInfo {

	@JsonView({ OnlineTop.class, OnlineInfoList.class, OnlineInfoDetail.class })
	private Long key;

	private Long infoKey;

	@JsonView({ OnlineTop.class, OnlineInfoDetail.class })
	@JsonProperty("attachedfileName")
	private String fileName;

	private String fileData;

	private Date updateDate;

}
