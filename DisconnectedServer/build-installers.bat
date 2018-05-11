@echo off
set version=%1

echo Building Windows Installer
rem "c:\Program Files (x86)\Inno Setup 5\ISCC.exe" "/o.\dist" ".\install.iss"

echo Building Linux Tarball

mkdir openiz-ds-%version%
cd openiz-ds-%version%



copy "..\bin\SignedRelease\Antlr3.Runtime.dll"
copy "..\bin\SignedRelease\ExpressionEvaluator.dll"
copy "..\bin\SignedRelease\jint.dll"
copy "..\bin\SignedRelease\MARC.HI.EHRS.SVC.Auditing.Core.dll"
copy "..\bin\SignedRelease\MohawkCollege.Util.Console.Parameters.dll"
copy "..\bin\SignedRelease\Mono.Data.Sqlite.dll"
copy "..\bin\SignedRelease\Newtonsoft.Json.dll"
copy "..\bin\SignedRelease\OpenIZ.BusinessRules.JavaScript.dll"
copy "..\bin\SignedRelease\OpenIZ.Core.Alert.dll"
copy "..\bin\SignedRelease\OpenIZ.Core.Applets.dll"
copy "..\bin\SignedRelease\OpenIZ.Core.Model.AMI.dll"
copy "..\bin\SignedRelease\OpenIZ.Core.Model.dll"
copy "..\bin\SignedRelease\OpenIZ.Core.Model.RISI.dll"
copy "..\bin\SignedRelease\OpenIZ.Core.Model.ViewModelSerializers.dll"
copy "..\bin\SignedRelease\OpenIZ.Core.PCL.dll"
copy "..\bin\SignedRelease\OpenIZ.Messaging.AMI.Client.dll"
copy "..\bin\SignedRelease\OpenIZ.Messaging.IMSI.Client.dll"
copy "..\bin\SignedRelease\OpenIZ.Messaging.RISI.Client.dll"
copy "..\bin\SignedRelease\OpenIZ.Mobile.Core.dll"
copy "..\bin\SignedRelease\OpenIZ.Mobile.Core.Xamarin.dll"
copy "..\bin\SignedRelease\OpenIZ.Mobile.Reporting.dll"
copy "..\bin\SignedRelease\OpenIZ.Protocol.Xml.dll"
copy "..\bin\SignedRelease\SharpCompress.dll"
copy "..\bin\SignedRelease\SQLite.Net.dll"
copy "..\bin\SignedRelease\SQLite.Net.Platform.Generic.dll"
copy "..\bin\SignedRelease\sqlite3.dll"
copy "..\bin\SignedRelease\libeay32md.dll"
copy "..\bin\SignedRelease\SqlCipher.dll"
copy "..\bin\SignedRelease\SQLite.Net.dll"
copy "..\bin\SignedRelease\SQLite.Net.Platform.SqlCipher.dll"
copy "..\bin\SignedRelease\sqlite3.dll"
copy "..\bin\SignedRelease\DisconnectedClient.Core.dll"
copy "..\bin\SignedRelease\DisconnectedServer.exe"
mkdir applets
copy "..\bin\signedrelease\applets\org.openiz.core.pak" .\applets\
copy "..\bin\signedrelease\applets\org.openiz.templates.pak"  .\applets\
cd ..

"C:\program files\7-zip\7z" a -r -ttar .\dist\openiz-ds-%version%.tar .\openiz-ds-%version%
"C:\program files\7-zip\7z" a -r -tzip .\dist\openiz-ds-%version%.zip .\openiz-ds-%version%
cd dist

"C:\program files\7-zip\7z" a -tbzip2 .\openiz-ds-%version%.tar.bz2 .\openiz-ds-%version%.tar
"C:\program files\7-zip\7z" a -tgzip .\openiz-ds-%version%.tar.gz .\openiz-ds-%version%.tar
rem "C:\Program Files (x86)\Windows Kits\8.1\bin\x86\signtool.exe" sign "openizdc-ds-%version%.exe"
cd ..
del /q /s openiz-ds-%version%
rmdir openiz-ds-%version%\applets
rmdir openiz-ds-%version%

exit /b