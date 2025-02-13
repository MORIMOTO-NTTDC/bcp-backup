package jp.co.nttdata.bcp.app.common.code;

import org.terasoluna.gfw.common.codelist.EnumCodeList;

import lombok.AllArgsConstructor;

/**
 * DisableのON/OFF
 */
@AllArgsConstructor
public enum IsDisabled implements EnumCodeList.CodeListItem {

	OFF("false", "OFF"),
	ON("true", "ON");

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
