/// <reference path="~/js/openiz.js"/>
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
 * Date: 2016-7-23
 */

/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/jquery.min.js"/>
/// <reference path="~/lib/angular.min.js"/>

layoutApp.controller('AboutApplicationController', ['$scope', '$state', function ($scope, $state) {

    $scope.data = {};
    // Get information asynchronously
    OpenIZ.App.getInfoAsync({
        includeUpdates: true,
        continueWith: function (data) {
            $scope.about = data;

            // Updates 
            if(data.update)
                for (var i in data.applet) {
                    data.applet[i].update = $.grep(data.update, function (e) { return e.id == data.applet[i].id; });
                    if (Array.isArray(data.applet[i].update))
                        data.applet[i].update = data.applet[i].update[0];
                }

            $scope.$apply();
            OpenIZ.App.getLogInfoAsync({
                continueWith: function (data) {
                    $scope.about.logInfo = data;
                    $scope.$apply();
                    OpenIZ.App.getLogInfoAsync({
                        query: { _id: $scope.about.logInfo[0].id },
                        continueWith: function (data) {
                            $scope.log = data[0];
                            $scope.logLength = 1024;
                            $scope.log.text = data[0].text.substring(data[0].text.length - $scope.logLength, data[0].text.length);
                            $scope.$apply();
                        }
                    });
                }
            });
        }
    });

    $scope.log = { size: 0 };
    $scope.logLength = 1024;
    // Load log file contents
    $scope.loadLogContents = function () {
        OpenIZ.App.getLogInfoAsync({
            query : { _id : $scope.logid },
            continueWith: function (data) {
                $scope.log=data[0];
                $scope.log.text = data[0].text.substring(data[0].text.length - $scope.logLength, data[0].text.length);
                $scope.$apply();
            }
        });
    };

    // Update the version
    $scope.update = function (appId) {
        if (confirm(OpenIZ.Localization.getString('locale.about.versions.update.confirm')) && 
            !$scope.isUpdating) {

            var doUpdate = function () {
                OpenIZ.App.showWait("a[id='btnUpdate" + appId + "']");
                $scope.isUpdating = true;
                OpenIZ.App.doUpdateAsync({
                    appId: appId,
                    continueWith: function (d) {
                        OpenIZ.App.close();
                    },
                    onException: function (e) {
                        if (e.type != "PolicyViolationException" && e.message)
                            alert(e.message);
                        else
                            console.error(e);
                    },
                    finally: function () {
                        OpenIZ.App.hideWait("a[id='btnUpdate" + appId + "']");
                    }
                });
            };

            OpenIZ.Authentication.$elevationCredentials = { continueWith: doUpdate };
            doUpdate();

        }
    }

    // Refresh queue state
    // @param {bool} noTimer When true, instructs the function not to re-run on a timer
    function refreshHealth(noTimer) {

        OpenIZ.App.getHealthAsync({
            continueWith: function (data) {
                $scope.health = data;
                $scope.$apply();
            }
        })
        if (!noTimer && $state.is('org-openiz-core.about'))
            setTimeout(refreshHealth, 5000);
    }

    refreshHealth();
}]);