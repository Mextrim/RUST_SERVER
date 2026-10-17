@echo off

rem Full verification of Rust server files (slow). Use if the server crashes on start or files look damaged.
cd /d "%~dp0steamcmd"
steamcmd.exe +force_install_dir "%~dp0server" +login anonymous +app_update 258550 validate +quit

pause
