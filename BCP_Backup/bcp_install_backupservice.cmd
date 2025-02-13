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
set SERVICEDISPNAME=%SERVICENAME:_= %
for /f "usebackq delims=" %%A in (`dir /s /b *_service.exe`) do set BINPATH=%%A
set DISCRIPTION="BCPバックアップのサービスアプリ S3バケットへの自動バックアップ、リストア等を行う"

rem 念のためサービスのプロセスを強制終了
taskkill /F /IM %SERVICENAME%.exe >nul 2>&1

rem 念のためサービスをアンインストール
sc delete %SERVICENAME% >nul 2>&1

rem サービスを手動モードでインストール（binPathにはフルパスを指定しないとうまくいかない）
sc create %SERVICENAME% start=demand binPath= "%BINPATH%" DisplayName= %SERVICEDISPNAME%

rem サービスの説明文を編集
sc description %SERVICENAME% %DISCRIPTION%

echo [+] BCPバックアップサービス（BCP_Backup_Service）をインストールしました。

:L_end
pause
exit /b