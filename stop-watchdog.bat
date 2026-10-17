@echo off
rem Asks the watchdog to stop. It exits after it notices the file.
rem The running server is left alone: it keeps going until you stop it yourself.

echo stop> "%~dp0stop_watchdog.txt"

echo Watchdog will stop shortly. The server keeps running.
echo Watch console: watchdog.log