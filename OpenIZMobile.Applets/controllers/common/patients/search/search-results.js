/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>

// Document ready, bind search results to their related contexts
$(document).ready(function () {
    $('table.oiz-patient-results').each(function (i,e) {

        // Get the current scope that we're in
        var scope = angular.element(e).scope();

        // If the current scope does not have required values bind them
        scope.search = scope.search || {};
        scope.search.query = scope.search.query || {};
        scope.search.results = null;
        scope.search.paging = scope.search.paging || {
            size: 10
        };

        /** 
         * @summary Advances to the next set of results
         */
        scope.search.search = scope.search.search || function () {
            OpenIZ.App.showWait(OpenIZ.Localization.getString("locale.patients.search.wait"));
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
            OpenIZ.App.showWait(OpenIZ.Localization.getString("locale.patients.search.wait"));
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
            OpenIZ.App.showWait(OpenIZ.Localization.getString("locale.patients.search.wait"));
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
            OpenIZ.App.showWait(OpenIZ.Localization.getString("locale.patients.search.wait"));

            // Current page increment
            scope.search.paging.current = pageNo;
            scope.search.query["_offset"] = (scope.search.paging.current - 1) * scope.search.paging.size;
            scope.search.query["_count"] = scope.search.paging.size;

            // Find async
            OpenIZ.Patient.findAsync({
                query: scope.search.query,
                continueWith: function (r) {
                    scope.search.results = r;
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

    });
});