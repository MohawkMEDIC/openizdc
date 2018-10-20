/// <reference path="angular.min.js"/>
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

/// <reference path="openiz.js"/>
/// <reference path="openiz-model.js"/>
/**
 * @version 0.9.6 (Edmonton)
 * @copyright (C) 2015-2018, Mohawk College of Applied Arts and Technology
 * @license Apache 2.0
 */

/**
 * @summary Represents OpenIZ bindings for angular.
 * @description The purpose of these functions are to provide a mechanism to leverage OpenIZ functionality within AngularJS
 * @class Angular
 */
angular.module('openiz', [])
    //.factory('authInterceptor', ['$log', function ($log) {
    //    return {
    //        request: function (config) {
    //            if (OpenIZ.Authentication.$session)
    //                config.headers['Authorization'] = 'BEARER ' + OpenIZ.Authentication.$session.id;
    //            if (!OpenIZ.App.magic)
    //                OpenIZ.App.magic = OpenIZApplicationService.GetMagic();
    //            config.headers["X-OIZMagic"] = OpenIZ.App.magic;
    //            return config;
    //        }
    //    };
    //}])
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
     * @method i18n
     * @memberof Angular
     * @summary Filter for localization
     * @use {{ KEY | i18n }}
     * @description This will translate the specified string into the current language per the manifest file. The mobile application (on android) will pre-process locatlizations so this use of the the filter will be a static translation
     * @example 
     *     <span class="label label-info">{ 'locale.someString' | i18n }</span>
     */
    .filter('i18n', ['$rootScope', 'localize', function ($rootScope, localize) {
        var filterFn = function (key) {
            return localize.getString(key);
        };
        filterFn.$stateful = false;
        return filterFn;
    }])
    /** 
     * @method translate
     * @memberof Angular
     * @summary Filter for localization
     * @use {{ KEY | translate }}
     * @description This filter is a dynamic translation which is not pre-processed and is intead done in JavaScript. On platforms with limited resources this may not be desirable.
     * @example 
     *     <span class="label label-info">{ 'locale.someString' | translate }</span>
     */
    .filter('translate', function () {
        return function (input) {
            return OpenIZ.Localization.getString(input);
        };
    })
    /** 
     * @deprecated
     */
    .filter('orderStatus', function () {
        return function (moodConcept, statusConcept) {
            if (moodConcept == OpenIZModel.ActMoodKeys.Eventoccurrence && statusConcept == OpenIZModel.StatusKeys.Active) {
                return "locale.stock.label.issued";
            } else if (moodConcept == OpenIZModel.ActMoodKeys.Eventoccurrence && statusConcept == OpenIZModel.StatusKeys.Completed) {
                return "locale.stock.label.fulfilled";
            } else if (statusConcept == OpenIZModel.StatusKeys.Obsolete) {
                return "locale.stock.label.cancelled";
            }
            return "locale.stock.label.pending";
        };
    })
     /** 
     * @deprecated
     */
    .filter('orderLabel', function () {
        return function (moodConcept, statusConcept) {
            if (moodConcept == OpenIZModel.ActMoodKeys.Eventoccurrence && statusConcept == OpenIZModel.StatusKeys.Active) {
                return "info";
            } else if (moodConcept == OpenIZModel.ActMoodKeys.Eventoccurrence && statusConcept == OpenIZModel.StatusKeys.Completed) {
                return "success";
            } else if (statusConcept == OpenIZModel.StatusKeys.Obsolete) {
                return "danger";
            }
            return "default";
        };
    })
    /**
     * @method oizSum
     * @memberof Angular
     * @summary Sums the values from the model (an array) together 
     * @example
     *      <span class="label label-info">{{ 'locale.total' | i18n }} : {{ act.participation | oizSum: 'quantity' }}</span>
     */
    .filter('oizSum', function () {
        return function (modelValue, propname) {
            // TODO: Find a better function for doing this
            var sum = 0;
            if (!Array.isArray(modelValue))
                ;
            else if (!propname) {
                for (var i in modelValue)
                    sum += modelValue[i]
            }
            else
                for (var i in modelValue)
                    sum += modelValue[i][propname];
            return sum;
        };
    })
    /**
     * @method oizEntityIdentifier
     * @memberof Angular
     * @summary Renders a model value which is an EntityIdentifier in a standard way
     * @see {OpenIZModel.EntityIdentifier}
     * @example
     *      <div class="col-md-2">{{ patient.identifier | oizEntityIdentifier }}</div>
     */
    .filter('oizEntityIdentifier', function () {
        return function (modelValue) {
            if (modelValue === undefined)
                return "";
            if (modelValue.NID !== undefined)
                return modelValue.NID.value;
            else
                for (var k in modelValue)
                    return modelValue[k].value;
        };
    })
    /**
     * @method oizConcept
     * @memberof Angular
     * @summary Renders a model concept into a standard display using the concept's display name
     * @see {OpenIZModel.Concept}
     * @example
     *      <div class="col-md-2">Gender:</div>
     *      <div class="col-md-2">{{ patient.genderConceptModel | oizConcept }}</div>
     */
    .filter('oizConcept', function () {
        return function (modelValue) {
            if (modelValue != null && modelValue.name != null)
                return OpenIZ.Util.renderConceptName(modelValue.name);
        }
    })
    /**
     * @method oizEntityName
     * @memberof Angular
     * @summary Renders an entity's name in a standard display format
     * @example
     *      <div class="col-md-2">Name:</div><div class="col-md-4">{{ patient.name.OfficialRecord | oizEntityName }}</div>
     */
    .filter('oizEntityName', function () {
        return function (modelValue) {
            return OpenIZ.Util.renderName(modelValue);
        }
    })
    /** 
     * @method oizEntityAddress
     * @memberof Angular
     * @summary Renders an entity's address in a standardized display form
     * @description This function will render the entity's specified address in the format Street, Precinct, City, County, State, Country
     * @example
     *      <div class="col-md-2">Address:</div><div class="col-md-6">{{ patient.address.Home | oizEntityAddress }}</div>
     */
    .filter('oizEntityAddress', function () {
        return function (modelValue) {
            return OpenIZ.Util.renderAddress(modelValue);
        }
    })
    /** 
     * @method datePrecisionFormat
     * @memberof Angular
     * @sumamry Renders the input date using the specified format identifier for date precision.
     * @param {int} format The format or date precision: Y, m, D, F, etc.
     * @see {OpenIZ.App.DatePrecisionFormats}
     * @description To override the formatting for the specified date precision you must override the linked setting in the openiz.js file by setting it in your program
     * @example mycontroller.js
     *      ...
     *       OpenIZ.App.DatePrecisionFormats = {
     *       DateFormatYear: 'YYYY',
     *       DateFormatMonth: 'YYYY-MM',
     *       DateFormatDay: 'YYYY-MM-DD',
     *       DateFormatHour: 'YYYY-MM-DD HH',
     *       DateFormatMinute: 'YYYY-MM-DD HH:mm',
     *       DateFormatSecond: 'YYYY-MM-DD HH:mm:ss'
     *   },  // My own precision formats
     * @example mmyview.html
     *      <div class="col-md-2">DOB:</div><div class="col-md-4">{{ patient.dateOfBirth | datePrecisionFormat: patient.dateOfBirthPrecision }}</div>
     *      <div class="col-md-2">Created On:</div><div class="col-md-4">{{ patient.creationTime | datePrecisionFormat: 'D' }}</div>
     */
    .filter('datePrecisionFormat', function () {
        return function (date, format) {
            var dateFormat;

            switch (format) {
                case 1:   // Year     "Y"
                case 'Y':
                    dateFormat = OpenIZ.App.DatePrecisionFormats.DateFormatYear;
                    break;
                case 2:   // Month    "m"
                case 'm':
                    dateFormat = OpenIZ.App.DatePrecisionFormats.DateFormatMonth;
                    break;
                case 3:   // Day      "D"
                case 'D':
                    dateFormat = OpenIZ.App.DatePrecisionFormats.DateFormatDay;
                    break;
                case 4:   // Hour     "H"
                case 'H':
                    dateFormat = OpenIZ.App.DatePrecisionFormats.DateFormatHour;
                    break;
                case 5:   // Minute   "M"
                case 'M':
                    dateFormat = OpenIZ.App.DatePrecisionFormats.DateFormatMinute;
                    break;
                case 6:   // Second   "S"
                case 'S':
                case 0:   // Full     "F"
                case 'F':
                default:
                    dateFormat = OpenIZ.App.DatePrecisionFormats.DateFormatSecond;
                    break;
            }

            if (date) {
                // Non timed
                switch (format) {
                    case 1:   // Year, Month, Day always expressed in UTC for Javascript will take the original server value and adjust.
                    case 'Y':
                    case 2:
                    case 'm':
                    case 3:
                    case 'D':
                        return moment(date).format(dateFormat);
                    default:
                        return moment(date).format(dateFormat);
                }
            }

            return null;
        };
    })
    /**
     * @method oizTag
     * @memberof Angular
     * @summary Turns any input text box into a tagged text box which breaks apart its components pieces into an array on the model
     * @example
     *      <div class="col-md-2">Name:</div>
     *      <div class="col-md-6">
     *          <input type="text" oiz-tag="oiz-tag" name="name" ng-model="patient.name.OfficialRecord.component.Given" class="form-control"/>
     *      </div>
     */
    .directive('oizTag', function ($timeout) {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ctrl) {

                // Parsers
                ctrl.$parsers.unshift(tagParser);
                ctrl.$formatters.unshift(tagFormatter);
                function tagParser(viewValue) {
                    return String(viewValue).split(',');
                }
                function tagFormatter(viewValue) {
                    if (typeof (viewValue) === Array)
                        return viewValue.join(viewValue)
                    return viewValue;
                }

                // Tag input
                scope.$watch(attrs.ngModel, function (nvalue, ovalue) {
                    if (typeof (nvalue) == "string" && ovalue != nvalue ||
                        Array.isArray(nvalue) && (!Array.isArray(ovalue) || ovalue.length != nvalue.length) ||
                        // HACK: For SPA
                        $(element).attr('has-bound') === undefined) {
                        $(element).attr('has-bound', true);
                        $(element).trigger('change');
                    }
                });

                $(element).tokenfield({
                    delimiter: ' ,',
                    createTokensOnBlur: true
                });

            }
        }
    })
    /**
     * @method oizDatabind
     * @memberof Angular
     * @summary Dynamically binds data from the IMS into a drop down
     * @param {string} value The model type to bind to
     * @param {string} data-filter The filter to apply to the data binding operation
     * @param {string} data-watch The model value to be watched for changes which will force the re-load of this drop down
     * @param {string} data-watch-target The target in the data-filter where the watched value should be placed
     * @param {string} data-display The property in the result set which should be displayed in the drop down. "scope" references the current result
     * @param {string} data-key The property in the result set which should be returned as the key
     * @param {bool} data-defaultFirst When true instructs the databinder to select the first value as the default
     * @param {string} data-default Sets the expression (javascript) which can populate the default value
     * @param {string} data-whenEmpty Sets the expression to use when there are no results in the data bind
     * @param {string} data-defaultKey Sets the key value of the default value
     * @example
     *  <select 
     *      oiz-databind="Place"
     *      data-filter='{"typeConcept" : "21ab7873-8ef3-4d78-9c19-4582b3c40631" }'
     *      data-display="scope.name.OfficialRecord.component.$other.value"
     *      data-key="scope.key"
     *      data-defaultFirst="true"
     *      ng-model="patient.address.Home.component.CensusTract"
     *  />
     */
    .directive('oizDatabind', function ($timeout) {
        return {
            link: function (scope, element, attrs, ctrl) {
                $timeout(function () {
                    var modelType = $(element).attr('oiz-databind');
                    var filterString = $(element).attr('data-filter');
                    var watchString = $(element).attr('data-watch');
                    var watchTargetString = $(element).attr('data-watch-target');
                    var displayString = $(element).attr('data-display');
                    var dataKey = $(element).attr('data-key');
                    var modelBinding = $(element).attr('ng-model');
                    var defaultFirst = $(element).attr('data-defaultFirst');
                    var defaultValueExpression = $(element).attr('data-default');
                    var defaultEmpty = $(element).attr('data-defaultEmpty');
                    var whenEmpty = $(element).attr('data-whenEmpty');
                    var defaultKey = attrs.defaultKey;
                    var $root = scope.$root;
                    var $scope = scope;
                    var defaultValue = null;

                    // Try to set defaut value
                    try {
                        defaultValue = eval(defaultValueExpression);
                    } catch (e) { }

                    var filter = {};
                    if (filterString !== undefined)
                        filter = JSON.parse(filterString);

                    if (!filter.statusConcept)
                        filter.statusConcept = 'C8064CBD-FA06-4530-B430-1A52F1530C27';

                    var bind = function () {
                        // Get the bind element
                        $(element)[0].disabled = true;
                        OpenIZ.Ims.get({
                            resource: modelType,
                            query: filter,
                            finally: function () {
                                $(element)[0].disabled = false;
                            },
                            continueWith: function (data) {
                                var currentValue = $(element).val();
                                var options = $(element)[0].options;
                                $('option', element[0]).remove(); // clear existing 


                                if (!data.item || !defaultFirst) {
                                    if (defaultKey)
                                        options[options.length] = new Option(OpenIZ.Localization.getString("locale.common.unknown"), defaultKey);
                                    else if (!defaultEmpty)
                                        options[options.length] = new Option(OpenIZ.Localization.getString("locale.common.unknown"));
                                    else
                                        options[options.length] = new Option("");
                                }
                                if ((!data.item || data.count == 0) && whenEmpty) {
                                    // when no items and a whenempty is used
                                    var scp = $scope; // frce JS closure
                                    eval(whenEmpty);
                                }

                                for (var i in data.item) {
                                    var text = null;
                                    if (displayString != null) {
                                        var scope = data.item[i];
                                        // HACK:
                                        text = eval(displayString);
                                    }
                                    else
                                        text = OpenIZ.Util.renderName(data.item[i].name.OfficialRecord);

                                    // Append element
                                    if (dataKey == null)
                                        options[options.length] = new Option(text, data.item[i].id);
                                    else
                                        options[options.length] = new Option(text, eval(dataKey));

                                    if (i == 0 && defaultFirst)
                                        defaultValue = defaultValue || dataKey == null ? data.item[i].id : eval(dataKey);

                                }

                                // Strip and bind select
                                if (currentValue && currentValue.indexOf("? string:") == 0) {
                                    $(element).val(currentValue.substring(9, currentValue.length - 2));
                                }
                                else if (defaultValue) {
                                    $(element).val(defaultValue);
                                }
                                $(element).trigger('change');

                            }
                        });
                    };

                    if (watchString !== null)
                        scope.$watch(watchString, function (newValue, oldValue) {
                            var $root = scope.$root;
                            if (watchTargetString !== null && newValue !== undefined) {
                                filter[watchTargetString] = newValue;
                                if (defaultValueExpression)
                                    defaultValue = eval(defaultValueExpression) || defaultValue;
                            }
                            bind();
                        });

                });
            }
        };
    })
    /**
     * @method oizEntitySearch
     * @memberof Angular
     * @summary Binds a select2 search box to the specified select input searching for the specified entities
     * @description This class is the basis for all drop-down searches in OpenIZ disconnected client. It is used whenever you would like to have a search inline in a form and displayed nicely
     * @param {string} value The type of object to be searched
     * @param {string} filter The additional criteria by which results should be filtered
     * @param {string} data-searchField The field which should be searched on. The default is name.component.value
     * @param {string} data-default The function which returns a list of objects which represent the default values in the search
     * @param {string} data-groupBy The property which sets the grouping for the results in the drop-down display
     * @param {string} data-groupDisplay The property on the group property which is to be displayed
     * @param {string} data-resultField The field on the result objects which contains the result
     */
    .directive('oizEntitysearch', function ($timeout) {
        return {
            scope: {
                defaultResults: '='
            },
            link: function (scope, element, attrs, ctrl) {
                $timeout(function () {
                    var modelType = attrs.oizEntitysearch;
                    var filterString = attrs.filter;
                    var displayString = attrs.display;
                    var searchProperty = attrs.searchfield || "name.component.value";
                    var defaultResults = attrs.default;
                    var groupString = attrs.groupBy;
                    var groupDisplayString = attrs.groupDisplay;
                    var resultProperty = attrs.resultfield || "id";
                    var filter = {}, defaultFilter = {};
                    if (filterString !== undefined)
                        filter = JSON.parse(filterString);

                    if (modelType != "SecurityUser" && modelType != "SecurityRole")
                        filter.statusConcept = 'C8064CBD-FA06-4530-B430-1A52F1530C27';

                    // Add appropriate styling so it looks half decent


                    // Bind select 2 search
                    $(element).select2({
                        defaultResults: function () {
                            var s = scope;
                            if (defaultResults != null) {
                                try {
                                    return eval(defaultResults);
                                } catch (e) {

                                }
                            }
                            else {
                                return $.map($('option', element[0]), function (o) {
                                    return { "id": o.value, "text": o.innerText };
                                });
                            }
                        },
                        dataAdapter: $.fn.select2.amd.require('select2/data/extended-ajax'),
                        ajax: {
                            url: ((modelType == "SecurityUser" || modelType == "SecurityRole") ? "/__auth/" : "/__ims/") + modelType,
                            dataType: 'json',
                            delay: 500,
                            method: "GET",
                            data: function (params) {
                                filter[searchProperty] = "~" + params.term;
                                filter["_count"] = 20;
                                filter["_offset"] = 0;
                                filter["_viewModel"] = "min";
                                return filter;
                            },
                            processResults: function (data, params) {
                                //params.page = params.page || 0;
                                var data = data.$type == "Bundle" ? data.item : data.item || data;
                                var retVal = { results: [] };

                                if (groupString == null && data !== undefined) {
                                    retVal.results = retVal.results.concat($.map(data, function (o) {
                                        var text = "";
                                        if (displayString) {
                                            scope = o;
                                            text = eval(displayString);
                                        }
                                        else if (o.name !== undefined) {
                                            if (o.name.OfficialRecord) {
                                                text = OpenIZ.Util.renderName(o.name.OfficialRecord);
                                            } else if (o.name.Assigned) {
                                                text = OpenIZ.Util.renderName(o.name.Assigned);
                                            }
                                        }
                                        o.text = o.text || text;
                                        o.id = o[resultProperty];
                                        return o;
                                    }));
                                }
                                else {
                                    // Get the group string
                                    for (var itm in data) {
                                        // parent obj
                                        try {
                                            var scope = eval('data[itm].' + groupString);
                                            var groupDisplay = "";
                                            if (groupDisplayString != null)
                                                groupDisplay = eval(groupDisplayString);
                                            else
                                                groupDisplay = scope;

                                            var gidx = $.grep(retVal.results, function (e) { return e.text == groupDisplay });
                                            if (gidx.length == 0)
                                                retVal.results.push({ "text": groupDisplay, "children": [data[itm]] });
                                            else
                                                gidx[0].children.push(data[itm]);
                                        }
                                        catch (e) {
                                            retVal.results.push(data[itm]);
                                        }
                                    }
                                }
                                return retVal;
                            },
                            cache: true
                        },
                        escapeMarkup: function (markup) { return markup; }, // Format normally
                        minimumInputLength: 2,
                        templateSelection: function (selection) {
                            var retVal = "";
                            switch (modelType) {
                                case "UserEntity":
                                case "Provider":
                                    retVal += "<span class='glyphicon glyphicon-user'></span>";
                                    break;
                                case "Place":
                                    retVal += "<span class='glyphicon glyphicon-map-marker'></span>";
                                    break;
                                case "Entity":
                                    retVal += "<span class='glyphicon glyphicon-tag'></span>";
                                    break;
                            }
                            retVal += "&nbsp;";


                            if (displayString != null) {
                                var scope = selection;
                                retVal += eval(displayString);
                            }
                            else if (selection.name != null && selection.name.OfficialRecord != null)
                                retVal += OpenIZ.Util.renderName(selection.name.OfficialRecord);
                            else if (selection.name != null && selection.name.Assigned != null)
                                retVal += OpenIZ.Util.renderName(selection.name.Assigned);
                            else if (selection.name != null && selection.name.$other != null)
                                retVal += OpenIZ.Util.renderName(selection.name.$other);
                            else if (selection.element !== undefined)
                                retVal += selection.element.innerText.trim();
                            else if (selection.text)
                                retVal += selection.text;

                            if (selection.address)
                                    retVal += " - <small>(<i class='fa fa-map-marker'></i> " + OpenIZ.Util.renderAddress(selection.address) + ")</small>";

                            return retVal;
                        },
                        keepSearchResults: true,
                        templateResult: function (result) {
                            if (result.loading) return result.text;

                            if (displayString != null) {
                                var scope = result;
                                return eval(displayString);
                            }
                            else if (result.classConcept != OpenIZModel.EntityClassKeys.ServiceDeliveryLocation && result.name != null && result.typeConceptModel != null && result.typeConceptModel.name != null && result.name.OfficialRecord) {
                                retVal = "<div class='label label-info'>" +
                                    result.typeConceptModel.name[OpenIZ.Localization.getLocale()] + "</div> " + OpenIZ.Util.renderName(result.name.OfficialRecord || result.name.$other);
                                if (result.address)
                                    retVal += " - <small>(<i class='fa fa-map-marker'></i> " + OpenIZ.Util.renderAddress(result.address) + ")</small>";
                                return retVal;
                            }
                            else if (result.classConcept == OpenIZModel.EntityClassKeys.ServiceDeliveryLocation && result.name != null && result.typeConceptModel != null && result.typeConceptModel.name != null) {
                                retVal = "<div class='label label-info'>" +
                                   result.typeConceptModel.name[OpenIZ.Localization.getLocale()] + "</div> " + OpenIZ.Util.renderName(result.name.OfficialRecord || result.name.Assigned || result.name.$other );
                                if (result.relationship && result.relationship.Parent && result.relationship.Parent.targetModel && result.relationship.Parent.targetModel.name)
                                    retVal += " - <small>(<i class='fa fa-map-marker'></i> " + OpenIZ.Util.renderName(result.relationship.Parent.targetModel.name.OfficialRecord || result.relationship.Parent.targetModel.name.Assigned) + ")</small>";
                                if (result.address)
                                    retVal += " - <small>(<i class='fa fa-map-marker'></i> " + OpenIZ.Util.renderAddress(result.address) + ")</small>";
                                return retVal;
                            }
                            else if (result.name != null && result.typeConceptModel != null && result.typeConceptModel.name != null && result.name.Assigned) {
                                var retVal = "<div class='label label-default'>" +
                                    result.typeConceptModel.name[OpenIZ.Localization.getLocale()] + "</div> " + OpenIZ.Util.renderName(result.name.Assigned || result.name.$other);

                                if (result.address)
                                    retVal += " - <small>(<i class='fa fa-map-marker'></i> " + OpenIZ.Util.renderAddress(result.address) + ")</small>";
                                return retVal;
                            }
                            else if (result.name != null && result.name.OfficialRecord)
                                return "<div class='label label-default'>" +
                                    result.$type + "</div> " + OpenIZ.Util.renderName(result.name.OfficialRecord);
                            else if (result.name != null && result.name.Assigned)
                                return "<div class='label label-default'>" +
                                    result.$type + "</div> " + OpenIZ.Util.renderName(result.name.Assigned)
                            else if (result.name != null && result.name.$other)
                                return "<div class='label label-default'>" +
                                    result.$type + "</div> " + OpenIZ.Util.renderName(result.name.$other)
                            else
                                return result.text;
                        }
                    });

                    //$(element).on("select2:opening", function (e) {
                    //    var s = scope;
                    //    if (defaultResults != null) {
                    //        return eval(defaultResults);
                    //    }
                    //    else {
                    //        return $.map($('option', element[0]), function (o) {
                    //            return { "id": o.value, "text": o.innerText };
                    //        });
                    //    }
                    //}
                    //);
                    // HACK: For angular values, after select2 has "selected" the value, it will be a ? string: ID ? value we do not want this
                    // we want the actual value, so this little thing corrects this bugginess
                    $(element).on("select2:select", function (e) {
                        if (e.currentTarget.value.indexOf("? string:") == 0) {
                            e.currentTarget.value = e.currentTarget.value.substring(9, e.currentTarget.value.length - 2);
                        }
                        e.currentTarget.options.selectedIndex = e.currentTarget.options.length - 1;
                        //{
                        //    while (e.currentTarget.options.length > 1)
                        //        e.currentTarget.options.splice(1);
                        //}
                    });
                });
            }
        };
    })
    /**
     * @method oizCollapseIndicator
     * @memberof Angular
     * @summary Replaces the specified tag with an indicator which illustrates whether the current panel is collapsed
     * @deprecated
     */
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
    })
    .directive('ngRepeatN', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            transclude: 'element',
            replace: true,
            scope: true,
            link: function (scope, element, attrs, ctrl, $transclude) {

                // the element to insert after
                scope.last = element;

                // the parent element
                scope.parentElem = element.parent();

                // list of elements in the repeater
                scope.elems = [element];

                // a getter function to resolve the parameter
                var getter = $parse(attrs.ngRepeatN);

                scope.$watch(function () {
                    return parseInt(attrs.ngRepeatN) || getter(scope);
                }, function (newValue, oldValue) {

                    var newInt = parseInt(newValue)
                    , oldInt = parseInt(oldValue)
                    , bothValues = !isNaN(newInt) && !isNaN(oldInt)
                    , childScope
                    , i
                    , limit;

                    // decrease number of repeated elements
                    if (isNaN(newInt) || (bothValues && newInt < oldInt)) {
                        limit = bothValues ? newInt : 0;
                        scope.last = scope.elems[limit];
                        for (i = scope.elems.length - 1; i > limit; i -= 1) {
                            scope.elems[i].remove();
                            scope.elems.pop();
                        }
                    }

                        // increase number of repeated elements
                    else {
                        i = scope.elems.length - 1;

                        for (i; i < newInt; i += 1) {
                            childScope = scope.$new();
                            childScope.$index = i;
                            $transclude(childScope, function (clone) {
                                scope.last.after(clone);
                                scope.last = clone;
                                scope.elems.push(clone);
                            });
                        }
                    }
                });
            }
        };
    }])
    .directive('siteTrim', function ($parse) {
        return {
            replace: true,
            restrict: 'A',
            template: "<span>{{site}}</span>",
            link: function (scope, element, attrs) {
                scope.site = attrs.siteTrim.substring(5, attrs.siteTrim.length) || OpenIZ.Localization.getString("locale.common.na");
            }
        };
    })
    // TODO: Why is this even a thing?
    .directive('chartLegend', function () {
        return {
            restrict: 'E',
            templateUrl: 'views/common/patients/view/partials/chart-legend.html',
            replace: true
        }
    })
    .directive('integerMinimumValue', function ($filter) {
        return {
            require: '?ngModel',
            link: function (scope, elem, attrs, ctrl) {
                if (!ctrl) return;

                var integerMinimumValue = attrs.integerMinimumValue;

                ctrl.$formatters.unshift(function (value) {
                    ctrl.$setValidity('integerMinimumValue', value >= integerMinimumValue);
                    return value;
                });

                ctrl.$parsers.unshift(function (viewValue) {
                    var plainNumber = viewValue.replace(/[^0-9]/g, '');
                    var isValid = plainNumber >= integerMinimumValue;

                    ctrl.$setValidity('integerMinimumValue', isValid);

                    if (plainNumber !== viewValue) {
                        ctrl.$setViewValue(plainNumber);
                        ctrl.$render();
                    }

                    return isValid ? plainNumber : undefined;
                });
            }
        }
    });