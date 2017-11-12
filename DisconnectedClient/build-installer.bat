@echo off
set version=%1

echo Building Windows Installer
"c:\Program Files (x86)\Inno Setup 5\ISCC.exe" "/o.\dist" /d"MyAppVersion=%version%" ".\install.iss"

"C:\Program Files (x86)\Windows Kits\8.1\bin\x86\signtool.exe" sign ".\dist\openizdc-%version%.exe"

