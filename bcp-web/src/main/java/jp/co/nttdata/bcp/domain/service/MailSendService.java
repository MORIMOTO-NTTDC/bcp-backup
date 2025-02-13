package jp.co.nttdata.bcp.domain.service;

import java.net.URI;
import java.util.Map;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.thymeleaf.context.Context;
import org.thymeleaf.spring6.SpringTemplateEngine;
import org.thymeleaf.templatemode.TemplateMode;
import org.thymeleaf.templateresolver.ClassLoaderTemplateResolver;

import jp.co.nttdata.bcp.domain.model.mail.MailInfo;
import lombok.extern.slf4j.Slf4j;
import software.amazon.awssdk.regions.Region;
import software.amazon.awssdk.services.sesv2.SesV2Client;
import software.amazon.awssdk.services.sesv2.model.Body;
import software.amazon.awssdk.services.sesv2.model.Content;
import software.amazon.awssdk.services.sesv2.model.Destination;
import software.amazon.awssdk.services.sesv2.model.EmailContent;
import software.amazon.awssdk.services.sesv2.model.Message;
import software.amazon.awssdk.services.sesv2.model.SendEmailRequest;

/**
 * メール送信用サービスクラス
 */
@Service
@Slf4j
public class MailSendService {

	/** 送信元 */
	@Value("${aws.ses.email.source}")
	private String from;

	/** AWS接続エンドポイント */
	@Value("${aws.ses.email.endpoint}")
	private String endpointUrl;

	/** AWS接続リージョン */
	@Value("${aws.resion}")
	private String resion;

	/**
	 * テンプレートから本文を作成する。
	 * @param templateFilepath テンプレートのファイルパス
	 * @param params 埋め込みパラメータのマップ
	 * @return 生成された本文
	 */
	public String createBody(String templateFilepath, Map<String, Object> params) {

		// テンプレートエンジンを使用するための設定インスタンスを生成します。
		ClassLoaderTemplateResolver templateResolver = new ClassLoaderTemplateResolver();
		/*
		 * テンプレートエンジンの種類を指定します。
		 * メールテンプレートとして使用するため、テキストを指定しています。
		 */
		templateResolver.setTemplateMode(TemplateMode.TEXT);
		/*
		 * テンプレートファイルとして読み込む文字エンコードを指定します。
		 * 以下のように指定すると「UTF-8」の文字エンコードなります。
		 */
		templateResolver.setCharacterEncoding("UTF-8");

		// テンプレートエンジンを使用するためのインスタンスを生成します。
		SpringTemplateEngine engine = new SpringTemplateEngine();
		engine.setTemplateResolver(templateResolver);

		// テンプレートエンジンを実行してテキストを取得します。
		Context context = new Context();
		context.setVariables(params);
		// 使用するテンプレートのファイル名とパラメータ情報を設定します。
		String text = engine.process(templateFilepath, context);

		return text;
	}

	/**
	 * メール送信
	 * @param mailInfo メール送信情報
	 */
	public void sendEmail(MailInfo mailInfo) {

		Region region = Region.of(resion);
		SesV2Client client = SesV2Client.builder()
				.region(region)
				.endpointOverride(URI.create(endpointUrl))
				.build();

		Destination destination = Destination.builder()
				.toAddresses(mailInfo.getTos())
				.ccAddresses(mailInfo.getCcs())
				.bccAddresses(mailInfo.getBccs())
				.build();

		Content sub = Content.builder()
				.data(mailInfo.getSubject())
				.build();

		Content content = Content.builder()
				.data(mailInfo.getBody())
				.build();

		Body body = Body.builder()
				.text(content)
				.build();

		Message msg = Message.builder()
				.subject(sub)
				.body(body)
				.build();

		EmailContent emailContent = EmailContent.builder()
				.simple(msg)
				.build();

		SendEmailRequest emailRequest = SendEmailRequest.builder()
				.destination(destination)
				.content(emailContent)
				.fromEmailAddress(from)
				.build();

		try {
			log.debug("Attempting to send an email through Amazon SES "
					+ "using the AWS SDK for Java...");
			client.sendEmail(emailRequest);
			log.debug("email was sent");

		} finally {
			client.close();
		}

	}

}
