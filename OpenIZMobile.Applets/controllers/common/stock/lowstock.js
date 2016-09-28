/// <reference path="../js/openiz-model.js"/>

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
 * Date: 2016-9-8
 */

/// <reference path="../js/openiz.js"/>

layoutApp.controller('LowStockController', ['$scope', function ($scope) {

    var ctx = $('#lowStockChart');
    var stockChart = new Chart(ctx, {
        type: 'bar',
        options: {
            scales: {
                yAxes: [{
                    ticks: {
                        beginAtZero: true
                    }
                }]
            }
        },
        data: {
            labels: [],
            datasets: []
        }
    });

    var lowStockQuery = "lotNumber=!null&quantity=<5&statusConcept=" + OpenIZModel.StatusConceptKeys.Active + "&_count=5";

    OpenIZ.ManufacturedMaterial.getManufacturedMaterials({
        query: lowStockQuery,
        continueWith: function (data) {

            if (data.item !== undefined) {
                for (var i = 0; i < data.item.length; i++) {

                    var vaccine = data.item[i];

                    stockChart.data.labels.push(vaccine.name.Assigned.component.$other.value.split("(")[0]);
                    stockChart.data.datasets.push({
                        label: vaccine.name.Assigned.component.$other.value.split("(")[0],
                        data: [],
                        backgroundColor: [
                            'rgba(255, 99, 132, 0.2)',
                            'rgba(54, 162, 235, 0.2)',
                            'rgba(255, 206, 86, 0.2)',
                            'rgba(75, 192, 192, 0.2)',
                            'rgba(153, 102, 255, 0.2)',
                        ]
                    });
                }

                // HACK
                for (var j = 0; j < stockChart.data.datasets.length; j++) {
                    stockChart.data.datasets[j].data[j] = data.item[j].quantity + 1;
                }

                stockChart.update(1, true);
            }
        },
        onException: function (ex) {
            console.log(ex);
        },
        finally: function()
        {
            stockChart.update();
        }
    });


}]);
