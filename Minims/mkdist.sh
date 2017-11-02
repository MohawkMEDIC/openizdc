#!/bin/sh

cd bin
rm -rfv dist
mkdir dist
cd dist

# create structure
mkdir openiz-sdk-0.9.9.2
cd openiz-sdk-0.9.9.2
mkdir openizsdk
mkdir openizsdk/bin
mkdir openizsdk/lib
cp ../../Release/*.dll ./openizsdk/lib
cp ../../Release/*.exe ./openizsdk/lib
cp ../../../../BrainBug/bin/Release/*.exe ./openizsdk/lib
cp ../../../../AppletCompiler/bin/Release/*.exe ./openizsdk/lib
cp ../../../../AppletCompiler/bin/Release/AjaxMin.dll ./openizsdk/lib
cp ../../../unix/*.sh ./openizsdk/bin
cp ../../../../GtkClient/Applets ./openizsdk/lib/Applets -r
dh_make -n -s -c apache -e justin.fyfe1@mohawkcollege.ca -y
cd debian
rm *.ex

cat > install << EOF
openizsdk/* /usr/share/openiz/sdk
EOF

cat > copyright << EOF
Format: https://www.debian.org/doc/packaging-manuals/copyright-format/1.0/
Upstream-Name: openiz-sdk
Source: <url://openiz.org>

Files: *
Copyright: 2015-2017 Mohawk College of Applied Arts and Technology <medic@mohawkcollege.ca>
License: Apache-2.0

License: Apache-2.0
 Licensed under the Apache License, Version 2.0 (the "License");
 you may not use this file except in compliance with the License.
 You may obtain a copy of the License at
 .
 https://www.apache.org/licenses/LICENSE-2.0
 .
 Unless required by applicable law or agreed to in writing, software
 distributed under the License is distributed on an "AS IS" BASIS,
 WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 See the License for the specific language governing permissions and
 limitations under the License.
 .
 On Debian systems, the complete text of the Apache version 2.0 license
 can be found in "/usr/share/common-licenses/Apache-2.0".

EOF

cat > control <<EOF
Source: openiz-sdk
Section: contrib/other
Priority: optional
Maintainer: Justin Fyfe <justin.fyfe1@mohawkcollege.ca>
Build-Depends: debhelper (>=9), mono-runtime (>=4)
Standards-Version: 3.9.6
Homepage: http://openiz.org
#Vcs-Git: git://anonscm.debian.org/collab-maint/openizdc.git
#Vcs-Browser: https://anonscm.debian.org/cgit/collab-maint/openizdc.git

Package: openiz-sdk
Architecture: any
Section: contrib/other
Priority: optional
Depends: ca-certificates-mono (>=4), libmono-2.0-1 (>=4), libmono-cil-dev (>=4), mono-4.0-gac (>=4), mono-4.0-service (>=4), mono-jay (>=4), mono-runtime (>=4), mono-runtime-sgen (>=4), mono-utils (>=4), mono-devel (>=4)
Homepage: http://openiz.org
Description: The OpenIZ Software Development Kit.
 The OpenIZ Software Development Kit allows developers of OpenIZ Applets (clinical protocols, user interfaces, business rules) to debug and test applications on the Linux Operating System.

EOF

cd ..
cp ../../../changelog ./debian/
debuild -d
cd ..
cd ..
cd ..
