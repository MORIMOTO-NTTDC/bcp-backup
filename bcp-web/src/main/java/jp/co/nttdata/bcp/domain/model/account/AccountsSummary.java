package jp.co.nttdata.bcp.domain.model.account;

import lombok.Data;

/**
 * アカウント状態
 */
@Data
public class AccountsSummary {

	private String total;

	private String error;

	private String disk;

	private String soft;
}
