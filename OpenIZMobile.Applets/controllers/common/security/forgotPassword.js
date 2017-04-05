/// <reference path="~/js/openiz-model.js"/>
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
 * Date: 2017-3-31
 */

/// <reference path="~/js/openiz.js"/>
layoutApp.controller('ForgotPasswordController', ['$scope', '$window', 'regexService', function ($scope, $window, regexService) {

    var controller = this;

    $scope.resetRequest = {
        purpose: "PasswordReset"
    };

    $scope.regexService = regexService;
    $scope.displayPage = "#usernameTab";
    $scope.verificationPlaceholder = "";

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

    $(document).on('shown.bs.tab', 'a[data-toggle="tab"][href="#challengeTab"]', function () {
        switch ($scope.resetRequest.mechanism) {
            case 'd919457d-e015-435c-bd35-42e425e2c60c':
                $scope.verificationPlaceholder = OpenIZ.Localization.getString("locale.forgotPassword.challengeRequest.placeholderEmail");
                break;
            case '08124835-6c24-43c9-8650-9d605f6b5bd6':
                $scope.verificationPlaceholder = OpenIZ.Localization.getString("locale.forgotPassword.challengeRequest.placeholderPhone");
                break;
            default:
                break;
        }
    });

    $(document).on('shown.bs.tab', 'a[data-toggle="tab"][href="#changePasswordTab"]', function () {
        $('#submitButton').show();
        $('#nextButton').hide();
    });

    /**
     * Reset the forgot password wizard on close
     */
    $("#passwordResetDialog").on('hidden.bs.modal', function () {
        $(':input', '#passwordResetDialog')
            .not(':button, :submit, :reset')
            .val('')
            .removeAttr('selected')
            .removeAttr('checked');

        $('#submitButton').hide();
        $('#nextButton').show();

        // Remove dirty and touched from inputs
        delete ($scope.resetRequest.mechanism);
        delete ($scope.resetRequest.mechanismModel);
        controller.forgotPasswordForm.$setPristine();
        controller.forgotPasswordForm.$setUntouched();

        // Set to first tab and reset progress
        $("#forgotPasswordWizard li:first a").tab('show');
        $scope.nextEnabled(false);
        $scope.onNext = null;
        $scope.resetResponse = false;
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
                OpenIZ.App.toast(OpenIZ.Localization.getString("locale.forgotPassword.error.onlineOnly"));
                return false;
            }

            // Try to use the login code
            OpenIZ.Authentication.loginAsync({
                userName: $scope.resetRequest.userName,
                tfaSecret: $scope.resetRequest.tfaSecret,
                continueWith: function (data) {
                    // Set scope next
                    $scope.onNext = null;
                    $scope.nextWizard();

                    // TODO: reset scope or hash window location here
                },
                onException: function (exception) {
                    OpenIZ.App.toast(OpenIZ.Localization.getString("locale.forgotPassword.error.invalidCode"));
                    console.log(exception.message || exception);
                },
                finally: function () {
                }
            });
            return false;
        }

        // Online only
        if (!OpenIZ.App.getOnlineState()) {
            OpenIZ.App.toast(OpenIZ.Localization.getString("locale.forgotPassword.error.onlineOnly"));
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
                OpenIZ.App.toast(OpenIZ.Localization.getString("locale.forgotPassword.error.validation"));
                console.log(ex.message || ex);
            },
            finally: function () {
                OpenIZ.App.hideWait();
            }
        });
    };

    /**
     * Submit the final password change
     */
    $scope.submitPasswordChange = function () {
        if (!OpenIZ.App.getOnlineState()) {
            console.log(OpenIZ.Localization.getString("locale.forgotPassword.error.onlineOnly"));
            return false;
        }

        // Try to change the password
        OpenIZ.App.showWait();
        OpenIZ.App.showWait('#submitButton');
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
                            OpenIZ.App.toast(OpenIZ.Localization.getString("err_oauth2_invalid_grant"));
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
                        OpenIZ.App.hideWait();
                    }
                });
            },
            onException: function (exception) {
                OpenIZ.App.hideWait('#submitButton');
                console.log(exception.message || exception);
            },
            finally: function () {
                OpenIZ.App.hideWait();
            }
        });

        return false;
    };

}]);