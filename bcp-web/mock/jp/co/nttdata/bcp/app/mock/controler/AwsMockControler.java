package jp.co.nttdata.bcp.app.mock.controler;

import java.nio.file.Files;
import java.nio.file.Path;
import java.util.HashMap;
import java.util.Map;

import jakarta.servlet.http.HttpServletRequest;
import jakarta.servlet.http.HttpServletResponse;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.ModelAttribute;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.ResponseBody;

import lombok.extern.slf4j.Slf4j;

/**
 * AWSのモック
 */
@Controller
@Slf4j
public class AwsMockControler {

	@PostMapping("/common/error/mock/v2/email/outbound-emails")
	@ResponseBody
	public String mockSendMail(@RequestBody Map<String, Object> body,
			HttpServletResponse response)
			throws Exception {

		log.debug("送信されたメールの内容:\n" + body.toString());

		response.setContentType("application/json");
		Path file = Path.of(this.getClass().getClassLoader().getResource("SendMailResponse.json").toURI());
		return Files.readString(file);
	}

	@PostMapping(path = "/common/error/mock/cognito-idp/", headers = "content-type=application/x-amz-json-1.1;charset=UTF-8")
	@ResponseBody
	public String mockContigoIdp(@ModelAttribute HashMap<String, Object> body, HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		log.debug("cognitoログイン");

		response.setContentType("application/json");
		Path file = Path.of(this.getClass().getClassLoader().getResource("InitiateAuthResponse.json").toURI());
		return Files.readString(file);

	}

	@PostMapping(path = "/common/error/mock/cognito-identity/", headers = "content-type=application/x-amz-json-1.1;charset=UTF-8")
	@ResponseBody
	public String mockContigoIdentity(@ModelAttribute HashMap<String, Object> body, HttpServletRequest request,
			HttpServletResponse response) throws Exception {

		String action = request.getHeader("x-amz-target");
		String fileName = null;
		if (action.equals("AWSCognitoIdentityService.GetId")) {
			log.debug("アイデンティティID取得");
			fileName = "GetIdResponse.json";
		} else if (action.equals("AWSCognitoIdentityService.GetCredentialsForIdentity")) {
			log.debug("認証情報を取得");
			fileName = "GetCredentialsForIdentityResponse.json";
		} else {
			return null;
		}

		response.setContentType("application/json");
		Path file = Path.of(this.getClass().getClassLoader().getResource(fileName).toURI());
		return Files.readString(file);

	}
}
