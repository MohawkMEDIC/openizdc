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
    scope.search.results = null;
    scope.search.mode = 0;
    scope.search.paging = scope.search.paging || {size: 10};
    scope.act = {};

    scope.$watch('search.dateOfBirthStringLow', function (nvalue, ovalue) {
        if(nvalue !== undefined) 
            $scope.search.query.dateOfBirth = ">=" + OpenIZ.Util.toDateInputString(new Date(nvalue));
    });
    scope.$watch('search.dateOfBirthStringHigh', function (nvalue, ovalue) {
        if (nvalue !== undefined)
            $scope.search.query.dateOfBirth = "<=" + OpenIZ.Util.toDateInputString(new Date(nvalue));
    });

    scope.search.search = scope.search.search || search;
    scope.search.next = scope.search.next || next;
    scope.search.previous = scope.search.previous || previous;
    scope.search.goPage = scope.search.goPage || goPage;
    scope.startEncounter = startEncounter;

    scope.search.searchSubmitted = false;
    var onlineOnly = false;
    
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
     * @summary Advances to the next set of results
     */
    function search(searchOnlineOnly) {
        onlineOnly = searchOnlineOnly
        scope.search.searchSubmitted = true;
        console.log(!scope.search.query.any ||
            scope.search.query.any.length == 0 ||
            (scope.search.query.any.length == 1 && scope.search.query.any[0] == "*"))

        if (!scope.search.query.any ||
            scope.search.query.any.length == 0 ||
            (scope.search.query.any.length == 1 && scope.search.query.any[0] == "*")) {
            scope.search.isSearching = true;
            scope.search.results = [];
            scope.search.isSearching = false;
            scope.$applyAsync();
            return;
        }

        if (!scope.searchForm || scope.searchForm.$valid) {
            if (onlineOnly)
                scope.search.query["_onlineOnly"] = onlineOnly;
            else
                delete (scope.search.query["_onlineOnly"]);

            scope.search.query["_offset"] = 0;
            scope.search.query["_count"] = scope.search.paging.size;
            scope.search.isSearching = true;
            $(onlineOnly ? "#patientOnlineSearchButton" : "#patientSearchButton").attr('disabled','disabled');
            var start = $scope.search.dateOfBirthStringLow;
            var end = $scope.search.dateOfBirthStringHigh;
            OpenIZ.Patient.findAsync({
                query: scope.search.query,
                continueWith: function (r) {

                    if (start !== null && start !== undefined && end != null && end != undefined && r.totalResults>0) {//Temporary fix until the query string can take a range
                        var inRange = [];
                        for (var i = 0; i < r.item.length; i++) {
                            if (r.item[i].dateOfBirth <= end && r.item[i].dateOfBirth >= start) {
                                inRange.push(r.item[i]);
                            };
                        };
                        r.item = inRange;
                    }

                    scope.search.results = r;
                    scope.search.paging = {
                        current: 1,
                        total: r.totalResults == 0 ? 1 : r.totalResults / scope.search.paging.size,
                        size: scope.search.paging.size,
                        pages: []
                    };
                    for (var i = 0; i < scope.search.paging.total; i++)
                        scope.search.paging.pages[i] = i + 1;

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
    * @summary Starts an encounter for a patient.
    */
    function startEncounter(patient) {
        scope.doStartEncounter(patient);
    };
}]);