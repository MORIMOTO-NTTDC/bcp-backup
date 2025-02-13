package jp.co.nttdata.bcp.app.common.validation;

import static org.junit.Assert.*;

import jakarta.validation.Validation;
import jakarta.validation.Validator;

import org.junit.Before;
import org.junit.Test;

import jp.co.nttdata.bcp.app.common.code.SyncStatus;
import lombok.Data;

public class CodeOfEnumTest {

	private Validator validator;
	
	
	/**
	 * 前処理
	 */
	@Before
	public void setUp() {
		validator = Validation.buildDefaultValidatorFactory().getValidator();
	}
	
	/**
	 * テスト実行
	 */
	@Test
	public void test() {
		TestForm testForm = new TestForm();
		
		// 異常系
		testForm.setValue("6");
		assertFalse(validator.validate(testForm).isEmpty());
		System.out.println(validator.validate(testForm));

		//正常系
		testForm.setValue("0");
		assertTrue(validator.validate(testForm).isEmpty());
		testForm.setValue("1");
		assertTrue(validator.validate(testForm).isEmpty());
		testForm.setValue("2");
		assertTrue(validator.validate(testForm).isEmpty());
		testForm.setValue("3");
		assertTrue(validator.validate(testForm).isEmpty());
		testForm.setValue("4");
		assertTrue(validator.validate(testForm).isEmpty());
		testForm.setValue("5");
		assertTrue(validator.validate(testForm).isEmpty());

	}
	
	@Data
	private class TestForm {
		
		@CodeOfEnum(enumClass = SyncStatus.class)
		private String value;
	}

}
