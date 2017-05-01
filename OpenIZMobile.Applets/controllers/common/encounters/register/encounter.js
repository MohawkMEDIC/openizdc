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
 * Date: 2016-8-17
 */

/// <reference path="~/js/openiz-model.js"/>
/// <refernece path="~/js/openiz-core.js"/>
/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('EncounterEntryController', ['$scope', '$timeout', function ($scope, $timeout) {

    // Get the current scope that we're in
    var scope = $scope;

    /** 
     * Removes a vaccination from the overdue list
     */
    scope.removeOverdue = scope.removeOverdue || function (bind, afterFocus) {

        var doBindMove = function () {
            delete (bind.targetModel._overdue);
            bind._enabled = true;
            bind._encounter.relationship._OverdueHasComponent.splice($.inArray(bind, bind._encounter.relationship._OverdueHasComponent), 1);
            bind._encounter.relationship.HasComponent.push(bind);
            if (afterFocus)
                $timeout(function () { $(afterFocus).focus() }, 200);
        };

        // Is there a previous or later step in the has component? if so warn
        var laterStep = $.grep(bind._encounter.relationship.HasComponent, /** @param {OpenIZModel.Act} e */ function (e) {
            return e.targetModel.protocol[0].protocol == bind.targetModel.protocol[0].protocol;
        });
        if (laterStep.length && confirm(OpenIZ.Localization.getString("locale.encounter.laterStepConflict"))) {
            bind._encounter.relationship.HasComponent.splice($.inArray(laterStep[0], bind._encounter.relationship.HasComponent), 1);
            doBindMove();
        }
        else if (!laterStep.length) {
            doBindMove();
        }
    }

    /** 
    * Add sub-encounter sub-encounter
    */
    scope.addSubEncounter = scope.addSubEncounter || function (bind, templateName, relateTo, relateAs) {
        OpenIZ.CarePlan.getActTemplateAsync({
            templateId: templateName,
            continueWith: function (d) {
                bind.push({
                    _created: true,
                    _enabled: true,
                    targetModel: d
                });

                if (relateTo && relateAs)
                    relateTo[relateAs] = new OpenIZModel.ActRelationship({
                        target: d.id
                    });
                scope.$apply();
            }
        });

    };

    /**
     * Bind a relationship model
     * @param {OpenIZModel.Act} act
     */
    scope.bindRelationship = function (act, relationshipType, targetId, final) {

        OpenIZ.Act.findAsync({
            query: { "_id": targetId },
            continueWith: function (targetAct) {

                act.relationship = act.relationship || {};
                act.relationship[relationshipType] = new OpenIZModel.ActRelationship({
                    relationshipType: OpenIZModel.ActRelationshipTypeKeys[relationshipType],
                    targetModel: targetAct,
                    target: targetId,
                    source: act.id
                });
                eval(final);

                $scope.$apply();
            }
        })
    }

    /** 
     * Delete sub-encounter
     */
    scope.delSubEncounter = function (bind, index, removeFrom, removeAs) {
        if (isNaN(index)) {
            for (var i in bind) {
                try {
                    if (bind[i].targetModel.templateModel.mnemonic == index)
                        bind.splice(i, 1);

                    if (removeFrom && removeAs)
                        delete (removeFrom[removeAs]);
                } catch (e) { }
            }
        }
        else
            bind.splice(index, 1);
    };

    // Encounter 
    scope.validateAct = scope.validateAct || function (act) {

        var validation = [];

        // Act not done!?
        if (act.negationInd && act.reasonConcept == null) // not done - There must be a reason why ... 
            validation.push(OpenIZ.Localization.getString('locale.encounter.validation.reasonRequired'));
        else {

        }

        return validation;

    };

    // Gets the next sequence based on current scope encounter
    scope.getNextDoseSequence = scope.getNextDoseSequence || function (productId) {
        var nextDose = 1;
        $.grep(scope.encounters, function (a) {
            if (nextDose <= a.doseSequence &&
                a.statusConcept == OpenIZModel.StatusKeys.Completed &&
                a.participation.Product &&
                a.participation.Product.player == productId)
                nextDose = a.doseSequence + 1;
        });
        return nextDose;
    };
}]);
