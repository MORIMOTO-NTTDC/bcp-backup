package jp.co.nttdata.bcp.config.web;

import java.util.LinkedHashMap;

import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.core.annotation.Order;
import org.springframework.security.access.AccessDeniedException;
import org.springframework.security.authentication.AuthenticationManager;
import org.springframework.security.authentication.AuthenticationProvider;
import org.springframework.security.authentication.dao.DaoAuthenticationProvider;
import org.springframework.security.config.Customizer;
import org.springframework.security.config.annotation.authentication.configuration.AuthenticationConfiguration;
import org.springframework.security.config.annotation.method.configuration.EnableMethodSecurity;
import org.springframework.security.config.annotation.web.builders.HttpSecurity;
import org.springframework.security.config.annotation.web.configuration.EnableWebSecurity;
import org.springframework.security.config.annotation.web.configuration.WebSecurityCustomizer;
import org.springframework.security.config.http.SessionCreationPolicy;
import org.springframework.security.core.userdetails.UserDetailsPasswordService;
import org.springframework.security.core.userdetails.UserDetailsService;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.security.web.SecurityFilterChain;
import org.springframework.security.web.access.AccessDeniedHandler;
import org.springframework.security.web.access.AccessDeniedHandlerImpl;
import org.springframework.security.web.access.DelegatingAccessDeniedHandler;
import org.springframework.security.web.access.expression.DefaultWebSecurityExpressionHandler;
import org.springframework.security.web.authentication.AnonymousAuthenticationFilter;
import org.springframework.security.web.authentication.LoginUrlAuthenticationEntryPoint;
import org.springframework.security.web.csrf.CookieCsrfTokenRepository;
import org.springframework.security.web.csrf.CsrfTokenRequestAttributeHandler;
import org.springframework.security.web.csrf.InvalidCsrfTokenException;
import org.springframework.security.web.csrf.MissingCsrfTokenException;
import org.springframework.security.web.util.matcher.AntPathRequestMatcher;
import org.terasoluna.gfw.security.web.logging.UserIdMDCPutFilter;

/**
 * Bean definition to configure SpringSecurity.
 */
@Configuration
@EnableWebSecurity
@EnableMethodSecurity
public class SpringSecurityConfig {

	/**
	 * 静的リソースはセキュリティ適応外
	 * @return Bean of configured {@link WebSecurityCustomizer}
	 */
	@Bean
	public WebSecurityCustomizer webSecurityCustomizer() {
		return web -> web.ignoring().requestMatchers(
				new AntPathRequestMatcher("/css/**"),
				new AntPathRequestMatcher("/img/**"),
				new AntPathRequestMatcher("/js/**"),
				new AntPathRequestMatcher("/*.js"),
				new AntPathRequestMatcher("/*.html")
				);
	}

	/**
	 * WebAPI用設定
	 * @param http Builder class for setting up authentication and authorization
	 * @return Bean of configured {@link SecurityFilterChain}
	 * @throws Exception Exception that occurs when setting HttpSecurity
	 */
	@Bean
	@Order(1)
	public SecurityFilterChain filterChainApiView(
			HttpSecurity http) throws Exception {
		http.securityMatcher(
				new AntPathRequestMatcher("/webapi/**"));
		http.sessionManagement(sessionManagement -> sessionManagement
				.sessionCreationPolicy(SessionCreationPolicy.STATELESS));
		http.csrf(csrf -> csrf.disable());
		http.authorizeHttpRequests(authz -> authz.requestMatchers(
				new AntPathRequestMatcher("/**")).permitAll());

		// ヘッダへの出力情報
		http.headers(header -> header.xssProtection(t -> t.disable())
				.contentSecurityPolicy(t -> t.policyDirectives("default-src 'self'"))
				);
		return http.build();
	}

