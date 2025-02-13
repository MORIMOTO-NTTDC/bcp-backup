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

import jp.co.nttdata.bcp.app.common.code.ModeType;
import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.online.form.OnlineInfoModifyForm;
import jp.co.nttdata.bcp.app.online.form.info.AtacchedFileForm;
import jp.co.nttdata.bcp.domain.model.Attachedfiles;
import jp.co.nttdata.bcp.domain.model.Infos;
import jp.co.nttdata.bcp.domain.model.login.BcpUserDetails;
import jp.co.nttdata.bcp.domain.repository.AttachedfilesMapper;
import jp.co.nttdata.bcp.domain.repository.InfosMapper;

/**
 * お知らせ編集
 */
@RestController
@Transactional
public class OnlineInfoModifyController {

	@Inject
	private InfosMapper infosMapper;

	@Inject
	private AttachedfilesMapper attachedfilesMapper;

	/**
	 * お知らせ編集.<br>
	 * 指定された登録済みのお知らせ情報（添付ファイル含む）を変更・削除する。
	 * 
	 * @param form 入力値
	 */
	@PreAuthorize("hasAnyRole('SYSTEM_ADMIN')")
	@PostMapping("/sc12_modify.do")
	public void infoModify(@Validated @RequestBody OnlineInfoModifyForm form,
			@AuthenticationPrincipal BcpUserDetails userDetails) {
		
		// 入力内容を移し替える
		Infos info = new Infos();
		BeanUtils.copyProperties(form, info);

		// ログインユーザのユーザIDを設定する
		info.setUserId(userDetails.getUsername());
		
		// 【お知らせ管理】の楽観的ロックを確認する。
		Infos infoRecord = infosMapper.selectByPrimaryKeyRecordLock(info.getKey());
		
		// お知らせ管理を取得できない場合
		if (infoRecord == null) {
			throw new BcpBuisinessException(404, "該当のお知らせは存在しません。");
		}
		// 取得した『更新回数』とform『更新回数』が不一致
		if (info.getVersion().intValue() != infoRecord.getVersion().intValue()) {
			throw new BcpBuisinessException(412, "該当のお知らせは他で更新されております。");
		}
		
		if (form.getMode().equals(ModeType.UPDATE.getCodeValue())) {
			// 処理区分が「0:更新」の場合
			
			// 更新回数+1
			info.setVersion(infoRecord.getVersion()+1);
			// お知らせ管理を更新
			infosMapper.updateByPrimaryKey(info);
			
			//添付ファイル名が存在しない場合
			if (form.getFiles() == null || form.getFiles().size() == 0
					|| !StringUtils.hasText(form.getFiles().get(0).getAttachedfileName())) {
				return;
			}

			// 添付ファイル管理を削除
			attachedfilesMapper.deleteByInfoKey(form.getKey());

			//添付ファイルリストの件数分【添付ファイル管理】を登録
			for (AtacchedFileForm file : form.getFiles()) {
				Attachedfiles attachedfile = new Attachedfiles();
				attachedfile.setInfoKey(info.getKey());
				// 入力内容を移し替える
				attachedfile.setFileName(file.getAttachedfileName());
				attachedfile.setFileData(file.getAttachedfileData());
				// 添付ファイル登録
				attachedfilesMapper.insertSelective(attachedfile);
			}
		} else if (form.getMode().equals(ModeType.DELETE.getCodeValue())) {
			// 処理区分が「1:削除」の場合
			// 添付ファイル管理を削除
			attachedfilesMapper.deleteByInfoKey(form.getKey());
			// お知らせ管理を削除
			infosMapper.deleteByPrimaryKey(form.getKey());
		}
	}
}
