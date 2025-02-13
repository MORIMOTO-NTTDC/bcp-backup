package jp.co.nttdata.bcp.app.common.code;

import org.terasoluna.gfw.common.codelist.EnumCodeList;

import lombok.AllArgsConstructor;

/**
 * ユーザ種別
 */
@AllArgsConstructor
public enum UserType implements EnumCodeList.CodeListItem {

	SYSTEM_ADMIN("0", "システム管理者"),
	VENDOR_ADMIN("1", "ベンダ等管理者"),
	VENDOR_USER("2", "ベンダ等担当者");

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
