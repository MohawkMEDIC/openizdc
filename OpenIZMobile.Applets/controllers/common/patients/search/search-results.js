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
        scope.search.pageSize = scope.search.pageSize || 10;

        /** 
         * @summary Advances to the next set of results
         */
        scope.search.search = scope.search.search || function () {
            OpenIZ.App.showWait(OpenIZ.Localization.getString("locale.patients.search.wait"));
            OpenIZ.Patient.findAsync({
                query: scope.search.query,
                offset: 0,
                count: scope.search.pageSize,
                continueWith: function (r) {
                    scope.search.results = r.item;
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
            OpenIZ.Patient.findAsync({
                query: scope.search.query, 
                offset: scope.search.results.offset + scope.search.pageSize,
                count: scope.search.pageSize,
                continueWith: function (r) {
                    scope.search.results = r.all('Patient');
                    scope.$apply();
                },
                onException: function (e) {
                    OpenIZ.App.toast(e.message);
                }
            });
        };

        /** 
         * @summary Goes to the previous page
         */
        scope.search.previous = scope.search.previous || function () {
            OpenIZ.Patient.findAsync({
                query: scope.search.query, 
                offset: scope.search.results.offset - scope.search.pageSize,
                count: scope.search.pageSize,
                continueWith: function (r) {
                    scope.search.results = r.all('Patient');
                    scope.$apply();
                },
                onException: function (e) {
                    OpenIZ.App.toast(e.message);
                }
            });
        };

        /**
         * @summary Goes to the specific page
         */
        scope.search.goPage = scope.search.goPage || function (pageNo) {
            OpenIZ.Patient.findAsync({
                query: scope.search.query,
                offset: pageNo * scope.search.pageSize,
                count: scope.search.pageSize,
                continueWith: function (r) {
                    scope.search.results = r.all('Patient');
                    scope.$apply();
                },
                onException: function (e) {
                    OpenIZ.App.toast(e.message);
                }
            });
        };

    });
});