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
                }],
                xAxes: [{
                    categoryPercentage: 0.6
                }]
            },
            legend: {
                display: false
            }
        },
        data: {
            labels: [],
            datasets: []
        }
    });

    var lowStockQuery = "lotNumber=!null&quantity=<5&statusConcept=C8064CBD-FA06-4530-B430-1A52F1530C27&_count=5";

    OpenIZ.ManufacturedMaterial.getManufacturedMaterials({
        query: lowStockQuery,
        continueWith: function (data) {

            if (data.item !== undefined) {
                var chartData = [];
                for (var i = 0; i < data.item.length; i++) {

                    var vaccine = data.item[i];
                    var vaccineName = vaccine.name.Assigned.component.$other.value.split("(")[0];
                    stockChart.data.labels.push(vaccineName);
                    chartData.push(vaccine.quantity + 1); //HACK 
                }

                stockChart.data.datasets.push({
                    label: OpenIZ.Localization.getString("locale.stock.vaccines.title"),
                    data:chartData,
                    backgroundColor: [
                        'rgba(255, 99, 132, 0.2)',
                        'rgba(54, 162, 235, 0.2)',
                        'rgba(255, 206, 86, 0.2)',
                        'rgba(75, 192, 192, 0.2)',
                        'rgba(153, 102, 255, 0.2)',
                    ],
                    borderColor: [
                        'rgba(255,99,132,1)',
                        'rgba(54, 162, 235, 1)',
                        'rgba(255, 206, 86, 1)',
                        'rgba(75, 192, 192, 1)',
                        'rgba(153, 102, 255, 1)',
                        'rgba(255, 159, 64, 1)'
                    ],
                    borderWidth: 1
                });

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
