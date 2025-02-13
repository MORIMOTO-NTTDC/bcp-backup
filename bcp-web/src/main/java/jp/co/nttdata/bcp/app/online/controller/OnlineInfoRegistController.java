package jp.co.nttdata.bcp.app.online.controller;

import jakarta.inject.Inject;

import org.springframework.beans.BeanUtils;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.util.StringUtils;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import jp.co.nttdata.bcp.app.online.form.OnlineInfoRegisForm;
import jp.co.nttdata.bcp.domain.model.Attachedfiles;
import jp.co.nttdata.bcp.domain.model.Infos;
import jp.co.nttdata.bcp.domain.model.login.BcpUserDetails;
import jp.co.nttdata.bcp.domain.repository.AttachedfilesMapper;
import jp.co.nttdata.bcp.domain.repository.InfosMapper;

/**
 * お知らせ登録
 */
@RestController
@Transactional
public class OnlineInfoRegistController {

	@Inject
	private InfosMapper infosMapper;

	@Inject
	private AttachedfilesMapper attachedfilesMapper;

	/**
	 * お知らせ登録.<br>
	 * お知らせ情報（添付ファイル含む）を新規登録する。
	 * @param form 入力値
	 * @param userDetails 認証情報
	 */
	@PreAuthorize("hasAnyRole('SYSTEM_ADMIN')")
	@PostMapping("/sc12_regist.do")
	public void passwordUpdate(@Validated @RequestBody OnlineInfoRegisForm form,
			@AuthenticationPrincipal BcpUserDetails userDetails) {

		// 入力内容を移し替える
		Infos info = new Infos();
		BeanUtils.copyProperties(form, info);

		// ログインユーザのユーザIDを設定する
		info.setUserId(userDetails.getUsername());

		// お知らせ登録
		Long _key = infosMapper.insertSelectiveAndGetKey(info);

		//添付ファイル名が存在しない場合
		if (form.getFiles() == null || form.getFiles().size() == 0
				|| !StringUtils.hasText(form.getFiles().get(0).getAttachedfileName())) {

			return;
		}

		//添付ファイルリストの件数分【添付ファイル管理】を登録
		form.getFiles().forEach(file -> {
			Attachedfiles attachedfile = new Attachedfiles();
			attachedfile.setInfoKey(_key);
			// 入力内容を移し替える
			attachedfile.setFileName(file.getAttachedfileName());
			attachedfile.setFileData(file.getAttachedfileData());
			// 添付ファイル登録
			attachedfilesMapper.insertSelective(attachedfile);
		});
	}

}
