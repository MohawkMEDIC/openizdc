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

angular.module('layout').controller('PatientIdentifiersController', ['$scope', '$rootScope', function ($scope, $rootScope) {

    $scope.addIdentifier = addIdentifier;
    $scope.scanBarcode = scanBarcode;
    $scope.removeIdentifier = removeIdentifier;
    $scope.searchDuplicates = searchDuplicates;
    $scope.Array = Array;

    // JF- This should be changed to be patient identifiers in the future to more closely link with actual scope
    $scope.identifiers = $scope.identifiers || [];
    $scope.regexValidation = $scope.regexValidation || [];

    angular.element(document).ready(init);

    function init() {
        $scope.$watch('patient.identifier', function (identifier, o) {
            if (identifier && (identifier != o || Object.keys(identifier).length != $scope.identifiers.length)) {
                $scope.identifiers = [];
                for (key in identifier) {
                    if (identifier[key]) {
                        if (!Array.isArray(identifier[key])) {
                            var value = identifier[key].value || "";
                            $scope.identifiers.push({ domainName: key, value: value});
                        } else {
                            for (var i = 0; i < identifier[key].length; i++) {
                                $scope.identifiers.push({ domainName: key, value: identifier[key][i].value});
                            }
                        }
                    }

                }
                if ($scope.identifiers.length === 0) {
                    $scope.identifiers.push({});
                }
            }
            // Update the identifier regex validation
            if ($('.identifier-domain-select').length > 0) {
                $('.identifier-domain-select').each(function (e) {
                    var regex = $(this).find(':selected').first().attr('data-validation');
                    $scope.regexValidation[e] = regex ? regex : '';
                });
            }
        }, true);

        //builds the identifier back onto the patient
        $scope.$watch('identifiers', function (identifiers, o) {
            if (identifiers && identifiers != o) {
                $scope.patient.identifier = {};
                for (key in identifiers) {
                    domainName = identifiers[key].domainName;
                    value = identifiers[key].value;

                    if (Array.isArray($scope.patient.identifier[domainName])) {
                        $scope.patient.identifier[domainName].push({
                            value: value
                        })
                    } else {
                        $scope.patient.identifier[domainName] = [{
                            value: value
                        }]
                    }
                }
            }
        }, true);
    }

    // Scan the specified barcode

    function scanBarcode(identifier) {
        identifier.value = OpenIZ.App.scanBarcode();
        searchDuplicates(identifier);
    };

    function searchDuplicates(identifier) {
        if ($scope.search && $scope.search.searchByBarcode && identifier.value !== undefined) {
            // Focus the next input after the scan
            var identifierIndex = $scope.identifiers.indexOf(identifier) + 1;
            if (identifierIndex < ($scope.identifiers.length)) {
                $('input[name="identifier"]')[identifierIndex].focus();
            }
            else {
                $('#givenName-tokenfield').focus();
            }
            
            // Search offline only
            $scope.search.searchByBarcode(identifier, false, function (count) {
                if (count > 0) {
                    focusDuplicates();
                }
                else if (OpenIZ.App.getOnlineState()) {
                    // No duplicates found, search online
                    $scope.search.searchByBarcode(identifier, true, function (count) {
                        if (count > 0) {
                            focusDuplicates();
                        }
                    });
                }
            });
        }
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

    function focusDuplicates() {
        $('#duplicates').focus();
        alert(OpenIZ.Localization.getString("locale.patient.search.childExists"));
    }
    
}]);