﻿/// <reference path="~/js/openiz-model.js"/>
/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-10-30
 */

/// <reference path="~/js/openiz.js"/>
/// <reference path="~/lib/toastr.min.js"/>
/// <reference path="~/lib/angular.min.js"/>
var layoutApp = angular.module('layout', ['openiz', 'ngSanitize', 'ui.router', 'angular.filter', 'oc.lazyLoad'])
    .config(['$compileProvider', '$stateProvider', '$urlRouterProvider', '$httpProvider', function ($compileProvider, $stateProvider, $urlRouterProvider, $httpProvider) {
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

        //$httpProvider.interceptors.push('authInterceptor');
        //$urlRouterProvider.otherwise('/');

    }])
    .run(function ($rootScope, $state, $templateCache) {

        angular.element(document).ready(init);

        function init() {
            $rootScope.isLoading = true;
            $rootScope.extendToast = null;
            $rootScope.$$oizStateDataList = [];

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

                // Update ages of preserve states and set state if needed
                var hasSavedState = false;
                delete (event.targetScope.$$oizState);
                for (var v in $rootScope.$$oizStateDataList) {
                    hasSavedState |= $rootScope.$$oizStateDataList[v].state == fromState.name;
                    $rootScope.$$oizStateDataList[v].age = ($rootScope.$$oizStateDataList[v].age || 0) + 1;
                    if ($rootScope.$$oizStateDataList[v].state == toState.name) {
                        event.targetScope.$$oizState = $rootScope.$$oizStateDataList[v];
                        $rootScope.$$oizStateDataList.splice(v, 1);
                    }
                }

                $rootScope.isLoading = true;
                toastr.clear();
                $rootScope.extendToast = null;
                if ($rootScope.confirmNavigation && !hasSavedState && !$rootScope.confirmNavigation(event, fromState)) {
                    event.preventDefault();
                    $state.go(fromState.name);
                    $rootScope.isLoading = false;
                }
                else {
                    if ($('.modal.in').length > 0 || $('.modal-backdrop').length > 0) {
                        $('.modal-open').removeClass('modal-open');
                        $('.modal-backdrop').remove();
                        $('body').css('padding-right', '');
                    }
                    $('.select2-container').remove();

                    window.scrollTo(0, 0);
                    $rootScope.isLoading = true;
                }
            });
            $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
                OpenIZ.App.hideWait();
                $rootScope.isLoading = false;

                // Refresh the session ?
                if ($rootScope.session && ($rootScope.session.exp - new Date() < 240000)) { // The session is about to expire so let's extend it!
                    OpenIZ.Authentication.refreshSessionAsync({
                        continueWith: function (s) {
                            $rootScope.session = s;
                        },
                        onException: function (e) {
                            console.error(e);
                        }
                    });
                }

            });

            $rootScope.page = {
                title: OpenIZ.App.getCurrentAssetTitle(),
                loadTime: new Date(),
                maxEventTime: new Date().tomorrow().trunc().addSeconds(-1), // Dislike Javascript
                minEventTime: new Date().yesterday(), // quite a bit
                locale: OpenIZ.Localization.getLocale(),
                onlineState: OpenIZ.App.getOnlineState(),
                currentTime: new Date(),
            };

            // Interval to refresh online state and to check session
            setInterval(function () {
                $rootScope.page.onlineState = OpenIZ.App.getOnlineState();
                if ($("#onlineStateIndicator")[0]) {
                    if ($rootScope.page.onlineState)
                        $("#onlineStateIndicator")[0].style.display = 'none';
                    else
                        $("#onlineStateIndicator")[0].style.display = 'inline-block';
                }

                $rootScope.page.currentTime = new Date();
                $rootScope.page.maxEventTime = new Date().tomorrow().trunc().addSeconds(-1); // Dislike Javascript
                $rootScope.page.minEventTime = new Date().yesterday(); // quite a bit

                if ($rootScope.session && ($rootScope.session.exp - new Date() < 120000)) {
                    var expiry = Math.round(($rootScope.session.exp - new Date()) / 1000);
                    var mins = Math.trunc(expiry / 60),
                        secs = expiry % 60;
                    if (("" + secs).length < 2)
                        secs = "0" + secs;
                    expiryStr = mins + ":" + secs;
                    var authString = OpenIZ.Localization.getString("locale.session.aboutToExpirePrefix") + " " + expiryStr + " " + OpenIZ.Localization.getString("locale.session.aboutToExpireSuffix");

                    if (expiry < 0) {
                        OpenIZ.Authentication.abandonSession({
                            continueWith:function() {
                                $rootScope.session = null;
                                $templateCache.removeAll();
                                $state.reload();
                                toastr.clear();
                            }
                        });
                    }
                    else if (!$rootScope.extendToast)
                        $rootScope.extendToast = toastr.error(authString, OpenIZ.Localization.getString("locale.session.expiration"), {
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
                    else {
                        $($rootScope.extendToast).children('.toast-message').html(authString);
                    }
                }
                else {
                    toastr.clear();
                }
                //$rootScope.$apply();
            }, 10000);


            // Get current session

            $rootScope.changeInputType = function (controlId, type) {
                $(controlId).attr('type', type);
                if ($(controlId).attr('data-max-' + type) != null) {
                    $(controlId).attr('max', $(controlId).attr('data-max-' + type));
                }
            };

            $rootScope.initSessionVars = function () {
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
                            },
                            onException: function (ex) { console.error(ex); }
                        });
                        $rootScope.$apply();
                    },
                    onException: function (ex) {
                        console.error(ex);
                    }
                });
            };

            $rootScope.initSessionVars();

            $rootScope.$watch('session', function (nv, ov) {
                if (nv) {
                    OpenIZ.App.getInfoAsync({
                        includeUpdates: true,
                        continueWith: function (data) {

                            if (data.update && data.update.length > 0) {
                                toastr.info(OpenIZ.Localization.getString("locale.about.updateToast.text"), OpenIZ.Localization.getString("locale.about.updateToast.title"), {
                                    closeButton: false,
                                    preventDuplicates: true,
                                    onclick: function () {
                                        window.location.hash = "/core/about";
                                    },
                                    positionClass: "toast-bottom-center",
                                    timeOut: 4000,
                                    extendedTimeOut: 0
                                });
                            }
                        },
                        onException: function (ex) {
                            console.error(ex);
                        }
                    });

                }
            });

            // Set the language for select2 localization
            var locale = OpenIZ.Localization.getLocale();

            // HACK: With the current version of select2 we cannot re-define the en locale, so lets change english to en-ca
            // so the english localization can be used and we can fix the unicode character issues
            if (locale === "en") {
                locale = "en-ca";
            }

            $.fn.select2.defaults.set('language', locale);

            // This fixes modal scrolling when closing a nested modal
            $(document).on('hidden.bs.modal', function () {

                if ($('.modal.in').length > 0) {
                    $('body').addClass('modal-open');
                }
            });
        }
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

    function initializePopups(direction) {
        $('[data-toggle="popover"]').popover({
            placement: direction
        });

        $('[data-toggle="popover"]').on('shown.bs.popover', function (event) {
            var offset = $('div.popover').offset().top - $(document).scrollTop();
            if (offset < 10) {
                $('div.popover').css('top', parseInt($('div.popover').css('top')) + 10 - offset);
            }
        });
    }

    var uiHelperService = {
        setDropdownPosition: setDropdownPosition,
        initializePopups: initializePopups
    }

    return uiHelperService;
}]);

layoutApp.service('regexService', [function () {
    var regexService = {
        passwordRegex: /^(?=.*[a-z])(?=.*[A-Z])[a-zA-Z\d]{8,}$/
    };

    return regexService;
}]);
