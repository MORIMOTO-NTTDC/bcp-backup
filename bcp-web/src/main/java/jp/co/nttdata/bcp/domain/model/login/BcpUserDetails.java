package jp.co.nttdata.bcp.domain.model.login;

import java.util.Collection;

import org.springframework.security.core.CredentialsContainer;
import org.springframework.security.core.GrantedAuthority;
import org.springframework.security.core.userdetails.UserDetails;

import lombok.Getter;
import lombok.Setter;

public class BcpUserDetails implements UserDetails, CredentialsContainer {

	/** ログインユーザのアカウント情報 */
	@Setter
	@Getter
	private UserAccount userAccount;

	/** 権限 */
	private final Collection<GrantedAuthority> authorities;

	/**
	 * コンストラクタ
	 * @param userAccountログインユーザのアカウント情報
	 * @param authorities 権限
	 */
	public BcpUserDetails(UserAccount userAccount, Collection<GrantedAuthority> authorities) {
		this.userAccount = userAccount;
		this.authorities = authorities;
	}

	/**
	 * {@inheritDoc}
	 */
	@Override
	public void eraseCredentials() {
		userAccount.setPassword(null);
	}

	/**
	 * {@inheritDoc}
	 */
	@Override
	public Collection<? extends GrantedAuthority> getAuthorities() {
		return authorities;
	}

	/**
	 * {@inheritDoc}
	 */
	@Override
	public String getPassword() {
		return userAccount.getPassword();
	}

	/**
	 * {@inheritDoc}
	 */
	@Override
	public String getUsername() {
		return userAccount.getUserId();
	}

	/**
	 * {@inheritDoc}
	 */
	@Override
	public boolean isAccountNonExpired() {
		return true;
	}

	/**
	 * {@inheritDoc}
	 */
	@Override
	public boolean isAccountNonLocked() {
		return true;
	}

	/**
	 * {@inheritDoc}
	 */
	@Override
	public boolean isCredentialsNonExpired() {
		return true;
	}

	/**
	 * {@inheritDoc}
	 */
	@Override
	public boolean isEnabled() {
		return true;
	}

}
