package jp.co.nttdata.bcp.app.online.controller;

import jakarta.inject.Inject;

import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import com.fasterxml.jackson.annotation.JsonView;

import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.online.form.OnlineInfoDetailForm;
import jp.co.nttdata.bcp.domain.model.info.InfoWithFiles;
import jp.co.nttdata.bcp.domain.model.info.InfoWithFiles.OnlineInfoDetail;
import jp.co.nttdata.bcp.domain.repository.InfosMapper;

/**
 * お知らせ詳細取得
 */
@RestController
@Transactional
public class OnlineInfoDetailController {

	@Inject
	private InfosMapper infosMapper;

	/**
	 * お知らせ詳細取得.<br>
	 * 指定された登録済みのお知らせ情報（添付ファイル含む）を取得する。
	 * @param form 入力値
	 */
	@PreAuthorize("hasAnyRole('SYSTEM_ADMIN')")
	@PostMapping("/sc12_detail.do")
	@JsonView(OnlineInfoDetail.class)
	public InfoWithFiles infoDetail(@Validated @RequestBody OnlineInfoDetailForm form) {

		// お知らせ詳細取得
		InfoWithFiles info = infosMapper.selectByKeyWithFiles(form.getKey());

		if (info == null) {
			throw new BcpBuisinessException(404, "該当のお知らせは存在しません。");
		}

		return info;
	}

}
