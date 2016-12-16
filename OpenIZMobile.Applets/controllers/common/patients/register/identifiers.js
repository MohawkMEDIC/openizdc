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
 * Date: 2016-7-30
 */

/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>

layoutApp.controller('PatientIdentifiersController', ['$scope', function ($scope) {

    $scope.rebindDomain = rebindDomain;
    $scope.addIdentifier = addIdentifier;
    $scope.scanBarcode = scanBarcode;
    $scope.removeIdentifier = removeIdentifier;
    

    // Rebind the domain scope
    function rebindDomain(authority, identifier) {
        $scope.patient.identifier[identifier.authority.domainName] = identifier;
        delete $scope.patient.identifier[authority];

    };
    
    // Scan the specified barcode

    function scanBarcode(authority, identifier) {
        identifier.value = OpenIZ.App.scanBarcode();
    };

    // Add identifier
    function addIdentifier() {
            if ($scope.patient.identifier["NEW"] != null &&
                $scope.patient.identifier["NEW"].authority.domainName != "NEW") {
                $scope.patient.identifier[$scope.patient.identifier["NEW"].authority] = $scope.patient.identifier["NEW"];
                $scope.patient.identifier["NEW"] = { authority: { domainName: "NEW" }, value: null };
            }
            else if ($scope.patient.identifier["NEW"] == null)
                $scope.patient.identifier["NEW"] = { authority: { domainName: "NEW" }, value: null };
        
    };

    // Remove identifier
    function removeIdentifier(id) {
        delete $scope.patient.identifier[id];
        if (Object.keys($scope.patient.identifier) == 0)
            $scope.addIdentifier();
    };
    
}]);