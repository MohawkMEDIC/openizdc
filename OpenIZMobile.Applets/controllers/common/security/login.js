/// <reference path="~/js/openiz-model.js"/>

/// <reference path="~/js/openiz.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>
/// <reference path="~/lib/bootstrap.min.js"/>

$(document).ready(function () {
    $('form.form-login').each(function (k, e) {
        
        // Get the current scope that we're in
        var scope = angular.element(e).scope();

        scope.login = scope.login || function (username, password) {
            $('#waitModal').modal('show');
            OpenIZ.Authentication.loginAsync({
                userName: username,
                password: password,
                continueWith: function (session) {
                    if (session == null) {
                        alert(OpenIZ.Localization.getString("err_oauth2_invalid_grant"));
                    }
                    else
                        window.location = OpenIZ.urlParams["returnUrl"];
                },
                onException: function (ex) {
                    $('#waitModal').modal('hide');

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