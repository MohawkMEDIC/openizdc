﻿This documentation represents the OpenIZ JavaScript bridge documentation.   

## What is the bridge?   
       
Put simply, the JavaScript bridge allows OpenIZ applets to interface with their host container without   
having to worry about the implementation of the host. OpenIZ applets include openiz.js/openiz-model.js   
and these files allow the applet to interact with the container & IMSI services.   

Containers will customize the openiz.js file to imlpement the basic OpenIZ interaction contracts. For    
example, if you're running an applet on iOS in the disconnected client, the iOS openiz.js file served   
to your applet will interact with Safari's integration. If you're using the default OpenIZ disconnected   
client written in Xamarin, the openiz.js file interacts with Xamarin's [JavascriptInterface] classes.   

Helpful links:   

* [Applet Schema](http://openiz.org/artifacts/0.1pre/applet "OpenIZ Applet Schema") which contains information about the format of applets   
* [IMSI Schema](http://openiz.org/artifacts/0.1pre/imsi "IMSI Schema") which contains information about the format of IMSI objects   
* [OpenIZ Disconnected Client](http://openizdc.codeplex.com "OpenIZ Xamarin disconnected client") project on codeplex.   

## License   

The bridge files are released under the Apache 2.0 license. Specific implementations of the bridge   
may carry a different license.  
