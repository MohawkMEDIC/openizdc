/// <reference path="../../js/openiz-model.js"/>
/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-10-30
 */

/// <reference path="../../js/openiz.js"/>
angular.module('layout').controller("DatabaseController", ["$scope", function ($scope) {

    // Compact the database
    $scope.doCompact = function () {

        var doAction = function () {
            OpenIZ.App.showWait("#compactButton");
            OpenIZ.App.compactAsync({
                continueWith: function() {
                    OpenIZ.Authentication.$elevationCredentials = {};
                    OpenIZ.App.getInfoAsync({
                        continueWith: function (data) {
                            $scope.about = data;
                            $scope.$apply();
                        }
                    });
                },
                onException: function (ex) {
                    if(ex.type != "PolicyViolationException")
                        alert(ex.message);
                },
                finally: function () {
                    OpenIZ.App.hideWait("#compactButton");
                }
            });
        }
        OpenIZ.Authentication.$elevationCredentials.continueWith = doAction;
        doAction();

    };

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
                    OpenIZ.Authentication.$elevationCredentials = {};
                },
                backup: backup,
                finally: function () {
                    OpenIZ.App.hideWait();
                },
                onException: function (ex) {
                    if (ex.type != "PolicyViolationException")
                        alert(ex.message || ex);
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
            OpenIZ.App.showWait('#btnRestore');
            OpenIZ.App.restoreDataAsync({
                continueWith: function () {
                    OpenIZ.App.close();
                    OpenIZ.Authentication.$elevationCredentials = {};
                },
                finally: function () {
                    OpenIZ.App.hideWait('#btnRestore');
                },
                onException: function (ex) {
                    if (ex.type != "PolicyViolationException")
                        alert(ex.message || ex);
                }
            });
        };
        OpenIZ.Authentication.$elevationCredentials.continueWith = doAction;
        doAction();
    };

    $scope.doBackup = function (override) {

        // Confirm purge
        if (!override) {
            $("#backupConfirmModal").modal("show");
            return;
        }
        else
            $("#backupConfirmModal").modal("hide");

        var doAction = function () {
            OpenIZ.App.showWait('#btnBackup');
            OpenIZ.App.backupDataAsync({
                continueWith: function () {
                    if (confirm(OpenIZ.Localization.getString("locale.settings.data.backup.restart")))
                        OpenIZ.App.close();
                    OpenIZ.Authentication.$elevationCredentials = {};
                },
                finally: function () {
                    OpenIZ.App.hideWait('#btnBackup');
                },
                onException: function (ex) {
                    if (ex.type != "PolicyViolationException")
                        alert(ex.message || ex);
                }
            });
        };
        OpenIZ.Authentication.$elevationCredentials.continueWith = doAction;
        doAction();
    };


    // Returns true if the parent scope has a backup
    $scope.hasBackup = false;
    
    OpenIZ.App.getBackupAsync({
        continueWith: function (backup) {
            $scope.hasBackup = backup;
            $scope.$apply();
        }
    });
    
}]);