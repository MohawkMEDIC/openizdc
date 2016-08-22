/// <reference path="../../js/openiz-model.js"/>
/// <reference path="../../js/openiz.js"/>
layoutApp.controller('ForgotPasswordController', ['$scope', function ($scope) {

    $scope.resetPassword = function (username) {

        if (username !== undefined)
        {

        }
        else
        {
            OpenIZ.App.toast("Unable to reset password");
        }
    };
}]);