	/**
	 * オンライン用設定
	 * @param http Builder class for setting up authentication and authorization
	 * @return Bean of configured {@link SecurityFilterChain}
	 * @throws Exception Exception that occurs when setting HttpSecurity
	 */
	@Bean
	@Order(2)
	public SecurityFilterChain filterChain(HttpSecurity http)
			throws Exception {

		http.logout(logout -> logout.logoutUrl("/logout.do")
				.invalidateHttpSession(true)
				.deleteCookies("JSESSIONID")
				.logoutSuccessUrl("/logoutSuccess"));

		http.exceptionHandling(ex ->
				// 未ログインでアクセスされた場合のURLを設定
				ex.authenticationEntryPoint(new LoginUrlAuthenticationEntryPoint("/common/error/unauthenticated"))
				// CSRFエラーや権限がない場合の定義
				.accessDeniedHandler(accessDeniedHandler()));
		http.addFilterAfter(userIdMDCPutFilter(),
				AnonymousAuthenticationFilter.class);

		http.sessionManagement(Customizer.withDefaults());

		// ログイン認証不要の処理を定義
		http.authorizeHttpRequests(authz -> authz.requestMatchers(
				new AntPathRequestMatcher("/csrf"),
				new AntPathRequestMatcher("/login*"), new AntPathRequestMatcher("/logoutSuccess"),
				new AntPathRequestMatcher("/password_reset.do"), new AntPathRequestMatcher("/common/error/**"))
				.permitAll()
				.requestMatchers(
						new AntPathRequestMatcher("/**"))
				.authenticated());

		// クロスサイトフォージュリー対策の定義
		http.csrf(
				csrf -> csrf
						.csrfTokenRepository(CookieCsrfTokenRepository.withHttpOnlyFalse())
						.csrfTokenRequestHandler(new CsrfTokenRequestAttributeHandler())
						.ignoringRequestMatchers(
								new AntPathRequestMatcher("/common/error/**")));

		// ヘッダへの出力情報
		http.headers(header -> header.xssProtection(t -> t.disable())
				.contentSecurityPolicy(t -> t.policyDirectives("default-src 'self'"))
				);
		return http.build();
	}

	@Bean("authenticationManager")
	public AuthenticationManager authenticationManager(final AuthenticationConfiguration authenticationConfiguration)
			throws Exception {
		return authenticationConfiguration.getAuthenticationManager();
	}

	@Bean("authenticationProvider")
	public AuthenticationProvider authenticationProvider(@Qualifier("passwordEncoder") PasswordEncoder passwordEncoder,
			@Qualifier("bcpUserDetailsService") UserDetailsService bcpUserDetailsService) {
		DaoAuthenticationProvider authProvider = new DaoAuthenticationProvider(passwordEncoder);
		authProvider.setUserDetailsService(bcpUserDetailsService);
		authProvider.setUserDetailsPasswordService((UserDetailsPasswordService) bcpUserDetailsService);
		return authProvider;
	}

	/**
	 * Configure {@link AccessDeniedHandler} bean.
	 * @return Bean of configured {@link AccessDeniedHandler}
	 */
	@Bean("accessDeniedHandler")
	public AccessDeniedHandler accessDeniedHandler() {
		LinkedHashMap<Class<? extends AccessDeniedException>, AccessDeniedHandler> errorHandlers = new LinkedHashMap<>();

		// Invalid CSRF authenticator error handler
		AccessDeniedHandlerImpl invalidCsrfTokenErrorHandler = new AccessDeniedHandlerImpl();
		invalidCsrfTokenErrorHandler.setErrorPage(
				"/common/error/invalidCsrfTokenError");
		errorHandlers.put(InvalidCsrfTokenException.class,
				invalidCsrfTokenErrorHandler);

		// Missing CSRF authenticator error handler
		AccessDeniedHandlerImpl missingCsrfTokenErrorHandler = new AccessDeniedHandlerImpl();
		missingCsrfTokenErrorHandler.setErrorPage(
				"/common/error/missingCsrfTokenError");
		errorHandlers.put(MissingCsrfTokenException.class,
				missingCsrfTokenErrorHandler);

		// Default error handler
		AccessDeniedHandlerImpl defaultErrorHandler = new AccessDeniedHandlerImpl();
		defaultErrorHandler.setErrorPage("/common/error/accessDeniedError");

		return new DelegatingAccessDeniedHandler(errorHandlers, defaultErrorHandler);
	}

	/**
	 * Configure {@link DefaultWebSecurityExpressionHandler} bean.
	 * @return Bean of configured {@link DefaultWebSecurityExpressionHandler}
	 */
	@Bean("webSecurityExpressionHandler")
	public DefaultWebSecurityExpressionHandler webSecurityExpressionHandler() {
		return new DefaultWebSecurityExpressionHandler();
	}

	/**
	 * Configure {@link UserIdMDCPutFilter} bean.
	 * @return Bean of configured {@link UserIdMDCPutFilter}
	 */
	@Bean("userIdMDCPutFilter")
	public UserIdMDCPutFilter userIdMDCPutFilter() {
		return new UserIdMDCPutFilter();
	}
}
