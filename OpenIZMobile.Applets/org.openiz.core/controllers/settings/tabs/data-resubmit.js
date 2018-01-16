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

angular.module('layout').controller('ResubmitDataController', ['$scope', '$rootScope', function ($scope, $rootScope) {

    // Resubmit data controller
    $scope.resubmit = {
        from: new Date(),
        to: new Date()
    };
    $scope.resubmit = resubmit;

    /** 
     * @summary Performs the resubmission
     */
    function resubmit(resubmitForm) {
        if (resubmitForm.$invalid) {
            alert(OpenIZ.Localization.getString("locale.settings.data.resubmit.formError"));
            return false;
        }

        // Perform the actual resubmission of the data
        var doResubmit = function () {
            OpenIZ.App.showWait("#resubmitButton");
            OpenIZ.Queue.resubmitAsync({
                from: $scope.resubmit.from,
                to: $scope.resubmit.to,
                continueWith: function (data) {
                    alert(OpenIZ.Localization.getString("locale.settings.data.resubmit.success"));
                },
                onException: function (ex) {
                    if (ex.type == "PolicyViolationException") return;
                    if (ex.message)
                        alert(OpenIZ.Localization.getString(ex.message));
                    else
                        console.error(ex.message);
                },
                finally: function (data) {
                    OpenIZ.App.hideWait("#resubmitButton");
                }
            });
        }

        OpenIZ.Authentication.$elevationCredentials = { continueWith: doResubmit };
        doResubmit();
    }
}]);
