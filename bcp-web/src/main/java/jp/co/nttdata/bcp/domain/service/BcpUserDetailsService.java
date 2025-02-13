package jp.co.nttdata.bcp.domain.service;

import java.util.Collection;

import jakarta.inject.Inject;

import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.authority.AuthorityUtils;
import org.springframework.security.core.userdetails.UserDetails;
import org.springframework.security.core.userdetails.UserDetailsPasswordService;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.core.userdetails.UsernameNotFoundException;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import jp.co.nttdata.bcp.app.common.code.UserType;
import jp.co.nttdata.bcp.domain.model.Users;
import jp.co.nttdata.bcp.domain.model.login.BcpUserDetails;
import jp.co.nttdata.bcp.domain.model.login.UserAccount;
import jp.co.nttdata.bcp.domain.repository.UsersMapper;

/**
 * UserDetailsServiceの実装クラス
 */
@Transactional
@Service
public class BcpUserDetailsService implements UserDetailsService, UserDetailsPasswordService {

	@Inject
	private UsersMapper usersMapper;

	/**
	 * 古いハッシュ方式の場合、新しいハッシュ方式のパスワードに更新する。<br>
	 * 当処理はフレームワークによって呼び出される。
	 * {@inheritDoc}
	 */
	@Override
	public UserDetails updatePassword(UserDetails userDetails, String newPassword) {
		Users user = new Users();
		user.setUserId(userDetails.getUsername());
		user.setPassword(newPassword);

		usersMapper.updatePasswordByUserId(user);

		return userDetails;
	}

	/**
	 * ログインユーザ情報を取得する。<br>
	 * 当処理はフレームワークによって呼び出される。
	 * {@inheritDoc}
	 */
	@Override
	public UserDetails loadUserByUsername(String username) throws UsernameNotFoundException {

		UserAccount userAccount = usersMapper.selectByUserIdOrEmail(username);
		if (userAccount == null) {
			throw new UsernameNotFoundException(username + "に該当するユーザがいない。");
		}

		return new BcpUserDetails(userAccount, getAuthorities(userAccount));
	}

	/**
	 * 権限情報を取得
	 * @param userAccount ログインユーザのアカウント情報
	 * @return 権限情報
	 */
	private Collection<GrantedAuthority> getAuthorities(UserAccount userAccount) {

		if (UserType.SYSTEM_ADMIN.getCodeValue().equals(userAccount.getUserType())) {
			// システム管理者
			return AuthorityUtils.createAuthorityList("ROLE_SYSTEM_ADMIN");
		} else if (UserType.VENDOR_ADMIN.getCodeValue().equals(userAccount.getUserType())) {
			// ベンダ管理者
			return AuthorityUtils.createAuthorityList("ROLE_VENDOR_ADMIN");
		} else {
			return AuthorityUtils.createAuthorityList("ROLE_VENDOR");

		}
	}

}
