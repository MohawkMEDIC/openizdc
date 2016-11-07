/// <reference path="../js/openiz-model.js"/>
/// <reference path="../js/openiz.js"/>

/*
 * Copyright 2016 PATH International
 * Developed by Mohawk College of Applied Arts and Technology 
 *
 * Licensed under Creative Commons ShareAlike Attribution Version 4.0 (the "License"); 
 * you may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * https://creativecommons.org/licenses/by-sa/4.0/legalcode
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: khannan
 * Date: 2016-9-10
 */

layoutApp.controller('DemographicSummaryController', ['$scope', function ($scope) {
    $scope.genderOptions = [];
    $("#genderSelect>option").map(function () {
        $scope.genderOptions.push({value:$(this).val(), text: $(this).text()});
    });


   /* $scope.saveGiven = function () {
        var controlData = {};
        controlData.data = $scope.patient;
        controlData.id = $scope.patient.id;
        OpenIZ.Patient.updateAsync({
            data: $scope.patient,
            continueWith: function (e) {
                console.log(e);
            },
            onException: TIMR.UserInterface.exceptionHandler,
        });
    };*/

    $scope.showGender = function () {
        
        var selected = ($scope.genderOptions, { value: $scope.genderOptions });
        if ($scope.patient !== undefined) {
            var newSelected = "";
            for (var i = 0; i < selected.value.length; i++) {
                if ($scope.patient.genderConcept == selected.value[i].value) {
                    return (selected.value[i].text);
                }
            }
        }
        return "";
    };

}]);
