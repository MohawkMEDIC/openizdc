/// <reference path="angular.min.js"/>

/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-7-18
 */

/// <reference path="openiz.js"/>

/**
 * Open IZ Localization for angular
 */

angular.module('openiz', [])
    // Localization service
    .provider('localize', function localizeProvider() {

        this.$get = ['$rootScope', '$filter', function ($rootScope, $filter) {
            var localize = {
                dictionary: OpenIZ.Localization.getStrings(OpenIZ.Localization.getLocale()),
                /**
                 * @summary Sets the locale of the user interface 
                 */
                setLanguage: function (locale) {
                    if (OpenIZ.Localization.getLocale() != locale) {
                        OpenIZ.Localization.setLocale(locale);
                        localize.dictionary = OpenIZ.Localization.getStrings(locale);
                        //$rootScope.$broadcast('localizeResourcesUpdated');
                        //$window.location.reload();
                        //$state.reload();
                        //$rootScope.$applyAsync();
                    }
                },
                /**
                 * @summary Gets the specified locale key
                 */
                getString: function (key) {
                    var entry = localize.dictionary[key];
                    if (entry != null)
                        return entry;
                    else {
                        var oiz = OpenIZ.Localization.getString(key);
                        if (oiz == null)
                            return key;
                        return oiz;
                    }
                }
            };
            return localize;
        }];
    })
    /** 
     * @summary Filter for localization
     * @use {{ KEY | i18n }}
     */
    .filter('i18n', ['$rootScope', 'localize', function ($rootScope, localize) {
        var filterFn = function (key) {
            return localize.getString(key);
        };
        filterFn.$stateful = false;
        return filterFn;
    }])
    .directive('oizTag', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ctrl) {
                ctrl.$parsers.unshift(tagParser);
                function tagParser(viewValue) {
                    return String(viewValue).split(',');
                }

            }
        }
    });