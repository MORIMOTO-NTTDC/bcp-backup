package jp.co.nttdata.bcp.app.online.controller;

import java.util.ArrayList;
import java.util.Date;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import jakarta.inject.Inject;

import org.springframework.beans.BeanUtils;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RestController;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.core.JsonProcessingException;

import jp.co.nttdata.bcp.domain.model.Links;
import jp.co.nttdata.bcp.domain.repository.LinksMapper;
import lombok.Data;

/**
 * 外部リンク一覧取得
 */
@RestController
@Transactional
public class OnlineLinkListController {

	@Inject
	private LinksMapper linksMapper;

	/**
	 * 外部リンク一覧取得.<br>
	 * 登録済みの外部リンク情報を一覧取得する。
	 * @param form 入力値
	 * @throws JsonProcessingException 
	 */
	@PostMapping(path = "/sc20_list.do")
	public Map<String, Object> linkList() throws JsonProcessingException {

		List<Links> links = linksMapper.selectAll();

		// 項目の移し替え
		List<LinkResponse> linkList = new ArrayList<OnlineLinkListController.LinkResponse>();
		links.forEach(link -> {
			LinkResponse linkResponse = new LinkResponse();
			BeanUtils.copyProperties(link, linkResponse);
			linkList.add(linkResponse);
		});

		//結果をレスポンスに設定
		Map<String, Object> response = new LinkedHashMap<String, Object>();
		response.put("links", linkList);

		return response;
	}

	/**
	 * レスポンスのLinksへの出力項目定義
	 */
	@Data
	class LinkResponse {
		private String title;

		private String linkText;

		private String url;

		@JsonFormat(pattern = "yyyy/MM/dd", timezone = "Asia/Tokyo")
		private Date updateDate;
	}

}
