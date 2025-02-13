package jp.co.nttdata.bcp.app.common.util;

import java.security.SecureRandom;
import java.util.Arrays;
import java.util.Collections;
import java.util.List;
import java.util.Random;

/**
 * パスワード生成用クラス
 */
public final class PasswordGenerator {

	/** パスワードに使用する小文字（lは除外） */
	private static final String ALPHA_L = "abcdefghijkmnopqrstuvwxyz";
	/** パスワードに使用する大文字（I,Oは除外） */
	private static final String ALPHA_U = "ABCDEFGHJKLMNPQRSTUVWXYZ";
	/** パスワードに使用する数字（0,1は除外） */
	private static final String NUM = "23456789";

	/**
	 * 10桁の大文字小文字数字から成り立つランダム文字列を生成する
	 * @return 生成された文字列
	 */
	public static String randomPasswordString() {

		Random ran = new SecureRandom();

		// 1から8までのランダムな数字を小文字の長さに設定する
		int alphaLLength = ran.nextInt(7) + 1;
		// 1から9-小文字の長さまでのランダムな数字を大文字の長さに設定する
		int alphaULength = ran.nextInt(9 - alphaLLength - 1) + 1;
		// 10-大文字の長さ-小文字の長さを数字の長さに設定する
		int numLength = 10 - alphaLLength - alphaULength;

		StringBuffer password = new StringBuffer();
		// ランダムな小文字列を生成
		for (int i = 0; i < alphaLLength; i++) {
			int pos = ran.nextInt(ALPHA_L.length());
			password.append(ALPHA_L.charAt(pos));
		}
		// ランダムな大文字列を生成
		for (int i = 0; i < alphaULength; i++) {
			int pos = ran.nextInt(ALPHA_U.length());
			password.append(ALPHA_U.charAt(pos));
		}
		// ランダムな数字列を生成
		for (int i = 0; i < numLength; i++) {
			int pos = ran.nextInt(NUM.length());
			password.append(NUM.charAt(pos));
		}

		//生成した文字列をシャッフルして返却
		final List<String> singles = Arrays.asList(password.toString().split(""));
		Collections.shuffle(singles, ran);
		return singles.stream().reduce((s1, s2) -> s1 + s2).get();

	}

}
