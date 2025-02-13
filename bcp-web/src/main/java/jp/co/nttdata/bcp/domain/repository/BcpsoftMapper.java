package jp.co.nttdata.bcp.domain.repository;

import jp.co.nttdata.bcp.domain.model.Bcpsoft;

public interface BcpsoftMapper {

	/**
	 * BCP端末ソフトバージョンをもとにレコードを取得
	 * 
	 * @param softVersion BCP端末ソフトバージョン
	 * @return BCPソフト管理のレコード
	 */
	Bcpsoft selectBytSoftVersion(String softVersion);
}