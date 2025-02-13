package jp.co.nttdata.bcp.app.common.validation;

import static org.junit.Assert.*;

import jakarta.validation.Validation;
import jakarta.validation.Validator;

import org.junit.Before;
import org.junit.Test;

import lombok.Data;

public class HalfFullWidthStringTest {

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

		// 異常系(NEC 選定 IBM 拡張文字)
		testForm.setValue("纊");
		assertFalse(validator.validate(testForm).isEmpty());
		System.out.println(validator.validate(testForm));

		testForm.setValue("＂");
		assertFalse(validator.validate(testForm).isEmpty());

		// 異常系(IBM 拡張文字)
		testForm.setValue("ⅰ");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("	黑");
		assertFalse(validator.validate(testForm).isEmpty());

		// 異常系(外字)
		testForm.setValue("");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("");
		assertFalse(validator.validate(testForm).isEmpty());

		// 異常系(機種依存文字エリア)
		testForm.setValue("①");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("⊿");
		assertFalse(validator.validate(testForm).isEmpty());

		// 異常系(Mac用外字及び縦組用)
		testForm.setValue("	");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setValue("黑");
		assertFalse(validator.validate(testForm).isEmpty());

		//MS932範囲外
		testForm.setValue("ⓒ");
		assertFalse(validator.validate(testForm).isEmpty());

		//正常系
		testForm.setValue("正常系");
		assertTrue(validator.validate(testForm).isEmpty());

	}

	@Data
	private class TestForm {

		@HalfFullWidthString
		private String value;
	}

}
