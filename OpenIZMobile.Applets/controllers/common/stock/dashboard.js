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
 * User: justi
 * Date: 2016-7-19
 */

layoutApp.controller('StockDashboardController', ['$scope', function ($scope) {

    $scope.orderQuery = "moodConcept=EC74541F-87C4-4327-A4B9-97F325501747&classConcept=A064984F-9847-4480-8BEA-DDDF64B3C77C";

    OpenIZ.Ims.get({
        resource: "Act",
        query: $scope.orderQuery,
        continueWith: function(data)
        {
        },
        onException: function(ex)
        {
            console.log(ex);
        }
    });

    $scope.lowStock = [];

    // TODO: add facility id here
    var lowStockQuery = "lotNumber=!null&quantity=<5&statusConcept=C8064CBD-FA06-4530-B430-1A52F1530C27&_count=5";

    OpenIZ.Ims.get({
        resource: "ManufacturedMaterial",
        query: lowStockQuery,
        continueWith: function (data) {

            if (data.item !== undefined)
            {
                for (var i = 0; i < data.item.length; i++) {
                    $scope.lowStock.push(data.item[i]);
                    
                }
            }
        },
        onException: function (ex) {
            console.log(ex);
        }
    });
}]);
