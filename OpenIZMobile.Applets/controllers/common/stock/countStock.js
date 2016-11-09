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

layoutApp.controller('CountStockController', ['$scope', 'queryUrlParameterService', '$stateParams', function ($scope, queryParameterService, $stateParams) {

    var params = queryParameterService.getUrlParameters();

    var query = "id=" + $stateParams.vaccineId;

    var manufacturedMaterial = null;

    $scope.dateRecorded = new Date();

    OpenIZ.Ims.get({
        query: query,
        resource: "ManufacturedMaterial",
        continueWith: function (data) {

            if (data.item !== undefined) {
                manufacturedMaterial = data.item[0];
                $scope.gtin = manufacturedMaterial.identifier.GTIN.value;
                $scope.lotNumber = manufacturedMaterial.lotNumber;
                $scope.expiryDate = manufacturedMaterial.expiryDate;
                $scope.vaccine = manufacturedMaterial.name.Assigned.component.$other.value;
                $scope.$digest();
            }
        },
        onException: function (ex) {
            console.log(ex);
        }
    });

    $scope.countStock = function () {
        OpenIZ.App.showWait();

        OpenIZ.Act.getActTemplateAsync({
            templateId: "Act.CountStock",
            continueWith: function (data) {
                var act = data;
                data.participation.Consumable.actModel.value = $scope.quantity;
                data.participation.Consumable.playerModel.id = $stateParams.vaccineId;
                data.participation.Consumable.actModel.actTime = $scope.dateRecorded;
                OpenIZ.Ims.post({
                    resource: "Act",
                    data: data,
                    continueWith: function (act) {
                    },
                    onException: function(ex){
                        console.log(ex);
                }
                })
                OpenIZ.App.hideWait();
            }
        });


    };

}]);
