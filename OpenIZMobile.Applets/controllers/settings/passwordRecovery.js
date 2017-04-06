/// <reference path="../../js/openiz-model.js"/>
/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * User: kirkleyd
 * Date: 2017-4-5
 */

/// <reference path="../../js/openiz.js"/>

layoutApp.controller('UserPasswordRecoveryController', ['$scope', '$rootScope', function ($scope, $rootScope) {

    $rootScope.$watch('session', function (nv, ov) {
        $scope.changeData = {
            phoneNumber: $rootScope.session.user.phoneNumber,
            email: $rootScope.session.user.email
        };
    });

    $scope.changePasswordRecovery = function (userName, existing, email, phoneNumber) {

        // Are we even online?
        if (!$rootScope.page.onlineState)
            console.log(OpenIZ.Localization.getString("locale.action.onlineOnly"));
        else {
            OpenIZ.App.showWait();
            OpenIZ.App.showWait('#changePasswordRecoveryButton');

            // Re-authenticate the user to verify their current password
            OpenIZ.Authentication.loginAsync({
                userName: userName,
                password: existing,
                continueWith: function (session) {
                    if (session != null) // success for auth, let's change the password
                    {
                        // Email set?
                        if (email && email != $rootScope.session.user.email ||
                            phoneNumber && phoneNumber != $rootScope.session.user.phoneNumber) {
                            OpenIZ.Authentication.saveUserAsync({
                                data: new OpenIZModel.SecurityUser({
                                    id: $rootScope.session.user.id,
                                    email: email,
                                    phoneNumber: phoneNumber
                                }),
                                continueWith: function (result) {
                                    OpenIZ.App.toast(OpenIZ.Localization.getString("locale.preferences.recoveryOptions.success"));
                                    $rootScope.session.user = result;
                                },
                                onException: function (ex) {
                                    if (ex.message != null)
                                        console.log(ex.message);
                                    else
                                        console.log(ex);
                                    OpenIZ.App.toast(OpenIZ.Localization.getString("locale.preferences.errors.changePasswordRecoveryFailure"));
                                }
                            });
                        }
                        else {
                            // email and password aren't different, no changes needed
                            OpenIZ.App.toast(OpenIZ.Localization.getString("locale.preferences.recoveryOptions.success"));
                        }
                    }
                },
                onException: function (ex) { // Some error with authentication
                    if (ex.message != null)
                        console.log(ex.message);
                    else
                        console.log(ex);
                    OpenIZ.App.toast(OpenIZ.Localization.getString("locale.preferences.errors.changePasswordRecoveryFailure"));
                },
                finally: function () {
                    OpenIZ.App.hideWait();
                    OpenIZ.App.hideWait('#changePasswordRecoveryButton');
                }
            })
        }
    };
}]);