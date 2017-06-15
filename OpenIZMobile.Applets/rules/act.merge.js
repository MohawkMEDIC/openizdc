/// <reference path="~/js/openiz-bre.js"/>
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