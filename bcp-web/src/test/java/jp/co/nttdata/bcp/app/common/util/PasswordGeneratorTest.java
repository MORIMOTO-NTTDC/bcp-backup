package jp.co.nttdata.bcp.app.common.util;

import static org.junit.Assert.*;

import jakarta.validation.Validation;
import jakarta.validation.Validator;

import org.junit.Before;
import org.junit.Test;

import jp.co.nttdata.bcp.app.common.validation.Password;
import lombok.Data;

public class PasswordGeneratorTest {

	private Validator validator;

	/**
	 * 前処理
	 */
	@Before
	public void setUp() {
		validator = Validation.buildDefaultValidatorFactory().getValidator();
	}

	@Test
	public void testRandomPasswordString() {

		TestForm testForm = new TestForm();

		//10000回回してもパスワード規則を満たしていることを確認
		for (int i = 0; i < 10000; i++) {
			testForm.setValue(PasswordGenerator.randomPasswordString());
			assertTrue(validator.validate(testForm).isEmpty());
		}
	}

	@Data
	private class TestForm {

		@Password
		private String value;
	}
}
