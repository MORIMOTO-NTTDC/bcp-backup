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
import jakarta.validation.constraints.Pattern;

import jp.co.nttdata.bcp.app.common.validation.HalfAlphaNumericSymble.List;

/**
 * 半角英数字記号チェック用アノテーション<br>
 * ※以下の7種類の半角記号は入力禁止文字とする。<br>
 * 　　　<、>、❘、"、?、*、/
 */
@Documented
@Constraint(validatedBy = {})
@Target({ METHOD, FIELD, ANNOTATION_TYPE, CONSTRUCTOR, PARAMETER, TYPE_USE })
@Retention(RUNTIME)
@Repeatable(List.class)
@ReportAsSingleViolation
@Pattern(regexp = "[a-zA-Z0-9 \\!\\#-\\)\\+-\\.\\:\\;\\=\\@\\[-\\`\\{\\}\\~]*")
public @interface HalfAlphaNumericSymble {
	String message() default "{jp.co.nttdata.bcp.app.common.validation.HalfAlphaNumericSymble.message}";

	Class<?>[] groups() default {};

	Class<? extends Payload>[] payload() default {};

	@Target({ METHOD, FIELD, ANNOTATION_TYPE, CONSTRUCTOR, PARAMETER, TYPE_USE })
	@Retention(RUNTIME)
	@Documented
	@interface List {
		HalfAlphaNumericSymble[] value();
	}
}