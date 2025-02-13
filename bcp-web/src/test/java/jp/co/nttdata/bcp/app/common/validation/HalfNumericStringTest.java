package jp.co.nttdata.bcp.app.common.validation;

import static org.junit.Assert.*;

import jakarta.validation.Validation;
import jakarta.validation.Validator;

import org.junit.Before;
import org.junit.Test;

import lombok.Data;

public class HalfNumericStringTest {

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
		testForm.setValue("123@456");
		assertFalse(validator.validate(testForm).isEmpty());
		System.out.println(validator.validate(testForm));

		testForm.setValue("123a456");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("123A456");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("123１456");
		assertFalse(validator.validate(testForm).isEmpty());

		
		//正常系
		testForm.setValue("0123456789");
		assertTrue(validator.validate(testForm).isEmpty());
		
	}
	
	@Data
	private class TestForm {
		
		@HalfNumericString
		private String value;
	}

}
