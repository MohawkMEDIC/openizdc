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
 * User: kirkleyd
 * Date: 2016-9-21
 */

/// <reference path="../js/openiz.js"/>

layoutApp.controller('WeightSummaryController', ['$scope', function ($scope) {

    $scope.loadWeightChart = function () {
        // Query for patient weight
        var dateLabels = ["2015-04-10", "2015-04-18", "2015-05-03", "2015-07-14", "2015-11-11", "2016-02-09", "2016-05-10"];
        var weights = [10, 12, 18, 24, 30, 28, 34];

        var ctxWeights = document.getElementById("weightSummaryChart").getContext("2d");

        var config = {
            type: 'line',
            data: {
                labels: ["2015-04-10", "2015-04-18", "2015-05-03", "2015-07-14", "2015-11-11", "2016-02-09", "2016-05-10"],
                datasets: [{
                    label: "My First dataset",
                    data: [10, 12, 18, 24, 30, 28, 34],
                }]
            },
            options: {
                scales: {
                    xAxes: [{
                        type: 'time',
                        time: {
                            displayFormats: {
                                'millisecond': 'MMM DD',
                                'second': 'MMM DD',
                                'minute': 'MMM DD',
                                'hour': 'MMM DD',
                                'day': 'MMM DD',
                                'week': 'MMM DD',
                                'month': 'MMM DD',
                                'quarter': 'MMM DD',
                                'year': 'MMM DD'
                            }
                        }
                    }],
                }
            }
        };

        console.log(config);

        var weightsChart = new Chart(ctxWeights, config);
    }

    //$(document).ready(function () {

    //    try {
    //        loadWeightChart();
    //    } catch (e) {
    //        console.log(e);
    //    }
    //});

    //var weightsChart = new Chart(ctxWeights, {
    //    type: 'line',
    //    data: {
    //        labels: dateLabels,
    //        datasets: [{
    //            label: "Weight",
    //            data: weights
    //        }]
    //    },
    //    options: {
    //        scales: {
    //            xAxes: [{
    //                type: 'time',
    //                time: {
    //                    displayFormats: {
    //                        'millisecond': 'MMM DD',
    //                        'second': 'MMM DD',
    //                        'minute': 'MMM DD',
    //                        'hour': 'MMM DD',
    //                        'day': 'MMM DD',
    //                        'week': 'MMM DD',
    //                        'month': 'MMM DD',
    //                        'quarter': 'MMM DD',
    //                        'year': 'MMM DD'
    //                    }
    //                }
    //            }]
    //        }
    //    }
    //})

}]);
