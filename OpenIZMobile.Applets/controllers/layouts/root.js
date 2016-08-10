/// <reference path="~/js/openiz-model.js"/>

/// <reference path="~/js/openiz.js"/>
/// <reference path="~/lib/angular.min.js"/>
var layoutApp = angular.module('layout', ['openiz']).run(function ($rootScope) {

    $rootScope.system = {};
    $rootScope.system.config = {};
    $rootScope.system.config.realmName = OpenIZ.Configuration.getRealm();
    $rootScope.page = {
        title: OpenIZ.App.getCurrentAssetTitle(),
        loadTime: new Date(),
        maxEventOccranceTime: new Date(), // Dislike Javascript
        minAppointmentTime: new Date(), // quite a bit
        locale: OpenIZ.Localization.getLocale()
    };

    $rootScope.page.maxEventOccranceTime.setDate($rootScope.page.maxEventOccranceTime.getDate() + 1); // <-- This is why
    $rootScope.page.minAppointmentTime.setDate($rootScope.page.minAppointmentTime.getDate() - 1); // why I can't call addDays or something?

    // Get current session
    $rootScope.session = OpenIZ.Authentication.getSession();

    $rootScope.OpenIZ = OpenIZ;
});

// Configure the safe ng-urls
layoutApp.config(['$compileProvider', function ($compileProvider) {
    $compileProvider.aHrefSanitizationWhitelist(/^\s*(http|tel):/);
    $compileProvider.imgSrcSanitizationWhitelist(/^\s*(http|tel):/);
}]);

angular.element(document).ready(function () {
    $("[data-toggle=popover]").popover({ container: 'body' });
    $('[data-toggle=popover]').on('shown.bs.popover', function () {
        $('[data-toggle=popover]').not(this).popover('hide');
    })
    $('#initialBlock').remove();
    $('#waitModal').removeClass('in');
    $('#waitModal').removeAttr('style');
    //OpenIZ.locale = OpenIZ.Localization.getLocale();
});
