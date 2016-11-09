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
        scope.search.paging = scope.search.paging || {
            size: 10
        };

        var updateResultEncounters = function () {
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
        scope.search.search = scope.search.search || function (nonInteractive) {
            console.log("hihihih");
            if (!nonInteractive) OpenIZ.App.showWait(OpenIZ.Localization.getString("locale.dialog.wait.text"));
            scope.search.query["_offset"] = 0;
            scope.search.query["_count"] = scope.search.paging.size;

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
                    OpenIZ.App.hideWait();
                }
            });

        };
        /** 
         * @summary Advances to the next set of results
         */
        scope.search.next = scope.search.next || function () {
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
        scope.search.previous = scope.search.previous || function () {
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
        scope.search.goPage = scope.search.goPage || function (pageNo) {
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

        scope.act = { };

        /**
        * @summary Starts an encounter for a patient.
        */
        scope.startEncounter = function (patient) {
            scope.doStartEncounter(patient);
        };

}]);