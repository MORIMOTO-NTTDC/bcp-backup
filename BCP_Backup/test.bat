@echo off
setlocal enabledelayedexpansion

:: vender-policy.json の内容を読み込む
set "policy="
for /f "delims=" %%i in (vender-policy.json) do (
    set "policy=!policy!%%i"
)

:: assume-role コマンドを実行
aws sts assume-role --role-arn "arn:xxx:xxx:xxx:xxxx" --role-session-name "anything" --policy "!policy!" --duration-seconds 900

endlocal
