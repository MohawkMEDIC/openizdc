/// <reference path="angular.min.js"/>
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
 * Date: 2016-7-30
 */

/// <reference path="openiz.js"/>
/// <reference path="openiz-model.js"/>

/**
 * Open IZ Localization for angular
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
    .filter('translate', function () {
        return function (input) {
            return OpenIZ.Localization.getString(input);
        };
    })
    .filter('orderStatus', function () {
        return function (moodConcept, statusConcept) {
            if (moodConcept == OpenIZModel.ActMoodKeys.Eventoccurrence && statusConcept == OpenIZModel.StatusKeys.Active) {
                return "locale.stock.label.shipped";
            } else if (moodConcept == OpenIZModel.ActMoodKeys.Eventoccurrence && statusConcept == OpenIZModel.StatusKeys.Completed) {
                return "locale.stock.label.fulfilled";
            } else if (statusConcept == OpenIZModel.StatusKeys.Obsolete) {
                return "locale.stock.label.cancelled";
            }
            return "locale.stock.label.pending";
        };
    })
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
    .filter('oizConcept', function () {
        return function (modelValue) {
            if (modelValue != null && modelValue.name != null)
                return OpenIZ.Util.renderConceptName(modelValue.name);
        }
    })
    .filter('oizEntityName', function () {
        return function (modelValue) {
            return OpenIZ.Util.renderName(modelValue);
        }
    })
    .filter('oizEntityAddress', function () {
        return function (modelValue) {
            return OpenIZ.Util.renderAddress(modelValue);
        }
    })
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

            if(date)
                return moment(date).format(dateFormat);
            return null;
        };
    })
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
                                filter["_count"] = 5;
                                filter["_offset"] = 0;
                                filter["_viewModel"] = "min";
                                return filter;
                            },
                            processResults: function (data, params) {
                                //params.page = params.page || 0;
                                var data = data.item || data;
                                var retVal = { results: [] };
                                if (groupString == null) {
                                    return {
                                        results: $.map(data, function (o) {
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
                                        })
                                    };
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
                scope.site = attrs.siteTrim.substring(5, attrs.siteTrim.length) || "N/A";
            }
        };
    })
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