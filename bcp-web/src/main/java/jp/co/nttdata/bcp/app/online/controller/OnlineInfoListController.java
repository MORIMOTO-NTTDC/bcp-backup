package jp.co.nttdata.bcp.app.online.controller;

import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import jakarta.inject.Inject;

import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import com.fasterxml.jackson.annotation.JsonView;

import jp.co.nttdata.bcp.app.online.form.OnlineInfoListForm;
import jp.co.nttdata.bcp.domain.model.info.InfoWithFiles;
import jp.co.nttdata.bcp.domain.model.info.InfoWithFiles.OnlineInfoList;
import jp.co.nttdata.bcp.domain.repository.InfosMapper;

/**
 * お知らせ一覧取得
 */
@RestController
@Transactional
public class OnlineInfoListController {

	@Inject
	private InfosMapper infosMapper;

	/**
	 * お知らせ一覧取得.<br>
	 * 登録済みのお知らせ情報を一覧取得する。
	 * @param form 入力値
	 */
	@PreAuthorize("hasAnyRole('SYSTEM_ADMIN')")
	@PostMapping("/sc12_list.do")
	@JsonView(OnlineInfoList.class)
	public Map<String, Object> infoList(@Validated @RequestBody OnlineInfoListForm form) {

		// お知らせ総件数取得
		int count = infosMapper.selectCount();

		// お知らせ一覧取得
		List<InfoWithFiles> infos = infosMapper.selectListByOffsetLimitWithFiles(form.getOffset(), form.getLimit());

		//結果をレスポンスに設定
		Map<String, Object> response = new LinkedHashMap<String, Object>();
		response.put("infos", infos);
		response.put("continue", (form.getOffset() + infos.size()) < count);

		return response;
	}

}
