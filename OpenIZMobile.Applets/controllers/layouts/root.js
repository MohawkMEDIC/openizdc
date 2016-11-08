/// <reference path="~/js/openiz-model.js"/>

/// <reference path="~/js/openiz.js"/>
/// <reference path="~/lib/angular.min.js"/>
var layoutApp = angular.module('layout', ['openiz', 'ngSanitize', 'ui.router'])
    .config(['$compileProvider', '$stateProvider', '$urlRouterProvider', function ($compileProvider, $stateProvider, $urlRouterProvider) {
        $compileProvider.aHrefSanitizationWhitelist(/^\s*(http|tel):/);
        $compileProvider.imgSrcSanitizationWhitelist(/^\s*(http|tel):/);

        OpenIZ.UserInterface.states.forEach(function (state) {
            $stateProvider.state(state);
        });

        //$urlRouterProvider.otherwise('/');

    }])
    .run(function ($rootScope) {
        OpenIZ.Configuration.getConfigurationAsync({
            continueWith: function (config) {
                $rootScope.system = {};
                $rootScope.system.config = config;
                $rootScope.$apply();
            }
        })
        $rootScope.$on("$stateChangeError", console.log.bind(console));

        $rootScope.page = {
            title: OpenIZ.App.getCurrentAssetTitle(),
            loadTime: new Date(),
            maxEventTime: new Date().tomorrow(), // Dislike Javascript
            minEventTime: new Date().yesterday(), // quite a bit
            locale: OpenIZ.Localization.getLocale(),
            onlineState: OpenIZ.App.getOnlineState()
        };

        setInterval(function () {
            $rootScope.page.onlineState = OpenIZ.App.getOnlineState();
            $rootScope.$applyAsync();
        }, 10000);


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


/**
 * @summary The queryUrlParameterService is used to get url parameters.
 *
 * @description The purpose of this service is to get url parameters.
 * @namespace queryUrlParameterService
 */
layoutApp.service('queryUrlParameterService', [function () {

    /**
     * @summary Gets the url parameters of the current page and returns an object with the URL parameter names and values as key-value pairs.
     * @memberof queryUrlParameterService
     * @method
     * @example
     * var params = getUrlParameters();
     * var id = params.id;
     * @returns An object of parameters from the URL of the current page.
     */
    function getUrlParameters() {
        var url = window.location.href;
        var regex = /[\?|\&]([^=]+)\=([^&]+)/g;
        var params = {};

        while ((match = regex.exec(url)) != null) {
            params[match[1]] = match[2];
        }

        return params;
    }

    var parameterService = {
        getUrlParameters: getUrlParameters
    }

    return parameterService;
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
