package jp.co.nttdata.bcp.app.webapi.controller;

import java.nio.file.Path;

import jakarta.inject.Inject;

import org.springframework.beans.factory.annotation.Value;
import org.springframework.core.io.PathResource;
import org.springframework.core.io.Resource;
import org.springframework.http.HttpHeaders;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.transaction.annotation.Transactional;
import org.springframework.validation.annotation.Validated;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;

import jp.co.nttdata.bcp.app.common.exceition.BcpBuisinessException;
import jp.co.nttdata.bcp.app.webapi.form.ApiBcpSoftDownloadForm;
import jp.co.nttdata.bcp.domain.model.Accounts;
import jp.co.nttdata.bcp.domain.repository.AccountsMapper;

/**
 * BCP管理ソフトダウンロード
 */
@Controller
@Transactional
public class ApiBcpSoftDownloadController {

	@Inject
	private AccountsMapper accountsMapper;

	@Inject
	@Value("${bcp.client.soft.file.path}")
	private String bcpsoftPath;

	/**
	 * BCP管理ソフトダウンロード.<br>
	 * 最新のBCP管理ソフトをダウンロードする
	 * @param form 入力値
	 * @return
	 */
	@PostMapping("/api01_soft_download.do")
	public ResponseEntity<Resource> accountDetail(@Validated @RequestBody ApiBcpSoftDownloadForm form) {

		Accounts account = accountsMapper.selectActiveByPrimaryKey(form.getAccountId());

		if (account == null) {
			throw new BcpBuisinessException(404, "アカウントID不一致");
		}

		//ダウンロード
		Path path = Path.of(bcpsoftPath);
		Resource resource = new PathResource(path);
		return ResponseEntity.ok()
				.contentType(MediaType.APPLICATION_OCTET_STREAM)
				.header(HttpHeaders.CONTENT_DISPOSITION,
						"attachment; filename=\"" + resource.getFilename() + "\"")
				.body(resource);
	}
}
