/// <reference path="../../js/openiz-model.js"/>
/// <reference path="../../js/openiz.js"/>

layoutApp.controller('UserPasswordController', ['$scope', '$rootScope', function ($scope, $rootScope) {
    
    $scope.changePassword = function (userName, existing, password, confirmation) {

        // Are we even online?
        if (!$rootScope.page.onlineState)
            alert(OpenIZ.Localization.getString("locale.error.onlineOnly"));
        else if(password != confirmation) 
            alert(OpenIZ.Localization.getString("locale.settings.passwordNoMatch"));
        else {
            
            OpenIZ.App.showWait();

            // Re-authenticate the user to verify their current password
            OpenIZ.Authentication.loginAsync({
                userName: userName,
                password: existing,
                continueWith: function (session) {
                    if(session != null) // success for auth, let's change the password
                        OpenIZ.Authentication.setPasswordAsync({
                            userName: userName,
                            password: password,
                            continueWith: function (result) {
                                OpenIZ.App.toast(OpenIZ.Localization.getString("locale.preferences.passwordChanged"));
                            },
                            onException: function (ex) {
                                if (ex.message != null)
                                    alert(ex.message);
                                else
                                    alert(ex);
                            },
                            finally: function () {
                                OpenIZ.App.hideWait();
                            }
                        })
                },
                onException: function (ex) { // Some error with authentication
                    if (ex.message != null)
                        alert(ex.message);
                    else
                        alert(ex);
                    OpenIZ.App.hideWait(); // We won't make it to the other finally :( 
                }
            })
        }
    };
}]);