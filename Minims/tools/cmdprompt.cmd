@echo off
set path=%1;%path%
cd "%1"

echo Open Immunize Development Tools
echo =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
echo.
echo There are several tools which are useful for debugging OpenIZ mobile applications.
echo.
echo	brainbug		-	A tool for extracting files from connected Android devices (requires ADB on path)
echo	appletcompiler		-	A tool which compiles an applet directory into a PAK file
echo	minims			-	A tool which allows you to debug your applets in real time in an edit/save/refresh cycle
echo	oizdt			-	Debugging tool for business rules and clinical protocols
echo	logviewer		-	A tool which opens a graphical tool for viewing/search log files
echo Successfully added to path..