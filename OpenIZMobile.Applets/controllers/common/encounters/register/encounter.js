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

/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('EncounterEntryController', ['$scope', function ($scope) {

    // Get the current scope that we're in
    var scope = $scope;

    /** 
    * Add sub-encounter sub-encounter
    */
    scope.addSubEncounter = scope.addSubEncounter || function (bind, templateName) {
        OpenIZ.CarePlan.getActTemplateAsync({
            templateId: templateName,
            continueWith: function (d) {
                bind.push({
                    _created: true,
                    _enabled: true,
                    targetModel: d
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
    scope.delSubEncounter = function (bind, index) {
        if (isNaN(index)) {
            for (var i in bind) {
                try {
                    if (bind[i].targetModel.template.mnemonic == index)
                        bind.splice(i, 1);
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
}]);
