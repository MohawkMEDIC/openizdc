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
 * User: fyfej
 * Date: 2017-1-16
 */

/// <reference path="../../js/openiz.js"/>
layoutApp.controller("DatabaseController", ["$scope", function ($scope) {

    $scope.doPurge = function (override, backup) {

        // Confirm purge
        if (!override) {
            $("#purgeConfirmModal").modal("show");
            return;
        }
        else
            $("#purgeConfirmModal").modal("hide");

        var doAction = function (force) {
            OpenIZ.App.showWait();
            OpenIZ.App.purgeDataAsync({
                continueWith: function () {
                    OpenIZ.App.close();
                },
                backup: backup,
                finally: function () {
                    OpenIZ.App.hideWait();
                },
                onException: function (ex) {
                    if (ex.type != "PolicyViolationException")
                        console.log(ex.message || ex);
                }
            });
        };
        OpenIZ.Authentication.$elevationCredentials.continueWith = doAction;
        doAction();
    };

    $scope.doRestore = function (override) {

        // Confirm purge
        if (!override) {
            $("#restoreConfirmModal").modal("show");
            return;
        }
        else
            $("#restoreConfirmModal").modal("hide");

        var doAction = function () {
            OpenIZ.App.showWait();
            OpenIZ.App.restoreDataAsync({
                continueWith: function () {
                    OpenIZ.App.close();
                },
                finally: function () {
                    OpenIZ.App.hideWait();
                },
                onException: function (ex) {
                    if (ex.type != "PolicyViolationException")
                        console.log(ex.message || ex);
                }
            });
        };
        OpenIZ.Authentication.$elevationCredentials.continueWith = doAction;
        doAction();
    };

    $scope.hasBackup = function () {
        if ($scope.$parent.about)
            return $.grep($scope.$parent.about.fileInfo, function (a) { return a.id == "bak"; }).length > 0;
    }
}]);