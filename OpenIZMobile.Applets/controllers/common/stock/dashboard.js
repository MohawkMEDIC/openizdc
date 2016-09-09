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
 * User: justi
 * Date: 2016-7-19
 */

/// <reference path="../js/openiz.js"/>

layoutApp.controller('StockDashboardController', ['$scope', function ($scope) {

    var ctx = $('#stockDashboardChart');
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
            labels: ["OPV", "BCG", "MR"],
            datasets: [{
                label: 'On Hand',
                data: [40, 20, 30],
                backgroundColor: [
                    'rgba(128,255,128,0.2)',
                    'rgba(255,128,128,0.2)',
                    'rgba(128,128,255,0.2)'
                ]
            }, {
                label: 'AMC',
                data: [35, 60, 10],
                backgroundColor: [
                    'rgba(128,128,128,0.2)',
                    'rgba(128,128,128,0.2)',
                    'rgba(128,128,128,0.2)'
                ]
            }]
        }
    });


}]);
