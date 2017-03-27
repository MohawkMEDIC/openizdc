/// <reference path="../../js/openiz-model.js"/>
/// <reference path="../../js/openiz.js"/>
layoutApp.controller("DatabaseController", ["$scope", function ($scope) {

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
            OpenIZ.App.showWait('#purgeButton');
            OpenIZ.App.restoreDataAsync({
                continueWith: function () {
                    OpenIZ.App.close();
                    OpenIZ.Authentication.$elevationCredentials = {};
                },
                finally: function () {
                    OpenIZ.App.hideWait('#purgeButton');
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