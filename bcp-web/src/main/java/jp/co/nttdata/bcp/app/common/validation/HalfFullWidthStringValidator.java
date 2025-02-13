package jp.co.nttdata.bcp.app.common.validation;

import java.io.UnsupportedEncodingException;

import jakarta.validation.ConstraintValidator;
import jakarta.validation.ConstraintValidatorContext;

import org.springframework.util.StringUtils;

/**
 * システムで許容する文字で構成されているかチェックを行うValidatorクラス
 */
public class HalfFullWidthStringValidator implements ConstraintValidator<HalfFullWidthString, String> {

	/**
	 * {@inheritDoc}
	 */
	@Override
	public void initialize(HalfFullWidthString constraintAnnotation) {
	}

	/**
	 * {@inheritDoc}
	 */
	@Override
	public boolean isValid(String value, ConstraintValidatorContext context) {
		if (!StringUtils.hasText(value)) {
			return true;
		}

		try {
			byte charArray[];
			charArray = value.getBytes("MS932");
			if (!new String(charArray, "MS932").equals(value)) {
				//バイト列を文字列に戻して同じでない場合は未対応文字があるためfalseを返す
				return false;
			}
			boolean secondByteFlg = false;
			for (int i = 0; i < charArray.length; i++) {
				// 2バイトチェックを実施している場合はスキップ
				if (secondByteFlg) {
					secondByteFlg = false;
					continue;
				}
				int charByte = charArray[i] & 0xFF;

				if (isLeadByte(charByte)) {
					secondByteFlg = true;
					// charByte が２バイト文字の第１バイトの場合
					if (i + 1 >= charArray.length) {
						// 第２バイトが存在しない場合：エラー
						return false;
					}
					int charByte2 = charArray[i + 1] & 0xFF;
					if (!isTrailByte(charByte2)) {
						// 第２バイトが不正：エラー
						return false;
					}

					int targetChar = (charByte << 8) | charByte2;

					if ((0xED40 <= targetChar) && (targetChar <= 0xEEFC)) {
						// 89-92区 (NEC選定IBM拡張文字)：機種依存 > 句点コード
						return false;
					}
					if ((0xFA40 <= targetChar) && (targetChar <= 0xfC4B)) {
						// 115-119区 (IBM拡張文字)：機種依存
						return false;
					}
					if ((0xF040 <= targetChar) && (targetChar <= 0xF9FC)) {
						// 外字
						return false;
					}
					if ((0x8540 <= targetChar) && (targetChar <= 0x889E)) {
						// 機種依存文字エリア
						return false;
					}
					if ((0xEAA5 <= targetChar) && (targetChar <= 0xFCFC)) {
						// Mac用外字及び縦組用
						return false;
					}
				}
			}
			return true;
		} catch (UnsupportedEncodingException e) {
			return false;
		}
	}

	/**
	 * charByte が SJIS ２バイト文字の第１バイトのときそのときに限り真を返す
	 * 
	 * @param charByte
	 *            1バイト目
	 * @return charByte が SJIS ２バイト文字の第１バイトのときそのときに限り真を返す．
	 */
	private static boolean isLeadByte(int charByte) {
		return ((0x81 <= charByte) && (charByte <= 0x9F))
				|| ((0xE0 <= charByte) && (charByte <= 0xFC));
	}

	/**
	 * charByte が SJIS ２バイト文字の第２バイトのときそのときに限り真を返す
	 * 
	 * @param charByte
	 *            2バイト目
	 * @return charByte が SJIS ２バイト文字の第２バイトのときそのときに限り真を返す．
	 */
	private static boolean isTrailByte(int charByte) {
		return (0x40 <= charByte) && (charByte <= 0xFC) && (charByte != 0x7F);
	}

}
