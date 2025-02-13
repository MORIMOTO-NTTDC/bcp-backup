@echo off
setlocal enabledelayedexpansion

set FOLDER_NAME=BCP_Backup_Src

set SRC_FOLDER=.

rem 日付を取得
set YYYYMMDD=%DATE:/=%

rem 枝番を初期化
set BRANCH_NUM=01

:CHECK_DIR
set DIST_FOLDER=..\BCP_SRC_ZIP\%YYYYMMDD%_%BRANCH_NUM%\%FOLDER_NAME%

rem フォルダが存在するか確認
if exist %DIST_FOLDER% (
    rem 枝番をインクリメント
    set /a BRANCH_NUM+=1
    rem 枝番を2桁にフォーマット
    if !BRANCH_NUM! lss 10 set BRANCH_NUM=0!BRANCH_NUM!
    goto CHECK_DIR
)

rem フォルダを作成
mkdir %DIST_FOLDER%

XCOPY "%SRC_FOLDER%" "%DIST_FOLDER%" /EXCLUDE:.\backup_source_exclude.list /E /F /H /K /R /Y /C /V

powershell -Command "Compress-Archive -Path %DIST_FOLDER% %DIST_FOLDER%\..\%FOLDER_NAME%.zip"

rem pause
