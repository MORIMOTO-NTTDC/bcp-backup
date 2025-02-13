package jp.co.nttdata.bcp.app.common.validation;

import static java.lang.annotation.ElementType.*;
import static java.lang.annotation.RetentionPolicy.*;

import java.lang.annotation.Documented;
import java.lang.annotation.Repeatable;
import java.lang.annotation.Retention;
import java.lang.annotation.Target;

import jakarta.validation.Constraint;
import jakarta.validation.Payload;
import jakarta.validation.ReportAsSingleViolation;

import jp.co.nttdata.bcp.app.common.validation.HalfFullWidthString.List;

/**
 * 全半角文字チェック用アノテーション<br>
 * ※以下の特殊文字を入力禁止とする。<br>
 * ・特殊文字の文字範囲
 * 　　NEC選定IBM拡張文字（0xED40～0xEEFC）<br>
 * 　　IBM拡張文字（0xFA40～0xFC4B）<br>
 * 　　外字（0xF040～0xF9FC）<br>
 * 　　機種依存文字エリア（0x8540～0x889E）<br>
 * 　　Mac用外字及び縦組用（0xEAA5～0xFCFC）<br>
 * ・ファイル名に使用できない文字
 * 　　\ / :  *  ? " < > |
 */
@Documented
@Constraint(validatedBy = { HalfFullWidthStringValidator.class })
@Target({ METHOD, FIELD, ANNOTATION_TYPE, CONSTRUCTOR, PARAMETER, TYPE_USE })
@Retention(RUNTIME)
@Repeatable(List.class)
@ReportAsSingleViolation
public @interface HalfFullWidthString {
	String message() default "{jp.co.nttdata.bcp.app.common.validation.HalfFullWidthString.message}";

	Class<?>[] groups() default {};

	Class<? extends Payload>[] payload() default {};

	@Target({ METHOD, FIELD, ANNOTATION_TYPE, CONSTRUCTOR, PARAMETER, TYPE_USE })
	@Retention(RUNTIME)
	@Documented
	@interface List {
		HalfFullWidthString[] value();
	}
}