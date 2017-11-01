/// <reference path="~/js/openiz.js"/>
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
                if (act.relationship &&
                    act.relationship.HasComponent &&
                    act.relationship.HasComponent.target) {

                    if (!act.relationship.HasComponent.targetModel)
                        OpenIZ.Act.findAsync({
                            query: { _id: act.relationship.HasComponent.target },
                            continueWith: function (d) {
                                act.relationship.HasComponent.targetModel = d;
                            },
                            synchronous: true
                        });

                    patient.deceasedDate = act.relationship.HasComponent.targetModel.actTime;

                }
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