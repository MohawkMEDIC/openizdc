/// <reference path="~/js/openiz.js"/>

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
 * Date: 2016-8-17
 */

/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/jquery.min.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/controllers/layouts/navbar.js"/>

layoutApp.controller('ViewAlertController', ['$scope', '$stateParams', function ($scope, $stateParams)
{
    $scope.selectedMessageID = null;

    $scope.deleteAlert = deleteAlert;
    $scope.updateAlert = updateAlert;
    $scope.closeMessage = closeMessage;

    init();

    function init() {
        $scope.selectedMessageID = $stateParams.alertId == '' ? null : $stateParams.alertId;

        if ($scope.selectedMessageID !== null)
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
                        console.log(ex);
                    else if (ex.message != undefined)
                        console.log("" + ex.message + " - " + ex.details);
                    else
                        console.log(ex);
                },
                continueWith: function (data)
                {
                    $scope.alert = data[0];
                    console.log($scope.alert);
                    $scope.alert.body = $scope.alert.body.replace("\n", "<br/>");
                    $scope.$apply();
                }
            });
        }
    }    

    function deleteAlert(alert)
    {
        alert.flags = 2;

        $scope.alert.creationTime = $scope.alert.time;

        OpenIZ.App.deleteAlertAsync({
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

    function updateAlert()
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

    function closeMessage() {
        $scope.selectedMessageID = null;
        delete $scope.alert;
    }
}]);