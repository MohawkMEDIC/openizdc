/// <reference path="~/js/openiz.js"/>

/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/jquery.min.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/controllers/layouts/navbar.js"/>

layoutApp.controller('ViewAlertController', ['$scope', '$stateParams', function ($scope, $stateParams)
{

    OpenIZ.App.getAlertsAsync({
        query: {
            id: $stateParams.alertId,
            _count: 1
        },
        onException: function (ex)
        {
            OpenIZ.App.hideWait();

            if (typeof (ex) == "string")
                alert(ex);
            else if (ex.message != undefined)
                alert("" + ex.message + " - " + ex.details);
            else
                alert(ex);
        },
        continueWith: function (data)
        {
            $scope.alert = data[0];
            console.log($scope.alert);
            $scope.alert.body = $scope.alert.body.replace("\n", "<br/>");
            $scope.$apply();
        }
    });

    $scope.deleteAlert = function (alert)
    {
        alert.flags = 2;

        $scope.alert.creationTime = $scope.alert.time;

        OpenIZ.App.saveAlertAsync({
            data: alert,
            continueWith: function (data)
            {
                OpenIZ.App.showWait();
                OpenIZ.App.toast(OpenIZ.Localization.getString("locale.alert.updateSuccessful"));
            },
            onException: function (ex)
            {
                console.log(ex);
                OpenIZ.App.toast(OpenIZ.Localization.getString("locale.alert.updateUnsuccessful"));
            },
            finally: function ()
            {
                OpenIZ.App.hideWait();
            }
        });
    };

    $scope.updateAlert = function ()
    {
        $scope.alert.flags = 2;

        $scope.alert.creationTime = $scope.alert.time;

        OpenIZ.App.saveAlertAsync({
            data: $scope.alert,
            continueWith: function (data)
            {
                OpenIZ.App.showWait();
                OpenIZ.App.toast(OpenIZ.Localization.getString("locale.alert.updateSuccessful"));
            },
            onException: function (ex)
            {
                console.log(ex);
                OpenIZ.App.toast(OpenIZ.Localization.getString("locale.alert.updateUnsuccessful"));
            },
            finally: function ()
            {
                OpenIZ.App.hideWait();
            }
        });
    };

}]);