package jp.co.nttdata.bcp.app.common.code;

import org.terasoluna.gfw.common.codelist.EnumCodeList;

import lombok.AllArgsConstructor;

/**
 * 処理区分 更新/削除
 */
@AllArgsConstructor
public enum ModeType implements EnumCodeList.CodeListItem {

	UPDATE("0", "更新"),
	DELETE("1", "削除");

	private final String value;
	private final String label;

	@Override
	public String getCodeValue() {
		return value;
	}

	@Override
	public String getCodeLabel() {
		return label;
	}
}
