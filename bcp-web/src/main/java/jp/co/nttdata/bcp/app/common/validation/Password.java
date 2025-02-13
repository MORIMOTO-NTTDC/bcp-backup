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

import jp.co.nttdata.bcp.app.common.validation.Password.List;

/**
 * パスワードチェック用アノテーション<br>
 * 以下を満たすことを検証する。<br>
 * 　　・半角英数記号で8文字以上、20文字以内
 * 　　・英大文字、英小文字、数字、記号の３種類は必ず使用
 */
@Documented
@Constraint(validatedBy = {})
@Target({ METHOD, FIELD, ANNOTATION_TYPE, CONSTRUCTOR, PARAMETER, TYPE_USE })
@Retention(RUNTIME)
@Repeatable(List.class)
@ReportAsSingleViolation
@Pattern(regexp = "^((?=.*[a-z])(?=.*[A-Z])(?=.*[0-9])|(?=.*[a-z])(?=.*[A-Z])(?=.*[\\!\\#-\\)\\+-\\.\\;\\=\\@\\[\\]-\\`\\{\\}\\~])|(?=.*[A-Z])(?=.*[0-9])(?=.*[\\!\\#-\\)\\+-\\.\\;\\=\\@\\[\\]-\\`\\{\\}\\~])|(?=.*[a-z])(?=.*[0-9])(?=.*[\\!\\#-\\)\\+-\\.\\;\\=\\@\\[\\]-\\`\\{\\}\\~]))([a-zA-Z0-9\\!\\#-\\)\\+-\\.\\;\\=\\@\\[\\]-\\`\\{\\}\\~]){8,20}$")
public @interface Password {
	String message() default "{jp.co.nttdata.bcp.app.common.validation.Password.message}";

	Class<?>[] groups() default {};

	Class<? extends Payload>[] payload() default {};

	@Target({ METHOD, FIELD, ANNOTATION_TYPE, CONSTRUCTOR, PARAMETER, TYPE_USE })
	@Retention(RUNTIME)
	@Documented
	@interface List {
		Password[] value();
	}
}