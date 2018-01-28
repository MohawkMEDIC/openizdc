/// <reference path="~/js/openiz.js"/>
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

/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>
angular.module('layout').controller('LayoutController', ['$scope', '$interval', '$rootScope', '$window', '$state', '$templateCache', function ($scope, $interval, $rootScope, $window, $state, $templateCache) {
    
    $rootScope.$watch('session', function (nv, ov) {
        if (nv && nv.user) {
            // Add menu items
            OpenIZ.App.getMenusAsync({
                continueWith: function (menus) {
                    $scope.menuItems = menus;
                    $scope.$applyAsync();
                }
            });
        }
        else
            $scope.menuItems = null;
    });

    // Session was expired on a background thread
    OpenIZ.Authentication.$sessionExpiredHandler = function () {
        if ($rootScope.session) {
            $rootScope.isLoading = true;
            $rootScope.session = null;
            $templateCache.removeAll();
            $state.reload();
        }
    }

    // Perform a logout of the session
    $scope.logout = $scope.logout || function () {
        if (confirm(OpenIZ.Localization.getString('locale.layout.navbar.logout.confirm'))) {
            OpenIZ.Authentication.abandonSession({
                continueWith: function (data) {
                    console.log(data);
                    $rootScope.isLoading = true;
                    $rootScope.session = null;
                    $templateCache.removeAll();
                    $state.reload();
                    //$window.location.reload();
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
                            "timeout": 2000,
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
            }
        });

        if ($rootScope.session != null) 
            OpenIZ.Queue.getQueueAsync({
                queueName: OpenIZ.Queue.QueueNames.DeadLetterQueue,
                continueWith: function (data) {
                    $scope.conflicts = data;
                },
                onException: function() {}
            });

        if ($rootScope.session != null)
            OpenIZ.App.getTicklesAsync({
                continueWith: function (data) {
                    $scope.tickles = data;

                    var already = [];
                    for (var t in data) {
                        if (data[t].type & 4) {
                            var alertOptions = {
                                "preventDuplicates": true,
                                "showDuration": 150,
                                "hideDuration": 250,
                                "timeout": data[t].exp - new Date(),
                                "closeButton": false,
                                "positionClass": "toast-bottom-center",
                            };

                            if (already.indexOf(data[t].text) > -1)
                                continue;
                            already.push(data[t].text);

                            if (data[t].type & 1)
                                toastr.info(data[t].text, null, alertOptions);
                            else if (data[t].type & 2)
                                toastr.error(data[t].text, null, alertOptions);

                        }
                    }
                },
                onException: function() {}
            });
    };

    setInterval($scope.checkMessages, 10000);
    //$scope.checkMessages();
    /**
     * Window resize event handling
     */
    $scope.$watch(function () {
        return window.innerWidth;
    }, function (value) {
        $scope.windowWidth = value;
    });
}]);
