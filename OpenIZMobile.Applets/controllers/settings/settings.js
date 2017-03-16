/// <reference path="../../js/openiz-model.js"/>

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
 * Date: 2016-7-17
 */

/// <reference path="../../js/openiz.js"/>

layoutApp.controller('SettingsController', ['$scope', function ($scope) {

    if (OpenIZ.Authentication.$session == null)
        OpenIZ.Authentication.$session = {};

    OpenIZ.App.showWait();
    OpenIZ.Configuration.getConfigurationAsync({
        continueWith: function (config) {
            $scope.config = config;
            $scope.config.security.port = 8080;
            $scope.config.network.useProxy = $scope.config.network.proxyAddress != null;
            $scope.config.network.proxyAddress = $scope.config.network.proxyAddress || null;
            $scope.ui = {
                dataCollapsed: true,
                securityCollapsed: true
            };

            $scope.config.data.mode = "sync"; //OpenIZ.App.getService("SynchronizationManagerService") == null ?
            //OpenIZ.App.getService("ImsiPersistenceService") == null ? "offline" : "online" : "sync";
            $scope.config.data.sync = {
                event: [],
                enablePoll: config.sync.pollInterval,
                pollInterval: config.sync.pollInterval
            };
            $scope.config.log.mode = $scope.config.log.trace[0].filter || "Warning";
            $scope.config.security.hasher = OpenIZ.App.getService("IPasswordHashingService") || "SHA256PasswordHasher";

            $scope.config.security.offline = {
                enable: false
            };
            $scope.$apply();
        },
        onException : function(error) {
            if(error.message != null)
                console.log(error.message);
            else
                console.log(error);
        },
        finally: function () {
            OpenIZ.App.hideWait();
        }
    });
    
    
    $scope.master = {};

    // leave realm
    $scope.leaveRealm = function (realm) {
        if (confirm(OpenIZ.Localization.getString("locale.settings.confirm.leaveRealm")))
            OpenIZ.Configuration.leaveRealmAsync();
    };

    // join realm
    $scope.joinRealm = function (realm) {

        var doJoin = function (force) {
            OpenIZ.App.showWait('#joinRealmButton');
            OpenIZ.Configuration.joinRealmAsync({
                domain: realm.domain,
                deviceName: realm.deviceName,
                enableTrace: realm.enableTrace,
                enableSSL : realm.enableSSL,
                force: force,
                continueWith: function (data) {
                    $scope.config.realmName = data.realmName;
                    alert(OpenIZ.Localization.getString("locale.settings.status.joinRealm"));
                    OpenIZ.App.hideWait('#joinRealmButton');
                },
                onException: function (error) {
                    if (error.type == 'DuplicateNameException')
                        if (confirm(OpenIZ.Localization.getString('locale.settings.status.duplicateName')))
                            doJoin(true);
                        else {
                            if (error.message != null)
                                alert(error.message);
                            else
                                console.log(error);
                        }
                },
                finally: function () {
                    OpenIZ.App.hideWait('#joinRealmButton');
                }
            });
        };
        OpenIZ.Authentication.$elevationCredentials.continueWith = doJoin;
        doJoin();
    };

    // Save config
    $scope.save = function (config) {
        
        if ($scope.config.realmName == null)
            alert(OpenIZ.Localization.getString("locale.settings.error.noRealm"));
        else {
            OpenIZ.App.showWait();
            OpenIZ.App.showWait('#saveConfigButton');
            OpenIZ.Configuration.saveAsync({
                data: config,
                continueWith: function () {
                    OpenIZ.App.hideWait('#saveConfigButton');
                    OpenIZ.App.close();
                }
            });
        }
    };
}]);
