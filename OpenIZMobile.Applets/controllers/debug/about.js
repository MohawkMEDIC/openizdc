/// <reference path="~/js/openiz.js"/>

/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/jquery.min.js"/>
/// <reference path="~/lib/angular.min.js"/>

layoutApp.controller('AboutApplicationController', ['$scope', function ($scope) {

    $scope.data = {};
    // Get information asynchronously
    OpenIZ.App.getInfoAsync({
        continueWith: function (data) {
            $scope.about = data;
            $scope.$apply();
        }
    })
}]);