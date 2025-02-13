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

rem PowerShellのスクリプトを許可に設定変更
rem powershell -Command Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
rem BCP起動アプリのショートカット作成とスタートアップ登録
rem powershell -F bcp_boot_app_startup.ps1
rem 設定を元に戻す
rem powershell -Command Set-ExecutionPolicy -ExecutionPolicy Undefined -Scope CurrentUser

rem BCP起動アプリのショートカット作成とスタートアップ登録
rem ※ExecutionPolicyを一時的に変更してpowershellを実行
powershell -ExecutionPolicy RemoteSigned .\bcp_boot_app_startup.ps1

if %ERRORLEVEL% == 0 (
  echo [+] BCP起動アプリのショートカットを作成しスタートアップに登録しました
)

:L_end
pause
exit /b
