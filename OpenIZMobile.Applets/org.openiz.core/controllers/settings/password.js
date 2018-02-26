/// <reference path="../../js/openiz-model.js"/>
/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-10-30
 */

/// <reference path="../../js/openiz.js"/>

angular.module('layout').controller('UserPasswordController', ['$scope', '$rootScope', 'regexService', function ($scope, $rootScope, regexService) {

    $scope.regexService = regexService;

    $scope.changePassword = function (userName, existing, password, confirmation) {

        // Are we even online?
        if (!$rootScope.page.onlineState)
            console.log(OpenIZ.Localization.getString("locale.action.onlineOnly"));
        else if (password != confirmation)
            console.log(OpenIZ.Localization.getString("locale.settings.passwordNoMatch"));
        else {
            OpenIZ.App.showWait();
            OpenIZ.App.showWait('#changePasswordButton');

            // Re-authenticate the user to verify their current password
            OpenIZ.Authentication.loginAsync({
                userName: userName,
                password: existing,
                continueWith: function (session) {
                    if (session != null) // success for auth, let's change the password
                    {
                        OpenIZ.Authentication.setPasswordAsync({
                            userName: userName,
                            password: password,
                            continueWith: function (result) {
                                // Remove dirty and touched from inputs
                                delete ($scope.changeData.existing);
                                delete ($scope.changeData.password);
                                delete ($scope.changeData.confirmation);
                                
                                $scope.changePasswordForm.$setPristine();
                                $scope.changePasswordForm.$setUntouched();

                                OpenIZ.App.toast(OpenIZ.Localization.getString("locale.preferences.passwordChanged"));

                                $scope.$apply();
                            },
                            onException: function (ex) {
                                if (ex.message != null)
                                    console.log(ex.message);
                                else
                                    console.log(ex);

                                OpenIZ.App.toast(OpenIZ.Localization.getString("locale.preferences.errors.changePasswordFailure"));
                            },
                            finally: function () {
                                OpenIZ.App.hideWait();
                                OpenIZ.App.hideWait('#changePasswordButton');
                            }
                        });
                    }
                },
                onException: function (ex) { // Some error with authentication
                    if (ex.message != null)
                        console.log(ex.message);
                    else
                        console.log(ex);
                    OpenIZ.App.toast(OpenIZ.Localization.getString("locale.preferences.errors.changePasswordFailure"));
                    OpenIZ.App.hideWait(); // We won't make it to the other finally :( 
                    OpenIZ.App.hideWait('#changePasswordButton');
                }
            })
        }
    };
}]);