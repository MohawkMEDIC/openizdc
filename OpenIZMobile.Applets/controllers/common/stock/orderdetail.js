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
 * Date: 2016-9-11
 */

layoutApp.controller('OrderDetailController', ['$scope', function ($scope) {

    $scope.query = "classConcept=A064984F-9847-4480-8BEA-DDDF64B3C77C&statusConcept=C8064CBD-FA06-4530-B430-1A52F1530C27&moodConcept=E658CA72-3B6A-4099-AB6E-7CF6861A5B61";
    $scope.order = {
        orderNo: 1234,
        orderDate: "11/9/2016",
        destinationHealthFacility: "Arusha Health Centre for Women",
        orderStatus: "Requested"
    };
    $scope.orderItems = [];
    OpenIZ.Ims.get({
        query: $scope.query,
        resource: "Act",
        continueWith: function (data)
        {
            if (data.item !== undefined)
            {
                for (var i = 0; i < data.item.length; i++) {
                    //$scope.orderItems.push(data.item[i]);
                }
            }
        },
        onException: function(ex)
        {
            console.log(ex);
        }
    });
}]);
