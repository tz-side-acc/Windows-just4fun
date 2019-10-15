@echo off
echo Are you sure you want to do this?
pause
goto a
:a
ipconfig /release
goto a