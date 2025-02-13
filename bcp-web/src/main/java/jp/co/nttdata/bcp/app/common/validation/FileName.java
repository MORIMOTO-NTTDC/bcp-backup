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

import jp.co.nttdata.bcp.app.common.validation.FileName.List;

/**
 * ファイル名チェック用アノテーション.<br>
 * ・ファイル名に使用できない文字<br>
 * 　　\ / :  *  ? " < > |
 */
@Documented
@Constraint(validatedBy = {})
@Target({ METHOD, FIELD, ANNOTATION_TYPE, CONSTRUCTOR, PARAMETER, TYPE_USE })
@Retention(RUNTIME)
@Repeatable(List.class)
@ReportAsSingleViolation
@Pattern(regexp = "[^\\\\\\/\\:\\*\\?\\\"\\<\\>\\|]*")
@HalfFullWidthString
public @interface FileName {
	String message() default "{jp.co.nttdata.bcp.app.common.validation.FileNameg.message}";

	Class<?>[] groups() default {};

	Class<? extends Payload>[] payload() default {};

	@Target({ METHOD, FIELD, ANNOTATION_TYPE, CONSTRUCTOR, PARAMETER, TYPE_USE })
	@Retention(RUNTIME)
	@Documented
	@interface List {
		FileName[] value();
	}
}