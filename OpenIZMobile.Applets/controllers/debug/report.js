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
/// <reference path="~/lib/jquery.min.js"/>
/// <reference path="~/lib/angular.min.js"/>

layoutApp.controller('ReportBugController', ['$scope', '$rootScope', function ($scope, $rootScope) {

    $rootScope.$watch('session', function(n, o) {
        if(n != null)
            $scope.report = {
                submitterKey: $rootScope.session.entity.id,
                submitterModel: $rootScope.session.entity,
                _includeData: true
            };
    });


    // Cancel
    $scope.close = function (form) {
        if(form.$touched && confirm(OpenIZ.Localization.getString("locale.confirm.loseChanges")) || !form.$touched)
            window.history.back();
    }

    // Submit
    $scope.submitBugReport = function (form) {
        if (form.$invalid) {
            alert(OpenIZ.Localization.getString("locale.common.invalidForm"));
            return;
        }

        // Submit
        OpenIZ.App.showWait();
        OpenIZ.App.submitBugReportAsync({
            data: $scope.report,
            continueWith: function (data) {
                $scope.report = data;
                $scope.$apply();
            },
            onException: function (error) {
                if (error.message)
                    alert(error.message);
                else
                    alert(error);
            },
            finally: function () {
                OpenIZ.App.hideWait();
            }
        });
    }
}]);