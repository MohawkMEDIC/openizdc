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