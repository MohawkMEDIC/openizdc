/// <reference path="../../js/openiz-model.js"/>
/// <reference path="../../js/openiz.js"/>

layoutApp.controller('UserPasswordController', ['$scope', function ($scope) {
    $scope.changePassword = function (userName, existing, password, confirmation) {
        OpenIZ.Security.changePassword(userName, existing, password, confirmation);
    };
}]);