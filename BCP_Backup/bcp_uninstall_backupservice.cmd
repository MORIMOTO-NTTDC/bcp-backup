@echo off
setlocal enabledelayedexpansion

rem カレントフォルダへ移動
cd %~dp0

rem 管理者権限チェック
openfiles > nul 2>&1
if not %ERRORLEVEL% == 0 (
  echo [-] 管理者権限で実行していません。
  goto L_end
)

set SERVICENAME="BCP_Backup_Service"

rem 念のためサービスのプロセスを強制終了
taskkill /F /IM %SERVICENAME%.exe >nul 2>&1

rem サービスをアンインストール
sc delete %SERVICENAME%
if %ERRORLEVEL% == 0 (
  echo [+] BCPバックアップサービス（BCP_Backup_Service）をアンインストールしました。
)

rem bcpSoftのレジストリキーを削除
reg delete HKLM\SOFTWARE\bcpSoft /va /f >nul
if %ERRORLEVEL% == 0 (
  echo [+] レジストリキーを削除しました。
)

:L_end
pause
exit /b
