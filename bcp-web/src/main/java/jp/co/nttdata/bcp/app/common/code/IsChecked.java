package jp.co.nttdata.bcp.app.common.code;

import org.terasoluna.gfw.common.codelist.EnumCodeList;

import lombok.AllArgsConstructor;

/**
 * チェックON/OFF
 */
@AllArgsConstructor
public enum IsChecked implements EnumCodeList.CodeListItem {

	OFF("0", "OFF"),
	ON("1", "ON");

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
