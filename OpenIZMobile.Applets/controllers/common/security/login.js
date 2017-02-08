/// <reference path="~/js/openiz-model.js"/>

/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-7-23
 */

/// <reference path="~/js/openiz.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>
/// <reference path="~/lib/bootstrap.min.js"/>

layoutApp.controller('LoginPartController', ['$scope', '$window', function ($scope, $window) {
        // Get the current scope that we're in

        $scope.showPasswordReset = $scope.showPasswordReset || function () {
            $('#passwordResetDialog').modal('show');
        };

        $scope.login = $scope.login || function (form, username, password) {

            if (!form.$valid)
            {
                console.log(OpenIZ.Localization.getString("locale.security.login.invalid"));
                return;
            }
            OpenIZ.App.showWait('#loginButton');
            OpenIZ.Authentication.loginAsync({
                userName: username,
                password: password,
                continueWith: function (session) {
                    if (session == null) {
                        alert(OpenIZ.Localization.getString("err_oauth2_invalid_grant"));
                    }
                    else {
                        if(window.location.hash == "")
                            window.location.hash = "#/";
                        $window.location.reload();
                    }
                },
                onException: function (ex) {
                    //OpenIZ.App.hideWait('#loginButton');
                    OpenIZ.App.hideWait('#loginButton');


                if (typeof (ex) == "string")
                    console.log(ex);
                else if (ex.message != undefined)
                    alert("" + OpenIZ.Localization.getString(ex.message) + " - " + OpenIZ.Localization.getString(ex.details));
                else
                    console.log(ex);
                },
                finally: function () {
                }
        });
    }; // scope.login

        
}]);