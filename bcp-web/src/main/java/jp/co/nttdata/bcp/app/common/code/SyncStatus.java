package jp.co.nttdata.bcp.app.common.code;

import org.terasoluna.gfw.common.codelist.EnumCodeList;

import lombok.AllArgsConstructor;

/**
 * 同期ステータス
 */
@AllArgsConstructor
public enum SyncStatus implements EnumCodeList.CodeListItem {

	NOT_SYNC("0", "未同期"),
	SUCCESS_UPLOAD("1", "アップロード成功"),
	SUCCESS_DOWNLOAD("2", "ダウンロード成功"),
	FAILURE_UPLOAD("3", "アップロード失敗"),
	FAILURE_DOWNLOAD("4", "ダウンロード失敗"),
	SYNC_IMPOSSIBLE("5", "同期不可"),
	;

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
