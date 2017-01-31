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
 * Date: 2016-7-23
 */

/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>
layoutApp.controller('LayoutController', ['$scope', '$interval', '$rootScope', '$window', function ($scope, $interval, $rootScope, $window) {
    // Add menu items
    OpenIZ.App.getMenusAsync({
        continueWith: function (menus) {
            $scope.menuItems = menus;
            $scope.$applyAsync();
        }
    });


    // Perform a logout of the session
    $scope.logout = $scope.logout || function () {
        if (confirm(OpenIZ.Localization.getString('locale.layout.navbar.logout.confirm'))) {
            OpenIZ.Authentication.abandonSession({
                continueWith: function (data) {
                    console.log(data);
                    window.location.hash = "#/";
                    $window.location.reload();
                },
                onException: function (ex) {
                    console.log(ex);
                }
            });
        }
    };

    // Set locale
    $scope.setLocale = $scope.setLocale || function (locale) {
        OpenIZ.Localization.setLocale(locale);
        window.location.hash = "#/";
        $window.location.reload(true);
    };

    $scope.checkMessages = function () {
        OpenIZ.App.getAlertsAsync({
            query: {
                flags: "!2"
            },
            continueWith: function (d) {
                if ($scope.messages == null || d.length != $scope.messages.length) {
                    var nmsg = d.length - ($scope.messages == null ? 0 : $scope.messages.length);

                    if ($rootScope.session != null) {
                        var title = null;
                        var alertBody = null;
                        var alertOptions = {
                            "preventDuplicates": true,
                            "showDuration": 150,
                            "hideDuration": 250,
                            "timeout": 2000
                        };

                        if ($scope.messages) {
                            if (nmsg == 1)
                                toastr.success(d[0].subject + "...", d[0].from, alertOptions);
                            else
                                toastr.success(OpenIZ.Localization.getString("locale.alerts.newAlerts"), alertOptions);
                        }
                        $scope.messages = d;
                        $scope.$apply();
                    }
                }
                setTimeout($scope.checkMessages, 30000);
            }
        });
    };
    setTimeout($scope.checkMessages, 30000);
    $scope.checkMessages();
}]);
