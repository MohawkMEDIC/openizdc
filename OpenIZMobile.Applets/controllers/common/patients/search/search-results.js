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
 * Date: 2016-7-17
 */

/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>

// Document ready, bind search results to their related contexts
layoutApp.controller('SearchResultsController', ['$scope', function ($scope) {

    // Get the current scope that we're in
    var scope = $scope;
                    
    // If the current scope does not have required values bind them
    scope.search = scope.search || {};
    scope.search.query = scope.search.query || {};
    scope.search.query = scope.search.orginalQuery || {};
    scope.search.results = null;
    scope.search.mode = 0;
    scope.search.paging = scope.search.paging || {size: 10};
    scope.act = {};

    angular.element(document).ready(init);

    function init() {
        scope.search.search = scope.search.search || search;
        scope.search.next = scope.search.next || next;
        scope.search.previous = scope.search.previous || previous;
        scope.search.goPage = scope.search.goPage || goPage;
        scope.startEncounter = startEncounter;
        scope.goResult = scope.goResult || goResult;

        scope.search.searchSubmitted = false;
        var onlineOnly = false;

        scope.search.minSearchDate = '2000-01-01';
    }

    //function updateResultEncounters() {
    //    for (var i in scope.search.results.item) {
    //        OpenIZ.Act.findAsync({
    //            query: {
    //                "classConcept": OpenIZModel.ActClassKeys.Encounter,
    //                "statusConcept": OpenIZModel.StatusKeys.Active,
    //                "moodConcept": OpenIZModel.ActMoodKeys.Eventoccurrence,
    //                "participation[RecordTarget].player": scope.search.results.item[i].id,
    //                "_count": 0
    //            },
    //            continueWith: function (result) {
    //                if (result != null)
    //                    scope.search.results.item[i]._isInEncounter = result.totalResults > 0;
    //                scope.$apply();
    //            }
    //        });
    //    }
    //};

    /**
     * @summary View a result
     */
    function goResult(patientId) {
        OpenIZ.UserInterface.patientController.view(patientId);
    }

    
    /** 
     * @summary Advances to the next set of results
     */
    function search(searchOnlineOnly) {
        onlineOnly = searchOnlineOnly
        scope.search.searchSubmitted = true;

        if (!scope.searchForm || scope.searchForm.$valid) {
            if (onlineOnly)
                scope.search.query["_onlineOnly"] = onlineOnly;
            else
                delete (scope.search.query["_onlineOnly"]);


            scope.search.query["_offset"] = 0;
            scope.search.query["_count"] = scope.search.paging.size;
            scope.search.query["_viewModel"] = "min";
            scope.search.query["_queryId"] = OpenIZ.App.newGuid();

            // Search shouldn't include null parameters
            for (var k in Object.keys(scope.search.query))
            {
                var key = Object.keys(scope.search.query)[k];
                if (key.startsWith("_")) continue;

                if (Array.isArray(scope.search.query[key])) {
                    for (var i in scope.search.query[key])
                        if(!scope.search.query[key][i])
                            delete (scope.search.query[key][i]);
                        else if (!scope.search.query[key][i].startsWith("~") && key.startsWith("name"))
                            scope.search.query[key][i] = "~" + scope.search.query[key][i].replace(/^\s+|\s+$/g, '');
                }
                else if (!scope.search.query[key])
                    delete (scope.search.query[key]);
                else if (scope.search.query[key].startsWith && !scope.search.query[key].startsWith("~") && key.startsWith("name"))
                    scope.search.query[key] = "~" + scope.search.query[key];

            }
            scope.search.isSearching = true;
            $(onlineOnly ? "#patientOnlineSearchButton" : "#patientSearchButton").attr('disabled','disabled');

            //scope.search.orginalQuery = angular.copy(scope.search.query);
            OpenIZ.Patient.findAsync({
                query: scope.search.query,
                continueWith: function (r) {

                    if (r.totalResults == 0 && scope.search.query._onlineFallback && !onlineOnly) {
                        scope.search.search(true);
                    }
                    else {
                        scope.search.results = r;
                        scope.search.paging = {
                            current: 1,
                            total: r.totalResults == 0 ? 1 : r.totalResults / scope.search.paging.size,
                            size: scope.search.paging.size,
                            pages: []
                        };
                        for (var i = 0; i < scope.search.paging.total; i++)
                            scope.search.paging.pages[i] = i + 1;
                    }

                    //updateResultEncounters();
                },
                onException: function (e) {
                    OpenIZ.App.toast(e.message);
                },
                finally: function () {
                    scope.search.isSearching = false;
                    //OpenIZ.App.hideWait(onlineOnly ? "#patientOnlineSearchButton" : "#patientSearchButton");
                    $(onlineOnly ? "#patientOnlineSearchButton" : "#patientSearchButton").removeAttr('disabled');
                    scope.$applyAsync();


                }
            });
        }
    };

    /** 
     * @summary Advances to the next set of results
     */
    function next() {
        $(onlineOnly ? "#patientOnlineSearchButton" : "#patientSearchButton").attr('disabled', 'disabled');

        if (scope.search.paging.current == scope.search.paging.count)
            return;

        // Current page increment
        scope.search.paging.current++;
        scope.search.query["_offset"] = (scope.search.paging.current - 1) * scope.search.paging.size;
        scope.search.query["_viewModel"] = "min";
        scope.search.query["_count"] = scope.search.paging.size;
        delete scope.search.results;
        scope.search.isSearching = true;
        // Find async
        OpenIZ.Patient.findAsync({
            query: scope.search.query,
            continueWith: function (r) {
                scope.search.results = r;
                //updateResultEncounters();
            },
            onException: function (e) {
                OpenIZ.App.toast(e.message);
            },
            finally: function () {
                scope.search.isSearching = false;
                $(onlineOnly ? "#patientOnlineSearchButton" : "#patientSearchButton").removeAttr('disabled');
                scope.$apply();

            }
        });
    };

    /** 
     * @summary Goes to the previous page
     */
    function previous() {
        $(onlineOnly ? "#patientOnlineSearchButton" : "#patientSearchButton").attr('disabled', 'disabled');

        if (scope.search.paging.current == 1)
            return;

        // Current page increment
        scope.search.paging.current--;
        scope.search.query["_viewModel"] = "min";
        scope.search.query["_offset"] = (scope.search.paging.current - 1) * scope.search.paging.size;
        scope.search.query["_count"] = scope.search.paging.size;
        delete scope.search.results;
        scope.search.isSearching = true;

        // Find async
        OpenIZ.Patient.findAsync({
            query: scope.search.query,
            continueWith: function (r) {
                scope.search.results = r;
                //updateResultEncounters();
            },
            onException: function (e) {
                OpenIZ.App.toast(e.message);
            },
            finally: function () {
                scope.search.isSearching = false;
                $(onlineOnly ? "#patientOnlineSearchButton" : "#patientSearchButton").removeAttr('disabled');
                scope.$apply();

            }
        });
    };

    /**
     * @summary Goes to the specific page
     */
    function goPage(pageNo) {
        $(onlineOnly ? "#patientOnlineSearchButton" : "#patientSearchButton").attr('disabled', 'disabled');


        // Current page increment
        scope.search.paging.current = pageNo;
        scope.search.query["_viewModel"] = "min";
        scope.search.query["_offset"] = (scope.search.paging.current - 1) * scope.search.paging.size;
        scope.search.query["_count"] = scope.search.paging.size;
        delete scope.search.results;
        scope.search.isSearching = true;

        // Find async
        OpenIZ.Patient.findAsync({
            query: scope.search.query,
            continueWith: function (r) {
                scope.search.results = r;
                //updateResultEncounters();
                window.scrollTo(0, 0);
            },
            onException: function (e) {
                OpenIZ.App.toast(e.message);
            },
            finally: function () {
                scope.search.isSearching = false;
                $(onlineOnly ? "#patientOnlineSearchButton" : "#patientSearchButton").removeAttr('disabled');
                scope.$apply();

            }
        });
    };


    /**
    * @summary Starts an encounter for a patient.
    */
    function startEncounter(patient) {
        scope.doStartEncounter(patient);
    };
}]);