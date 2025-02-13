package jp.co.nttdata.bcp.domain.service;

import java.net.URI;
import java.util.HashMap;
import java.util.Map;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;

import jp.co.nttdata.bcp.domain.model.cognito.TokenInfo;
import lombok.extern.slf4j.Slf4j;
import software.amazon.awssdk.regions.Region;
import software.amazon.awssdk.services.cognitoidentity.CognitoIdentityClient;
import software.amazon.awssdk.services.cognitoidentity.model.GetCredentialsForIdentityRequest;
import software.amazon.awssdk.services.cognitoidentity.model.GetCredentialsForIdentityResponse;
import software.amazon.awssdk.services.cognitoidentity.model.GetIdRequest;
import software.amazon.awssdk.services.cognitoidentity.model.GetIdResponse;
import software.amazon.awssdk.services.cognitoidentityprovider.CognitoIdentityProviderClient;
import software.amazon.awssdk.services.cognitoidentityprovider.model.AdminInitiateAuthRequest;
import software.amazon.awssdk.services.cognitoidentityprovider.model.AdminInitiateAuthResponse;
import software.amazon.awssdk.services.cognitoidentityprovider.model.AuthFlowType;
import software.amazon.awssdk.services.cognitoidentityprovider.model.CognitoIdentityProviderException;

/**
 * Cognito認証用サービスクラス<br>
 * 参考URL:https://www.aws-room.com/entry/cognito-s3
 */
@Service
@Slf4j
public class CognitoService {

	private static final String PROVIDER_NAME = "cognito-idp.{region}.amazonaws.com/{user_pool_id}";

	/** アイデンティティプールID */
	@Value("${aws.cognito.identitypoolid}")
	private String identityPoolId;

	/** アカウントID */
	@Value("${aws.cognito.accountid}")
	private String accountId;

	/** アプリケーション・クライアント ID */
	@Value("${aws.cognito.clientid}")
	private String clientId;

	/** ユーザープールの ID */
	@Value("${aws.cognito.userpoolid}")
	private String userPoolId;

	/** Amazon Cognito ユーザープール 接続エンドポイント */
	@Value("${aws.cognito-idp.endpoint}")
	private String idpEndpointUrl;

	/** Amazon Cognito ID プール 接続エンドポイント */
	@Value("${aws.cognito-identity.endpoint}")
	private String identityEndpointUrl;

	/** AWS接続リージョン */
	@Value("${aws.resion}")
	private String resion;

	/**
	 * Cognitoにログインする
	 * 
	 * @param userId CognitoユーザID
	 * @param password Cognitoパスワード（平文）
	 * @return 取得したIDトークン
	 */
	public String login(String userId, String password) throws CognitoIdentityProviderException {

		CognitoIdentityProviderClient identityProviderClient = CognitoIdentityProviderClient.builder()
				.region(Region.of(resion))
				.endpointOverride(URI.create(idpEndpointUrl))
				.build();

		Map<String, String> authParameters = new HashMap<String, String>();
		authParameters.put("USERNAME", userId);
		authParameters.put("PASSWORD", password);

		AdminInitiateAuthRequest req = AdminInitiateAuthRequest.builder()
				.authFlow(AuthFlowType.ADMIN_NO_SRP_AUTH)
				.clientId(clientId)
				.userPoolId(userPoolId)
				.authParameters(authParameters)
				.build();

		AdminInitiateAuthResponse response = identityProviderClient.adminInitiateAuth(req);
		log.debug("Admin initiate auth");
		identityProviderClient.close();

		log.debug("ID Token:" + response.authenticationResult().idToken());
		return response.authenticationResult().idToken();
	}

	/**
	 * アイデンティティIDを取得
	 * 
	 * @param idToken IDトークン
	 * @return アイデンティティID
	 */
	public String getId(String idToken) throws CognitoIdentityProviderException {

		CognitoIdentityClient cognitoClient = CognitoIdentityClient.builder()
				.region(Region.of(resion))
				.endpointOverride(URI.create(identityEndpointUrl))
				.build();

		try {

			Map<String, String> logins = new HashMap<String, String>();
			String providerName = PROVIDER_NAME
					.replace("{region}", resion).replace("{user_pool_id}", userPoolId);
			logins.put(providerName, idToken);

			GetIdRequest request = GetIdRequest.builder()
					.identityPoolId(identityPoolId)
					.accountId(accountId)
					.logins(logins)
					.build();

			GetIdResponse response = cognitoClient.getId(request);
			
			log.debug("Identity ID:" + response.identityId());

			return response.identityId();

		} finally {
			cognitoClient.close();

		}
	}

	/**
	 * 認証情報を取得
	 * 
	 * @param identityId アイデンティティID
	 * @param idToken IDトークン
	 * @return 取得したトークン
	 */
	public TokenInfo getCredentialsForIdentity(String identityId, String idToken)
			throws CognitoIdentityProviderException {

		CognitoIdentityClient cognitoClient = CognitoIdentityClient.builder()
				.region(Region.of(resion))
				.endpointOverride(URI.create(identityEndpointUrl))
				.build();

		try {
			Map<String, String> logins = new HashMap<String, String>();
			String providerName = PROVIDER_NAME
					.replace("{region}", resion).replace("{user_pool_id}", userPoolId);
			logins.put(providerName, idToken);

			GetCredentialsForIdentityRequest getCredentialsForIdentityRequest = GetCredentialsForIdentityRequest
					.builder()
					.identityId(identityId)
					.logins(logins)
					.build();

			GetCredentialsForIdentityResponse response = cognitoClient
					.getCredentialsForIdentity(getCredentialsForIdentityRequest);
			TokenInfo tokenInfo = new TokenInfo();
			tokenInfo.setAccessKeyId(response.credentials().accessKeyId());
			tokenInfo.setSecretAccessKey(response.credentials().secretKey());
			tokenInfo.setSessionToken(response.credentials().sessionToken());
			return tokenInfo;

		} finally {
			cognitoClient.close();
		}

	}

}
