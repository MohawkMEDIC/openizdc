// OpenIZ Self-Hosted SHIM


OpenIZApplicationService.GetStatus = function () {
    return window.external.GetStatus();
}

OpenIZApplicationService.ShowToast = function (string) {
    return window.external.ShowToast(string);
}

OpenIZApplicationService.GetOnlineState = function () {
    return window.external.GetOnlineState();
}

OpenIZApplicationService.IsAdminAvailable = function () {
    return window.external.IsAdminAvailable();
}

OpenIZApplicationService.IsClinicalAvailable = function () {
    return window.external.IsClinicalAvailable();
}

OpenIZApplicationService.BarcodeScan = function () {
    return window.external.BarcodeScan();
}

OpenIZApplicationService.Close = function () {
    return window.external.Close();
}

OpenIZApplicationService.GetLocale = function () {
    return window.external.GetLocale();
}

OpenIZApplicationService.SetLocale = function (lang) {
    return window.external.SetLocale(lang);
}


OpenIZApplicationService.NewGuid = function () {
    return window.external.NewGuid();
}

navigator.userAgent = window.external.GetMagic();