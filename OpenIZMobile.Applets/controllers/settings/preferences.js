/// <reference path="../../js/openiz-model.js"/>
/// <reference path="../../js/openiz.js"/>

layoutApp.controller('PreferencesController', ['$scope', function ($scope) {
    $scope.isNetworkAvailable = function () {
        return OpenIZ.App.networkAvailable();
    };
}]);