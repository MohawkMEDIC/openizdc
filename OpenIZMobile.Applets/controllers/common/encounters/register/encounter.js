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
     * Cascades an encounter date change
     */
    scope.encounterDateChanged = scope.encounterDateChanged || function (encounter) {
        for (var e in encounter.relationship.HasComponent)
            encounter.relationship.HasComponent[e].targetModel.actTime = encounter.actTime;
    }

    /**
     * Synchronizes all doses on the same date with the date of the specified act
     */
    scope.synchronizeDates = scope.synchronizeDates || function (act) {
        if (!act.targetModel.actTime) return;

        for (var e in act._encounter.relationship._OverdueHasComponent)
            if (act._encounter.relationship._OverdueHasComponent[e].targetModel.$type == act.targetModel.$type &&
                OpenIZ.Util.toDateInputString(act._encounter.relationship._OverdueHasComponent[e].targetModel.actTime) == OpenIZ.Util.toDateInputString(act._originalTime)) {
                act._encounter.relationship._OverdueHasComponent[e].targetModel.actTime = act.targetModel.actTime;
                //act._encounter.relationship._OverdueHasComponent[e]._originalTime = act.targetModel.actTime;
            }
    }

    /** 
     * Fills in missing data for the specified participation
     */
    scope.fillParticipationPlayerEntity = scope.fillParticipationPlayerEntity || /** @param {OpenIZModel.ActParticipation} bind */ function (bind) {
        if(bind.player)
        {
            OpenIZ.Ims.get({
                resource: "Entity",
                query: { _id: bind.player, _viewModel: "min" },
                continueWith: function (ent) {
                    bind.playerModel = ent;
                    scope.$apply();
                }
            });
        }
    }

    /** Moves a record to overdue status */
    scope.makeOverdue = scope.makeOverdue || function (bind) {
        bind.targetModel.tag = bind.targetModel.tag || {};
        bind.targetModel.tag.backEntry = true;
        delete (bind.targetModel.participation.Consumable);
        delete (bind.targetModel.isNegated);
        delete (bind.targetModel.reasonConcept);
        delete (bind.targetModel.reasonConceptModel);
        bind._encounter.relationship.HasComponent.splice($.inArray(bind, bind._encounter.relationship.HasComponent), 1);
        bind._encounter.relationship._OverdueHasComponent.push(bind);

        bind._enabled = true;
        // Call care planner to suggest the next item
        OpenIZ.App.showWait("#makeOvd_" + bind.targetModel.id);
        OpenIZ.CarePlan.getCarePlanAsync({
            query: "_patientId=" + scope.patient.id + "&_protocolId=" + bind.targetModel.protocol[0].protocol + "&_viewModel=min",
            onDate: new Date(),
            continueWith: function (dat) {
                for (var i in dat.item) {
                    if (dat.item[i].doseSequence == bind.targetModel.doseSequence + 1) {
                        var enc = new OpenIZ.Act.createFulfillment(dat.item[i]);
                        enc.actTime = bind._encounter.actTime;

                        var relationship = new OpenIZModel.ActRelationship({
                            targetModel: enc,
                            target: enc.id,
                        });
                        relationship._created = false;
                        relationship._enabled = bind._enabled;
                        relationship._encounter = bind._encounter;
                        relationship._originalTime = dat.item[i].actTime;
                        bind._encounter.relationship.HasComponent.push(relationship);

                        scope.$apply();
                    }
                    else if (dat.item[i].doseSequence == bind.targetModel.doseSequence) {
                        bind.targetModel.actTime = dat.item[i].actTime;
                    }
                }
            },
            finally: function () {
                OpenIZ.App.hideWait("#makeOvd_" + bind.targetModel.id);
            }
        });
    }

    /** 
     * Removes a vaccination from the overdue list
     */
    scope.removeOverdue = scope.removeOverdue || function (bind, afterFocus) {
        var doBindMove = function () {
            delete (bind.targetModel.tag.backEntry);
            bind._enabled = true;
            bind.statusConcept = OpenIZModel.StatusKeys.Active;
            delete bind.statusConceptModel;
            bind.targetModel.actTime = bind._encounter.actTime;
            bind._encounter.relationship._OverdueHasComponent.splice($.inArray(bind, bind._encounter.relationship._OverdueHasComponent), 1);
            bind._encounter.relationship.HasComponent.push(bind);
            if (afterFocus)
                $timeout(function () { $(afterFocus).focus() }, 200);
        };

        // Is there a previous or later step in the has component? if so warn
        var laterStep = $.grep(bind._encounter.relationship.HasComponent, /** @param {OpenIZModel.Act} e */ function (e) {
            return e.targetModel.protocol[0].protocol == bind.targetModel.protocol[0].protocol && e.targetModel.id != bind.targetModel.id;
        });
        if (laterStep.length && confirm(OpenIZ.Localization.getString("locale.encounters.errors.laterStepConflict"))) {
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
                d.tag = d.tag || {};
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
            query: {
                "_id": targetId,
                "_viewModel": "full"
            },
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


    /** 
     * Validate the act
     */
    scope.validateAct = scope.validateAct || function (act) {
        var validation = [];

        // Act not done!?
        if (act.negationInd && act.reasonConcept == null) // not done - There must be a reason why ...
            validation.push(OpenIZ.Localization.getString('locale.encounter.validation.reasonRequired'));
        
        return validation;
    };

    /** 
     * Gets all valid dose sequences for the specified product
     */
    scope.getDoseSequences = scope.getDoseSequences || function (productId) {
        var retVal = [];
        var scp = scope;
        while (!scp.encounters && scp.$parent) scp = scp.$parent;

        $.each(scp.encounters, function (i, a) {
            if (a.participation && a.participation.Product &&
                a.participation.Product.player == productId &&
                a.moodConcept == OpenIZModel.ActMoodKeys.Propose)
                retVal.push(a.doseSequence);
        });
        return retVal;
    }

    /** 
     * Cascade dose sequences
     */
    scope.cascadeDoseSequences = scope.cascadeDoseSequences || function (bind, doseSequenceId) {
        $.each(bind, function (i, a) {
            // Set dose sequence if it is applicable
            if (a.targetModel.participation && a.targetModel.participation.Product && !a._overrideDose) {
                var allowedDoses = scope.getDoseSequences(a.targetModel.participation.Product.player);
                if (allowedDoses.indexOf(doseSequenceId) > -1)
                    a.targetModel.doseSequence = doseSequenceId;
            }
        });
    }

    // Gets the next sequence based on current scope encounter
    scope.getNextDoseSequence = scope.getNextDoseSequence || function (productId) {
        var nextDose = 1;
        var scp = scope;
        while (!scp.encounters && scp.$parent) scp = scp.$parent;
        $.grep(scp.encounters, function (a) {
            if (nextDose <= a.doseSequence &&
                a.statusConcept == OpenIZModel.StatusKeys.Completed &&
                a.participation.Product &&
                a.participation.Product.player == productId)
                nextDose = a.doseSequence + 1;
        });
        return nextDose;
    };

    // Gets the maximum date for an act based on the previous sequence identifier
    scope.getMaxDate = scope.getMaxDate || function (act) {
        var retVal = new Date();
        // Find next dose sequence
        if (!act._enabled || act.targetModel.doseSequence == null) return retVal;

        // Find next dose
        var prodId = act.targetModel.participation.Product.player;
        var seqId = act.targetModel.doseSequence + 1;
        for (var k in act._encounter.relationship._OverdueHasComponent)
            if (act._encounter.relationship._OverdueHasComponent[k].targetModel.participation &&
                act._encounter.relationship._OverdueHasComponent[k].targetModel.participation.Product &&
                act._encounter.relationship._OverdueHasComponent[k].targetModel.participation.Product.player == prodId &&
                act._encounter.relationship._OverdueHasComponent[k].targetModel.doseSequence == seqId &&
                act._encounter.relationship._OverdueHasComponent[k]._enabled)
                return act._encounter.relationship._OverdueHasComponent[k].targetModel.actTime;
        return retVal;
    };

    // Gets the minimum date for an act based on the previous sequence identifier
    scope.getMinDate = scope.getMinDate || function (act) {
        var retVal = scope.patient.dateOfBirth;
        // Find next dose sequence
        if (!act._enabled || act.targetModel.doseSequence == null) return retVal;

        // Find next dose
        var prodId = act.targetModel.participation.Product.player;
        var seqId = act.targetModel.doseSequence - 1;
        for (var k in act._encounter.relationship._OverdueHasComponent)
            if (act._encounter.relationship._OverdueHasComponent[k].targetModel.participation &&
                act._encounter.relationship._OverdueHasComponent[k].targetModel.participation.Product &&
                act._encounter.relationship._OverdueHasComponent[k].targetModel.participation.Product.player == prodId &&
                act._encounter.relationship._OverdueHasComponent[k].targetModel.doseSequence == seqId &&
                act._encounter.relationship._OverdueHasComponent[k]._enabled)
                return act._encounter.relationship._OverdueHasComponent[k].targetModel.actTime;
        return retVal;
    };

    scope.hasCauseOfDeath = scope.hasCauseOfDeath || function (encounter) {
        return encounter.relationship.HasComponent.some(x => x.targetModel && x.targetModel.templateModel && x.targetModel.templateModel.mnemonic === "act.observation.causeofdeath");
    }

    scope.addCauseOfDeath = scope.addCauseOfDeath || function (patient, act, encounter) {
        if (patient._deceased) {
            if (!scope.hasCauseOfDeath(encounter))
                scope.addSubEncounter(encounter.relationship.HasComponent, 'act.observation.causeofdeath', act.targetModel.relationship, 'IsCauseOf');
        }
        else
            scope.delSubEncounter(encounter.relationship.HasComponent, 'act.observation.causeofdeath', act.targetModel.relationship, 'IsCauseOf');
    }

    // TODO: change these functions to not adjust the date, however returning a new date object causes an infinite digest cycle
    scope.getMaxDateValidation = function (date) {
        if (date) {
            date.setHours(23);
            date.setMinutes(59);
            date.setSeconds(59);
            date.setMilliseconds(999);
            return date;
        }
        else {
            return false;
        }
    }

    scope.toArray = scope.toArray || function (ptcpt)
    {
        return !ptcpt ? ptcpt : Array.isArray(ptcpt) ? ptcpt : [ptcpt];
    }

    // Encounter
    scope.validateAct = scope.validateAct || function (act)
    {
        var validation = [];

        // Act not done!?
        if (act.negationInd && act.reasonConcept == null) // not done - There must be a reason why ...
            validation.push(OpenIZ.Localization.getString('locale.encounter.validation.reasonRequired'));
       
        return validation;
    };

    scope.getMinDateValidation = function (date) {
        if (date) {
            date.setHours(0);
            date.setMinutes(0);
            date.setSeconds(0);
            date.setMilliseconds(0);
            return date;
        }
        else {
            return false;
        }
    }
}]);
