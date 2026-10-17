@echo off
setlocal

rem ===== Server settings =====
rem Copy this file to run.bat and fill in your own values.
rem Never commit run.bat: it holds the RCON password.
set "SERVER_NAME=My Rust Server"
set "IDENTITY=moddedrust"
set "PORT=28015"
set "RUSTPLUS_PORT=28083"
set "RCON_PORT=28016"
set "RCON_PASSWORD=CHANGE_ME"
set "MAP_SIZE=3500"
set "MAP_SEED=12345"
set "MAX_PLAYERS=50"

rem ===== Refuse to start a second server instance =====
tasklist /FI "IMAGENAME eq RustDedicated.exe" 2>nul | find /I "RustDedicated.exe" >nul
if not errorlevel 1 (
  echo A Rust server is already running. Close it before starting this script.
  pause
  exit /b 1
)

rem ===== Update Rust server (downloads only changed files) =====
cd /d "%~dp0steamcmd"
steamcmd.exe +force_install_dir "%~dp0server" +login anonymous +app_update 258550 +quit

rem ===== Start Rust server with Carbon =====
cd /d "%~dp0server"
RustDedicated.exe -batchmode -nographics -silent-crashes -logfile "server.log" ^
  +server.hostname "%SERVER_NAME%" ^
  +server.description "Describe your server here" ^
  +server.identity "%IDENTITY%" ^
  +server.port %PORT% ^
  +server.level "Procedural Map" ^
  +server.seed %MAP_SEED% ^
  +server.worldsize %MAP_SIZE% ^
  +server.maxplayers %MAX_PLAYERS% ^
  +rcon.port %RCON_PORT% ^
  +rcon.password "%RCON_PASSWORD%" ^
  +rcon.web 1 ^
  +app.port %RUSTPLUS_PORT%

rem RLX_NO_PAUSE is set by the watchdog so the script exits immediately instead of pausing.
if not "%RLX_NO_PAUSE%"=="1" pause