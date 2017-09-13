@echo off
set version=0.9.7.5

"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" openizmobile.sln /t:Rebuild /p:Configuration=SignedRelease /p:Platform=x86
"C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" openizmobile.sln /t:Rebuild /p:Configuration=SignedRelease
cd minims

echo Building Windows Installer
"c:\Program Files (x86)\Inno Setup 5\ISCC.exe" "/o.\dist" ".\install.iss" /d"MyAppVersion=%version%"

echo Building Linux Tarball

mkdir openiz-sdk-%version%
cd openiz-sdk-%version%
copy "..\bin\SignedRelease\Antlr3.Runtime.dll"
copy "..\bin\SignedRelease\ExpressionEvaluator.dll"
copy "..\bin\SignedRelease\jint.dll"
copy "..\bin\SignedRelease\MARC.HI.EHRS.SVC.Auditing.Core.dll"
copy "..\bin\SignedRelease\Microsoft.VisualStudio.QualityTools.UnitTestFramework.dll"
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
copy "..\bin\SignedRelease\OpenIZ.Protocol.Xml.Test.dll"
copy "..\bin\SignedRelease\SharpCompress.dll"
copy "..\bin\SignedRelease\SQLite.Net.dll"
copy "..\bin\SignedRelease\SQLite.Net.Platform.Generic.dll"
copy "..\bin\SignedRelease\sqlite3.dll"
copy "..\bin\SignedRelease\System.Data.Portable.dll"
copy "..\bin\SignedRelease\System.Transactions.Portable.dll"
copy "..\bin\SignedRelease\zlibnet.dll"
copy "..\bin\SignedRelease\zxing.portable.dll"
copy "..\bin\SignedRelease\sqlite3.exe"
copy "..\bin\SignedRelease\minims.exe"
copy "..\..\BrainBug\bin\SignedRelease\brainbug.exe"
copy "..\..\BrainBug\bin\SignedRelease\SharpCompress.dll"
copy "..\..\BrainBug\bin\SignedRelease\zlibnet.dll"
copy "..\..\BrainBug\bin\SignedRelease\brainbug.exe"
copy "..\..\BrainBug\bin\SignedRelease\SharpCompress.dll"
copy "..\..\BrainBug\bin\SignedRelease\zlibnet.dll"
copy "..\..\AppletCompiler\bin\Release\AppletCompiler.exe"
copy "..\..\AppletCompiler\bin\Release\AjaxMin.dll"
copy "..\..\AppletCompiler\bin\Release\OpenIZ.Core.Applets.dll"
copy "..\..\AppletCompiler\bin\Release\OpenIZ.Core.Model.dll"
copy "..\..\..\OpenIZ\bin\Release\LogViewer.exe"
copy "..\..\OpenIZMobile\Assets\Applets\org.openiz.core.pak"
copy "..\bin\SignedRelease\tools\cmdprompt.cmd"
copy "..\..\..\OpenIZ\bin\Release\oizdt.exe"
copy "..\..\..\OpenIZ\bin\Release\MARC.Everest.dll"
copy "..\..\..\OpenIZ\bin\Release\oizdt.config.empty"
copy "..\..\..\OpenIZ\bin\Release\MARC.HI.EHRS.SVC.Core.dll"
copy "..\..\..\OpenIZ\bin\Release\OpenIZ.Core.dll"
copy "..\..\..\OpenIZ\bin\Release\OpenIZ.BusinessRules.JavaScript.dll"
copy "..\..\..\OpenIZ\bin\Release\oizdt.config.empty" ".\openiz.exe.config"
copy "..\..\..\OpenIZ\bin\Release\OpenIZ.Protocol.Xml.dll"
copy "..\..\..\OpenIZ\bin\Release\System.IdentityModel.Tokens.Jwt.dll"
copy "..\..\..\OpenIZ\Solution Items\MARc.HI.EHRS.SVC.Configuration.dll"
mkdir schema
cd schema
copy "..\..\..\..\OpenIZ\bin\Release\Schema\*.xsd"
cd ..
cd ..

"C:\program files\7-zip\7z" a -r -ttar .\dist\openiz-sdk-%version%.tar .\openiz-sdk-%version%
"C:\program files\7-zip\7z" a -r -tzip .\dist\openiz-sdk-%version%.zip .\openiz-sdk-%version%
cd dist

"C:\program files\7-zip\7z" a -tbzip2 .\openiz-sdk-%version%.tar.bz2 .\openiz-sdk-%version%.tar
"C:\program files\7-zip\7z" a -tgzip .\openiz-sdk-%version%.tar.gz .\openiz-sdk-%version%.tar
"C:\Program Files (x86)\Windows Kits\8.1\bin\x86\signtool.exe" sign "openizdc-sdk-%version%.exe"
cd ..
del /q /s openiz-sdk-%version%
rmdir openiz-sdk-%version%\schema
rmdir openiz-sdk-%version%

cd ..\DisconnectedClient


echo Building Windows Installer
"c:\Program Files (x86)\Inno Setup 5\ISCC.exe" "/o.\dist" ".\install.iss" /d"MyAppVersion=%version%"

"C:\Program Files (x86)\Windows Kits\8.1\bin\x86\signtool.exe" sign ".\dist\openizdc-%version%.exe"

