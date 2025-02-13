package jp.co.nttdata.bcp.app.common.validation;

import java.util.List;
import java.util.stream.Collectors;
import java.util.stream.Stream;

import jakarta.validation.ConstraintValidator;
import jakarta.validation.ConstraintValidatorContext;

import org.terasoluna.gfw.common.codelist.EnumCodeList;

/**
 * コードチェック用Enumアノテーション</br>
 * Enumで定義したコード値に該当するかチェックを行うValidatorクラス
 */
public class CodeOfEnumValidator implements ConstraintValidator<CodeOfEnum, String> {
	
	private List<String> acceptedValues;
	
	/**
	 * {@inheritDoc}
	 */
	@Override
	public void initialize(CodeOfEnum constraintAnnotation) {
		acceptedValues = Stream.of(constraintAnnotation.enumClass().getEnumConstants())
				.map(EnumCodeList.CodeListItem::getCodeValue)
				.collect(Collectors.toList());
	}

	/**
	 * {@inheritDoc}
	 */
	@Override
	public boolean isValid(String value, ConstraintValidatorContext context) {
		if (value == null) {
			return true;
		}
		return acceptedValues.contains(value.toString());
	}
}
