@echo off
set /p ITSROOT=Enter extracted ITS folder path: 
dotnet run --project "%~dp0..\TSL.HMI.csproj" -- "%ITSROOT%"
pause
