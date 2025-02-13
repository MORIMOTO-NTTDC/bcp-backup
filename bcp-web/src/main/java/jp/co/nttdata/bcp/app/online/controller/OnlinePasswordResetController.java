package jp.co.nttdata.bcp.app.online.controller;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashMap;
import java.util.Map;

import jakarta.inject.Inject;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.security.crypto.password.PasswordEncoder;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.common.util.PasswordGenerator;
import jp.co.nttdata.bcp.app.online.form.OnlinePasswordReset;
import jp.co.nttdata.bcp.domain.model.Users;
import jp.co.nttdata.bcp.domain.model.mail.MailInfo;
import jp.co.nttdata.bcp.domain.repository.UsersMapper;
import jp.co.nttdata.bcp.domain.service.MailSendService;

/**
 * パスワード初期化
 */
@RestController
@Transactional
public class OnlinePasswordResetController {

	@Inject
	private UsersMapper usersMapper;

	@Inject
	private PasswordEncoder passwordEncoder;

	@Inject
	private MailSendService mailSendService;

	@Value("${aws.ses.email.subject.pwdreset}")
	private String subject;

	@Value("${aws.ses.email.pwdreset}")
	private String templatePath;

	/**
	 * パスワード初期化.<br>
	 * パスワードの初期化（再設定）を行い、ユーザにメール通知する。
	 * @param form 入力値
	 */
	@PostMapping("/password_reset.do")
	public void passwordReset(@Validated @RequestBody OnlinePasswordReset form) {

		// ユーザ情報取得
		Users result = usersMapper.selectIserByemailAddress(form.getMailAddress(), true);

		if (result == null) {
			//取得したユーザ管理が0件の場合
			throw new BcpBuisinessException(404, "登録されていないメールアドレスです。");
		}

		//パスワード生成
		String password = PasswordGenerator.randomPasswordString();

		//メール本文生成
		//埋め込みパラメータ
		Map<String, Object> params = new HashMap<String, Object>();
		params.put("userId", result.getUserId());
		params.put("userName", result.getUserName());
		params.put("password", password);
		String body = mailSendService.createBody(templatePath, params);

		//メール送信情報
		MailInfo mailInfo = new MailInfo();
		//To
		mailInfo.setTos(new ArrayList<String>(Arrays.asList(result.getMailAddress())));
		//Subject
		mailInfo.setSubject(subject);
		//本文
		mailInfo.setBody(body);

		//メール送信
		mailSendService.sendEmail(mailInfo);

		//パスワードをハッシュ化
		Users user = new Users();
		user.setPassword(passwordEncoder.encode(password));

		// 主キーでパスワード更新
		user.setKey(result.getKey());
		usersMapper.updateByPrimaryKeySelective(user);
	}

}
