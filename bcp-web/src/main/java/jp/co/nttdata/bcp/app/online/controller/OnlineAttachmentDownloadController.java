package jp.co.nttdata.bcp.app.online.controller;

import jakarta.inject.Inject;

import org.springframework.beans.BeanUtils;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import com.fasterxml.jackson.annotation.JsonProperty;

import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.online.form.OnlineAttachmentDownloadForm;
import jp.co.nttdata.bcp.domain.model.Attachedfiles;
import jp.co.nttdata.bcp.domain.repository.AttachedfilesMapper;
import lombok.Data;

/**
 * 添付ファイルダウンロード
 */
@RestController
@Transactional
public class OnlineAttachmentDownloadController {
	
	@Inject
	private AttachedfilesMapper attachedfilesMapper;

	/**
	 * 添付ファイルダウンロード.<br>
	 * お知らせに付随する添付ファイルをダウンロードする。
	 * 
	 * @param form 入力値
	 * @return 添付ファイル情報
	 */
	@PostMapping(path = "/sc11_download.do")
	public AttachedfileResponse attachmentDownload(@Validated @RequestBody OnlineAttachmentDownloadForm form) {

		// 添付ファイル管理から添付ファイルを取得
		Attachedfiles attachedfiles = attachedfilesMapper.selectByPrimaryKey(form.getKey());
		
		// 添付ファイルがない場合はエラー
		if (attachedfiles == null) {
			throw new BcpBuisinessException(404, "添付ファイルが存在しないためダウンロードできません。");
		}

		AttachedfileResponse response = new AttachedfileResponse();
		BeanUtils.copyProperties(attachedfiles, response);
		
		return response;
	}

	/**
	 * Attachedfilesエンティティで定義されている項目について、レスポンスに出力する項目を定義する.
	 */
	@Data
	class AttachedfileResponse {

	    @JsonProperty("attachedfileName")
	    private String fileName;


	    @JsonProperty("attachedfileData")
	    private String fileData;
	    
	}
}
