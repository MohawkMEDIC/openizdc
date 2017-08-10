// OpenIZ Self-Hosted SHIM
OpenIZApplicationService.GetStatus = function () {
    return window.status;
}

OpenIZApplicationService.ShowToast = function (string) {
    console.info("TOAST: " + string);
}

OpenIZApplicationService.GetOnlineState = function () {
    return navigator.onLine;
}

OpenIZApplicationService.IsAdminAvailable = function () {
    return navigator.onLine;
}

OpenIZApplicationService.IsClinicalAvailable = function () {
    return navigator.onLine;
}

OpenIZApplicationService.BarcodeScan = function () {
    return OpenIZApplicationService.NewGuid().substring(0, 8);
}

OpenIZApplicationService.Close = function () {
    window.close();
}

OpenIZApplicationService.GetLocale = function () {
    return (navigator.language || navigator.userLanguage).substring(0, 2);
}

OpenIZApplicationService.NewGuid = function () {
    var retVal = "";
    $.ajax({
        url: "/__app/uuid",
        success: function (data) { retVal = data; },
        async: false
    });
    return retVal;
}

OpenIZApplicationService.GetMagic = function () {
    return navigator.userAgent.substring(10);
}