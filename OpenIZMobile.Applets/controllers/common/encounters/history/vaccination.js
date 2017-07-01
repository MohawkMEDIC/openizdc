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
 * Date: 2016-7-30
 */

/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('VaccinationHistoryController', ['$scope','$rootScope', function($scope, $rootScope) {
    //    $('.oiz-vaccination-history').each(function (i, e) {
    // Get the current scope that we're in
    //        var scope = angular.element(e).scope();

    var scope = $scope.$parent;
    // Init data
    scope.patient = scope.patient || new OpenIZModel.Patient({});
    scope.patient.participation = scope.patient.participation || {};
    scope.display = scope.display || {};
    scope.vaccinationCheck = vaccinationCheck;
    scope.vaccinationCheckDisabled = vaccinationCheckDisabled;
    // Iterate through vaccinations and organize them by antigen
    // TODO: Change this to be an AJAX call
    scope.display._vaccineAdministrations = {};
    scope.harmonizeDoseTimes = harmonizeDoseTimes;

    // Harmonize dose sequence times
    function harmonizeDoseTimes(doseSequence, index) {
        if (!doseSequence._enabled) return;
        for (var antigenId in scope.display._vaccineAdministrations) {
            if (scope.display._vaccineAdministrations[antigenId] &&
                scope.display._vaccineAdministrations[antigenId][index] &&
                OpenIZ.Util.toDateInputString(scope.display._vaccineAdministrations[antigenId][index].actTime) == OpenIZ.Util.toDateInputString(doseSequence._originalTime))
                scope.display._vaccineAdministrations[antigenId][index].actTime = doseSequence.actTime;
        }
    }

    function vaccinationCheck(event, sequence, currentIndex) {
        if (!sequence._enabled) {
            for (var i = currentIndex+1; i < event.length; i++) {
                event[i]._enabled = false;
            }
        }
    }

    function vaccinationCheckDisabled(event, sequence, currentIndex) {
        if (sequence.moodConcept === OpenIZModel.ActMoodKeys.Eventoccurrence || (sequence.startTime > $rootScope.page.maxEventTime)) {
            return true;
        }
        if (currentIndex > 0 && event[currentIndex - 1] !== null) {
            for (var i = currentIndex - 1; i >= 0; i--) {
                if (event[i] !== null && !event[i]._enabled) {
                    return true;
                }
            }
        }
        return false;
    }

    scope.$watch(function (s) { return s.patient.participation }, function (newValue, oldValue) {
        scope.display._vaccineAdministrations = {};
        if(newValue != null && newValue.RecordTarget !== undefined)
        {
            if (newValue.RecordTarget instanceof Array)
            {
                // Sort based on dose
                newValue.RecordTarget.sort(function (a, b)
                {
                    if (a.actModel.actTime > b.actModel.actTime)
                        return 1;
                    else if (a.actModel.actTime < b.actModel.actTime)
                        return -1;
                    else
                    {
                        if (a.actModel.participation["Product"].playerModel.name.Assigned.component.$other.value > b.actModel.participation["Product"].playerModel.name.Assigned.component.$other.value) {
                            return 1;
                        }
                        else if(a.actModel.participation["Product"].playerModel.name.Assigned.component.$other.value <b.actModel.participation["Product"].playerModel.name.Assigned.component.$other.value){
                            return -1;
                        }
                        else {
                            return 0;
                        }
                    }
                });

                // Record target

                for (var ptcpt in newValue.RecordTarget)
                {

                    var acts = [];

                    // Patient encounters are the grouping of objects
                    var participation = newValue.RecordTarget[ptcpt];
                    if (participation.actModel.$type != 'PatientEncounter')
                        acts.push(participation);
                    else
                        acts = participation.actModel.relationship;

                    for (var act in acts)
                    {

                        var model = acts[act].actModel || acts[act].targetModel;
                        // Ignore anything except substance admins
                        if (model.$type != 'SubstanceAdministration' || model.typeConceptModel == undefined || (model.typeConceptModel.mnemonic != 'InitialImmunization' && model.typeConceptModel.mnemonic != 'Immunization' && model.typeConceptModel.mnemonic != 'BoosterImmunization'))
                            continue;

                        var antigenId = model.participation.Product.playerModel.name.Assigned.component.$other.value;
                        if (scope.display._vaccineAdministrations[antigenId] == null)
                        {
                            scope.display._vaccineAdministrations[antigenId] = [];
                            for (var i = 0; i < model.doseSequence; i++)
                                scope.display._vaccineAdministrations[antigenId].push(null);
                            //model._enabled = true;
                        }
                        scope.display._vaccineAdministrations[antigenId].push(model);
                        //                scope.$apply();
                    }
                }
            }
        }
    });
}]);