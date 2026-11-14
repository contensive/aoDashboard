@echo off
setlocal

@echo Build aoDashboard
rem TODO: configure build.ps1 for this repo before using.

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0build.ps1"

if not "%1"=="/nopause" pause