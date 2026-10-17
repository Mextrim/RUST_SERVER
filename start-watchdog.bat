@echo off
rem Starts the watchdog in a minimized window.
rem The watchdog keeps the server running and restarts it if it stops.
rem Stop it later with stop-watchdog.bat.

set "ROOT=%~dp0"

if exist "%ROOT%stop_watchdog.txt" del /q "%ROOT%stop_watchdog.txt"

start "" /min powershell.exe -NoProfile -ExecutionPolicy Bypass -WindowStyle Hidden -File "%ROOT%watchdog.ps1"

echo Watchdog started. Server will come up in a minute or two.
echo Watch console: watchdog.log
echo Stop with: stop-watchdog.bat