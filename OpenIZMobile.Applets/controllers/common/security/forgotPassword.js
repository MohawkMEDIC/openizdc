/// <reference path="../../js/openiz-model.js"/>
/// <reference path="../../js/openiz.js"/>
layoutApp.controller('ForgotPasswordController', ['$scope', function ($scope) {

    $scope.resetPassword = function (username) {

        if (username !== undefined)
        {
            OpenIZ.Authentication.getUserAsync({
                data: {
                    userName: username
                },
                continueWith: function(data)
                {
                    window.location.href = '~/views/common/security/resetPassword.html';
                },
                onException: function(error)
                {
                    OpenIZ.App.toast(OpenIZ.Localization.getString("locale.security.errors.unableToResetPassword"));
                }
            });
        }
        else
        {
            OpenIZ.App.toast(OpenIZ.Localization.getString("locale.security.errors.unableToResetPassword"));
        }
    };
}]);