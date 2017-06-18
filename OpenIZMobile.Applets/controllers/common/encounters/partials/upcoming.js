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
 * Date: 2017-3-31
 */

/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('UpcomingAppointmentController', ['$scope', '$stateParams', '$rootScope', function ($scope, $stateParams, $rootScope) {
    $scope._isCalendarInitialized = false;

    $scope.$watch("encounters.length", function (newValue, oldValue) {
        if ($scope.encounters && $scope.encounters.length > 0 && newValue !== oldValue) {
            updateCarePlan();
        }
    });

    $scope.canStartAppointment = function (apt) {
        return apt.actTime < new Date().addDays(70);
    }

    function updateCarePlan() {
        //OpenIZ.CarePlan.getCarePlanAsync({
        //    query: "_patientId=" + $stateParams.patientId + "&_appointments=true&_viewModel=full&stopTime=>" + OpenIZ.Util.toDateInputString(new Date()),
        //    onDate: new Date(),
        //    /** @param {OpenIZModel.Bundle} proposals */
        //    continueWith: function (proposalsToday) {
        $scope.appointments = {};
        OpenIZ.CarePlan.getCarePlanAsync({
            query: "_patientId=" + $stateParams.patientId + "&_appointments=true&_viewModel=full",
            minDate: new Date(),
            maxDate: new Date().addDays(90),
            continueWith: function (proposals) {

                if (!proposals.item) {
                    $scope.appointments = [];
                }
                else {
                    // Grab the first appointment
                    $scope.appointments = proposals.item;
                    $scope.appointments.sort(
                        function (a, b) {
                            return a.actTime > b.actTime ? 1 : -1;
                        }
                    );

                    for (var i in $scope.appointments) {
                        if ($scope.appointments[i].startTime < $rootScope.page.loadTime) {
                            $scope.appointments[i].startTime = $rootScope.page.loadTime;
                        }
                        if (!Array.isArray($scope.appointments[i]) && !Array.isArray($scope.appointments[i].relationship.HasComponent))
                            $scope.appointments[i].relationship.HasComponent = [$scope.appointments[i].relationship.HasComponent];
                    }
                }

                $scope.$apply();
            },
            onException: function (ex) {
                if (ex.type === "FileNotFoundException")
                    ;
                else if (ex.message)
                    alert(ex.message);
                else
                    console.error(ex);
            }
        });
    }
    //    },
    //    onException: function (ex) {
    //        if (ex.message)
    //            alert(ex.message);
    //        else
    //            console.error(ex);
    //    }
    //});

}]);