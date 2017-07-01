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
 * User: kirkleyd
 * Date: 2017-5-28
 */

/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>

layoutApp.controller('MothersInformationController', ['$scope', function ($scope) {

    $scope.scanBarcode = scanBarcode;
    $scope.mothersRegexValidation = '';

    angular.element(document).ready(init);

    function init() {
        $scope.$watch('patient.relationship.Mother.targetModel.identifier.authority.domainName', function () {
            var regex = $('#mothersIdentifierType').find(':selected').first().attr('data-validation');
            $scope.mothersRegexValidation = regex ? regex : '';
        });
    }

    function scanBarcode(mother) {
        if (!mother.targetModel.identifier) {
            mother.targetModel.identifier = {};
        }
        mother.targetModel.identifier.value = OpenIZ.App.scanBarcode();
    };
}]);