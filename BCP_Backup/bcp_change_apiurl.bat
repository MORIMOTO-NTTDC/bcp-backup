rem @echo off
setlocal enabledelayedexpansion

set DIST_FOLDER=.
set BEFORE_URL=http://10.144.16.10/bcp-web/webapi
set AFTER_URL=http://10.144.16.10/bcp-web/webapi

CALL :replace_url %DIST_FOLDER%\boot\BCP_Backup_Boot.exe.config

CALL :replace_url %DIST_FOLDER%\client\BCP_Backup_Client.exe.config

CALL :replace_url %DIST_FOLDER%\service\BCP_Backup_Service.exe.config

CALL :replace_url %DIST_FOLDER%\tool\bcpbackup.exe.config

pause

exit /b

REM 各ファイル中の apiUrlの値(before_url) を引数の値に変更
:replace_url

chcp 65001 >nul

setlocal enabledelayedexpansion
del tmp >nul

for /f "delims=" %%a in (%1) do (
set line=%%a
echo !line:%BEFORE_URL%=%AFTER_URL%!>>tmp
)

copy /Y tmp %1 >nul
del tmp >nul
endlocal

chcp 932 >nul

exit /b
