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

layoutApp.controller('DataAdministrationController', ['$scope', function ($scope) {

    $scope.data = {};
    // Get information asynchronously
    OpenIZ.App.getInfoAsync({
        continueWith: function (data) {
            $scope.about = data;
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

   
}]);