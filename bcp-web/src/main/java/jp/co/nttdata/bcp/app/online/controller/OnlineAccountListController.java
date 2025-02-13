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
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import com.fasterxml.jackson.annotation.JsonFormat;

import jp.co.nttdata.bcp.app.online.form.OnlineAccountListForm;
import jp.co.nttdata.bcp.domain.model.Accounts;
import jp.co.nttdata.bcp.domain.model.Vendors;
import jp.co.nttdata.bcp.domain.model.account.AccountSearchList;
import jp.co.nttdata.bcp.domain.model.login.BcpUserDetails;
import jp.co.nttdata.bcp.domain.model.login.UserAccount;
import jp.co.nttdata.bcp.domain.repository.AccountsMapper;
import jp.co.nttdata.bcp.domain.repository.VendorsMapper;
import lombok.Data;

/**
 * アカウント一覧取得
 */
@RestController
@Transactional
public class OnlineAccountListController {

	@Inject
	private AccountsMapper accountsMapper;

	@Inject
	private VendorsMapper vendorsMapper;

	/**
	 * アカウント詳細取得.<br>
	 * 指定された登録済みのアカウント情報を取得する。
	 * 
	 * @param form 入力値
	 * @param bcpUserDetails セッション情報
	 * @return アカウント設定情報
	 */
	@PostMapping(path = "/sc14_list.do")
	public Map<String, Object> accountList(@Validated @RequestBody OnlineAccountListForm form,
			@AuthenticationPrincipal BcpUserDetails bcpUserDetails) {

		// 入力内容を移し替える
		AccountSearchList account = new AccountSearchList();
		UserAccount userAccount = new UserAccount();
		BeanUtils.copyProperties(form, account);
		BeanUtils.copyProperties(bcpUserDetails.getUserAccount(), userAccount);

		// アカウント管理からアカウント一覧を取得
		Long accountsCnt = accountsMapper.countListByOffsetLimitWithAccounts(account, userAccount);
		List<Accounts> accounts = new ArrayList<>();
		if (accountsCnt != 0) {
			accounts = accountsMapper.selectListByOffsetLimitWithAccounts(account, userAccount, form.getOffset(),
					form.getLimit());
		}

		// ベンダ管理からベンダ一覧を取得
		List<Vendors> vendors = vendorsMapper.selectListVendorWithParentS3(userAccount);

		// 項目の移し替え
		List<AccountResponse> accountResponses = new ArrayList<>();
		accounts.forEach(e -> {
			AccountResponse accountRes = new AccountResponse();
			BeanUtils.copyProperties(e, accountRes);
			accountResponses.add(accountRes);
		});
		List<VendorResponse> vendorResponses = new ArrayList<>();
		vendors.forEach(e -> {
			VendorResponse vendorRes = new VendorResponse();
			BeanUtils.copyProperties(e, vendorRes);
			vendorResponses.add(vendorRes);
		});

		// 結果をレスポンスに設定
		Map<String, Object> response = new LinkedHashMap<>();
		response.put("vendors", vendorResponses);
		response.put("accounts", accountResponses);
		response.put("continue", (form.getOffset() + accounts.size()) < accountsCnt);

		return response;
	}

	/**
	 * レスポンスのAccountsへの出力項目定義
	 */
	@Data
	class AccountResponse {

		private String accountId;

		private String accountName;

		private String accountAddress;

		private String vendorId;

		private Boolean uploadEnable;

		private Integer uploadTiming;

		private Integer backupCapa;

		private Integer usedSize;

		private Boolean downloadEnable;

		@JsonFormat(pattern = "yyyy/MM/dd HH:mm:ss", timezone = "Asia/Tokyo")
		private Date lastdate;

		private String status;

		private String errorInfo;

		private String softVersion;
	}

	/**
	 * レスポンスのvendorsへの出力項目定義
	 */
	@Data
	class VendorResponse {

		private String id;

		private String name;

		private String s3backet;
	}
}
