/// <reference path="~/js/openiz-bre.js"/>
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
 * Date: 2017-6-28
 */

/// <reference path="~/js/openiz-model.js"/>

/** 
 * @summary Performs the merge functionality
 * @param {OpenIZModel.Act} act 
 */
function __ActMergeFunction(act) {

    act.tag = act.tag || {};
    if (!act.participation.RecordTarget ||
            act.tag["hasDuplicateFix"])
        return act;

    // Now we want to grab the record target and see if they are a merge?
    var recordTarget = act.participation.RecordTarget.player;
    OpenIZ.Ims.get({
        query: {
            target: recordTarget,
            relationshipType: OpenIZModel.EntityRelationshipTypeKeys.Replaces
        },
        resource: "EntityRelationship",
        /** @param {OpenIZModel.Bundle} data */
        continueWith: function (data) {
            if (data.totalResults == 0) return; // no replacements

            act.participation.RecordTarget.player = data.item[0].source; // Replace the target player

        }
    });

    if (!OpenIZBre.IsInFrontEnd)
        act.tag['hasDuplicateFix'] = true;

    return act;
}

/** 
 * Before insert trigger which is used to correct acts which point at a merged record. This occurs when 
 * a bundle is persisted and the record targets point at an auto-merged record
 */
OpenIZBre.AddBusinessRule("Act", "BeforeInsert",
    /** @param {OpenIZModel.Act} act */
    function (act) {
        return __ActMergeFunction(act);
    });
OpenIZBre.AddBusinessRule("SubstanceAdministration", "BeforeInsert",
    /** @param {OpenIZModel.Act} act */
    function (act) {
        return __ActMergeFunction(act);
    });
OpenIZBre.AddBusinessRule("QuantityObservation", "BeforeInsert",
    /** @param {OpenIZModel.Act} act */
    function (act) {
        return __ActMergeFunction(act);
    });
OpenIZBre.AddBusinessRule("CodedObservation", "BeforeInsert",
    /** @param {OpenIZModel.Act} act */
    function (act) {
        return __ActMergeFunction(act);
    });