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

layoutApp.controller('TransferStockController', ['$scope', function ($scope)
{
	OpenIZ.Act.getActTemplateAsync({
		templateId: "Act.TransferStock",
		continueWith: function (e)
		{
			$scope.act = e;
		},
		onException: function (ex)
		{
			console.log(ex);
		}
	});

	function loadVaccines(lotNumber)
	{
	    var lotNumberQuery = "statusConcept=" + OpenIZModel.StatusConceptKeys.Active;

	    if (lotNumber !== undefined && lotNumber !== null && lotNumber !== "") {
	        lotNumberQuery += "&lotNumber=" + lotNumber;
	    }
	    else {
	        lotNumberQuery += "&lotNumber=!null";
	    }

	    $scope.stock = [];

	    OpenIZ.ManufacturedMaterial.getManufacturedMaterials({
	        query: lotNumberQuery,
	        continueWith: function (data)
	        {
	            if (data.item !== undefined) {
	                for (var i = 0; i < data.item.length; i++) {
	                    $scope.stock.push(data.item[i]);
	                }
	            }

	            $("#transfer-stock-loading-bar").hide();
	        },
	        onException: function (ex)
	        {
	            console.log(ex);
	            $("#transfer-stock-loading-bar").hide();
	        }
	    });
	}

	loadVaccines(undefined);

	$scope.lotNumberChanged = function ()
	{

	    console.log($scope.lotNumber);

	    if ($scope.lotNumber !== undefined && $scope.lotNumber !== null && $scope.lotNumber !== "")
	    {
	        loadVaccines($scope.lotNumber.lotNumber);
	    }
	};

	$scope.transferStock = function ()
	{
		OpenIZ.Ims.post({
			resource: "Act",
			data: $scope.act,
			continueWith: function (data)
			{
				console.log(data);
			},
			onException: function (ex)
			{
				console.log(ex);
			}
		})
	};
}]);
