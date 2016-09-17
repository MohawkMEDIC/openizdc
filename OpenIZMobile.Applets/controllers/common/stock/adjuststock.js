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

layoutApp.controller('AdjustStockController', ['$scope', 'queryUrlParameterService', function ($scope, queryParameterService) {

    var params = queryParameterService.getUrlParameters();

    var query = "id=" + params.id;

    var manufacturedMaterial = null;

    OpenIZ.Ims.get({
        query: query,
        resource: "ManufacturedMaterial",
        continueWith: function (data) {

            console.log(data);

            if (data.item !== undefined) {
                manufacturedMaterial = data.item[0];

                $scope.gtin = manufacturedMaterial.identifier.GTIN.value;
                $scope.lotNumber = manufacturedMaterial.lotNumber;
                $scope.expiryDate = manufacturedMaterial.expiryDate;
                $scope.vaccine = manufacturedMaterial.name.Assigned.component.$other.value;
            }
        },
        onException: function (ex) {
            console.log(ex);
        }
    });

    $scope.adjustStock = function () {

        OpenIZ.App.showWait();

        manufacturedMaterial.quantity = $scope.quantity;

        OpenIZ.Ims.put({
            id: params.id,
            versionId: params.version,
            data: manufacturedMaterial,
            resource: "ManufacturedMaterial",
            continueWith: function (data) {

                OpenIZ.App.toast(OpenIZ.Localization.getString("locale.stock.updateSuccessful"));

                window.location.href = "searchstock.html";
            },
            onException: function(ex)
            {
                OpenIZ.App.toast(OpenIZ.Localization.getString("locale.stock.updateUnsuccessful"));
            },
            finally: function()
            {
                OpenIZ.App.hideWait();
            }
        });
    };

}]);
