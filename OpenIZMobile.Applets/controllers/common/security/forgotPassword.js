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
 * Date: 2016-10-11
 */

/// <reference path="~/js/openiz.js"/>
layoutApp.controller('ForgotPasswordController', ['$scope', '$window', function ($scope, $window) {

    $scope.resetRequest = {
        purpose: "PasswordReset"
    };

    $scope.displayPage = "#usernameTab";

    // Get mechanisms
    OpenIZ.Authentication.getTfaMechanisms({
        continueWith: function (data) {
            $scope.tfaMechanisms = data;
            $scope.$apply();
        },
        onException: function (ex) {
            OpenIZ.App.toast(ex.message || ex);
        }
    });
    
    $("a[data-toggle='tab'][href='#changePasswordTab']").on('shown.bs.tab', function () {
        $scope.onNext = function () {
            if (!OpenIZ.App.getOnlineState()) {
                console.log(OpenIZ.Localization.getString("locale.error.onlineOnly"));
                return false;
            }

            // Try to change the password
            OpenIZ.App.showWait();
            OpenIZ.Authentication.setPasswordAsync({
                userName: $scope.resetRequest.userName,
                password: $scope.resetRequest.password,
                continueWith: function (data) {
                    OpenIZ.App.toast(OpenIZ.Localization.getString("locale.alert.updateSuccessful"));

                    // Authenticate fully
                    OpenIZ.Authentication.loginAsync({
                        userName: $scope.resetRequest.userName,
                        password: $scope.resetRequest.password,
                        continueWith: function (session) {
                            if (session == null) {
                                console.log(OpenIZ.Localization.getString("err_oauth2_invalid_grant"));
                            }
                            else if (OpenIZ.urlParams["returnUrl"] != null)
                                if (window.location.hash == "")
                                    window.location.hash = "#/";
                            $window.location.reload();
                        },
                        onException: function (exception) {
                            console.log(exception.message || exception);
                        },
                        finally: function () {
                            $("#forgotPasswordWizard").modal("hide");
                            OpenIZ.App.hideWait()
                        }
                    });
                },
                onException: function (exception) {
                    console.log(exception.message || exception);
                },
                finally: function () {
                    OpenIZ.App.hideWait();
                }
            })

            return false;

        };
    });

    /** 
     * Set the reset mechanism
     */
    $scope.setMechanism = function (mechanism) {
        $scope.resetRequest.mechanismModel = mechanism;
        $scope.nextEnabled(true);
    }
    

    $scope.nextEnabled = function (state) {
        if (state)
            $("#nextButton").removeAttr("disabled");
        else
            $("#nextButton").attr("disabled", "disabled");
    };

    /**
     * Proceed to next page!
     */
    $scope.nextWizard = function () {

        // First we want to verify the step occurring 
        if ($scope.onNext == null || $scope.onNext()) {
            $("#forgotPasswordWizard li.active~li:first a").tab('show');
            $scope.nextEnabled(false);
        }
    }

    /** 
     * Send the reset function
     */
    $scope.sendReset = function () {

        // Next step will be verification ... set that up
        $scope.onNext = function () {

            // Online only
            if (!OpenIZ.App.getOnlineState()) {
                alert(OpenIZ.Localization.getString("locale.error.onlineOnly"));
                return false;
            }

            // Try to use the login code
            OpenIZ.App.showWait();
            OpenIZ.Authentication.loginAsync({
                userName: $scope.resetRequest.userName,
                tfaSecret: $scope.resetRequest.tfaSecret,
                continueWith: function (data) {
                    // Set scope next
                    $scope.onNext = null;
                    $scope.nextWizard();
                },
                onException: function (exception) {
                    console.log(exception.message || exception);
                },
                finally: function () {
                    OpenIZ.App.hideWait();
                }
            });
            return false;
        }

        // Online only
        if (!OpenIZ.App.getOnlineState()) {
            console.log(OpenIZ.Localization.getString("locale.error.onlineOnly"));
            return false;
        }

        OpenIZ.App.showWait();
        OpenIZ.Authentication.sendTfaSecretAsync({
            data: $scope.resetRequest,
            continueWith: function (data) {
                $scope.resetResponse = true;
                $scope.$apply();
            },
            onException: function (ex) {
                console.log(ex.message || ex);
            },
            finally: function () {
                OpenIZ.App.hideWait();
            }
        });
    };

}]);