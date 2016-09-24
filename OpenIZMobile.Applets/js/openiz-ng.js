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
    .provider('localize', function localizeProvider()
    {

        this.$get = ['$rootScope', '$filter', function ($rootScope, $filter)
        {
            var localize = {
                dictionary: OpenIZ.Localization.getStrings(OpenIZ.Localization.getLocale()),
                /**
                 * @summary Sets the locale of the user interface 
                 */
                setLanguage: function (locale)
                {
                    if (OpenIZ.Localization.getLocale() != locale)
                    {
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
                getString: function (key)
                {

                    // make sure we always have the latest locale
                    //localize.dictionary = OpenIZ.Localization.getStrings(OpenIZ.Localization.getLocale());

                    var entry = localize.dictionary[key];
                    if (entry != null)
                        return entry;
                    else
                    {
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
    .filter('i18n', ['$rootScope', 'localize', function ($rootScope, localize)
    {
        var filterFn = function (key)
        {
            return localize.getString(key);
        };
        filterFn.$stateful = false;
        return filterFn;
    }])
    .filter('oizEntityIdentifier', function ()
    {
        return function (modelValue)
        {
            if (modelValue === undefined)
                return "";
            if (modelValue.NID !== undefined)
                return modelValue.NID.value;
            else
                for (var k in modelValue)
                    return modelValue[k].value;
        };
    })
    .filter('oizEntityName', function ()
    {
        return function (modelValue)
        {
            return OpenIZ.Util.renderName(modelValue);
        }
    })
    .filter('oizEntityAddress', function ()
    {
        return function (modelValue)
        {
            return OpenIZ.Util.renderAddress(modelValue);
        }
    })
    .directive('oizTag', function ($timeout)
    {
        return {
            require: 'ngModel',
            link: function (scope, element, attrs, ctrl)
            {

                // Parsers
                ctrl.$parsers.unshift(tagParser);
                ctrl.$formatters.unshift(tagFormatter);
                function tagParser(viewValue)
                {
                    return String(viewValue).split(',');
                }
                function tagFormatter(viewValue)
                {
                    if (typeof (viewValue) === Array)
                        return viewValue.join(viewView)
                    return viewValue;
                }

                // Tag input
                scope.$watch(attrs.ngModel, function (nvalue, ovalue)
                {
                    if (ovalue != nvalue &&
                        ovalue === undefined)
                        $(element).trigger('change');
                });

                $(element).tokenfield({
                    delimiter: ' ,',
                    createTokensOnBlur: true
                });
            }
        }
    })
    .directive('oizDatabind', function ($timeout)
    {
        return {
            link: function (scope, element, attrs, ctrl)
            {
                $timeout(function ()
                {
                    var modelType = $(element).attr('oiz-databind');
                    var filterString = $(element).attr('data-filter');
                    var watchString = $(element).attr('data-watch');
                    var watchTargetString = $(element).attr('data-watch-target');
                    var displayString = $(element).attr('data-display');

                    var filter = {};
                    if (filterString !== undefined)
                        filter = JSON.parse(filterString);
                    filter.statusConcept = 'C8064CBD-FA06-4530-B430-1A52F1530C27';

                    var bind = function ()
                    {
                        // Get the bind element
                        $(element)[0].disabled = true;
                        OpenIZ.Ims.get({
                            resource: modelType,
                            query: filter,
                            finally: function ()
                            {
                                $(element)[0].disabled = false;
                            },
                            continueWith: function (data)
                            {
                                var options = $(element)[0].options;
                                $('option', element[0]).remove(); // clear existing 
                                options[options.length] = new Option(OpenIZ.Localization.getString("locale.common.unknown"));
                                for (var i in data.item)
                                {
                                    var text = null;
                                    if (displayString != null)
                                    {
                                        var scope = data.item[i];
                                        // HACK:
                                        text = eval(displayString);
                                    }
                                    else
                                        text = OpenIZ.Util.renderName(data.item[i].name.OfficialRecord);

                                    // Append element
                                    options[options.length] = new Option(text, data.item[i].id);
                                }
                            }
                        });
                    };

                    if (watchString !== null)
                        scope.$watch(watchString, function (newValue, oldValue)
                        {
                            if (watchTargetString !== null && newValue !== undefined)
                                filter[watchTargetString] = newValue;

                            bind();
                        });

                });
            }
        };
    })
    .directive('oizEntitysearch', function ($timeout)
    {
        return {
            link: function (scope, element, attrs, ctrl)
            {
                $timeout(function ()
                {

                    var modelType = $(element).attr('oiz-entitysearch');
                    var filterString = $(element).attr('data-filter');
                    var displayString = $(element).attr('data-display');
                    var defaultFilterString = $(element).attr('data-default');
                    var groupString = $(element).attr('data-group-by');
                    var groupDisplayString = $(element).attr('data-group-display');

                    var filter = {}, defaultFilter = {};
                    if (filterString !== undefined)
                        filter = JSON.parse(filterString);
                    if (defaultFilterString !== undefined)
                        filter = JSON.parse(defaultFilterString);
                    filter.statusConcept = 'C8064CBD-FA06-4530-B430-1A52F1530C27';

                    // Add appropriate styling so it looks half decent
                    $(element).attr('style', 'width:100%; height:100%');

                    // Bind select 2 search
                    $(element).select2({
                        dataAdapter: $.fn.select2.amd.require('select2/data/extended-ajax'),
                        defaultResults: function ()
                        {

                            if ($(element[0]).attr('data-default-results') != null)
                            {
                                return JSON.parse($(element[0]).attr('data-default-results'));
                            }
                            else
                            {
                                return $.map($('option', element[0]), function (o)
                                {
                                    return { "id": o.value, "text": o.innerText };
                                });
                            }
                        },
                        ajax: {
                            url: "/__ims/" + modelType,
                            dataType: 'json',
                            delay: 500,
                            method: "GET",
                            data: function (params)
                            {
                                filter["name.component.value"] = "~" + params.term;
                                filter["_count"] = 5;
                                filter["_offset"] = 0;
                                return filter;
                            },
                            processResults: function (data, params)
                            {
                                $('option', element[0]).remove(); // clear existing 

                                //params.page = params.page || 0;
                                var data = data.item || data;
                                var retVal = { results: [] };
                                if (groupString == null)
                                    return {
                                        results: $.map(data, function (o)
                                        {
                                            o.text = o.text || (o.name !== undefined ? OpenIZ.Util.renderName(o.name.OfficialRecord) : "");
                                            return o;
                                        })
                                    };
                                else
                                {
                                    // Get the group string
                                    for (var itm in data)
                                    {
                                        // parent obj
                                        try
                                        {
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
                                        catch (e)
                                        {
                                            retVal.results.push(data[itm]);
                                        }
                                    }
                                }

                                return retVal;
                            },
                            cache: true
                        },
                        escapeMarkup: function (markup) { return markup; }, // Format normally
                        minimumInputLength: 4,
                        templateSelection: function (result)
                        {
                            var retVal = "";
                            switch (modelType)
                            {
                                case "UserEntity":
                                case "Provider":
                                    retVal += "<span class='glyphicon glyphicon-user'></span>";
                                    break;
                                case "Place":
                                    retVal += "<span class='glyphicon glyphicon-map-marker'></span>";
                                    break;
                            }
                            retVal += "&nbsp;";
                            if (displayString != null)
                            {
                                var scope = result;
                                retVal += eval(displayString);
                            }
                            else if (result.name != null)
                                retVal += OpenIZ.Util.renderName(result.name.OfficialRecord);
                            else if (result.element !== undefined)
                                retVal += result.element.innerText;
                            else if (result.text != "")
                                retVal += result.text;
                            return retVal;
                        },
                        keepSearchResults: true,
                        templateResult: function (result)
                        {
                            if (displayString != null)
                            {
                                var scope = result;
                                return eval(displayString);
                            }
                            else if (result.name != null && result.typeConceptModel != null && result.typeConceptModel.name != null)
                                return "<div class='label label-default'>" +
                                    result.typeConceptModel.name[OpenIZ.Localization.getLocale()] + "</div> " + OpenIZ.Util.renderName(result.name.OfficialRecord);
                            else if (result.name != null)
                                return "<div class='label label-default'>" +
                                    result.$type + "</div> " + OpenIZ.Util.renderName(result.name.OfficialRecord);
                            else
                                return result.text;
                        }
                    });
                });
            }
        };
    })
    .directive('oizCollapseindicator', function ()
    {
        return {
            link: function (scope, element, attrs, ctrl)
            {
                $(element).on('hide.bs.collapse', function ()
                {
                    var indicator = $(this).attr('data-oiz-chevron');
                    $(indicator).removeClass('glyphicon-chevron-down');
                    $(indicator).addClass('glyphicon-chevron-right');
                });
                $(element).on('show.bs.collapse', function ()
                {
                    var indicator = $(this).attr('data-oiz-chevron');
                    $(indicator).addClass('glyphicon-chevron-down');
                    $(indicator).removeClass('glyphicon-chevron-right');
                });
            }
        };
    });