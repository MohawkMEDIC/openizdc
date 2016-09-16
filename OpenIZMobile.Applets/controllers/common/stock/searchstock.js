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
 * Date: 2016-9-15
 */

layoutApp.controller('SearchStockController', ['$scope', function ($scope) {

    $scope.vaccines = [];

    $scope.searchStock = function () {

        var query = "name.component.value=~" + $scope.name + "&statusConcept=" + OpenIZModel.StatusConceptKeys.Active;

        OpenIZ.Ims.get({
            query: query,
            resource: "ManufacturedMaterial",
            continueWith: function (data) {

                console.log(data);

                if (data.item !== undefined) {
                    for (var i = 0; i < data.item.length; i++) {
                        $scope.vaccines.push(data.item[i]);
                    }
                }
            },
            onException: function (ex) {
                console.log(ex);
            }
        });
    };

    $scope.update = function (manufacturedMaterial) {

        console.log(manufacturedMaterial);

        OpenIZ.Ims.put({
            id: manufacturedMaterial.id,
            versionId: manufacturedMaterial.versionId,
            resource: "ManufacturedMaterial",
            data: manufacturedMaterial,
            controlData: function(data)
            {
                console.log(data);

                OpenIZ.App.toast(OpenIZ.Localization.getString("locale.stock.stockUpdatedSuccessfully"));
            },
            onException: function(ex)
            {
                console.log(ex);

                OpenIZ.App.toast(OpenIZ.Localization.getString("locale.stock.errors.unableToUpdateStock"));
            }
        })
    };

}]);
