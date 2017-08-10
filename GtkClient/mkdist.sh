#!/bin/sh

cd bin
rm -rfv dist
mkdir dist
cd dist

# create structure
mkdir openizdc-0.9.7.5
cd openizdc-0.9.7.5
mkdir openizdc
mkdir openizdc/bin
mkdir openizdc/lib
cp ../../Release/*.dll ./openizdc/lib
cp ../../Release/*.exe ./openizdc/lib
cp ../../Release/Applets ./openizdc/lib/Applets -r
cp ../../../openizdc.sh ./openizdc/bin
cp ../../../openizdc.desktop ./
cp ../../../openiz.png ./

dh_make -n -s -c apache -e justin.fyfe1@mohawkcollege.ca -y
cd debian
rm *.ex

cat > install << EOF
openizdc/* /usr/share/openiz/client
openizdc.desktop usr/share/applications
openiz.png usr/share/icons
EOF

cat > copyright << EOF
Format: https://www.debian.org/doc/packaging-manuals/copyright-format/1.0/
Upstream-Name: openizdc
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
Source: openizdc
Section: contrib/other
Priority: optional
Maintainer: Justin Fyfe <justin.fyfe1@mohawkcollege.ca>
Build-Depends: debhelper (>=9), mono-runtime (>=4), libwebkit1.1-cil (>=1)
Standards-Version: 3.9.6
Homepage: http://openiz.org
#Vcs-Git: git://anonscm.debian.org/collab-maint/openizdc.git
#Vcs-Browser: https://anonscm.debian.org/cgit/collab-maint/openizdc.git

Package: openizdc
Architecture: any
Description: The OpenIZ disconnected client provides offline access to Immunization Data.
 The OpenIZ disconnected client provides offline access to Immunization Data stored on the OpenIZ IMS platform. The disconnected client synchronizes its data after being offline for a period of weeks or months with the centralized server. All server side business rules and forecasting tools are executed locally while the service is offline. This version of the Disconnected Client is the GTK+ version designed for Linux Operating Systems.


EOF

cd ..
cp ../../../changelog ./debian/
debuild -d
cd ..
cd ..
mkdir ../dist
cp ./bin/dist/*.deb ../dist
