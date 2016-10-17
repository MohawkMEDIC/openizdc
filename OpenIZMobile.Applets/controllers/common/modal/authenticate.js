/// <reference path="~/js/openiz-model.js"/>

/// <reference path="~/js/openiz.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>
/// <reference path="~/lib/bootstrap.min.js"/>
layoutApp.controller('AuthenticationDialogController', ['$scope', function ($scope) {

    $scope.authenticate = function (form) {
        if(!form.$valid) 
        {
            alert(OpenIZ.Localization.getString('locale.error.login.invalid'));
            return;
        }

        // Do an authentication that is not session binding
        OpenIZ.Authentication.loginAsync({
            userName: $scope.username,
            password: $scope.password,
            scope: "/elevation",
            continueWith: function(session) {
                if (session == null) {
                    alert(OpenIZ.Localization.getString("err_oauth2_invalid_grant"));
                }
                else {
                    OpenIZ.Authentication.$elevationCredentials = {
                        userName : $scope.username,
                        password : $scope.password,
                        $enabled : true,
                        continueWith: OpenIZ.Authentication.$elevationCredentials.continueWith
                    };
                    OpenIZ.Authentication.hideElevationDialog();
                    if(OpenIZ.Authentication.$elevationCredentials.continueWith != null)
                        OpenIZ.Authentication.$elevationCredentials.continueWith();
                }
            },
            onException: function(error) {
                if(error.error != null)
                    alert(error.error);
                else if(error.message != null)
                    alert(error.message);
                else
                    alert(error);
            }
        })
    }
    
}]);