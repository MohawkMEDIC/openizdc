/// <reference path="~/js/openiz-model.js"/>

/// <reference path="~/js/openiz.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>
/// <reference path="~/lib/bootstrap.min.js"/>

angular.element(document).ready(function () {
    $('form.form-login').each(function (k, e) {
        
        // Get the current scope that we're in
        var scope = angular.element(e).scope();

        scope.showPasswordReset = scope.showPasswordReset || function () {
            $('#passwordResetDialog').modal('show');
        };

        scope.login = scope.login || function (form, username, password) {

            if (!form.$valid)
            {
                alert(OpenIZ.Localization.getString("locale.security.login.invalid"));
                return;
            }
            OpenIZ.App.showWait();
            OpenIZ.Authentication.loginAsync({
                userName: username,
                password: password,
                continueWith: function (session) {
                    if (session == null) {
                        alert(OpenIZ.Localization.getString("err_oauth2_invalid_grant"));
                    }
                    else if(OpenIZ.urlParams["returnUrl"] != null)
                        window.location = OpenIZ.urlParams["returnUrl"];
                },
                onException: function (ex) {
                    OpenIZ.App.hideWait();


                    if (typeof (ex) == "string")
                        alert(ex);
                    else if (ex.message != undefined)
                        alert("" + ex.message + " - " + ex.details);
                    else
                        alert(ex);
                }
            });
        }; // scope.login

    });
});