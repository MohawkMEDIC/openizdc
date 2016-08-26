/// <reference path="~/js/openiz-model.js"/>

/// <reference path="~/js/openiz.js"/>
/// <reference path="~/lib/angular.min.js"/>
var layoutApp = angular.module('layout', ['openiz', 'ngSanitize']).run(function ($rootScope) {

    $rootScope.system = {};
    $rootScope.system.config = {};
    $rootScope.system.config.realmName = OpenIZ.Configuration.getRealm();
    $rootScope.page = {
        title: OpenIZ.App.getCurrentAssetTitle(),
        loadTime: new Date(),
        maxEventTime: new Date(), // Dislike Javascript
        minEventTime: new Date(), // quite a bit
        locale: OpenIZ.Localization.getLocale(),
        onlineState : OpenIZ.App.getOnlineState()
    };

    setInterval(function () {
        $rootScope.page.onlineState = OpenIZ.App.getOnlineState();
        $rootScope.$applyAsync();
    }, 10000);

    $rootScope.page.maxEventTime.setDate($rootScope.page.maxEventTime.getDate() + 1); // <-- This is why
    $rootScope.page.minEventTime.setDate($rootScope.page.minEventTime.getDate() - 1); // why I can't call addDays or something?

    // Get current session
    OpenIZ.Authentication.getSessionAsync({
        continueWith: function (session) {
            $rootScope.session = session;
            if (session != null && session.entity != null) {
                session.entity.telecom = session.entity.telecom || {};
                if (Object.keys(session.entity.telecom).length == 0)
                    session.entity.telecom.MobilePhone = { value: "" };
            }
            $rootScope.$apply();
        }
    });

    $rootScope.changeInputType = function (controlId, type) {
        $(controlId).attr('type', type);
        if ($(controlId).attr('data-max-' + type) != null) {
            $(controlId).attr('max', $(controlId).attr('data-max-' + type));
        }
    };

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
