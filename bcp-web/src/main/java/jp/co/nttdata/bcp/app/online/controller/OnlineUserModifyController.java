package jp.co.nttdata.bcp.app.online.controller;

import jakarta.inject.Inject;

import org.springframework.beans.BeanUtils;
import org.springframework.dao.DuplicateKeyException;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import jp.co.nttdata.bcp.app.common.code.ModeType;
import jp.co.nttdata.bcp.app.common.code.UserType;
import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.online.form.OnlineUserModifyForm;
import jp.co.nttdata.bcp.domain.model.Users;
import jp.co.nttdata.bcp.domain.model.Vendors;
import jp.co.nttdata.bcp.domain.model.login.BcpUserDetails;
import jp.co.nttdata.bcp.domain.model.login.UserAccount;
import jp.co.nttdata.bcp.domain.repository.UsersMapper;
import jp.co.nttdata.bcp.domain.repository.VendorsMapper;

/**
 * ユーザ編集
 */
@RestController
@Transactional
public class OnlineUserModifyController {

	@Inject
	private UsersMapper usersMapper;

	@Inject
	private VendorsMapper vendorssMapper;

	/**
	 * ユーザ編集.<br>
	 * 指定された登録済みのユーザ情報を変更・削除する。
	 * 
	 * @param form 入力値
	 */
	@PreAuthorize("hasAnyRole('SYSTEM_ADMIN', 'VENDOR_ADMIN')")
	@PostMapping("/sc13_modify.do")
	public void userModify(@Validated @RequestBody OnlineUserModifyForm form,
			@AuthenticationPrincipal BcpUserDetails userDetails) {

		UserAccount userAccount = userDetails.getUserAccount();

		// 更新の場合、入力値のベンダIDのチェックを行う
		if (ModeType.UPDATE.getCodeValue().equals(form.getMode())) {
			//ベンダ管理者の場合
			if (UserType.VENDOR_ADMIN.getCodeValue().equals(userAccount.getUserType())) {
				if (userAccount.getVendorId().equals(userAccount.getParentId())) {
					// 親ベンダの場合
					Vendors vendor = vendorssMapper.selectByVendorIdAndParentId(form.getVendorId(),
							userAccount.getParentId());

					if (vendor == null) {
						// 配下のベンダでない場合
						throw new BcpBuisinessException(403, "権限なし");
					}

				} else {
					// 代理店の場合
					if (!userAccount.getVendorId().equals(form.getVendorId())) {
						// 入力されたベンダIDとセッションのベンダIDが異なる場合
						throw new BcpBuisinessException(403, "権限なし");
					}
				}
			}
		}

		// 入力内容を移し替える
		Users user = new Users();
		BeanUtils.copyProperties(form, user);

		// ユーザ情報の楽観的ロック
		Users userRecord = usersMapper.selectByPrimaryKeyRecordLock(user.getKey());

		// ユーザ情報を取得できない場合
		if (userRecord == null) {
			throw new BcpBuisinessException(404, "該当のユーザは存在しません。");
		}
		// 取得した『更新回数』とform『更新回数』が不一致
		if (user.getVersion().intValue() != userRecord.getVersion().intValue()) {
			throw new BcpBuisinessException(412, "該当のユーザは他で更新されております。");
		}

		//ベンダ管理者の場合
		if (UserType.VENDOR_ADMIN.getCodeValue().equals(userAccount.getUserType())) {
			if (userAccount.getVendorId().equals(userAccount.getParentId())) {
				// 親ベンダの場合
				if (!userAccount.getParentId().equals(userRecord.getParentId())) {
					// 配下のベンダのユーザでない場合
					throw new BcpBuisinessException(403, "権限なし");
				}

			} else {
				// 代理店の場合
				if (!userAccount.getVendorId().equals(userRecord.getVendorId())) {
					// 入力されたベンダIDと取得したベンダIDが異なる場合
					throw new BcpBuisinessException(403, "権限なし");
				}
			}
		}

		if (form.getMode().equals(ModeType.UPDATE.getCodeValue())) {
			// 処理区分が「0:更新」の場合

			// ユーザ情報を更新
			try {
				if ("0000".equals(userRecord.getVendorId())) {
					// システム管理者の場合は、ユーザIDの更新は行わない
					user.setUserId(null);
				}
				user.setVersion(userRecord.getVersion() + 1);
				usersMapper.updateAllWithParentVendor(user);
			} catch (DuplicateKeyException e) {
				// メールアドレス（ユニーク項目）の重複エラーの場合
				throw new BcpBuisinessException(409, "既に使用されているメールアドレスです。", e);
			}
		} else if (form.getMode().equals(ModeType.DELETE.getCodeValue())) {
			// 処理区分が「1:削除」の場合
			// ユーザ情報を削除
			usersMapper.deleteByPrimaryKey(user.getKey());
		}
	}
}
