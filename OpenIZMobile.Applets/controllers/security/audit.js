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
 * Date: 2017-6-28
 */

// Audit controller
/// <reference path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-audit.js"/>
/// <reference path="~/js/openiz-model.js"/>

layoutApp.controller('AuditController', ['$scope', function ($scope) {

    $scope.audit = { selected: null };
    $scope.audit.query = {
        "type.code": "!SecurityAuditCode-AuditLogUsed",
        _offset: 0,
        _count: 10
    };
    $scope.getAudit = getAudit;
    $scope.searchAudits = searchAudits;
    $scope.trunc = function (i) { return Math.trunc(i); };
    $scope.resetQueryTimestamp = resetQueryTimestamp;
    $scope.resetQueryTimestamp();

    // Get audits
    function searchAudits() {

        //if ($scope.audit.query.timestamp && $scope.audit.query.timestamp)
        //    $scope.audit.query.timestamp = "~" + OpenIZ.Util.toDateInputString($scope.audit.query.timestamp);

        $scope.isLoading = true;
        OpenIZ.Audit.getAuditAsync({
            query: $scope.audit.query,
            continueWith: function (data) {
                $scope.audit.log = data;
            },
            finally: function () {
                $scope.isLoading = false;
                $scope.$apply();

            }
        });
    }

    $scope.$watch('audit.query', function (ov, nv) {
        searchAudits();
    }, true);

    function getAudit(id) {
        if (id) {
            $scope.isLoading = true;
            OpenIZ.Audit.getAuditAsync({
                query: { _id: id},
                continueWith: function (data) {
                    $scope.audit.current = data.CollectionItem[0].Audit[0];
                   
                },
                finally: function () {
                    $scope.isLoading = false;
                    $scope.$apply();    
                }
            });
        }
    };

    function resetQueryTimestamp() {
        $scope.audit.queryTimestamp = moment().toDate();
    }
}]);
