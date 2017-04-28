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
    $scope.Array = Array;
    $scope.identifiers = $scope.identifiers || [];
    var once = true;

    // JF- ??? Why does this need to exist?
    $scope.$watch('patient.identifier', function (identifier, o) {
        if (identifier && identifier != o && once) {
            once = false;
            $scope.identifiers = [];
            for (key in identifier) {
                if (identifier[key]) {
                    if (!Array.isArray(identifier[key])) {
                        var value = identifier[key].value || "";
                        $scope.identifiers.push({ domainName: key, value: value, authority: identifier[key].authority });
                    } else {
                        for (var i = 0; i < identifier[key].length; i++) {
                            $scope.identifiers.push({ domainName: key, value: identifier[key][i].value, authority: identifier[key][i].authority });
                        }
                    }
                }
               
            }
            if ($scope.identifiers.length === 0) {
                $scope.identifiers.push({});
            }
        }
    }, true);

    //builds the identifier back onto the patient
    $scope.$watch('identifiers', function (identifiers, o) {
        if (identifiers && identifiers != o) {
            $scope.patient.identifier = {};
            for (key in identifiers) {
                authority = identifiers[key].authority;
                domainName = identifiers[key].domainName;
                value = identifiers[key].value;
                if (Array.isArray($scope.patient.identifier[domainName])) {
                    $scope.patient.identifier[domainName].push({
                        authority: authority,
                        value: value
                    })
                } else {
                    $scope.patient.identifier[domainName] = [{
                        authority: authority,
                        value: value
                    }]
                }
            }
        }
    }, true);

    // Rebind the domain scope
    function rebindDomain(authority, identifier, index) {
        
    };
    // Scan the specified barcode

    function scanBarcode(identifier) {
        identifier.value = OpenIZ.App.scanBarcode();
    };

    // Add identifier
    function addIdentifier(identifiers) {
        identifiers.push({});
    }

    // Remove identifier
    function removeIdentifier(identifiers, index) {
        if (identifiers && identifiers.length > 1) {
            identifiers.splice(index, 1);
        }
    };
    
}]);