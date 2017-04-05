/// <reference path="~/js/openiz-model.js"/>
/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-7-23
 */

/// <reference path="~/js/openiz.js"/>
/// <reference path="~/lib/toastr.min.js"/>
/// <reference path="~/lib/angular.min.js"/>
var layoutApp = angular.module('layout', ['openiz', 'ngSanitize', 'ui.router', 'angular.filter'])
    .config(['$compileProvider', '$stateProvider', '$urlRouterProvider', function ($compileProvider, $stateProvider, $urlRouterProvider) {
        $compileProvider.aHrefSanitizationWhitelist(/^\s*(http|tel):/);
        $compileProvider.imgSrcSanitizationWhitelist(/^\s*(http|tel):/);

        var hasStartup = false;
        OpenIZ.UserInterface.states.forEach(function (state) {
            hasStartup |= state.url == '/';
            $stateProvider.state(state);
        });

        if (!hasStartup)
            $stateProvider.state({
                name: 'org-openiz-core-index', url: '/', abstract: false, views: {
                    '': { controller: '', templateUrl: '/org.openiz.core/views/landing.html' },
                }
            });

        //$urlRouterProvider.otherwise('/');

    }])
    .run(function ($rootScope) {

        $rootScope.isLoading = true;
        $rootScope.extendToast = null;

        // HACK: Sometimes HASH is empty ... ugh... 
        // Once we fix the panels and tabs in BS this can be removed
        if (window.location.hash == "")
            window.location.hash = "#/";

        OpenIZ.Configuration.getConfigurationAsync({
            continueWith: function (config) {
                $rootScope.system = {};
                $rootScope.system.config = config;
                $rootScope.$apply();
            }
        });

        $rootScope.$on("$stateChangeError", function () {
            console.log.bind(console);
            OpenIZ.App.hideWait();
            $rootScope.isLoading = false;
        });
        $rootScope.$on('$stateChangeStart', function (event, toState, toParams, fromState, fromParams) {
            if ($('.modal.in').length > 0 || $('.modal-backdrop').length > 0) {
                $('.modal-open').removeClass('modal-open');
                $('.modal-backdrop').remove();
                $('body').css('padding-right', '');
            }
            window.scrollTo(0, 0);
            $rootScope.isLoading = true;
        });
        $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
            OpenIZ.App.hideWait();
            $rootScope.isLoading = false;
        });

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

            if ($rootScope.session && ($rootScope.session.exp - new Date() < 120000))
            {
                var expiry = Math.round(($rootScope.session.exp - new Date()) / 1000);
                var mins = Math.trunc(expiry / 60),
                    secs = expiry % 60;
                if (("" + secs).length < 2)
                    secs = "0" + secs;
                expiryStr = mins + ":" + secs;
                var authString = OpenIZ.Localization.getString("locale.session.aboutToExpirePrefix") + expiryStr + OpenIZ.Localization.getString("locale.session.aboutToExpireSuffix");

                if(expiry < 0)
                    window.location.reload(true);
                else if (!$rootScope.extendToast)
                    $rootScope.extendToast = toastr.error(authString, OpenIZ.Localization.getString("locale.session.expiration"),  {
                        closeButton: false,
                        preventDuplicates: true,
                        onclick: function () {
                            OpenIZ.Authentication.refreshSessionAsync({
                                continueWith: function (s) {
                                    $rootScope.session = s;
                                },
                                onException: function (e) {
                                    if (e.message) OpenIZ.App.toast(e.message);
                                    else console.error(e);
                                }
                            });
                            $rootScope.extendToast = null;
                            toastr.clear();
                        },
                        positionClass: "toast-bottom-center",
                        timeOut: 0,
                        extendedTimeOut: 0
                    });
                else
                    $($rootScope.extendToast).children('.toast-message').html(authString);
                    //$rootScope.extendToast.show();
            }
            else {
                
            }
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

                OpenIZ.Configuration.getUserPreferencesAsync({
                    continueWith: function (prefs) {
                        $rootScope.session.prefs = {};
                        for (var p in prefs.application.setting) {
                            var set = prefs.application.setting[p];
                            $rootScope.session.prefs[set.key] = set.value;
                        }
                        $rootScope.$apply();
                    }
                });
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


layoutApp.service('uiHelperService', [function () {
    function setDropdownPosition(dropdownButton, dropdownMenu) {
        if (dropdownMenu.outerWidth() > (dropdownButton.parent().width() + dropdownButton.offset().left)) {
            dropdownMenu.addClass('dropdown-menu-left');
            dropdownMenu.removeClass('dropdown-menu-right');
        }
        else {
            dropdownMenu.addClass('dropdown-menu-right');
            dropdownMenu.removeClass('dropdown-menu-left');
        }
    }

    var uiHelperService = {
        setDropdownPosition: setDropdownPosition
    }

    return uiHelperService;
}]);

