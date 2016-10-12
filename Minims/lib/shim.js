// OpenIZ Self-Hosted SHIM


OpenIZApplicationService.GetStatus = function () {
    return '[ "Dummy Status", 0 ]';
}

OpenIZApplicationService.ShowToast = function (string) {
    console.info("TOAST: " + string);
}

OpenIZApplicationService.GetOnlineState = function () {
    return true;
}

OpenIZApplicationService.BarcodeScan = function () {
    return OpenIZApplicationService.NewGuid().substring(0, 8);
}