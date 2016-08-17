/// <reference path="../../js/openiz-model.js"/>
/// <reference path="../../js/openiz.js"/>
layoutApp.controller('UserProfileController', ['$scope', function ($scope) {
    $scope.savePreferences = function (user) {

        $scope.securityUser = new OpenIZModel.SecurityUser(user);

        OpenIZ.Security.updateUser($scope.securityUser);
    };
}]);