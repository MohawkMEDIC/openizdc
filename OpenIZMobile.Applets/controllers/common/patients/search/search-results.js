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
 * User: fyfej
 * Date: 2016-11-14
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

    scope.$watch('search.dateOfBirthString', function (nvalue, ovalue) {
        if(nvalue !== undefined) 
            $scope.search.query.dateOfBirth = OpenIZ.Util.toDateInputString(new Date(nvalue));
    });

    scope.search.search = scope.search.search || search;
    scope.search.next = scope.search.next || next;
    scope.search.previous = scope.search.previous || previous;
    scope.search.goPage = scope.search.goPage || goPage;
    scope.startEncounter = startEncounter;


    
    function updateResultEncounters() {
        for (var i in scope.search.results.item) {
            OpenIZ.Act.findAsync({
                query: {
                    "classConcept": OpenIZModel.ActClassKeys.Encounter,
                    "statusConcept": OpenIZModel.StatusKeys.Active,
                    "moodConcept": OpenIZModel.ActMoodKeys.Eventoccurrence,
                    "participation[RecordTarget].player": scope.search.results.item[i].id,
                    "_count": 0
                },
                continueWith: function (result) {
                    if (result != null)
                        scope.search.results.item[i]._isInEncounter = result.totalResults > 0;
                    scope.$apply();
                }
            });
        }
    };

    /** 
     * @summary Advances to the next set of results
     */
    function search(onlineOnly) {
        
        if (onlineOnly)
            scope.search.query["_onlineOnly"] = onlineOnly;
        else
            delete (scope.search.query["_onlineOnly"]);
        scope.search.query["_offset"] = 0;
        scope.search.query["_count"] = scope.search.paging.size;
        scope.search.isSearching = true;
        OpenIZ.Patient.findAsync({
            query: scope.search.query,
            continueWith: function (r) {
                scope.search.results = r;
                scope.search.paging = {
                    current: 1,
                    total: r.totalResults == 0 ? 1 : r.totalResults / scope.search.paging.size,
                    size: scope.search.paging.size,
                    pages: []
                };
                for (var i = 0; i < scope.search.paging.total; i++)
                    scope.search.paging.pages[i] = i + 1;
                updateResultEncounters();
                scope.$apply();
            },
            onException: function (e) {
                OpenIZ.App.toast(e.message);
            },
            finally: function () {
                scope.search.isSearching = false;

            }
        });

    };

    /** 
     * @summary Advances to the next set of results
     */
    function next() {
        OpenIZ.App.showWait(OpenIZ.Localization.getString("locale.dialog.wait.text"));
        if (scope.search.paging.current == scope.search.paging.count)
            return;

        // Current page increment
        scope.search.paging.current++;
        scope.search.query["_offset"] = (scope.search.paging.current - 1) * scope.search.paging.size;
        scope.search.query["_count"] = scope.search.paging.size;

        // Find async
        OpenIZ.Patient.findAsync({
            query: scope.search.query, 
            continueWith: function (r) {
                scope.search.results = r;
                updateResultEncounters();
                scope.$apply();
            },
            onException: function (e) {
                OpenIZ.App.toast(e.message);
            },
            finally: function () {
                OpenIZ.App.hideWait();
            }
        });
    };

    /** 
     * @summary Goes to the previous page
     */
    function previous() {
        OpenIZ.App.showWait(OpenIZ.Localization.getString("locale.dialog.wait.text"));
        if (scope.search.paging.current == 1)
            return;

        // Current page increment
        scope.search.paging.current--;
        scope.search.query["_offset"] = (scope.search.paging.current - 1) * scope.search.paging.size;
        scope.search.query["_count"] = scope.search.paging.size;

        // Find async
        OpenIZ.Patient.findAsync({
            query: scope.search.query,
            continueWith: function (r) {
                scope.search.results = r;
                updateResultEncounters();
                scope.$apply();
            },
            onException: function (e) {
                OpenIZ.App.toast(e.message);
            },
            finally: function () {
                OpenIZ.App.hideWait();
            }
        });
    };

    /**
     * @summary Goes to the specific page
     */
    function goPage(pageNo) {
        OpenIZ.App.showWait(OpenIZ.Localization.getString("locale.dialog.wait.text"));

        // Current page increment
        scope.search.paging.current = pageNo;
        scope.search.query["_offset"] = (scope.search.paging.current - 1) * scope.search.paging.size;
        scope.search.query["_count"] = scope.search.paging.size;

        // Find async
        OpenIZ.Patient.findAsync({
            query: scope.search.query,
            continueWith: function (r) {
                scope.search.results = r;
                updateResultEncounters();
                scope.$apply();
            },
            onException: function (e) {
                OpenIZ.App.toast(e.message);
            },
            finally: function () {
                OpenIZ.App.hideWait();
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