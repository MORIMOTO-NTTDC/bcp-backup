package jp.co.nttdata.bcp.app.webapi.controller;

import jakarta.inject.Inject;

import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.webapi.form.ApiBcpSoftCheckForm;
import jp.co.nttdata.bcp.domain.model.Bcpsoft;
import jp.co.nttdata.bcp.domain.repository.BcpsoftMapper;

/**
 * 端末ソフトバージョンチェック
 */
@RestController
@Transactional
public class ApiBcpSoftCheckController {

	@Inject
	private BcpsoftMapper bcpsoftMapper;

	/**
	 * 端末ソフトバージョンチェック.<br>
	 * BCP端末ソフトのバージョンチェックを行う。
	 * 
	 * @param form 入力値
	 */
	@PostMapping("/api01_soft_check.do")
	public void softCheck(@Validated @RequestBody ApiBcpSoftCheckForm form) {

		Bcpsoft bcpSoft = bcpsoftMapper.selectBytSoftVersion(form.getSoftVersion());

		if (bcpSoft == null) {
			throw new BcpBuisinessException(401, "古いソフト");
		}

	}
}
