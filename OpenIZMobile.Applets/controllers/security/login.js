/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/js/openiz.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('LoginController', ['$scope', function ($scope) {

    $scope.config = {
        realmName: OpenIZ.Configuration.getRealm()
    };


    $scope.login = function (username, password) {
        $('#loginButton').attr('disabled', 'disabled');
        OpenIZ.Authentication.loginAsync({
            userName: username,
            password: password,
            continueWith: function (session) {
                if (session == null) {
                    alert(OpenIZ.Localization.getString("err_oauth2_invalid_grant"));
                    $('#loginButton').removeAttr('disabled');
                }
                else
                    window.location = OpenIZ.urlParams["returnUrl"];
            },
            onException: function (ex) {
                $('#loginButton').removeAttr('disabled');

                if (typeof (ex) == "string")
                    alert(ex);
                else if (ex.message != undefined)
                    alert("" + ex.message + " - " + ex.details);
                else
                    alert(ex);
            }
        });
    };

    $scope.setLocale = function (lang) {
        OpenIZ.Localization.setLocale(lang);
        window.location.reload();
    }


}]);