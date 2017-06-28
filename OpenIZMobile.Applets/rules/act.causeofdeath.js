/// <reference path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-bre.js"/>
/// <reference path="~/js/openiz-model.js"/>

OpenIZBre.AddBusinessRule("CodedObservation", "AfterInsert", 
    /** @param {OpenIZModel.Act} act */
    function (act) {

    if (act.statusConcept == OpenIZModel.StatusKeys.Completed &&
        act.classConcept &&
        act.typeConcept == "6fb8487c-cd6f-44f1-ab63-27dc65405792" && // Clinical status
        act.value == "6df3720b-857f-4ba2-826f-b7f1d3c3adbb" && // Dead
        act.participation &&
        act.participation.RecordTarget &&
        act.participation.RecordTarget.player
        )
    {
        // Get the patient and set them as deceased
        console.info("Patient " + act.participation.RecordTarget.player + " is deceased, will mark as deceased");
        OpenIZ.Patient.findAsync({
            query: { _id: act.participation.RecordTarget.player },
            continueWith: /** @param {OpenIZModel.Patient} patient */ function(patient) {
                
                // Set the deceased date
                if(act.relationship &&
                    act.relationship.HasComponent &&
                    act.relationship.HasComponent.targetModel)
                    patient.deceasedDate = act.relationship.HasComponent.targetModel.actTime;
                else
                    patient.deceasedDate = act.actTime;

                // Save the patient
                OpenIZ.Patient.updateAsync({ data: patient });
            },
            onException: /** @param {OpenIZModel.Exception} ex */ function(ex) {
                console.error("Error making patient deceased: " + (ex.message || ex));
            }
        })
    }
    return act;
});