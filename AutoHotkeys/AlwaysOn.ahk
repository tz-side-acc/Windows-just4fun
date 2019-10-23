#NoEnv  ; Recommended for performance and compatibility with future AutoHotkey releases.
#Warn  ; Enable warnings to assist with detecting common errors.
SendMode Input  ; Recommended for new scripts due to its superior speed and reliability.
SetWorkingDir %A_ScriptDir%  ; Ensures a consistent starting directory.

<^Lwin::
Run, C:\users\Tze Zhao\desktop\AutoHotkeys\Script1.ahk
return

; Turn display off
<!LWin::
SendMessage, 0x112, 0xF170, 2,, Program Manager
return

; Start Google Assistant
; +LWin::
Run, C:\users\Tze Zhao\desktop\Commands\Batch\tz-assistant.bat
Sleep 3000
Send, {Enter}
return


^m::
Run, 2019 Year 5 Post Promo Timetable.pdf
return

>^/::
Run, C:\users\Tze Zhao\desktop\autohotkeys\SQL.ahk
return

^!x::ExitApp 