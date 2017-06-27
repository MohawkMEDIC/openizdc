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
 * User: justi
 * Date: 2016-7-17
 */

/// <reference path="../../js/openiz.js"/>

layoutApp.controller('SettingsController', ['$scope', 'uiHelperService', function ($scope, uiHelperService) {

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
                enable: true
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
    
    uiHelperService.initializePopups('top');
    
    $scope.master = {};

    // leave realm
    $scope.leaveRealm = function (realm) {
        if (confirm(OpenIZ.Localization.getString("locale.settings.confirm.leaveRealm")))
            OpenIZ.Configuration.leaveRealm();
    };

    // join realm
    $scope.joinRealm = function (realm) {

        var doJoin = function (force) {
            if (!$('#joinRealmButton')[0].hasAttribute('disabled')) {
                OpenIZ.App.showWait('#joinRealmButton');
            }

            var backupCredentials = {
                continueWith: OpenIZ.Authentication.$elevationCredentials.continueWith,
                userName: OpenIZ.Authentication.$elevationCredentials.userName,
                password: OpenIZ.Authentication.$elevationCredentials.password,
                $enabled: OpenIZ.Authentication.$elevationCredentials.$enabled
            };

            OpenIZ.Configuration.joinRealmAsync({
                domain: realm.domain,
                deviceName: realm.deviceName,
                enableTrace: realm.enableTrace,
                enableSSL: realm.enableSSL,
                port : realm.port,
                force: force,
                continueWith: function (data) {
                    $scope.config.realmName = data.realmName;
                    alert(OpenIZ.Localization.getString("locale.settings.status.joinRealm"));
                },
                onException: function (error) {
                    if (error.type == 'DuplicateNameException') {
                        if (confirm(OpenIZ.Localization.getString('locale.settings.status.duplicateName'))) {
                            OpenIZ.Authentication.$elevationCredentials = backupCredentials;

                            doJoin(true);
                        }
                        else {
                            if (error.message != null)
                                alert(error.message);
                            else
                                console.log(error);
                        }
                    }
                    else if (error.type != "UnauthorizedAccessException")
                        alert(OpenIZ.Localization.getString("locale.settings.status.generalRealmError"))
                },
                finally: function () {
                    OpenIZ.App.hideWait('#joinRealmButton');
                    $scope.$apply();
                }
            });
        };
        OpenIZ.Authentication.$elevationCredentials.continueWith = doJoin;
        doJoin();
    };

    // Save config
    $scope.save = function (config) {
        $scope.settingsForm.$setSubmitted();

        // Check if the form is valid
        if (!$scope.settingsForm.$valid) {
            // Focus any required field
            console.log($('[name=' + $scope.settingsForm.$error.required[0].$name + ']'));
            $('[name=' + $scope.editPatientForm.$error.required[0].$name + ']').focus();
            return;
        }

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
