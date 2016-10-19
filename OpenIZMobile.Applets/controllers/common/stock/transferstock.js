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

	$scope.lotNumbers = [];

	function loadVaccines(lotNumber)
	{
	    var vaccineQuery = "statusConcept=C8064CBD-FA06-4530-B430-1A52F1530C27";

		if (lotNumber !== undefined && lotNumber !== null && lotNumber !== "")
		{
			vaccineQuery += "&lotNumber=" + lotNumber;
		}
		else
		{
			vaccineQuery += "&lotNumber=!null";
		}

		$scope.stock = [];

		OpenIZ.ManufacturedMaterial.getManufacturedMaterials({
			query: vaccineQuery,
			continueWith: function (data)
			{
				if (data.item !== undefined)
				{
					for (var i = 0; i < data.item.length; i++)
					{
						if (data.item[i].quantity !== 0)
						{
							$scope.stock.push(data.item[i]);
						}

						if (data.item[i].lotNumber !== undefined && data.item[i].lotNumber !== null && data.item[i].lotNumber !== "")
						{
							if ($scope.lotNumbers.indexOf(data.item[i].lotNumber) === -1)
							{
								$scope.lotNumbers.push(data.item[i].lotNumber);
							}
						}
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

		if ($scope.lotNumber !== undefined && $scope.lotNumber !== null && $scope.lotNumber !== "")
		{
			$("#transfer-stock-loading-bar").show();

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
			},
			onException: function (ex)
			{
				console.log(ex);
			}
		})
	};
}]);
