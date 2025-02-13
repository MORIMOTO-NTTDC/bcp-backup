rem @echo off
setlocal enabledelayedexpansion

set SRC_FOLDER=.

rem 日付を取得
set YYYYMMDD=%DATE:/=%

rem 枝番を初期化
set BRANCH_NUM=01

set si_url=http://localhost:8080/bcp-web/webapi

:CHECK_DIR
set DIST_FOLDER=..\BCP_DEBUG_ZIP\%YYYYMMDD%_%BRANCH_NUM%

rem フォルダが存在するか確認
if exist %DIST_FOLDER% (
    rem 枝番をインクリメント
    set /a BRANCH_NUM+=1
    rem 枝番を2桁にフォーマット
    if !BRANCH_NUM! lss 10 set BRANCH_NUM=0!BRANCH_NUM!
    goto CHECK_DIR
)

rem フォルダを作成
mkdir %DIST_FOLDER%\client

XCOPY "BCP_Backup_Client\bin\debug" "%DIST_FOLDER%\client" /E /F /H /K /R /Y /C /V

CALL :replace %DIST_FOLDER%\client\BCP_Backup_Client.exe.config %si_url%

mkdir %DIST_FOLDER%\service

XCOPY "BCP_Backup_Service\bin\debug" "%DIST_FOLDER%\service" /E /F /H /K /R /Y /C /V

CALL :replace %DIST_FOLDER%\service\BCP_Backup_Service.exe.config %si_url%

powershell -Command "Compress-Archive -Path %DIST_FOLDER%\client,%DIST_FOLDER%\service %DIST_FOLDER%\bcps3sync_debug.zip"

pause

exit /b

REM 各ファイル中の http://localhost:8080/bcp を引数の値に変更
:replace

chcp 65001 >nul

set BEFORE_STRING=http://localhost:8080/bcp
set AFTER_STRING=%2

setlocal enabledelayedexpansion
del tmp >nul

for /f "delims=" %%a in (%1) do (
set line=%%a
echo !line:%BEFORE_STRING%=%AFTER_STRING%!>>tmp
)

copy /Y tmp %1 >nul
del tmp >nul
endlocal

chcp 932 >nul

exit /b
