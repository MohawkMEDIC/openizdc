/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/js/openiz.js"/>
layoutApp.controller('ForgotPasswordController', ['$scope', function ($scope) {

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
                alert(OpenIZ.Localization.getString("locale.error.onlineOnly"));
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
                                alert(OpenIZ.Localization.getString("err_oauth2_invalid_grant"));
                            }
                            else if (OpenIZ.urlParams["returnUrl"] != null)
                                window.location = OpenIZ.urlParams["returnUrl"];
                        },
                        onException: function (exception) {
                            alert(exception.message || exception);
                        },
                        finally: function () {
                            OpenIZ.App.hideWait()
                        }
                    });
                },
                onException: function (exception) {
                    alert(exception.message || exception);
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
                    alert(exception.message || exception);
                },
                finally: function () {
                    OpenIZ.App.hideWait();
                }
            });
            return false;
        }

        // Online only
        if (!OpenIZ.App.getOnlineState()) {
            alert(OpenIZ.Localization.getString("locale.error.onlineOnly"));
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
                alert(ex.message || ex);
            },
            finally: function () {
                OpenIZ.App.hideWait();
            }
        });
    };

}]);