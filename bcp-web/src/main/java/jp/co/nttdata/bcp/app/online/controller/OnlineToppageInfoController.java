package jp.co.nttdata.bcp.app.online.controller;

import java.util.ArrayList;
import java.util.Date;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import jakarta.inject.Inject;

import org.springframework.beans.BeanUtils;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RestController;

import com.fasterxml.jackson.annotation.JsonFormat;
import com.fasterxml.jackson.annotation.JsonProperty;

import jp.co.nttdata.bcp.domain.model.account.AccountsSummary;
import jp.co.nttdata.bcp.domain.model.info.InfoWithFiles;
import jp.co.nttdata.bcp.domain.model.login.BcpUserDetails;
import jp.co.nttdata.bcp.domain.model.login.UserAccount;
import jp.co.nttdata.bcp.domain.repository.AccountsMapper;
import jp.co.nttdata.bcp.domain.repository.InfosMapper;
import lombok.Data;

/**
 * トップページ情報取得
 */
@RestController
@Transactional
public class OnlineToppageInfoController {

	@Inject
	private InfosMapper infosMapper;

	@Inject
	private AccountsMapper accountsMapper;

	/**
	 * トップページ情報取得.<br>
	 * トップページに表示するお知らせやアカウント状況サマリーを取得する。
	 * 
	 * @param bcpUserDetails セッション情報
	 * @return トップページ情報
	 */
	@PostMapping(path = "/sc11.do")
	public Map<String, Object> toppageInfo(@AuthenticationPrincipal BcpUserDetails bcpUserDetails) {
		
		// 入力内容を移し替える
		UserAccount userAccount = new UserAccount();
		BeanUtils.copyProperties(bcpUserDetails.getUserAccount(), userAccount);
		
		// お知らせ管理からお知らせ情報を取得
		List<InfoWithFiles> infoWithFiles = infosMapper.selectInfoToppageList();
		
		// アカウント管理からアカウント情報を取得
		AccountsSummary accountWithVendors = accountsMapper.selectAccountsSummary(userAccount);
		
		// 項目の移し替え
		List<InfoResponse> infoResponses = new ArrayList<>();
		infoWithFiles.forEach(e -> {
			InfoResponse infoRes = new InfoResponse();
			BeanUtils.copyProperties(e, infoRes);
			// filesの移し替えも実施
			List<FilesResponse> filesResponses = new ArrayList<>();
			e.getFiles().forEach(f -> {
				FilesResponse fileRes = new FilesResponse();
				BeanUtils.copyProperties(f, fileRes);
				filesResponses.add(fileRes);
			});
			infoRes.setFiles(filesResponses);
			infoResponses.add(infoRes);
		});
		
		// 結果をレスポンスに設定
		Map<String, Object> response = new LinkedHashMap<String, Object>();
		response.put("infos", infoResponses);
		response.put("accountsSummary", accountWithVendors);
		
		return response;
	}

	/**
	 * レスポンスのinfosへの出力項目定義
	 */
	@Data
	class InfoResponse {
	    
		private Long key;

		@JsonFormat(pattern = "yyyy/MM/dd", timezone = "Asia/Tokyo")
		private Date startDate;

		@JsonFormat(pattern = "yyyy/MM/dd", timezone = "Asia/Tokyo")
		private Date endDate;

		private String title;

		private String article;

		private String userId;

		private List<FilesResponse> files;
	}

	/**
	 * レスポンスのfilesへの出力項目定義
	 */
	@Data
	class FilesResponse {
		
		private Long key;
		
		@JsonProperty("attachedfileName")
		private String fileName;
	}
}
