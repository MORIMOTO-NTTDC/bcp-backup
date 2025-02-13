package jp.co.nttdata.bcp.app.common.aspect;

import java.lang.reflect.InvocationTargetException;

import org.apache.commons.beanutils.BeanUtils;
import org.aspectj.lang.JoinPoint;
import org.aspectj.lang.annotation.Aspect;
import org.aspectj.lang.annotation.Before;
import org.slf4j.MDC;
import org.springframework.stereotype.Component;

/**
 * ユーザ情報としてaccountIdの値をログ出力<br>
 * WebAPIはユーザ情報を持たないため、その代わりにフォーム内のアカウントIDの値をログのユーザ情報として出力する。
 */
@Aspect
@Component
public class AccountIdMDCPutAspect {

	/**
	 * ユーザ情報としてaccountIdの値をログ出力
	 */
	@Before("within(jp.co.nttdata.bcp.app.webapi.controller.*) && @annotation(org.springframework.web.bind.annotation.PostMapping)")
	public void startLog(JoinPoint jp) {

		for (Object arg : jp.getArgs()) {
			// フォームからaccountIdを取得
			if (arg.getClass().getName().endsWith("Form")) {
				try {
					MDC.put("USER", (String) BeanUtils.getProperty(arg, "accountId"));
				} catch (IllegalArgumentException | IllegalAccessException | InvocationTargetException
						| NoSuchMethodException e) {
					// 対象項目がない場合は何もしない
				}
				return;
			}
		}

	}
}
