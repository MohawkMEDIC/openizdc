// OpenIZ Self-Hosted SHIM

OpenIZApplicationService.GetVersion = function () {
    return "x.x.x.x";
}

OpenIZApplicationService.GetStatus = function () {
    return '[ "Dummy Status", 0 ]';
}

OpenIZApplicationService.ShowToast = function (string) {
    console.info("TOAST: " + string);
}

OpenIZApplicationService.GetOnlineState = function () {
    return true;
}