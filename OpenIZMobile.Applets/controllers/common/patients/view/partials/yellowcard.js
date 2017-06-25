/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-3-31
 */

/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('YellowCardController', ['$scope', '$stateParams', function ($scope, $stateParams)
{
    var scope = $scope.$parent;
    // Init data
    scope.patient = scope.patient || new OpenIZModel.Patient({});
    scope.patient.participation = scope.patient.participation || {};
    scope.display = scope.display || {};
    // Iterate through vaccinations and organize them by antigen
    // TODO: Change this to be an AJAX call
    scope.display._vaccineAdministrations = {};
    scope.showVaccineTab = showVaccineTab;

    $scope.yellowcardLegend = [
        { title: OpenIZ.Localization.getString('locale.legend.overdue'), color: '#d13333', icon: 'glyphicon-exclamation-sign' },
        { title: OpenIZ.Localization.getString('locale.legend.performAsap'), color: '#337ab7', icon: 'glyphicon-info-sign' },
        { title: OpenIZ.Localization.getString('locale.legend.completed'), color: '#3c763d', icon: 'glyphicon-ok' },
        { title: OpenIZ.Localization.getString('locale.legend.upcoming'), color: '#31708f', icon: 'glyphicon-th-large' },
        { title: OpenIZ.Localization.getString('locale.legend.appointment'), color: '#31708f', icon: 'glyphicon-calendar' }
    ];
    
    // for patient dob
    scope.$watch('encounters', function (newValue, oldValue) { refreshYellowCard(newValue, oldValue) });

    // for encounters & care plan
    scope.$watch('encounters.length', function (newValue, oldValue) { refreshYellowCard(newValue, oldValue) });

    scope.$watch('patient.deceasedDate', function (newValue, oldValue) { refreshYellowCard(newValue, oldValue) });

    var refreshYellowCard = function (newValue, oldValue) {
        
        if (newValue !== null && newValue !== undefined && $scope.encounters) {

            if ($scope.encounters) {
                // Sort based on dose
                //delete scope.display;
                //scope.display = {};
                //scope.display._vaccineAdministrations = {};
                var newEncounters = $scope.encounters;
            	// Sort based on dose
                //newEncounters.sort(function (a, b)
                //{
                //	if (a.actTime > b.actTime)
                //		return 1;
                //	else if (a.actTime < b.actTime)
                //		return -1;
                //	else
                //	{
                //	    if (a.actModel !== null && a.actModel !== undefined) {
                //	        if (a.actModel.participation["Product"].playerModel.name.Assigned.component.$other.value > b.actModel.participation["Product"].playerModel.name.Assigned.component.$other.value) {
                //	            return 1;
                //	        }
                //	        else if (a.actModel.participation["Product"].playerModel.name.Assigned.component.$other.value < b.actModel.participation["Product"].playerModel.name.Assigned.component.$other.value) {
                //	            return -1;
                //	        }
                //	        else {
                //	            return 0;
                //	        }
                //	    }
                //	    else {
                //	        return 0;
                //	    }
                //	}
                //});

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
                        if (model.$type != 'SubstanceAdministration' || model.typeConceptModel == undefined || (model.typeConceptModel.mnemonic != 'InitialImmunization' && model.typeConceptModel.mnemonic != 'Immunization' && model.typeConceptModel.mnemonic != 'BoosterImmunization') || !model.participation || !model.participation.Product)
                            continue;
                        var antigenId = model.participation.Product.playerModel.name.Assigned.component.$other.value;
                        if (scope.display._vaccineAdministrations[antigenId] == null) {
                            scope.display._vaccineAdministrations[antigenId] = [];
                            for (var i = 0; i < model.doseSequence; i++)
                                scope.display._vaccineAdministrations[antigenId].push(null);
                            //model._enabled = true;
                        }

                        if (model.moodConcept != OpenIZModel.ActMoodKeys.Eventoccurrence && scope.display._vaccineAdministrations[antigenId][model.doseSequence] == null ||
                            (model.moodConcept == OpenIZModel.ActMoodKeys.Eventoccurrence && model.statusConcept != OpenIZModel.StatusKeys.Nullified)
                            ) {
                            var existing = scope.display._vaccineAdministrations[antigenId][model.doseSequence];

                            if(existing && model.creationTime > existing.creationTime  || !existing)
                                scope.display._vaccineAdministrations[antigenId][model.doseSequence] = model;
                        }
                    }
                    
                    ;;
                   
                }

            }
        }
    };

    function showVaccineTab() {
        $('a[data-target="#vaccinations"]').tab('show');
    }
}]);