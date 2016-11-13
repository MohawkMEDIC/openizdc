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
 * Date: 2016-8-17
 */


/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('PatientLayoutController', ['$scope', '$rootScope', '$stateParams', function ($scope, $rootScope, $stateParams)
{
    // Get the patient 
    OpenIZ.Patient.findAsync({
        query: { _id: $stateParams.patientId, _count: 1, _all: true },
        continueWith: function (data)
        {
            $scope.patient = data;
            // Set multiple birth text
            switch (data.multipleBirthOrder)
            {
                case 1:
                    $scope.patient.multipleBirthOrderText = OpenIZ.Localization.getString('locale.patient.demographics.multipleBirth.first');
                    break;
                case 2:
                    $scope.patient.multipleBirthOrderText = OpenIZ.Localization.getString('locale.patient.demographics.multipleBirth.second');
                    break;
                case 3:
                    $scope.patient.multipleBirthOrderText = OpenIZ.Localization.getString('locale.patient.demographics.multipleBirth.third');
                    break;
                case 0:
                    $scope.patient.multipleBirthOrderText = OpenIZ.Localization.getString('locale.patient.demographics.multipleBirth.unknown');
                    break;
            };

            $scope.$apply();
        },
        onException: function (ex)
        {
            OpenIZ.App.hideWait();
            if (typeof (ex) == "string")
            {
                alert(ex);
            }
            else if (ex.message != undefined)
            {
                alert("" + ex.message + " - " + ex.details);
            }
            else
            {
                alert(ex);
            }
        }
    });
}]);