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
 * User: justi
 * Date: 2016-8-17
 */

/// <reference path="../../js/openiz.js"/>

layoutApp.controller('UserPasswordController', ['$scope', '$rootScope', function ($scope, $rootScope) {

    $rootScope.$watch('session', function (nv, ov) {
        $scope.changeData = {
            phone: $rootScope.session.user.phoneNumber,
            email: $rootScope.session.user.email
        };
    });

    $scope.changePassword = function (userName, existing, password, confirmation, email, telephone) {

        // Are we even online?
        if (!$rootScope.page.onlineState)
            console.log(OpenIZ.Localization.getString("locale.error.onlineOnly"));
        else if (password != confirmation)
            console.log(OpenIZ.Localization.getString("locale.settings.passwordNoMatch"));
        else {

            OpenIZ.App.showWait();

            // Re-authenticate the user to verify their current password
            OpenIZ.Authentication.loginAsync({
                userName: userName,
                password: existing,
                continueWith: function (session) {
                    if (session != null) // success for auth, let's change the password
                    {
                        // Password set?
                        if(password)
                            OpenIZ.Authentication.setPasswordAsync({
                                userName: userName,
                                password: password,
                                continueWith: function (result) {
                                    OpenIZ.App.toast(OpenIZ.Localization.getString("locale.preferences.passwordChanged"));

                                },
                                onException: function (ex) {
                                    if (ex.message != null)
                                        console.log(ex.message);
                                    else
                                        console.log(ex);
                                },
                                finally: function () {
                                    OpenIZ.App.hideWait();
                                }
                            });

                        // Email set?
                        if (email && email != $rootScope.session.user.email ||
                            telephone && telephone != $rootScope.session.user.phoneNumber)
                            OpenIZ.Authentication.saveUserAsync({
                                data: new OpenIZModel.SecurityUser({
                                    id: $rootScope.session.user.id,
                                    email: email,
                                    phoneNumber: telephone
                                }),
                                continueWith: function (result) {
                                    OpenIZ.App.toast(OpenIZ.Localization.getString("locale.preferences.recoveryOptions.success"));
                                    $rootScope.session.user = result;
                                }
                            });


                    }
                },
                onException: function (ex) { // Some error with authentication
                    if (ex.message != null)
                        console.log(ex.message);
                    else
                        console.log(ex);
                    OpenIZ.App.hideWait(); // We won't make it to the other finally :( 
                }
            })
        }
    };
}]);