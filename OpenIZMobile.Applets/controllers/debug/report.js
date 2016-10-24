/// <reference path="~/js/openiz.js"/>

/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/jquery.min.js"/>
/// <reference path="~/lib/angular.min.js"/>

layoutApp.controller('ReportBugController', ['$scope', '$rootScope', function ($scope, $rootScope) {

    $rootScope.$watch('session', function(n, o) {
        if(n != null)
            $scope.report = {
                submitterKey: $rootScope.session.entity.id,
                submitterModel: $rootScope.session.entity,
                _includeData: true
            };
    });


    // Cancel
    $scope.close = function (form) {
        if(form.$touched && confirm(OpenIZ.Localization.getString("locale.confirm.loseChanges")) || !form.$touched)
            window.history.back();
    }

    // Submit
    $scope.submitBugReport = function (form) {
        if (form.$invalid) {
            alert(OpenIZ.Localization.getString("locale.common.invalidForm"));
            return;
        }

        // Submit
        OpenIZ.App.showWait();
        OpenIZ.App.submitBugReportAsync({
            data: $scope.report,
            continueWith: function (data) {
                $scope.report = data;
                $scope.$apply();
            },
            onException: function (error) {
                if (error.message)
                    alert(error.message);
                else
                    alert(error);
            },
            finally: function () {
                OpenIZ.App.hideWait();
            }
        });
    }
}]);