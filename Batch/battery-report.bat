@echo off

:: Generate battery report
powercfg /batteryreport 
for /f %%i in ('find /v /c "" ^< battery-report.html') do set /a lines=%%i
set /a startLine=%lines% - 1


:: for /F "delims=" %%i in (battery-report.html) do set xprvar=%%i
:: echo %xprvar:~29, 1%h %xprvar:~31, 2%min

:: Create new file
more /e +%startLine% battery-report.html > lastLines.txt

:: print
for /f "delims=" %%x in (lastlines.txt) do set str=%%x
echo Predicted battery life is %str:~29, 1%h %str:~31, 2%min

del lastlines.txt
del battery-report.html

timeout /T 2