package jp.co.nttdata.bcp.app.common.validation;

import static org.junit.Assert.*;

import jakarta.validation.Validation;
import jakarta.validation.Validator;

import org.junit.Before;
import org.junit.Test;

import lombok.Data;

public class FileNameTest {

	
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
		testForm.setFilename("円マークを含む\\aa");
		assertFalse(validator.validate(testForm).isEmpty());
		System.out.println(validator.validate(testForm));

		testForm.setFilename("スラッシュを含む/aa");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setFilename("コロンを含む:aa");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setFilename("アスタリスクを含む*aa");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setFilename("クエスチョンマークを含む?aa");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setFilename("ダブルクォーテーションを含む\"aa");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setFilename("大なりを含む<aa");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setFilename("小なりを含む>aa");
		assertFalse(validator.validate(testForm).isEmpty());
		
		testForm.setFilename("パイプを含む|aa");
		assertFalse(validator.validate(testForm).isEmpty());

		testForm.setFilename("機種依存文字を含む①aa");
		assertFalse(validator.validate(testForm).isEmpty());

		
		//正常系
		testForm.setFilename("あいうえお12345ABCDEFGabcdefg !#$%&'()+,-.;=@[]^_`{}~");
		assertTrue(validator.validate(testForm).isEmpty());
		
	}

	
	
	
	@Data
	private class TestForm {
		
		@FileName
		private String filename;
	}
}
