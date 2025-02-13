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

import org.terasoluna.gfw.common.codelist.EnumCodeList;

import jp.co.nttdata.bcp.app.common.validation.CodeOfEnum.List;

/**
 * コードチェック用Enumアノテーション
 */
@Documented
@Constraint(validatedBy = { CodeOfEnumValidator.class })
@Target({ METHOD, FIELD, ANNOTATION_TYPE, CONSTRUCTOR, PARAMETER, TYPE_USE })
@Retention(RUNTIME)
@Repeatable(List.class)
@ReportAsSingleViolation
public @interface CodeOfEnum {
	Class<? extends EnumCodeList.CodeListItem> enumClass();
	
	String message() default "{jp.co.nttdata.bcp.app.common.validation.ValueOfEnum.message}";

	Class<?>[] groups() default {};

	Class<? extends Payload>[] payload() default {};

	@Target({ METHOD, FIELD, ANNOTATION_TYPE, CONSTRUCTOR, PARAMETER, TYPE_USE })
	@Retention(RUNTIME)
	@Documented
	@interface List {
		CodeOfEnum[] value();
	}
}