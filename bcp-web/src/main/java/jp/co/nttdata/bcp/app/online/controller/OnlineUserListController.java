package jp.co.nttdata.bcp.app.online.controller;

import java.util.ArrayList;
import java.util.Date;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

import jakarta.inject.Inject;

import org.apache.ibatis.session.RowBounds;
import org.springframework.beans.BeanUtils;
import org.springframework.security.access.prepost.PreAuthorize;
import org.springframework.security.core.annotation.AuthenticationPrincipal;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import com.fasterxml.jackson.annotation.JsonFormat;

import jp.co.nttdata.bcp.app.online.form.OnlineUserListForm;
import jp.co.nttdata.bcp.domain.model.Users;
import jp.co.nttdata.bcp.domain.model.Vendors;
import jp.co.nttdata.bcp.domain.model.login.BcpUserDetails;
import jp.co.nttdata.bcp.domain.model.login.UserAccount;
import jp.co.nttdata.bcp.domain.model.user.UserWithVendor;
import jp.co.nttdata.bcp.domain.repository.UsersMapper;
import jp.co.nttdata.bcp.domain.repository.VendorsMapper;
import lombok.Data;

/**
 * ユーザ一覧取得
 */
@RestController
@Transactional
public class OnlineUserListController {

	@Inject
	private UsersMapper usersMapper;

	@Inject
	private VendorsMapper vendorsMapper;

	/**
	 * ユーザ一覧取得.<br>
	 * 登録済みユーザ情報を一覧取得する。
	 * 
	 * @param form 入力値
	 * @param bcpUserDetails セッション情報
	 * @return ユーザ情報
	 */
	@PreAuthorize("hasAnyRole('SYSTEM_ADMIN', 'VENDOR_ADMIN')")
	@PostMapping(path = "/sc13_list.do")
	public Map<String, Object> userList(@Validated @RequestBody OnlineUserListForm form,
			@AuthenticationPrincipal BcpUserDetails bcpUserDetails) {
		
		// 入力内容を移し替える
		Users user = new Users();
		UserAccount userAccount = new UserAccount();
		BeanUtils.copyProperties(form, user);
		BeanUtils.copyProperties(bcpUserDetails.getUserAccount(), userAccount);
		
		// ユーザ管理からユーザ一覧を取得
		Long usersCnt = usersMapper.countListByOffsetLimitWithUsers(user, userAccount);
		List <UserWithVendor> users = new ArrayList<UserWithVendor>();
		if (usersCnt != 0) {
			users = usersMapper.selectListByOffsetLimitWithUsers(user, userAccount,
					new RowBounds(form.getOffset(), form.getLimit()));
		}
		// ベンダ管理からベンダ一覧を取得
		List<Vendors> vendors = vendorsMapper.selectListVendor(userAccount);

		// 項目の移し替え
		List<UserResponse> userResponses = new ArrayList<>();
		users.forEach(e -> {
			UserResponse userRes = new UserResponse();
			BeanUtils.copyProperties(e, userRes);
			userResponses.add(userRes);
		});
		List<VendorResponse> vendorResponses = new ArrayList<>();
		vendors.forEach(e -> {
			VendorResponse vendorRes = new VendorResponse();
			BeanUtils.copyProperties(e, vendorRes);
			vendorResponses.add(vendorRes);
		});
		
		// 結果をレスポンスに設定
		Map<String, Object> response = new LinkedHashMap<String, Object>();
		response.put("vendors", vendorResponses);
		response.put("users", userResponses);
		response.put("continue", (form.getOffset() + users.size()) < usersCnt);

		return response;
	}

	/**
	 * レスポンスのusersへの出力項目定義
	 */
	@Data
	class UserResponse {
		
		private Long key;

		private String userId;

		private String userName;

		private String vendorId;

		private String mailAddress;

		private Boolean admin;

		@JsonFormat(pattern = "yyyy/MM/dd HH:mm:ss", timezone = "Asia/Tokyo")
		private Date updateDate;

		private String vendorName;

	}

	/**
	 * レスポンスのvendorsへの出力項目定義
	 */
	@Data
	class VendorResponse {

		private String id;

		private String name;
	}
}
