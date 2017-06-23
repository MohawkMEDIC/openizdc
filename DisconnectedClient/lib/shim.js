// OpenIZ Self-Hosted SHIM


OpenIZApplicationService.GetStatus = function () {
    return OpenIZApplicationServer.getStatus();
}

OpenIZApplicationService.ShowToast = function (string) {
    return OpenIZApplicationService.ShowToast(string);
}

OpenIZApplicationService.GetOnlineState = function () {
    return OpenIZApplicationService.getOnlineState();
}

OpenIZApplicationService.IsAdminAvailable = function () {
    return OpenIZApplicationService.isAdminAvailable();
}

OpenIZApplicationService.IsClinicalAvailable = function () {
    return OpenIZApplicationService.isClinicalAvailable();
}

OpenIZApplicationService.BarcodeScan = function () {
    return OpenIZApplicationService.barcodeScan();
}

OpenIZApplicationService.Close = function () {
    return OpenIZApplicationService.close();
}

OpenIZApplicationService.GetLocale = function () {
    return OpenIZApplicationService.getLocale();
}

OpenIZApplicationService.SetLocale = function (lang) {
    return OpenIZApplicationService.setLocale(lang);
}


OpenIZApplicationService.NewGuid = function () {
    return OpenIZApplicationService.newGuid();
}