package jp.co.nttdata.bcp.app.common.validation;

import static org.junit.Assert.*;

import jakarta.validation.Validation;
import jakarta.validation.Validator;

import org.junit.Before;
import org.junit.Test;

import lombok.Data;

public class PasswordTest {

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
		
		// 異常系（対応外の記号）
		testForm.setValue("1234545/ab");
		assertFalse(validator.validate(testForm).isEmpty());
		System.out.println(validator.validate(testForm));

		testForm.setValue("12345*aa");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("12345?aa");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("12345\"aa");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("12345<aa");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("12345>aa");
		assertFalse(validator.validate(testForm).isEmpty());
		
		testForm.setValue("12345|aa");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("12345:aa");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("12345\\aa");
		assertFalse(validator.validate(testForm).isEmpty());

		//異常系（全角文字）
		testForm.setValue("12345|aaＡ");
		assertFalse(validator.validate(testForm).isEmpty());
		
		//異常系（３種類を含まない）
		testForm.setValue("abcd1234");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("abcdABCD");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("abcd@#$%");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("ABCD1234");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("ABCD@#$%");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("1234@#$%");
		assertFalse(validator.validate(testForm).isEmpty());

		//異常系（桁数不足）
		testForm.setValue("abcd@12");
		assertFalse(validator.validate(testForm).isEmpty());

		//異常系（桁数超過）
		testForm.setValue("abcdefghijklmnop@1234");
		assertFalse(validator.validate(testForm).isEmpty());
		
		//正常系(許容可能文字総当たりで確認)
		testForm.setValue("abc@1234");
		assertTrue(validator.validate(testForm).isEmpty());
		testForm.setValue("defghijklmnopqr@1234");
		assertTrue(validator.validate(testForm).isEmpty());
		testForm.setValue("1234567890stuvwxyzAB");
		assertTrue(validator.validate(testForm).isEmpty());
		testForm.setValue("CDEFGHIJKLMNOPQRSTa1");
		assertTrue(validator.validate(testForm).isEmpty());
		testForm.setValue("UVWXYZ1!#$%&'()+,-.");
		assertTrue(validator.validate(testForm).isEmpty());
		testForm.setValue(";=@[]^_`{}~a1");
		assertTrue(validator.validate(testForm).isEmpty());

		//正常系(３種)
		testForm.setValue("abcdABCD1234");
		assertTrue(validator.validate(testForm).isEmpty());
		testForm.setValue("abcdABCD!#$%");
		assertTrue(validator.validate(testForm).isEmpty());
		testForm.setValue("abcd1234!#$%");
		assertTrue(validator.validate(testForm).isEmpty());
		testForm.setValue("ABCD1234!#$%");
		assertTrue(validator.validate(testForm).isEmpty());

	}
	
	@Data
	private class TestForm {
		
		@Password
		private String value;
	}

}
