/// <reference path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>

angular.module('layout').controller('ResubmitDataController', ['$scope', '$rootScope', function ($scope, $rootScope) {

    // Resubmit data controller
    $scope.resubmit = {
        from: new Date(),
        to: new Date()
    };
    $scope.resubmit = resubmit;

    /** 
     * @summary Performs the resubmission
     */
    function resubmit(resubmitForm) {
        if (resubmitForm.$invalid) {
            alert(OpenIZ.Localization.getString("locale.settings.data.resubmit.formError"));
            return false;
        }

        // Perform the actual resubmission of the data
        var doResubmit = function () {
            OpenIZ.App.showWait("#resubmitButton");
            OpenIZ.Queue.resubmitAsync({
                from: $scope.resubmit.from,
                to: $scope.resubmit.to,
                continueWith: function (data) {
                    alert(OpenIZ.Localization.getString("locale.settings.data.resubmit.success"));
                },
                onException: function (ex) {
                    if (ex.type == "PolicyViolationException") return;
                    if (ex.message)
                        alert(OpenIZ.Localization.getString(ex.message));
                    else
                        console.error(ex.message);
                },
                finally: function (data) {
                    OpenIZ.App.hideWait("#resubmitButton");
                }
            });
        }

        OpenIZ.Authentication.$elevationCredentials = { continueWith: doResubmit };
        doResubmit();
    }
}]);
