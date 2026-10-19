@echo off
setlocal

rem Uploads Dashboard to the Addon Collection Library on contensive.com.

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0..\deploy-to-addon-library.ps1" ^
    -CollectionName "Dashboard" ^
    -CollectionPath "%~dp0..\..\collections\Dashboard" ^
    -DeploymentPath "C:\Deployments\aoDashboard-Dashboard" ^
    -UiPath "%~dp0..\..\ui\Dashboard"

set exitCode=%errorlevel%
pause
exit /b %exitCode%
