/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2016-11-14
 */

/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('YellowCardController', ['$scope', function ($scope) {
    //    $('.oiz-vaccination-history').each(function (i, e) {
    // Get the current scope that we're in
    //        var scope = angular.element(e).scope();

    var scope = $scope.$parent;
    // Init data
    scope.patient = scope.patient || new OpenIZModel.Patient({});
    scope.patient.participation = scope.patient.participation || {};
    scope.display = scope.display || {};

    // Iterate through vaccinations and organize them by antigen
    // TODO: Change this to be an AJAX call
    scope.display._vaccineAdministrations = {};

    $scope.yellowcardLegend = [
    { title: OpenIZ.Localization.getString('locale.legend.overdue'), color: '#d13333', icon: 'glyphicon-exclamation-sign' },
    { title: OpenIZ.Localization.getString('locale.legend.completed'), color: '#3c763d', icon: 'glyphicon-ok' },
    { title: OpenIZ.Localization.getString('locale.legend.upcoming'), color: '#31708f', icon: 'glyphicon-th-large' }
    ];

    scope.$watch(function (s) { return s.encounters != undefined ? s.encounters.length : null; }, function (newValue, oldValue) {
        
        if (newValue != null && newValue != 0) {

            if (scope.encounters) {
                // Sort based on dose
                var oldIndex = oldValue == undefined ? 0 : oldValue;
                var newEncounters = scope.encounters.slice(oldIndex, newValue);
                
            	// Sort based on dose
                newEncounters.sort(function (a, b)
                {
                	if (a.actTime > b.actTime)
                		return 1;
                	else if (a.actTime < b.actTime)
                		return -1;
                	else
                	{
                		return 0;
                	}
                });

                // Record target

                for (var ptcpt in newEncounters) {

                    var acts = [];

                    // Patient encounters are the grouping of objects
                    var participation = newEncounters[ptcpt];
                    if (participation.$type != 'PatientEncounter')
                        acts.push(participation);
                    else if (participation.relationship && participation.relationship.HasComponent)
                        acts = $.map(participation.relationship.HasComponent, function (e) { return e.targetModel; });
                    else
                        acts.push(participation);

                    for (var act in acts) {

                        var model = acts[act];
                        // Ignore anything except substance admins
                        if (model.$type != 'SubstanceAdministration' || model.typeConceptModel == undefined || (model.typeConceptModel.mnemonic != 'InitialImmunization' && model.typeConceptModel.mnemonic != 'Immunization' && model.typeConceptModel.mnemonic != 'BoosterImmunization') ||
                            !model.participation.Product)
                            continue;
                        var antigenId = model.participation.Product.playerModel.name.Assigned.component.$other.value;
                        if (scope.display._vaccineAdministrations[antigenId] == null) {
                            scope.display._vaccineAdministrations[antigenId] = [];
                            for (var i = 0; i < model.doseSequence; i++)
                                scope.display._vaccineAdministrations[antigenId].push(null);
                            //model._enabled = true;
                        }

                        if(model.moodConcept != OpenIZModel.ActMoodKeys.Eventoccurrence && scope.display._vaccineAdministrations[antigenId][model.doseSequence] == null ||
                            (model.moodConcept == OpenIZModel.ActMoodKeys.Eventoccurrence)
                            )
                            scope.display._vaccineAdministrations[antigenId][model.doseSequence] = model;
                        //               scope.$apply();
                    }
                    
                    ;;
                   
                }
                $scope.vaccineArray = [];
                $.each(scope.display._vaccineAdministrations, function (index, value) {
                    $scope.vaccineArray.push(scope.display._vaccineAdministrations[index])
                    $scope.vaccineArray[$scope.vaccineArray.length-1].antigenName = index
                });
                $scope.vaccineArray.sort(function (a, b) {
                    var textOne = a.antigenName;
                    var textTwo = b.antigenName;
                    return (textOne < textTwo) ? -1 : (textOne > textTwo) ? 1 : 0;
                });

            }
        }
    });
}]);