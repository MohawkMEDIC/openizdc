#!/bin/sh
rm -rfv ./dist
cd GtkClient
./mkdist.sh
cd ..
cd Minims
./mkdist.sh
cd ..

mkdir ./dist
cp ./GtkClient/bin/dist/*.deb ./dist
cp ./Minims/bin/dist/*.deb ./dist