@echo off

REM htmlファイルやJSファイルを結合するだけのバッチ
REM 引数にDEBUGを付けている場合はモック用の資材にする

set /P X=実行中...<NUL

cd /d %~dp0

set src=.\src\
set modules=%src%modules\

set dest=.\dest\
set html=%dest%index.html
set js=%dest%js\app.js

set resources=..\src\main\webapp\resources\

call :INIT

for /r "%modules%" %%A in (*.html) do call :APPEND %%A %html%
for /r "%modules%" %%A in (*.js)   do call :APPEND %%A %js%

call :FINAL %1

echo 完了
exit /b

:INIT
rd /s /q %dest% >nul 2>&1
mkdir %dest%
mkdir %dest%js
mkdir %dest%css
mkdir %dest%img
copy /b %src%index_header.html %html% >nul 2>&1
type nul > %js%
exit /b

:FINAL
if "%1"=="DEBUG" copy /b %html% + %src%index_debug.html %html% >nul 2>&1
copy /b %html% + %src%index_footer.html %html% >nul 2>&1
copy /b %js% + %src%main.js %js% >nul 2>&1
copy /b %src%css %dest%css\ >nul 2>&1
copy /b %src%img %dest%img\ >nul 2>&1
copy /b %src%vendor\bootstrap.css %dest%css\vendor.css >nul 2>&1
copy /b %dest%css\vendor.css + %src%vendor\bootstrap-datepicker.css %dest%css\ >nul 2>&1
copy /b %src%vendor\vendor.js %dest%js\ >nul 2>&1

if "%1"=="DEBUG" (
copy /b %src%data.js %dest%data.js >nul 2>&1
copy /b %src%jquery.mockjax.js %dest% >nul 2>&1
) else (
rmdir %resources%css /S /Q
rmdir %resources%img /S /Q
rmdir %resources%js /S /Q
del %resources%* /Q

xcopy /E /Y %dest% %resources%
)

exit /b

:APPEND
copy /b %~2 + %~1 %~2 >nul 2>&1
exit /b
