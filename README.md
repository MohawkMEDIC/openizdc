# OpenIZ Disconnected Client
The Open Immunize Disconnected Client (OpenIZ DC) is a complementary application to the OpenIZ IMS (Immunization Management Service). The IMS provides a headless service which is responsible for maintaining immunization state, centralized reporting, and forecasting. The disconnected client represents a mobile application framework which is capable of editing the data contained on the IMS in both online and offline mode.

There is no need to worry about complicated forking of the disconnected client core. The disconnected client downloads the applets installed on 
any server it is joined to. You can use the vanilla Disconnected Client with any OpenIZ IMS server, and the user interface, business rules, 
protocols, etc. from the server are automatically downloaded into whatever device you're using.

## Major Features

The major features of the disconnected client are:

+ Extensible applet framework based on HTML5 + JavaScript
+ Ability to work offline for weeks at a time and synchronize data when internet connectivity is available
+ Secured and encrypted local storage so data is safe offline
+ OTA update capability for applets, so features are simultaneously pushed to clients when internet is available
+ Full offline security auditing
+ Full offline applications of business rules and clinical protocols

The disconnected client currently runs on three platforms:

+ Android 4.4 (KitKat) +
+ Microsoft Windows 7+
+ Linux Operating Systems running Mono 5.x

## Android 4.4

The android version of the disconnected client is perfect for small clinics or those looking to have the ultimate in mobile medical records. 
The android version is written in Xamarin and supports Android KitKat devices running the Android System Web View 45 or greater. 

## Microsoft Windows 

The Microsoft Windows version of the disconnected client is perfect for medium sized clinics which server 10,000 patients or more. The version
is highly optimized for large environments and designed to operate quickly. The Windows Version is written in Microsoft .NET 4.5.2 and the 
included installer provides all the necessary dependencies for you.

## Linux Operating Systems (beta)

There is a GTK+ version of the Disconnected Client which is currently under active development. This version, like the Windows version, is meant to 
provide clinics with larger populations an opportunity to leverage all the features of the OpenIZ disconnected client. It currently relies
on GTK+ 2.x, WebKit and Mono 5.x. 

We are hoping to get Ubuntu packages ready for this version of the application soon.
