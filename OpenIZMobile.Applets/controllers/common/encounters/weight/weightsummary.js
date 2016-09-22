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
 * User: kirkleyd
 * Date: 2016-9-21
 */

layoutApp.controller('WeightSummaryController', ['$scope', function ($scope)
{
    $scope.$on('loadWeightChart', function (event, arg)
    {
        var patientId = OpenIZ.urlParams["patientId"];

        var weights = [];

        if (patientId !== undefined && patientId !== null)
        {
            var query = "_id=" + patientId;

            OpenIZ.Ims.get({
                resource: "Patient",
                query: query,
                continueWith: function (data)
                {
                    for (var i = 0; i < data.participation.RecordTarget.length; i++)
                    {
                        var actModel = data.participation.RecordTarget[i].actModel;

                        if (actModel !== undefined &&
                            actModel.classConcept !== undefined &&
                            actModel.moodConcept !== undefined &&
                            actModel.classConcept.toUpperCase() === "28D022C6-8A8B-47C4-9E6A-2BC67308739E" &&
                            actModel.moodConcept.toUpperCase() === OpenIZModel.ActMoodKeys.EventOccurrence)
                        {
                            console.log(actModel);
                        }
                    }

                    console.log(data);
                },
                onException: function (ex)
                {
                    console.log(ex);
                }
            });
        }

        // Query for patient weight
        var childBirthdate = moment("2014-04-10");
        var dateLabels = ["2014-04-10", "2014-07-18", "2014-10-03", "2015-02-14", "2015-08-11", "2016-04-09", "2016-11-10"];
        var weights = [3, 4, 6, 8, 12, 20, 24];
        var weightsDataset = [];

        // Populate the weights dataset
        for (var i = 0; i < dateLabels.length; i++) {
            weightsDataset.push({ x: dateLabels[i], y: weights[i] });
        }

        // Grab the ideal weights from the database
        var idealWeightAgeInMonths = [0, 3, 6, 9, 12, 24, 36, 48, 60, 72, 84, 96, 108, 120];
        var idealWeights = [3.3, 6.0, 7.8, 9.2, 10.2, 12.3, 14.6, 16.7, 18.7, 20.7, 22.9, 25.3, 28.1, 31.4];

        var idealWeightDataset = [];
        var lastDate = moment(dateLabels[dateLabels.length - 1]);
        // Calculate the ideal weight dataset for the child
        for (var i = 0; i < dateLabels.length; i++) {
            var idealWeightDate = moment(childBirthdate).add(idealWeightAgeInMonths[i], 'months');
            var idealWeightDateLabel = moment(idealWeightDate).format('YYYY-MM-DD');
            idealWeightDataset.push({ x: idealWeightDateLabel, y: idealWeights[i] });

            // Break the loop after adding the first ideal weight point that is after the last recorded date
            if (lastDate.diff(idealWeightDate) < 0) {
                break;
            }
        }

        var ctxWeights = document.getElementById("weightSummaryChart").getContext("2d");

        var config = {
            type: 'line',
            data: {
                datasets: [{
                    label: 'Child Weight',
                    data: weightsDataset,
                    backgroundColor: 'rgba(75, 192, 192, 0.2)',
                    borderColor: 'rgba(75, 192, 192, 1)',
                    pointBackgroundColor: 'rgba(75, 192, 192, 1)',
                    pointRadius: 5
                },
                {
                    label: 'Ideal Weight',
                    data: idealWeightDataset,
                    backgroundColor: 'rgba(255, 99, 132, 0.2)',
                    borderColor: 'rgba(255,99,132,1)',
                    pointBackgroundColor: 'rgba(255, 99, 132, 1)',
                    pointRadius: 5
                }]
            },
            options: {
                tooltips: {
                    callbacks: {
                        label: function (tooltipItem, data) {
                            return data.datasets[tooltipItem.datasetIndex].label + ": " + tooltipItem.yLabel + " kg";
                        }
                    }
                },
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
                    yAxes: [{
                        scaleLabel: {
                            display: true,
                            labelString: "Weight (Kilograms)"
                        }
                    }]
                }
            }
        };

        console.log(config);
        var weightsChart = new Chart(ctxWeights, config);
    });
}]);