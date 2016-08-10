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

                    // make sure we always have the latest locale
                    //localize.dictionary = OpenIZ.Localization.getStrings(OpenIZ.Localization.getLocale());

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
    .filter('oizEntityIdentifier', function() {
        return function (modelValue) {
            if (modelValue.NID !== undefined)
                return modelValue.NID.value;
            else
                for (var k in modelValue)
                    return modelValue.NID;

        };
    })
    .filter('oizEntityName', function () {
        return function(modelValue) {
            if (modelValue === null || modelValue === undefined)
                return "";
            else if (modelValue.join !== undefined)
                return modelValue.join(' ');
            else if (modelValue.component !== undefined) {
                var nameStr = "";
                if (modelValue.component.Given !== undefined) {
                    if (typeof (modelValue.component.Given) === "string")
                        nameStr += modelValue.component.Given;
                    else if (modelValue.component.Given.join !== undefined)
                        nameStr += modelValue.component.Given.join(' ');
                    nameStr += " ";
                }
                if (modelValue.component.Family !== undefined) {
                    if (typeof (modelValue.component.Family) === "string")
                        nameStr += modelValue.component.Family;
                    else if (modelValue.component.Family.join !== undefined)
                        nameStr += modelValue.component.Family.join(' ');
                }
                if (modelValue.component.$other !== undefined) {
                    if (typeof (modelValue.component.$other) === "string")
                        nameStr += modelValue.component.$other;
                    else if (modelValue.component.$other.join !== undefined)
                        nameStr += modelValue.component.$other.join(' ');
                    else if(modelValue.component.$other.value !== undefined)
                        nameStr += modelValue.component.$other.value;

                }
                return nameStr;
            }
            else
                return modelValue;
        }
    })
    .directive('oizTag', function () {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ctrl) {
                ctrl.$parsers.unshift(tagParser);
                ctrl.$formatters.unshift(tagFormatter);
                function tagParser(viewValue) {
                    return String(viewValue).split(',');
                }
                function tagFormatter(viewValue) {
                    if (typeof (viewValue) === Array)
                        return viewValue.join(viewView)
                    return viewValue;
                }
            }
        }
    })
    .directive('oizCollapseindicator', function () {
        return {
            link: function (scope, element, attrs, ctrl) {
                $(element).on('hide.bs.collapse', function () {
                    var indicator = $(this).attr('data-oiz-chevron');
                    $(indicator).removeClass('glyphicon-chevron-down');
                    $(indicator).addClass('glyphicon-chevron-right');
                });
                $(element).on('show.bs.collapse', function () {
                    var indicator = $(this).attr('data-oiz-chevron');
                    $(indicator).addClass('glyphicon-chevron-down');
                    $(indicator).removeClass('glyphicon-chevron-right');
                });
            }
        };
    });