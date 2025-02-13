@echo off
setlocal

REM aws sts assume-roleコマンドを実行し、出力を一時ファイルに保存
aws sts assume-role --role-arn "arn:xxx:xxx:xxx:xxxx" --role-session-name "anything" --policy "{\"Version\":\"2012-10-17\",\"Statement\":[{\"Action\":[\"s3:GetObject\",\"s3:PutObject\",\"s3:ListBucket\",\"s3:DeleteObject\"],\"Effect\":\"Allow\",\"Resource\":[\"arn:aws:s3:::vender-01-bucket/*\"],\"Sid\":\"\"}]}" --duration-seconds 43600 > temp.json

REM 認証情報を表示
type temp.json

REM jqを使用して認証情報を抽出
for /f "tokens=*" %%i in ('jq -r ".Credentials.AccessKeyId" temp.json') do set AccessKeyId=%%i
for /f "tokens=*" %%i in ('jq -r ".Credentials.SecretAccessKey" temp.json') do set SecretAccessKey=%%i
for /f "tokens=*" %%i in ('jq -r ".Credentials.SessionToken" temp.json') do set SessionToken=%%i

REM 一時ファイルを削除
del temp.json

echo AccessKeyId: %AccessKeyId%
echo SecretAccessKey: %SecretAccessKey%
echo SessionToken: %SessionToken%

REM レジストリに登録
REG ADD HKLM\SOFTWARE\bcpSoft /f /v accessKeyId /d %AccessKeyId%
REG ADD HKLM\SOFTWARE\bcpSoft /f /v secretAccessKey /d %SecretAccessKey%
REG ADD HKLM\SOFTWARE\bcpSoft /f /v sessionToken /d %SessionToken%

endlocal
