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

/**
 * This rule represents a simple business rule that will ensure that the current patient for which an encounter is being submitted
 * does not have a prior AEFI for the submitted vaccinations
 */

/// <reference path="~/js/openiz-bre.js"/>
/// <reference path="~/js/openiz.js"/>

OpenIZBre.AddValidator("SubstanceAdministration", function (sbadm) {
    retVal = [];

    var drugConcept = null;
    if (sbadm.participation.Product)
        drugConcept = sbadm.participation.Product.targetModel.typeConcept;
    else
        drugConcept = sbadm.participation.Consumable.targetModel.typeConcept;

    // First, we want to check if the substance being administrated has an AEFI
    OpenIZ.Act.findAsync({
        query: {
            "classConcept": OpenIZModel.ActClassKeys.Condition,
            "statusConcept": OpenIZModel.StatusKeys.Active,
            "relationship[HasSubject].targetModel@CodedObservation.value": drugConcept
        },
        sync: true,
        /** @param {OpenIZModel.Bundle} e */
        continueWith: function (e) {
            if (e.item && e.totalResults > 0)
                retVal.push(new OpenIZBre.DetectedIssue("locale.encounters.cdss.sbadm.previousAefi", OpenIZBre.IssuePriority.Warning));
        }
    });

    return retVal;
});