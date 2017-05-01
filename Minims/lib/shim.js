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

OpenIZApplicationService.Close = function () {
    alert("You need to restart the MiniIMS service for the changes to take effect");
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