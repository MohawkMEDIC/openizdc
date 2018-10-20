/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * User: fyfej
 * Date: 2017-10-30
 */

/**
 * @version 0.9.6 (Edmonton)
 * @copyright (C) 2015-2018, Mohawk College of Applied Arts and Technology
 * @license Apache 2.0
 */

window.alert_ = window.alert;
window.alert = function () {
    alert_.apply(window, arguments)
};

/// <reference path="~/js/openiz-bre.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/select2.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

// SHIM
var OpenIZApplicationService = window.OpenIZApplicationService || {};

/**
 * @callback OpenIZ~continueWith
 * @summary The function call which is called whenever an asynchronous operation completes successfully
 * @param {Object} data The result data from the asynchronous callback
 * @param {Object} state State data which is to be passed to the async callback
 */
/**
 * @callback OpenIZ~onException
 * @summary The exception handling callback whenever an asynchronous operation does not complete successfully
 * @param {OpenIZModel.Exception} exception The exception which was thrown as a result of the operation.
 * @param {Object} state State data which is to be passed to the async callback
 */
/**
 * @callback OpenIZ~finally
 * @summary The callback which is always executed from an asynchronous call regardless of whehter the operation was successful or not
 * @param {Object} state State data which is to be passed to the async callback
 */
/**
 * @summary OpenIZ Javascript binding class.
 *
 * @description The purpose of this object is to facilitate and organize OpenIZ applet integration with the backing  * OpenIZ container. For example, to allow an applet to get the current on/offline status, or authenticate a user.
 * @namespace OpenIZ
 * @property {Object} _session the current session
 * @property {Object} urlParams the current session
 */
var OpenIZ = OpenIZ || {

    /** 
     * @summary URL Parameters
     */
    urlParams: {},
    /** 
     * @summary Provides a mechanism for user interface interaction with OpenIZ
     * @class
     * @static
     * @property {OpenIZ.UserInterface.PatientControllerPrototype} patientController The patient controller which OpenIZ components can call  
     * @memberof OpenIZ
     */
    UserInterface: {
        /**
         * @class
         * @summary A prototype patient controller which is called by OpenIZ core components
         * @memberof OpenIZ.UserInterface
         * @description This class is overridden by application to control navigation from core OpenIZ components to patient specific ones
         * @example To redirect to my-view on patient view
         *  OpenIZ.UserInterface.PatientControllerPrototype.prototype.view = function(patientId) { $state.transitionTo('myview', { pat: patientId }); };
         */
        PatientControllerPrototype: function () {

            /** 
             * @summary Instructs the handler to open the patient view
             * @method
             * @param {uuid} patientId The patient to be shown
             * @param {object} state The state provider
             */
            this.view = function (patientId) { alert("Default implementation, not implemented "); };
            /** 
             * @summary Instructs the handler to open the patient view
             * @method
             * @param {uuid} patientId The patient to be shown
             * @param {object} state The state provider
             */
            this.download = function (patientId) { alert("Default implementation, not implemented "); };
            /** 
             * @summary Instructs the handler to open the patient view
             * @method
             * @param {OpenIZModel.Patient} patient The patient which is to be saved
             */
            this.save = function (patient, controlData) {
                var controlData = {
                    data: patient,
                    id: patient.id,
                    versionId: patient.version,
                    continueWith: function (data) {
                        console.log(data);
                        switch (data.multipleBirthOrder) {
                            case 1:
                                data.multipleBirthOrderText = OpenIZ.Localization.getString('locale.patient.demographics.multipleBirth.first');
                                break;
                            case 2:
                                data.multipleBirthOrderText = OpenIZ.Localization.getString('locale.patient.demographics.multipleBirth.second');
                                break;
                            case 3:
                                data.multipleBirthOrderText = OpenIZ.Localization.getString('locale.patient.demographics.multipleBirth.third');
                                break;
                            case 0:
                                data.multipleBirthOrderText = OpenIZ.Localization.getString('locale.patient.demographics.multipleBirth.unknown');
                                break;
                        };
                        if (data.multipleBirthOrder != undefined && data.multipleBirthOrder != null) {
                            data.multipleBirthOrder = data.multipleBirthOrder.toString();
                        }
                        OpenIZ.Patient.getAsync({
                            id: patient.id,
                            continueWith: controlData.continueWith,
                            onException: controlData.onException
                        })
                    },
                    onException: controlData.onException,
                    finally: controlData.finally,
                    state: 0
                };

                delete (patient.genderConceptModel);

                if (patient.relationship && patient.relationship.DedicatedServiceDeliveryLocation && patient.relationship.DedicatedServiceDeliveryLocation.targetModel) {
                    delete (patient.relationship.DedicatedServiceDeliveryLocation.targetModel);
                }

                // Address has changed, so let's change it!
                if (patient.address.HomeAddress.villageId != null) {
                    // Get the village
                    OpenIZ.Place.findAsync({
                        query: { _id: patient.address.HomeAddress.villageId, _viewModel: "min" },
                        continueWith: function (data) {
                            patient.address.HomeAddress.component = data.address.Direct.component;
                            OpenIZ.Patient.updateAsync(controlData);

                        },
                        onException: controlData.onException
                    });
                }
                else
                    OpenIZ.Patient.updateAsync(controlData);
            };

        },
        /**
         * @summary Specifies the global patient controller to use for the OpenIZ application session
         */
        patientController: null
    },
    /**
     * @summary Provides operations for managing {@link OpenIZModel.Act} instances.
     * @memberof OpenIZ
     * @static
     * @class
     * @description This wrapper class will assist developers in interfacing with the MiniIms when operating on Acts.
     */
    Act: {
        /**
         * @summary Asynchronously updates an encounter object in the IMS
         * @memberof OpenIZ.Act
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {uuid} controlData.id The identifier of the act that is to be updated
         * @param {object} controlData.data The act data to be updated
         * @see OpenIZ.Ims.delete
         * @see OpenIZModel.Act
         * @example 
         * $scope.act.statusConcept = OpenIZModel.StatusKeys.Completed;
         * OpenIZ.Act.updateAsync({
         *      data: $scope.act,
         *      id: $scope.act.id,
         *      continueWith: function(data) {
         *          alert("Act was updated. New version>" + data.versionId);
         *      }
         * });
         */
        updateAsync: function (controlData) {
            OpenIZ.Ims.put({
                resource: "Act",
                data: controlData.data,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                id: controlData.id,
                state: controlData.state
            });
        },
        /**
         * @summary Asynchronously deletes an encounter object in the IMS
         * @description Obsoletion is used to mark an act as "this information is no longer applicable". 
         * @memberof OpenIZ.Act
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {uuid} controlData.id The identifier of the act that is to be updated
         * @see OpenIZ.Ims.delete
         * @see OpenIZModel.Act
         * @example
         * OpenIZ.Act.obsoleteAsync({
         *      id: $scope.act.id,
         *      continueWith: function(data) {
         *          alert("Act was deleted");
         *      }
         *  });
         */
        obsoleteAsync: function (controlData) {
            OpenIZ.Ims.delete({
                resource: "Act",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                id: controlData.id,
                data: controlData.data,
                state: controlData.state
            });
        },
        /**
         * @summary Asynchronously cancels an act object in the IMS
         * @description Cancellation is used to mark an act as "this act was supposed to occur, it started, but was cancelled". 
         * @memberof OpenIZ.Act
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {uuid} controlData.id The identifier of the act that is to be updated
         * @see OpenIZ.Ims.delete
         * @see OpenIZModel.Act
         * @example
         * OpenIZ.Act.obsoleteAsync({
         *      id: $scope.act.id,
         *      continueWith: function(data) {
         *          alert("Act was deleted");
         *      }
         *  });
         */
        cancelAsync: function (controlData) {
            OpenIZ.Ims.cancel({
                resource: "Act",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                id: controlData.id,
                data: controlData.data,
                state: controlData.state
            });
        },
        /**
         * @summary Asynchronously nullifies an act object in the IMS
         * @description When an act is nullified, it is deemed to have never existed. This is used when an act is done in error. The act itself
         * still remains on the patient's file, however it is in a nullified or "errored" state. When updating act details it is recommended that
         * instead of updating the act directly, that it be nullified and replaced with a relationship of Replaces.
         * @memberof OpenIZ.Act
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {uuid} controlData.id The identifier of the act that is to be updated
         * @see OpenIZ.Ims.nullify
         * @see OpenIZModel.Act
         */
        nullifyAsync: function (controlData) {
            OpenIZ.Ims.nullify({
                resource: "Act",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                id: controlData.id,
                data: controlData.data,
                state: controlData.state
            });
        },
        /**
         * @summary Creates a fulfillment relationship act
         * @description This method creates a new act which fulfills the specified act
         * @param {OpenIZModel.Act} act The act which the new act should fulfill
         * @description When an act fulfills another it is said to be a part of the same chain of events. This method helps to 
         * maintain this chain. For example, if someone orders some stock, or if the care planner proposes something, an act can be
         * created which "fulfills" the other. I.e. "something proposed I do X, I am fulfilling that proposal"
         */
        createFulfillment: function (act) {

            var fulfills = new OpenIZModel.Act();
            
            // Clone act
            switch (act.$type) {
                case "SubstanceAdministration":
                    fulfills = new OpenIZModel.SubstanceAdministration(act);
                    break;
                case "Observation":
                    fulfills = new OpenIZModel.Observation(act);
                    break;
                case "QuantityObservation":
                    fulfills = new OpenIZModel.QuantityObservation(act);
                    break;
                case "CodedObservation":
                    fulfills = new OpenIZModel.CodedObservation(act);
                    break;
                case "TextObservation":
                    fulfills = new OpenIZModel.TextObservation(act);
                    break;
                case "PatientEncounter":
                    fulfills = new OpenIZModel.PatientEncounter(act);
                    break;
                case "ControlAct":
                    fulfills = new OpenIZModel.ControlAct(act);
                    break;
                default:
                    break;
            }

            // Re-assign the identifier
            fulfills.tag = {};
            if (act.stopTime < new Date())
                fulfills.tag.backEntry = true;
            
            // Assign the tag for original date
            fulfills.tag.originalDate = Math.floor(new Date(OpenIZ.Util.toDateInputString(act.actTime)).getTime() / 1000);

            // Original date - Hack this is due to the fact that original protocol does not have
            //fulfills.tag["originalProtocolRange"] = fulfills.startTime + "," + fulfills.stopTime;
            
            fulfills.id = OpenIZ.App.newGuid();
            fulfills.moodConcept = OpenIZModel.ActMoodKeys.Eventoccurrence;
            fulfills.moodConceptModel = null;
            fulfills.creationTime = new Date();
            fulfills.createdBy = fulfills.createdByModel = null;
            fulfills.statusConcept = OpenIZModel.StatusKeys.Active;
            fulfills.statusConceptModel = null;
            fulfills.etag = null;
            //fulfills.protocol = act.protocol;
            // Add fulfillment relationship
            fulfills.relationship = fulfills.relationship || {};
            fulfills.relationship.Fulfills = new OpenIZModel.ActRelationship();
            fulfills.relationship.Fulfills.target = act.id;
            fulfills.relationship.Fulfills.targetModel = act;

            // Create new KEYS
            var roles = Object.keys(fulfills.participation);
            for (var v in roles) {
                var role = roles[v];
                if (fulfills.participation[role].splice !== undefined)
                    for (var k in fulfills.participation[role])
                        fulfills.participation[role][k] = new OpenIZModel.ActParticipation({
                            act: fulfills.id,
                            player: fulfills.participation[role][k].player,
                            playerModel: fulfills.participation[role][k].playerModel
                        });
                else
                    fulfills.participation[role] = new OpenIZModel.ActParticipation({
                        act: fulfills.id,
                        player: fulfills.participation[role].player,
                        playerModel: fulfills.participation[role].playerModel
                    })
            }

            if (fulfills.tag.backEntry) {
                fulfills.actTime = act.actTime;
            }
            else {
                fulfills.actTime = new Date();
            }
            fulfills.startTime = fulfills.stopTime = null;

            return fulfills;
        },
        /**
          * @summary Perform a search of acts asynchronously
          * @memberof OpenIZ.Act
          * @method
          * @param {Object} controlData An object containing search, offset, count and callback data
          * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
          * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
          * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
          * @param {object} controlData.query The query filters to apply to the search
          * @param {int} controlData.query._count The limit of results to return from the ims
          * @param {int} controlData.query._offset The offset of the search result window
          * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
          * @see OpenIZ.Ims.get
          * @example Get all active acts
          * OpenIZ.Act.findAsync({
          *     query: {
          *         statusConcept: OpenIZModel.StatusKeys.Active,
          *         classConcept: OpenIZModel.ActClassKeys.Observation,
          *         _count: 10,
          *         _offset: 0
          *     },
          *     continueWith: function(bundle) {
          *         alert("I found " + bundle.totalResults + " here are results 0 to 10");
          *     }
          * });
          * @example Get all active acts saving the query results (improves performance)
          * OpenIZ.Act.findAsync({
          *     query: {
          *         statusConcept: OpenIZModel.StatusKeys.Active,
          *         classConcept: OpenIZModel.ActClassKeys.Observation,
          *         _count: 10,
          *         _offset: 0,
          *         _queryId: OpenIZ.App.newGuid()
          *     },
          *     continueWith: function(bundle) {
          *         alert("I found " + bundle.totalResults + " here are results 0 to 10");
          *     }
          * });
          * @example Get a specific Act
          * OpenIZ.Act.findAsync({
          *     query: {
          *         _id: "UUID OF THE ACT"
          *     },
          *     continueWith: function(act) {
          *         alert("This act is a " + act.classConcept);
          *     }
          * });
          */
        findAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "Act",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                query: controlData.query,
                state: controlData.state
            });
        },
        /**
         * @summary Performs a patient insert asynchronously
         * @memberof OpenIZ.Act
         * @method
         * @see OpenIZ.Ims.post
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {OpenIZModel.Act} controlData.data The data which is to be inserted on the IMS
         * @example
         *  var act = new OpenIZModel.SubstanceAdministration({
         *      doseQuantity: 1,
         *      participation: {
         *          Product: {
         *              target: "UUID OF TARGET PRODUCT"
         *          },
         *          RecordTarget: {
         *              target: "UUID OF PATIENT"
         *          },
         *          Performer: {
         *              target: "UUID OF CLINICIAN"
         *          }
         *      },
         *      sequence: 1,
         *      isNegated: false
         *  });
         *
         *  OpenIZ.Act.insertAsync({
         *      data: act,
         *      continueWith: function(data) {
         *          alert("Immunization saved as :" + data.id);
         *      },
         *      onException: function(e) {
         *          alert("Oops! Something went wrong!" + e);
         *      }
         *  });
         */
        insertAsync: function (controlData) {
            OpenIZ.Ims.post({
                resource: "Act",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                data: controlData.data,
                state: controlData.state,
                finally: controlData.finally
            });
        },
        /**
         * @summary Get an empty act template object asynchronously. See the OpenIZ documentation for more information about templates
         * @method
         * @memberof OpenIZ.CarePlan
         * @param {object} controlData The data which controls the asynchronous operation.
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.templateId The identifier of the template which to retrieve. Templates should be registered with the application manifest
         * @example
         * OpenIZ.Act.getActTemplateAsync({
         *      templateId: "Act.SubstanceAdmin.Immunization", 
         *      continueWith: function(template) {
         *          $scope.act = template;
         *      }
         * });
         */
        getActTemplateAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "Act/Template",
                query: { "templateId": controlData.templateId, "_viewModel": "full" },
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
        /**
          * @summary Perform a search of an act subtypes asynchronously
          * @description This method differs from findAsync in that it reports intermediate results and exhausts all results rather than 
          * relying on a user to page the results.
          * @memberof OpenIZ.Act
          * @method
          * @param {Object} controlData An object containing search, offset, count and callback data
          * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
          * @param {OpenIZ~continueWith} controlData.intermediateResults The callback to call when a subset of results are fetched
          * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
          * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
          * @param {object} controlData.query The query filters to apply to the search
          * @param {int} controlData.query._count The limit of results to return from the ims
          * @param {int} controlData.query._offset The offset of the search result window
          * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
          * @see OpenIZ.Ims.get
          * @example Fetch all records related to a specific patient
          * OpenIZ.Act.findClinicalActAsync({
          *     query: {
          *         "participation[RecordTarget].player" : patientId
          *     },
          *     intermediateResults: function(bundle) {
          *         // Update your user interface here and do what you like before the next page is fetched
          *     },
          *     continueWith: function(bundle) {
          *         alert("All the patient's data was fetched");
          *     }
          * });
          */
        findClinicalActAsync: function (controlData) {
            var actResources = [
                "Act"
            ];
            /*
            "SubstanceAdministration",
            "QuantityObservation",
            "CodedObservation",
            "TextObservation",
            "PatientEncounter"
        ];*/

            var doSearch = function (index, queryId, offset, count) {

                // Perform query
                var query = controlData.query;
                query._queryId = queryId;
                query._offset = offset;
                query._count = count;

                OpenIZ.Ims.get({
                    resource: actResources[index],
                    /** @param {OpenIZModel.Bundle} data */
                    continueWith: function (data) {

                        controlData.intermediateResults(data);

                        // Call the intermediary results function
                        if (data.offset + data.count < data.totalResults) // More results
                            doSearch(index, queryId, offset + count, count);
                        else if (++index < actResources.length)
                            doSearch(index, queryId, offset + count, count);
                        else
                            controlData.continueWith(data);
                    },
                    onException: controlData.onException,
                    finally: function () {
                        if (index >= actResources.length)
                            controlData.finally
                    },
                    query: query,
                    state: controlData.state
                });
            }

            // Start the process
            doSearch(0, OpenIZ.App.newGuid(), 0, 50);
        },
    },
    /**
     * @summary Provides interaction with the openiz audit console
     * @see OpenIZAudit
     * @static
     * @class
     * @memberof OpenIZ
     */
    Audit: {
        /**
         * @summary Query audit data from the local audit repository
         * @memberof OpenIZ.Audit
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The audit query to be executed
         * @method
         */
        getAuditAsync: function (controlData) {
            OpenIZ.Util.simpleGet("/__audit/audit", controlData);
        }
    },
    /**
     * @summary Interoperation with the IMS
     * @see OpenIZModel
     * @static
     * @class
     * @memberof OpenIZ
     */
    Ims: {
        /**
         * @summary Post data to the IMS
         * @description This function is responsible for actually interoperating with the IMS running on the disconnected client interface.
         * The method will ensure that all callbacks are transferred using the standard OpenIZ callback mechanisms.
         * @memberof OpenIZ.Ims
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.resource The IMSI resource id to be posted to
         * @param {object} controlData.data The IMSI resource data to be posted to the IMS
         * @method
         */
        post: function (controlData) {
            $.ajax({
                method: 'POST',
                url: "/__ims/" + controlData.resource,
                // || WARNING: JAVASCRIPT RANT AHEAD              ||
                // ||                                             ||
                // || Why!? Why!? Why is this even a line of code?||
                // || I specified JSON and application/json yet   ||
                // || the 1337 haxors at jQ decide not to encode  ||
                // || the JSON data I send as JSON!? Why!?        ||
                // \/ Stuff like this is why I dislike JavaScript \/
                data: JSON.stringify(controlData.data),
                dataType: "json",
                contentType: 'application/json',
                success: function (xhr, data) {
                    try {
                        if (controlData.continueWith !== undefined)
                            controlData.continueWith(xhr, controlData.state);

                    }
                    catch (e) {
                        if (controlData.onException !== undefined)
                            controlData.onException(e, controlData.state);
                    }
                    finally {
                        if (controlData.finally !== undefined)
                            controlData.finally(controlData.state);
                    }

                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === null)
                        console.error(error);
                    else if (error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                error.caused_by
                            ), controlData.state);
                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ), controlData.state);

                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);
                },
                async: !(controlData.synchronous || false)
            });
        },
        /**
         * @summary Put (Update) data in the IMS
         * @memberof OpenIZ.Ims
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.resource The IMSI resource id to be posted to
         * @param {uuid} controlData.id The identifier of the resource to be updated
         * @param {uuid} controlData.versionId The version identifier of the resource to be updated
         * @param {object} controlData.data The IMSI resource data to be posted to the IMS
         * @method
        */
        put: function (controlData) {
            $.ajax({
                method: 'PUT',
                url: "/__ims/" + controlData.resource + "?_id=" + controlData.id + "&_versionId=" + controlData.versionId,
                // || WARNING: JAVASCRIPT RANT AHEAD              ||
                // ||                                             ||
                // || Why!? Why!? Why is this even a line of code?||
                // || I specified JSON and application/json yet   ||
                // || the 1337 haxors at jQ decide not to encode  ||
                // || the JSON data I send as JSON!? Why!?        ||
                // \/ Stuff like this is why I dislike JavaScript \/
                data: JSON.stringify(controlData.data),
                dataType: "json",
                contentType: 'application/json',
                success: function (xhr, data) {
                    try {
                        if (controlData.continueWith !== undefined)
                            controlData.continueWith(xhr, controlData.state);

                    }
                    catch (e) {
                        if (controlData.onException !== undefined)
                            controlData.onException(e, controlData.state);
                    }
                    finally {
                        if (controlData.finally !== undefined)
                            controlData.finally(controlData.state);
                    }

                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === null)
                        console.error(error);
                    else if (error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                error.caused_by
                            ), controlData.state);

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ), controlData.state);

                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);
                }
            });
        },
        /**
         * @summary Get data from the IMS
         * @memberof OpenIZ.Ims
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.resource The IMSI resource id to be posted to
         * @param {object} controlData.query The IMSI query (see documentation for structure) to execute
         * @param {int} controlData.query._count The limit of results to return from the ims
         * @param {int} controlData.query._offset The offset of the search result window
         * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
         * @method
         */
        get: function (controlData) {
            $.ajax({
                method: 'GET',
                url: "/__ims/" + controlData.resource,
                data: controlData.query,
                dataType: "json",
                accept: 'application/json',
                contentType: 'application/json',
                success: function (xhr, data) {
                    try {
                        if (controlData.continueWith !== undefined)
                            controlData.continueWith(xhr, controlData.state);

                    }
                    catch (e) {
                        if (controlData.onException !== undefined)
                            controlData.onException(e, controlData.state);
                    }
                    finally {
                        if (controlData.finally !== undefined)
                            controlData.finally(controlData.state);
                    }

                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === undefined)
                        console.error(error);
                    else if (error != undefined && error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                error.caused_by
                            ), controlData.state);
                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ), controlData.state);

                    // Do finally
                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);

                },
                async: !(controlData.synchronous || false)
            });
        },
        /**
         * @summary Deletes data from the IMS
         * @memberof OpenIZ.Ims
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.resource The IMSI resource id to be posted to
         * @param {object} controlData.id The identifier of the object to delete from the IMS 
         * @method
         */
        delete: function (controlData) {
            $.ajax({
                method: 'DELETE',
                url: "/__ims/" + controlData.resource + "?_id=" + controlData.id,
                accept: 'application/json',
                contentType: 'application/json',
                data: JSON.stringify(controlData.data),
                dataType: "json",
                success: function (xhr, data) {
                    try {
                        if (controlData.continueWith !== undefined)
                            controlData.continueWith(xhr, controlData.state);

                    }
                    catch (e) {
                        if (controlData.onException !== undefined)
                            controlData.onException(e, controlData.state);
                    }
                    finally {
                        if (controlData.finally !== undefined)
                            controlData.finally(controlData.state);
                    }

                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === undefined)
                        console.error(error);
                    else if (error != undefined && error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                error.caused_by
                            ), controlData.state);
                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ), controlData.state);

                    // Do finally
                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);

                }
            });
        },
        /**
         * @summary Nullifies data from the IMS
         * @method
         * @memberof OpenIZ.Ims
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.resource The IMSI resource id to be posted to
         * @param {object} controlData.id The identifier of the object to nullify from the IMS 
         */
        nullify: function (controlData) {
            $.ajax({
                method: 'NULLIFY',
                url: "/__ims/" + controlData.resource + "?_id=" + controlData.id,
                accept: 'application/json',
                contentType: 'application/json',
                data: JSON.stringify(controlData.data),
                dataType: "json",
                success: function (xhr, data) {
                    try {
                        if (controlData.continueWith !== undefined)
                            controlData.continueWith(xhr, controlData.state);

                    }
                    catch (e) {
                        if (controlData.onException !== undefined)
                            controlData.onException(e, controlData.state);
                    }
                    finally {
                        if (controlData.finally !== undefined)
                            controlData.finally(controlData.state);
                    }
                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === undefined)
                        console.error(error);
                    else if (error != undefined && error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                error.caused_by
                            ), controlData.state);
                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ), controlData.state);

                    // Do finally
                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);

                }
            });
        },
        /**
         * @summary Cancel act data from the IMS
         * @memberof OpenIZ.Ims
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.resource The IMSI resource id to be posted to
         * @param {object} controlData.id The identifier of the object to delete from the IMS 
         * @method
         */
        cancel: function (controlData) {
            $.ajax({
                method: 'CANCEL',
                url: "/__ims/" + controlData.resource + "?_id=" + controlData.id,
                accept: 'application/json',
                contentType: 'application/json',
                data: JSON.stringify(controlData.data),
                dataType: "json",
                success: function (xhr, data) {
                    try {
                        if (controlData.continueWith !== undefined)
                            controlData.continueWith(xhr, controlData.state);

                    }
                    catch (e) {
                        if (controlData.onException !== undefined)
                            controlData.onException(e, controlData.state);
                    }
                    finally {
                        if (controlData.finally !== undefined)
                            controlData.finally(controlData.state);
                    }
                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === undefined)
                        console.error(error);
                    else if (error != undefined && error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                error.caused_by
                            ), controlData.state);
                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ), controlData.state);

                    // Do finally
                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);

                }
            });
        }
    },
    /**
     * @summary Stock Functions
     * @description In particular stock actions comprise a series of calculations
     * @memberof OpenIZ
     * @static
     * @class
     */
    Stock: {
        /**
         * @summary TODO:
         * @method
         * @memberof OpenIZ.Stock
         * @param {int} qYear TODO
         * @param {int} pSupply TODO
         * @return TODO
         */
        calculateQPeriod: function (qYear, pSupply) {
            return qYear / 12 * pSupply;
        },
        /**
         * @summary TODO:
         * @method
         * @memberof OpenIZ.Stock
         * @return TODO
         */
        calculateSReserve: function (qPeriod, reservePercent) {
            return qPeriod * reservePercent;
        },
        /**
         * @summary TODO:
         * @method
         * @memberof OpenIZ.Stock
         * @return TODO
         */
        calculateSMax: function (qPeriod, sReserve) {
            return qPeriod + sReserve;
        },
        /**
         * @summary TODO:
         * @method
         * @memberof OpenIZ.Stock
         * @return TODO
         */
        calculateSReorder: function (sReserve, qPeriod, lTime, pSupply) {
            return sReserve + qPeriod * lTime / pSupply;
        },
        /**
         * @summary TODO:
         * @method
         * @memberof OpenIZ.Stock
         * @return TODO
         */
        calculateQOrder: function (sMax, sAvailable, qPeriod, lTime, pSupply) {
            return sMax - sAvailable + qPeriod * lTime / pSupply;
        },
        /**
         * @summary TODO:
         * @method
         * @memberof OpenIZ.Stock
         * @return TODO
         */
        calculateQNeeded: function (sStart, qRecieved, sEnd, sLost) {
            return (sStart + qRecieved) - (sEnd + sLost);
        },
        /**
         * @summary TODO:
         * @method
         * @memberof OpenIZ.Stock
         * @return TODO
         */
        calculateAll: function (qYear, pSupply, reservePercent, lTime, sAvailable) {
            var object = {};
            object.qPeriod = OpenIZ.Stock.calculateQPeriod(qYear, pSupply);
            object.sReserve = OpenIZ.Stock.calculateSReserve(object.qPeriod, reservePercent);
            object.sMax = OpenIZ.Stock.calculateSMax(object.qPeriod, object.sReserve);
            object.sReorder = OpenIZ.Stock.calculateSReorder(object.sReserve, object.qPeriod, lTime, pSupply);
            object.qOrder = OpenIZ.Stock.calculateQOrder(object.sMax, sAvailable, object.qPeriod, lTime, pSupply);
            return object;
        }
    },
    /**
     * @summary Utility functions which assist in execution and listing of reports
     * @static
     * @class
     * @memberof OpenIZ
     */
    Risi: {
        /**
         * @method
         * @summary Search the RISI for reports matching the specified query
         * @param {object} controlData The control data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.name The Query from which to use 
         */
        getReportsAsync: function (controlData) {
            OpenIZ.Util.simpleGet("/__risi/report", {
                query: { _name: controlData.name },
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally
            });
        },
        /**
         * @method
         * @summary Execute a report on the RISI 
         * @param {object} controlData The control data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.name The name of the report to run
         * @param {object} controlData.view The view of the report to fetch
         * @param {object} controlData.query The view of the report to fetch
         */
        executeDatasetAsync: function (controlData) {
            var query = controlData.query || {};
            query._name = controlData.name;
            query._report = controlData.report;

            // Execute the report
            OpenIZ.Util.simpleGet("/__risi/data", {
                query: query,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
        /**
         * @method
         * @summary Execute a report on the RISI 
         * @param {object} controlData The control data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.name The name of the report to run
         * @param {object} controlData.view The view of the report to fetch
         * @param {object} controlData.query The view of the report to fetch
         */
        executeReportAsync: function (controlData) {
            var query = {};
            query._name = controlData.name;
            query._view = controlData.view;

            if (controlData.query)
                for (var k in Object.keys(controlData.query)) {
                    var key = Object.keys(controlData.query)[k];
                    if (controlData.query[key] && controlData.query[key].toISOString)
                        query[key] = controlData.query[key].toISOString();
                    else
                        query[key] = controlData.query[key];
                }

            // Execute the report
            OpenIZ.Util.simpleGet("/__risi/report.htm", {
                query: query,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            }, true);
        }
    },
    /**
     * @summary Utility functions which assist in the writing of other OpenIZ functions
     * @static
     * @class
     * @memberof OpenIZ
     */
    Util: {
        
        /**
         * @summary Perform a simple post of JSON data to the backend
         * @method
         * @memberof OpenIZ.Util
         * @param {object} controlData The control data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} url The URL from which to post to 
         * @param {object} controlData.data The query to be posted as JSON
         */
        simplePost: function (url, controlData) {
            controlData.onException = controlData.onException || OpenIZ.Util.logException;

            $.ajax({
                method: 'POST',
                url: url,
                data: JSON.stringify(controlData.data),
                dataType: "json",
                contentType: 'application/json',
                success: function (xhr, data) {

                    if (controlData.continueWith !== undefined)
                        controlData.continueWith(xhr, controlData.state);

                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);
                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === null)
                        console.error(error);
                    else if (error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                error.caused_by
                            ), controlData.state);

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ), controlData.state);

                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);
                }
            });
        },
        /**
         * @summary Perform a simple PUT of JSON data to the backend
         * @method
         * @memberof OpenIZ.Util
         * @param {object} controlData The control data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} url The URL from which to post to 
         * @param {object} controlData.query The query to be sent on the put
         * @param {object} controlData.data The query to be posted as JSON
         */
        simplePut: function (url, controlData) {
            controlData.onException = controlData.onException || OpenIZ.Util.logException;

            $.ajax({
                method: 'PUT',
                url: controlData == null || controlData.query == null ? url : url + "?" + controlData.query,
                data: JSON.stringify(controlData.data),
                dataType: "json",
                contentType: 'application/json',
                success: function (xhr, data) {

                    if (controlData.continueWith !== undefined)
                        controlData.continueWith(xhr, controlData.state);

                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);
                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === null)
                        console.error(error);
                    else if (error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                error.caused_by
                            ), controlData.state);

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ), controlData.state);

                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);
                }
            });
        },
        /**
         * @summary Perform a simple delete of JSON data to the backend
         * @method
         * @memberof OpenIZ.Util
         * @param {object} controlData The control data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} url The URL from which to post to 
         * @param {object} controlData.query The query to be sent on the DELETE
         */
        simpleDelete: function (url, controlData) {
            controlData.onException = controlData.onException || OpenIZ.Util.logException;

            $.ajax({
                method: 'DELETE',
                url: controlData == null || controlData.query == null ? url : url + "?" + controlData.query,
                dataType: "json",
                contentType: 'application/json',
                success: function (xhr, data) {
                    try {
                        if (controlData.continueWith !== undefined)
                            controlData.continueWith(xhr, controlData.state);

                    }
                    catch (e) {
                        if (controlData.onException !== undefined)
                            controlData.onException(e, controlData.state);
                    }
                    finally {
                        if (controlData.finally !== undefined)
                            controlData.finally(controlData.state);
                    }

                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === null)
                        console.error(error);
                    else if (error !== undefined && error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                error.caused_by
                            ), controlData.state);

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ), controlData.state);

                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);
                }
            });
        },
        /**
         * @summary Perform a simple get not necessarily against the IMS
         * @method
         * @memberof OpenIZ.Util   
         * @param {string} url The URL from which to retrieve 
         * @param {Object} controlData The control data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The query to be included
         */
        simpleGet: function (url, controlData, useRaw) {
            controlData.onException = controlData.onException || OpenIZ.Util.logException;
            if (!useRaw)
                $.getJSON(url, controlData.query, function (data) {

                    try {
                        if (data != null && data.error !== undefined)
                            controlData.onException(new OpenIZModel.Exception(data.type, data.error, null, null), controlData.state
                            );
                        else if (data != null) {
                            controlData.continueWith(data, controlData.state);
                        }
                        else
                            controlData.onException(new OpenIZModel.Exception("Exception", "err_general",
                                data,
                                   null
                            ), controlData.state);

                    }
                    catch (e) {
                        if (controlData.onException !== undefined)
                            controlData.onException(e, controlData.state);
                    }
                    finally {
                        if (controlData.finally !== undefined)
                            controlData.finally(controlData.state);
                    }

                }).error(function (data) {
                    var error = data.responseJSON;
                    if (error != null && error.error !== undefined) //  error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                error.caused_by
                            ), controlData.state);

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ), controlData.state);
                }).always(function () {
                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);
                });
            else
                $.get(url, controlData.query, function (data) {

                    try {
                        if (data != null && data.error !== undefined)
                            controlData.onException(new OpenIZModel.Exception(data.type, data.error, null, null), controlData.state
                            );
                        else if (data != null) {
                            controlData.continueWith(data, controlData.state);
                        }
                        else
                            controlData.onException(new OpenIZModel.Exception("Exception", "err_general",
                                data, null
                            ), controlData.state);

                    }
                    catch (e) {
                        if (controlData.onException !== undefined)
                            controlData.onException(e, controlData.state);
                    }
                    finally {
                        if (controlData.finally !== undefined)
                            controlData.finally(controlData.state);
                    }

                }).error(function (data) {
                    var error = data.responseJSON;
                    if (error != null && error.error !== undefined) //  error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description, error.caused_by
                            ), controlData.state);

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data
                            ), controlData.state);
                }).always(function () {
                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);
                });
        },
        
        /**
         * @summary Log an exception to the console
         * @method
         * @memberof OpenIZ.Util
         * @param {OpenIZModel.Exception} e The exception to be logged to the console
         */
        logException: function (e) {
            console.warn(e);
        },
        /** 
         * @summary Renders the specified concept name from a DOM option
         * @memberof OpenIZ.Util
         * @method
         * @param {OpenIZModel.ConceptName} name The concept name to be rendered
         */
        renderConceptFromDom: function (val) {
            if (val)
                return $("option[value=" + val + "]").first().text();
        },
        /** 
         * @summary Renders the specified concept name
         * @memberof OpenIZ.Util
         * @method
         * @param {OpenIZModel.ConceptName} name The concept name to be rendered
         */
        renderConceptName: function (name) {
            var retVal = "";
            if (name == null)
                retVal = "";
            else if (typeof (name) == "String") retVal = name;
            else if (name[OpenIZ.Localization.getLocale()] != null)
                retVal = name[OpenIZ.Localization.getLocale()];
            else
                retVal = name[Object.keys(name)];

            if (Array.isArray(retVal))
                return retVal[0];
            else
                return retVal;
        },
        /**
         * @summary Render address for display
         * @method
         * @memberof OpenIZ.Util
         * @param {OpenIZModel.EntityAddress} entity The addres of the entity or the entity itself to render the address of
         * @return {string} The address formatted as an appropriate string for simple formatting
         */
        renderAddress: function (entity) {
            if (entity === undefined) return;

            var address = entity.component !== undefined ? entity :
                entity.address !== undefined ? (entity.address.Direct || entity.address.HomeAddress || result.name.$other) :
                (entity.Direct || entity.HomeAddress || entity.$other);
            var retVal = "";
            if (address.component) {
                if (address.component.AdditionalLocator)
                    retVal += address.component.AdditionalLocator + ", ";
                if (address.component.StreetAddressLine)
                    retVal += address.component.StreetAddressLine + ", ";
                if (address.component.Precinct)
                    retVal += address.component.Precinct + ", ";
                if (address.component.City)
                    retVal += address.component.City + ", ";
                if (address.component.County != null)
                    retVal += address.component.County + ", ";
                if (address.component.State != null)
                    retVal += address.component.State + ", ";
                if (address.component.Country != null)
                    retVal += address.component.Country + ", ";
            }
            return retVal.substring(0, retVal.length - 2);
        },
        /**
         * @summary Render act as a simple string
         * @memberof OpenIZ.Util
         * @method
         * @param {OpenIZModel.Act} act The act to render as a simple string
         * @return {string} The rendered string 
         */
        renderAct: function (act) {
            switch (act.$type) {
                case "SubstanceAdministration":
                    return OpenIZ.Localization.getString("locale.encounters.administer") +
                        OpenIZ.Util.renderName(act.participation.Product.name.OfficialRecord);
                case "QuantityObservation":
                case "CodedObservation":
                case "TextObservation":
                    return OpenIZ.Localization.getString('locale.encounters.observe') +
                        act.typeConceptModel.name[OpenIZ.Localization.getLocale()];
                default:
                    return "";
            }
        },
        /** 
         * @summary Render a manufactured material as a simple string
         * @method
         * @memberof OpenIZ.Util
         * @param {OpenIZModel.ManufacturedMaterial} material The material which is to be rendered as a string
         * @return {string} The material rendered as a string in format "<<name>> [LN# <<ln>>]"
         */
        renderManufacturedMaterial: function (material) {
            var name = OpenIZ.Util.renderName(material.name.OfficialRecord || material.name.Assigned);
            return name + "[LN#: " + material.lotNumber + "]";
        },
        /** 
         * @summary Renders a name as a simple string
         * @method
         * @meberof OpenIZ.Util
         * @param {OpenIZModel.EntityName} entityName The entity name to be rendered in the appropriate format
         * @return {string} The rendered entity name
         */
        renderName: function (entityName) {
            if (entityName === null || entityName === undefined)
                return "";
            else if (entityName.join !== undefined)
                return entityName.join(' ');
            else if (entityName.component !== undefined) {
                var nameStr = "";
                if (entityName.component.Given !== undefined) {
                    if (typeof (entityName.component.Given) === "string")
                        nameStr += entityName.component.Given;
                    else if (entityName.component.Given.join !== undefined)
                        nameStr += entityName.component.Given.join(' ');
                    nameStr += " ";
                }
                if (entityName.component.Family !== undefined) {
                    if (typeof (entityName.component.Family) === "string")
                        nameStr += entityName.component.Family;
                    else if (entityName.component.Family.join !== undefined)
                        nameStr += entityName.component.Family.join(' ');
                }
                if (entityName.component.$other !== undefined) {
                    if (typeof (entityName.component.$other) === "string")
                        nameStr += entityName.component.$other;
                    else if (entityName.component.$other.join !== undefined)
                        nameStr += entityName.component.$other.join(' ');
                    else if (entityName.component.$other.value !== undefined)
                        nameStr += entityName.component.$other.value;

                }
                return nameStr;
            }
            else if (typeof (entityName) === "string")
                return entityName;
            else
                return "";
        },
        /**
         * @summary Changes the specified date string into an appropriate ISO string
         * @memberof OpenIZ.Util
         * @method
         * @param {Date} date The date to be formatted
         * @return {string} A DATE as an ISO String only
         */
        toDateInputString: function (date) {
            if (!date) return null; // null
            else if (date.getYear) { // javascript date
                var yr = (1900 + date.getYear()).toString(),
                    mo = (1 + date.getMonth()).toString(),
                    da = date.getDate().toString();
                var pad = "00";
                mo = pad.substring(0, 2 - mo.length) + mo;
                da = pad.substring(0, 2 - da.length) + da;
                return yr + "-" + mo + "-" + da;
            }
            else // other date lib
                return date.toISOString();
        },
        /**
         * @summary Start a task asynchronously
         * @memberof OpenIZ.Util
         * @method
         * @param {Function} syncFn The synchronous function to be executed
         * @param {Object} control Control data for the async method
         * @param {Function} control.continueWith A callback method to be called when the operation completes successfully
         * @param {Function} control.onException A callback method to be called when the operation throws an exception
         * @example
         * OpenIZ.Util.startTaskAsync(function() {
         *      return doSomeSynchronousWork();
         * }, {
         *      continueWith: function(result) { console.info("Result of doSomeSynchronousWork() = " + result); },
         *      onException: function(ex) { console.error("doSomeSynchronousWork threw exception " + ex); }
         * });
         * @deprecated
         */
        startTaskAsync: function (syncFn, controlData) {
            return setTimeout(function () {
                try {
                    controlData.continueWith(syncFn(), controlData.state);
                }
                catch (ex) {
                    if (controlData.onException === undefined)
                        console.error(ex);
                    else
                        controlData.onException(ex, controlData.state);
                }
            }, 0);
        }
    },

    /** 
    * @summary The authentication section is used to interface with OpenIZ's authentication sub-systems including session management information, etc.
    * @see OpenIZModel.SecurityUser
     * @static
     * @class
    * @memberof OpenIZ
    */
    Authentication: {
        /** 
         * @summary Cached session
         */
        $session: null,
        /**
         * @summary Credentials to use for elevation in lieu of the current session
         */
        $elevationCredentials: {},
        /** 
         * @summary HAndler for when the session is expired
         */
        $sessionExpiredHandler: function () {
            window.location.reload();
        },
        /** 
         * @summary Send a TFA secret
         * @method
         * @memberof OpenIZ.Authentication
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         */
        sendTfaSecretAsync: function (controlData) {
            OpenIZ.Util.simplePost("/__auth/tfa", controlData);
        },
        /** 
         * @summary Get the TFA mechanisms
         * @method
         * @memberof OpenIZ.Authentication
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         */
        getTfaMechanisms: function (controlData) {
            OpenIZ.Util.simpleGet("/__auth/tfa", controlData);
        },
        /**
         * @summary Show an elevation dialog
         * @method
         * @memberof OpenIZ.Authentication
         */
        showElevationDialog: function () {
            $("#authenticationDialog").modal('show');
        },
        /** 
         * @summary Hide the elevation dialog
         * @method
         * @memberof OpenIZ.Authentication
         */
        hideElevationDialog: function () {
            OpenIZ.App.hideWait('#loginButton');
            $("#authenticationDialog").modal('hide');
        },
        /**
         * @summary Perform a login operation asynchronously
         * @memberof OpenIZ.Authentication
         * @method
         * @see OpenIZ.Util.simpleGet
         * @param {string} controlData.userName The username to use when authenticating
         * @param {string} controlData.password The password of the user to authenticate with
         * @param {string} controlData.scope The scope to append to the OAUTH request
         * @param {string} controlData.tfaSecret The scope to append to the OAUTH request
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @example
         * OpenIZ.Authentication.loginAsync({
         *      userName: "bob",
         *      password: "Mohawk1",
         *      continueWith: function(result) { // do something here },
         *      onException: function(ex) { // handle exception }
         * });
         */
        loginAsync: function (controlData) {
            // Perform auth request
            $.ajax(
             {
                 method: 'POST',
                 url: '/__auth/authenticate',
                 data: {
                     username: controlData.userName,
                     password: controlData.password,
                     grant_type: controlData.tfaSecret != null ? 'tfa' : 'password',
                     tfaSecret: controlData.tfaSecret,
                     scope: controlData.scope
                 },
                 dataType: "json",
                 contentType: 'application/x-www-urlform-encoded',
                 success: function (data, status) {
                     if (data != null && data.error !== undefined)
                         controlData.onException(new OpenIZModel.Exception(data.type, data.error, null), controlData.state
                         );
                     else if (data != null) {
                         if (!controlData.scope || controlData.scope == "")
                             OpenIZ.Authentication.$session = data;
                         controlData.continueWith(data, controlData.state);
                     }
                     else
                         controlData.onException(new OpenIZModel.Exception("Exception", "err_general",
                             data
                         ), controlData.state);

                     if (controlData.finally !== undefined)
                         controlData.finally(controlData.state);
                 },
                 error: function (data) {
                     var error = data.responseJSON;
                     if (error != null && error.error !== undefined) // oauth 2 error
                         controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                 error.error_description, error.caused_by
                             ), controlData.state);

                     else // unknown error
                         controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                 data
                             ), controlData.state);
                     if (controlData.finally !== undefined)
                         controlData.finally(controlData.state);

                 }
             });
        },
        /**
         * @summary Set password for the specified user asynchronously
         * @memberof OpenIZ.Authentication
         * @method
         * @param {object} controlData The data which controls the asynchronous process
         * @param {string} controlData.userName The name of the user to set the password of
         * @param {string} controlData.password The password to set
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @example
         *  OpenIZ.Authentication.setPasswordAsync({
         *      userName : "bob",
         *      password : "Mohawk321",
         *      continueWith : function(d) { // TODO: Do something with result },
         *      onException: function(ex) { // handle exception }
         * });
         */
        setPasswordAsync: function (controlData) {
            $.ajax(
            {
                method: 'POST',
                url: '/__auth/passwd',
                data: {
                    username: controlData.userName,
                    password: controlData.password
                },
                dataType: "json",
                contentType: 'application/x-www-urlform-encoded',
                success: function (data, status) {
                    if (data != null && data.error !== undefined)
                        controlData.onException(new OpenIZModel.Exception(data.type, data.error, null), controlData.state
                        );
                    else if (data != null)
                        controlData.continueWith(data, controlData.state);
                    else
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general",
                            data
                        ));

                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);
                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (error != null && error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description, data.caused_by
                            ), controlData.state);

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data
                            ), controlData.state);
                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);

                }
            });
        },
        /**
        * @summary Gets the current session 
        * @memberof OpenIZ.Authentication
        * @method
        * @param {object} controlData The data which controls the asynchronous process
        * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
        * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
        * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
        * @returns {OpenIZModel.SessionInfo} An instance of Session representing the current session
        */
        getSessionAsync: function (controlData) {
            OpenIZ.Util.simpleGet('/__auth/get_session', {
                continueWith: function (data) {
                    OpenIZ.Authentication.$session = data;
                    if (controlData.continueWith) controlData.continueWith(data);
                },
                finally: controlData.finally,
                onException: controlData.onException,
                state: controlData.state
            });
        },
        /**
         * @summary Destroys the current session
         * @memberof OpenIZ.Authentication
         * @method
         * @memberof OpenIZ.Authentication
         * @return {bool} Whether the session was successfully abandoned
         */
        abandonSession: function (controlData) {
            $.ajax(
            {
                method: "POST",
                url: "/__auth/abandon",
                dataType: "json",
                contentType: 'application/x-www-urlform-encoded',
                success: function (data, status) {

                    OpenIZ.Authentication.$session = null;

                    controlData.continueWith(data, controlData.state);

                    if (controlData.finally !== undefined) {
                        controlData.finally(controlData.state);
                    }
                },
                error: function (data) {
                    var error = data.responseJSON;

                    if (error != null && error.error !== undefined) {
                        // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error, error.error_description, null), controlData.state);
                    }
                    else {
                        // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error, data, null), controlData.state);
                    }

                    if (controlData.finally !== undefined) {
                        controlData.finally(controlData.state);
                    }
                }
            });
        },
        /** 
         * @summary Refreshes the current session asynchronously
         * @memberof OpenIZ.Authentication
         * @method
         * @param {Object} controlData Task control data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @example
         * OpenIZ.Authentication.refreshSessionAsync({
         *      continueWith: function(r) { // do something with result },
         *      onException: function(ex) { // handle exception }
         * });
         */
        refreshSessionAsync: function (controlData) {
            // Perform auth request
            $.ajax(
             {
                 method: 'POST',
                 url: '/__auth/authenticate',
                 data: {
                     grant_type: 'refresh'
                 },
                 dataType: "json",
                 contentType: 'application/x-www-urlform-encoded',
                 success: function (data, status) {
                     if (data != null && data.error !== undefined)
                         controlData.onException(new OpenIZModel.Exception(data.type, data.error, null), controlData.state
                         );
                     else if (data != null) {
                         OpenIZ.Authentication.$session = data;

                         controlData.continueWith(data, controlData.state);
                         OpenIZ.Authentication.$session = data;
                     }
                     else
                         controlData.onException(new OpenIZModel.Exception("Exception", "err_general",
                             data
                         ), controlData.state);

                     if (controlData.finally !== undefined)
                         controlData.finally(controlData.state);
                 },
                 error: function (data) {
                     var error = data.responseJSON;
                     if (error != null && error.error !== undefined) // oauth 2 error
                         controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                 error.error_description, error.caused_by
                             ), controlData.state);

                     else // unknown error
                         controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                 data
                             ), controlData.state);
                     if (controlData.finally !== undefined)
                         controlData.finally(controlData.state);

                 }
             });
        },
        /**
        * @summary Performs a query against the UserEntity
        * @memberof OpenIZ.Authentication
        * @param {Object} controlData Task control data
        * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
        * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
        * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
        * @param {object} controlData.query The query object which represents the filters for the object
        * @param {int} controlData.query._count The limit of results to return from the ims
        * @param {int} controlData.query._offset The offset of the search result window
        * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
        * @method
        * @see OpenIZ.Ims.get
        */
        getUserAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "UserEntity",
                query: controlData.query,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
        /**
        * @summary Performs a query against the UserEntity
        * @memberof OpenIZ.Authentication
        * @param {Object} controlData Task control data
        * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
        * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
        * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
        * @param {OpenIZModel.SecurityUser} controlData.data The query object which represents the filters for the object
        * @method
        * @see OpenIZ.Ims.get
        */
        saveUserAsync: function (controlData) {
            OpenIZ.Util.simplePost("/__auth/SecurityUser", {
                resource: "SecurityUser",
                data: controlData.data,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
    },
    /** 
     * @summary Represents functions for interacting with the protocol service
     * @see OpenIZModel.Patient
     * @see OpenIZModel.Act
     * @static
     * @class
     * @memberof OpenIZ
     */
    CarePlan: {
        /**
         * @summary Refreshes the cached care plan
         * @memberof OpenIZ.CarePlan
         * @method
         * @param {object} controlData The data which controls the asynchronous operation
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {OpenIZModel.Patient} controlData.data The seed data which should be passed to the forecasting engine in order to calculate the plan
         * @param {string} controlData.query The additional query parameters which should be passed to the forecaster
         */
        refreshAsync: function (controlData) {
            OpenIZ.Util.simplePost("/__plan/refresh", controlData);
        },
        /**
         * @summary Interprets the observation, setting the interpretationConcept property of the observation
         * @param {OpenIZModel.QuantityObservation} obs The observation which is to be interpretation
         * @param {string} ruleSet The rule set to be applied for the clinical decision
         * @memberof OpenIZ.CarePlan
         * @method
         */
        interpretObservation: function (obs, patient, ruleSet) {
            if (obs.value === undefined || obs.value === null) return;
            obs.participation = obs.participation || {};
            obs.participation.RecordTarget = obs.participation.RecordTarget || {};
            obs.participation.RecordTarget.playerModel = patient;

            var postVal = OpenIZBre.ExecuteRule("BeforeInsert", obs);
            obs.interpretationConcept = postVal.interpretationConcept;

            obs.participation.RecordTarget.playerModel = null;
            return postVal.interpretationConcept;
        },
        /**
         * @summary Generate a care plan for the specified patient
         * @memberof OpenIZ.CarePlan
         * @method
         * @param {object} controlData The data which controls the asynchronous operation
         * @param {date} controlData.minDate If the care plan result is to be filtered, then the lower bound of the care plan to retrieve
         * @param {date} controlData.maxDate If the care plan result is to be filtered on an upper bound then the care plan to retrieve
         * @param {date} controlData.onDate Specifies the care plan service only return those objects where the proposed action should occur on the specified date
         * @param {uuid} controlData.classConcept Specifies the classification of acts which should be returned
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {OpenIZModel.Patient} controlData.data The seed data which should be passed to the forecasting engine in order to calculate the plan
         * @param {string} controlData.query The additional query parameters which should be passed to the forecaster
         * @example
         * OpenIZ.CarePlan.getCarePlanAsync({
         *     maxDate: new Date(), // Only retrieve objects that should have already occurred
         *     classConcept: "932A3C7E-AD77-450A-8A1F-030FC2855450", // Only retrieve substance administrations
         *     data: { // Manually construct a patient to pass
         *          dateOfBirth: new Date(), // The patient was born today
         *          genderConceptModel: { mnemonic: "Male" }, // the patient is a male
         *      },
         *      continueWith: function(careplan) {
         *          alert("There are " + careplan.participation.RecordTarget.length + " proposed objects");
         *      },
         *      onException: function(ex) {
         *          alert(ex.message);
         *      }
         * });
         */
        getCarePlanAsync: function (controlData) {
            var url = "/__plan/patient?moodConcept=ACF7BAF2-221F-4BC2-8116-CEB5165BE079";
            if (controlData.minDate !== undefined)
                url += "&actTime=>" + controlData.minDate.toISOString();
            if (controlData.maxDate !== undefined)
                url += "&actTime=<" + controlData.maxDate.toISOString();
            if (controlData.onDate !== undefined)
                url += "&startTime=<" + controlData.onDate.toISOString();
            if (controlData.classConcept !== undefined)
                url += "&classConcept=" + controlData.classConcept;
            if (controlData.query !== undefined)
                url += "&" + controlData.query;
            console.info("Generating care plan...");
            $.ajax({
                method: 'POST',
                url: url,
                data: JSON.stringify(controlData.data),
                dataType: "json",
                contentType: 'application/json',
                success: function (xhr, data) {
                    console.info("Retrieved care plan...");
                    // console.info(JSON.stringify(xhr));
                    controlData.continueWith(xhr, controlData.state);
                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);
                },
                error: function (data) {
                    var error = data.responseJSON;

                    if (controlData.onException !== undefined) {
                        if (error.error !== undefined) // error
                        {
                            controlData.onException(new OpenIZModel.Exception(error.type, error.error, error.error_description, error.caused_by), controlData.state);
                        }
                        else {
                            // unknown error
                            controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error, data), controlData.state);
                        }
                    }
                    if (controlData.finally !== undefined) {
                        controlData.finally(controlData.state);
                    }
                }
            });
        },
        /**
         * @summary Get an empty entity template object asynchronously. See the OpenIZ documentation for more information about templates
         * @method
         * @memberof OpenIZ.CarePlan
         * @param {object} controlData The data which controls the asynchronous operation.
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.templateId The identifier of the template which to retrieve. Templates should be registered with the application manifest
         * @example
         * OpenIZ.CarePlan.getEntityTemplateAsync({
         *      templateId: "Entity.Patient.Baby", 
         *      continueWith: function(template) {
         *          $scope.act = template;
         *      }
         * });
         * @deprecated
         */
        getEntityTemplateAsync: function (controlData) {
            OpenIZ.Entity.getEntityTemplateAsync(controlData);
        },
        /**
         * @summary Get an empty act template object asynchronously. See the OpenIZ documentation for more information about templates
         * @method
         * @memberof OpenIZ.CarePlan
         * @param {object} controlData The data which controls the asynchronous operation.
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.templateId The identifier of the template which to retrieve. Templates should be registered with the application manifest
         * @example
         * OpenIZ.CarePlan.getActTemplateAsync({
         *      templateId: "Act.SubstanceAdmin.Immunization", 
         *      continueWith: function(template) {
         *          $scope.act = template;
         *      }
         * });
         * @deprecated
         */
        getActTemplateAsync: function (controlData) {
            OpenIZ.Act.getActTemplateAsync(controlData);
        }
    },
    /**
    * @summary Represents application specific functions for interoperating with the mobile application itself
     * @static
     * @class
    * @memberof OpenIZ
    */
    App: {
        // OpenIZ.Core.Model.Constants.DatePrecisionFormats, OpenIZ.Core.Model, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null
        /**
         * @enum {String}
         * @memberof OpenIZModel
         * @public
         * @readonly
         * @summary Date formats for using date precision
         */
        DatePrecisionFormats: {
            DateFormatYear: 'YYYY',
            DateFormatMonth: 'YYYY-MM',
            DateFormatDay: 'YYYY-MM-DD',
            DateFormatHour: 'YYYY-MM-DD HH',
            DateFormatMinute: 'YYYY-MM-DD HH:mm',
            DateFormatSecond: 'YYYY-MM-DD HH:mm:ss'
        },  // Date Precision Formats
        _originalText: {},
        /**
         * @summary Fetches active tickles from the service
         * @memberof OpenIZ.App
         * @method
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {String} controlData.appId The application identifier to be updated
         */
        getTicklesAsync: function (controlData) {
            OpenIZ.Util.simpleGet("/__app/tickle", {
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally
            });
        },
        /**
         * @summary Instructs the update service to perform the update of the software
         * @memberof OpenIZ.App
         * @method
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {String} controlData.appId The application identifier to be updated
         */
        doUpdateAsync: function (controlData) {
            OpenIZ.Util.simplePost("/__app/update", {
                continueWith: controlData.continueWith,
                data: { appId: controlData.appId },
                onException: controlData.onException,
                finally: controlData.finally
            });
        },
        /**
         * @summary Instructs the mini ims to compact all databases related to the OpenIZ data structures
         * @method
         * @memberof OpenIZ.App
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {bool} controlData.backup Whether a backup should be taken
         */
        compactAsync: function (controlData) {
            OpenIZ.Util.simplePost("/__app/data", {
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally
            });
        },
        /**
         * @summary Purges all data from the application
         * @method
         * @memberof OpenIZ.App
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {bool} controlData.backup Whether a backup should be taken
         */
        purgeDataAsync: function (controlData) {
            OpenIZ.Util.simpleDelete("/__app/data", {
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                query: "backup=" + controlData.backup
            });
        },
        /**
         * @summary Purges all data from the application
         * @method
         * @memberof OpenIZ.App
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {bool} controlData.backup Whether a backup should be taken
         */
        restoreDataAsync: function (controlData) {
            OpenIZ.Util.simplePost("/__app/data/restore", {
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally
            });
        },
        /**
         * @summary Backs up all data from the application
         * @method
         * @memberof OpenIZ.App
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         */
        backupDataAsync: function (controlData) {
            OpenIZ.Util.simplePost("/__app/data/backup", {
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally
            });
        },
        /**
         * @summary Indicates whether the database has a backup
         * @method
         * @memberof OpenIZ.App
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         */
        getBackupAsync: function (controlData) {
            OpenIZ.Util.simpleGet("/__app/data/backup", {
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally
            });
        },
        /**
         * @summary Loads an asset synchronously from the data/ directory
         * @method
         * @memberof OpenIZ.App
         */
        loadDataAsset: function (dataId) {
            return atob(OpenIZApplicationService.GetDataAsset(dataId));
        },
        /**
         * @summary Submits a bug report asynchronously
         * @method
         * @memberof OpenIZ.App
         * @param {object} controlData The data which controls the asynchronous operation.
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {OpenIZData.BugReport} controlData.data The bug report to submit
         */
        submitBugReportAsync: function (controlData) {
            OpenIZ.Util.simplePost('/__app/bug', controlData);
        },
        /**
         * @summary Gets log information from the IMS service
         * @method
         * @memberof OpenIZ.App
         * @param {object} controlData The data which controls the asynchronous operation.
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The query or filter apply
         * @param {string} controlData.query._id The identifier of the file to retrieve
         */
        getLogInfoAsync: function (controlData) {
            return OpenIZ.Util.simpleGet('/__app/log', controlData);
        },
        /**
         * @description Because JavaScript lacks native UUID generation, this function calls a JNI method to generate a new UUID which can be appended to IMS objects
         * @return {uuid} A newly generated uuid
         * @method
         * @memberof OpenIZ.App
         */
        newGuid: function () {
            return OpenIZApplicationService.NewGuid();
        },
        /**
         * @summary Get application information data using typical async information parameters
         * @method
         * @memberof OpenIZ.App
         * @param {object} controlData The data which controls the asynchronous operation.
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @see OpenIZModel.ApplicationInfo
         */
        getInfoAsync: function (controlData) {
            if (controlData.includeUpdates)
                OpenIZ.Util.simpleGet('/__app/info.max', controlData);
            else
                OpenIZ.Util.simpleGet('/__app/info', controlData);
        },
        /**
        * @summary Get health information data using typical async information parameters
        * @method
        * @memberof OpenIZ.App
        * @param {object} controlData The data which controls the asynchronous operation.
        * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
        * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
        * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
        */
        getHealthAsync: function (controlData) {
            OpenIZ.Util.simpleGet('/__app/health', controlData);
        },
        /**
         * @summary Get the online state of the application
         * @method
         * @memberof OpenIZ.App
         * @return {bool} Indicator whether the application is online.
         * @deprecated
         */
        getOnlineState: function () {
            return OpenIZApplicationService.GetOnlineState();
        },
        /**
         * @summary Get the online state of the application
         * @method
         * @memberof OpenIZ.App
         * @return {bool} Indicator whether the application is online.
         * @deprecated
         */
        isAdminAvailable: function () {
            return OpenIZApplicationService.IsAdminAvailable();
        },
        /**
         * @summary Get the online state of the application
         * @method
         * @memberof OpenIZ.App
         * @return {bool} Indicator whether the application is online.
         * @deprecated
         */
        isClinicalAvailable: function () {
            return OpenIZApplicationService.IsClinicalAvailable();
        },
        /**
         * @summary Indicates whether the status dialog is shown
         * @memberof OpenIZ.App
         */
        statusShown: false,
        /**
         * @summary Resolves the specified template
         * @param {string} templateId The identifier of the template to resolve
         * @return {string} The URL of the form template which is to be used to capture data for the specified template
         */
        resolveTemplateForm: function (templateId) {
            if (templateId)
                return OpenIZApplicationService.GetTemplateForm(templateId);
        },
        /**
        * @summary Resolves the specified template for view
        * @param {string} templateId The identifier of the template to resolve
        * @return {string} The URL of the form template which is to be used to display data for the specified template
        */
        resolveTemplateView: function (templateId) {
            if (templateId)
                return OpenIZApplicationService.GetTemplateView(templateId);
        },
        /**
        * @summary Resolves the specified template for view
        * @param {string} templateId The identifier of the template to resolve
        * @return {Array} A dictionary object where the key is the templateID and the value is the display name
        */
        getTemplateDefinitions: function () {
            if (OpenIZApplicationService.GetTemplates)
                return JSON.parse(OpenIZApplicationService.GetTemplates());
            else {
                return [
                    'act.substanceadmin.immunization',
                    'act.observation.weight',
                    'act.substanceadmin.supplement',
                    'act.concern.aefi',
                    'act.problem',
                    'act.observation.causeofdeath'
                ];
            }
        },
        /**
         * @summary Update the status of the status dialog
         * @private
         * @method
         * @memberof OpenIZ.App
         */
        updateStatus: function () {

            var p = JSON.parse(OpenIZApplicationService.GetStatus());

            if (p[1] > 0 && p[1] < 1) {
                $("#waitModalText").text(OpenIZ.Localization.getString("locale.dialog.wait.background") + ":" + p[0]);
                $("#waitProgressBar").attr('style', 'width:' + (p[1] * 100) + "%");
            }
            else {
                $("#waitModalText").text(OpenIZ.Localization.getString("locale.dialog.wait.text") + ":" + p[0]);
                $("#waitProgressBar").attr('style', 'width:100%');
            }

            if (OpenIZ.App.statusShown)
                setTimeout(OpenIZ.App.updateStatus, 1000);

        },
        /**
         * @summary Show the wait loader
         * @param {String} textStr The text on the alert panel to show
         * @method
         * @memberof OpenIZ.App
         */
        showWait: function (controlItem) {
            OpenIZ.App._originalText[controlItem] = $(controlItem).html();
            $(controlItem).attr('disabled', 'disabled');
            $(controlItem).html("<img src='/org.openiz.core/img/ajax-loader.gif' class='spinloader'> " + OpenIZ.Localization.getString("locale.dialog.wait.text"));
        },
        /**
         * @summary Returns whether the internet is available
         * @method
         * @memberof OpenIZ.App
         * @return {bool} An indicator whether the application environment has access to the internet
         */
        networkAvailable: function () {
            try { return OpenIZApplicationService.GetNetworkState(); }
            catch (e) { return false; }
        },
        /**
         * @summary Hide the waiting panel
         * @method
         * @memberof OpenIZ.App
         */
        hideWait: function (controlItem) {
            $(controlItem).removeAttr('disabled');
            $(controlItem).html(OpenIZ.App._originalText[controlItem]);

        },
        /**
         * @summary Gets the specified service implementation in memory
         * @method
         * @memberof OpenIZ.App
         * @param {String} serviceName The name of the service 
         * @return {String} The service class which implements the specified contract
         */
        getService: function (serviceName) {
            for (var s in OpenIZ.Configuration.$configuration.application.service)
                if (OpenIZ.Configuration.$configuration.application.service[s].lastIndexOf(serviceName) > 0)
                    return OpenIZ.Configuration.$configuration.application.service[s];
            return null;
        },
        /** 
         * @summary Gets the current version of the OpenIZ host
         * @method
         * @memberof OpenIZ.App
         * @return {String} The version code of the hosting environment
         */
        getVersion: function () {
            return OpenIZApplicationService.GetVersion();
        },
        /**
         * @summary Gets the current asset title
         * @memberof OpenIZ.App
         * @method
         * @returns {string} The title of the applet
         */
        getCurrentAssetTitle: function () {
            return $(document).find("title").text();
        },
        /**  
         * @summary Get the menus async
         * @method
         * @memberof OpenIZ.App
         * @param {object} controlData The data which controls the operation of the asynchronous operation
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @returns {Object} Representing menu items the current user has access to
         */
        getMenusAsync: function (controlData) {
            OpenIZ.Util.simpleGet("/__app/menu", controlData);

        },
        /**
         * @summary Uses the device camera to scan a barcode from the device
         * @memberof OpenIZ.App
         * @method
         * @returns {string} The value of the barcode detected by the scanner
         */
        scanBarcode: function () {
            try {
                var value = OpenIZApplicationService.BarcodeScan();
                console.log('Barcode scan complete. Data: ' + value);
                return value;
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception("Exception", OpenIZ.Localization.getString("err_scan_barcode"), e.message, e);
            }
        },
        /**
         * @summary  Navigate backward, even if the back functionality crosses applets
         * @memberof OpenIZ.App
         * @method
         */
        back: function () {
            try {
                var value = OpenIZApplicationService.Back();
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
         * @summary Closes the applet and kills the Android view
         * @memberof OpenIZ.App
         * @method
         */
        close: function () {
            try {
                var value = OpenIZApplicationService.Close();
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
         * @summary Displays a TOAST on the user's screen
         * @param {String} text The text of the toast to be shown
         * @memberof OpenIZ.App
         * @method
         */
        toast: function (text) {
            try {
                var value = OpenIZApplicationService.ShowToast(text);
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
         * @summary Navigates to the specified applet passing any context variables to it via "context"
         * @param {String} appletId The ID of the applet to be navigated
         * @param {Object} context Any context variables to be passed to the applet
         * @memberof OpenIZ.App
         * @method
         * @deprecated
         */
        navigateApplet: function (appletId, context) {
            try {
                OpenIZApplicationService.Navigate(appletId, JSON.stringify(context));
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
         * @summary Queries for the specified alert
         * @param {object} controlData The data which controls the operation of the asynchronous operation
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The query to be executed against the alerts service
         * @param {int} controlData.query._count The limit of results to return from the ims
         * @param {int} controlData.query._offset The offset of the search result window
         * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
         * @method
         * @memberof OpenIZ.App
         */
        getAlertsAsync: function (controlData) {
            OpenIZ.Util.simpleGet('/__app/alerts', controlData);
        },
        /**
         * @summary Save an alert asynchronously
         * @method
         * @memberof OpenIZ.App
         * @param {object} controlData The data which controls the operation of the asynchronous operation
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZModel.Alert} controlData.data The alert data to be posted to the alerts service
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         */
        saveAlertAsync: function (controlData) {
            $.ajax({
                method: 'POST',
                url: "/__app/alerts",
                // || WARNING: JAVASCRIPT RANT AHEAD              ||
                // ||                                             ||
                // || Why!? Why!? Why is this even a line of code?||
                // || I specified JSON and application/json yet   ||
                // || the 1337 haxors at jQ decide not to encode  ||
                // || the JSON data I send as JSON!? Why!?        ||
                // \/ Stuff like this is why I dislike JavaScript \/
                data: JSON.stringify(controlData.data),
                dataType: "json",
                contentType: 'application/json',
                success: function (xhr, data) {

                    if (controlData.continueWith !== undefined)
                        controlData.continueWith(xhr, controlData.state);

                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);
                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === null)
                        console.error(error);
                    else if (error !== undefined && error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description, error.caused_by
                            ), controlData.state);

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ), controlData.state);

                    if (controlData.finally !== undefined)
                        controlData.finally(controlData.state);
                }
            });
        },
        /**
             * @summary Delete an alert asynchronously
             * @method
             * @memberof OpenIZ.App
             * @param {object} controlData The data which controls the operation of the asynchronous operation
             * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
             * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
             * @param {uuid} controlData.data The alert id to be delete from the alerts service
             * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
             */
        deleteAlertAsync: function (controlData) {
            OpenIZ.Util.simpleDelete("/__app/alerts", {
                query: "id=" + controlData.id,
                continueWith: controlData.continueWith,
                finally: controlData.finally,
                onException: controlData.onException,
                state: controlData.state
            });

        },
        /**
         * @summary Indicates that the server should clear an object from the cache
        * @method
        * @memberof OpenIZ.App
        * @param {object} controlData The data which controls the operation of the asynchronous operation
        * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
        * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
        * @param {object} controlData.data The object which is to be removed from the cache. Minimally a $type and id
        * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
        */
        deleteCacheAsync: function (controlData) {
            OpenIZ.Util.simpleDelete("/__app/cache", {
                query: "id=" + controlData.data.id + "&type=" + controlData.data.$type,
                continueWith: controlData.continueWith,
                finally: controlData.finally,
                onException: controlData.onException,
                state: controlData.state
            });
        }
    },
    /**
     * @summary Represents functions related to the localization of applets
     * @static
     * @class
     * @memberof OpenIZ
     */
    Localization: {
        /**
         * @summary Gets the specified localized string the current display language from the resources file
         * @memberof OpenIZ.Localization
         * @method
         * @param {String} stringId The identifier of the string
         * @returns The specified string
         */
        getString: function (stringId) {
            try {
                var retVal = OpenIZApplicationService.GetString(stringId);
                if (retVal == undefined)
                    return stringId || "";
                return retVal;
            }
            catch (e) {
                console.error(e);
                return stringId;
            }
        },
        /**
         * @summary Gets the current user interface locale name
         * @memberof OpenIZ.Localization
         * @method
         * @returns The ISO language code of the current UI 
         */
        getLocale: function () {
            if (!OpenIZ.Localization.$locale)
                OpenIZ.Localization.$locale = OpenIZApplicationService.GetLocale();

            return OpenIZ.Localization.$locale; //(navigator.language || navigator.userLanguage).substring(0, 2);
        },
        /**
         * @summary Sets the current user interface locale
         * @memberof OpenIZ.Localization
         * @method
         * @param {String} lcoale The locale to set the user interface to
         * @returns The locale the user interface is now operating in
         */
        setLocale: function (locale) {
            if (OpenIZApplicationService.SetLocale !== undefined)
                return OpenIZApplicationService.SetLocale(locale);
            delete (OpenIZ.Localization.$locale);
        },
        /**
         * @memberof OpenIZ.Localization
         * @method
         * @summary Gets the complete localization string data
         * @returns {Object} The string list of strings
         */
        getStrings: function (locale) {
            try {
                // Go to OpenIZ applet infrastructure
                var data = OpenIZApplicationService.GetStrings(locale);
                if (data == null)
                    return null;
                else
                    return JSON.parse(data);
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception("Exception", "Error getting string list", e.message, e);
            }

        }
    },

    /**
     * @summary Represents functions related to the concept dictionary in particular those dealing with {@link OpenIZModel.Concept}, and {@link OpenIZModel.ConceptSet}s
     * @static
     * @class
     * @memberof OpenIZ
     */
    Concept: {
        /**
         * @summary Perform a search asynchronously
         * @memberof OpenIZ.Concept
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The query filters to apply to the search
         * @param {int} controlData.query._count The limit of results to return from the ims
         * @param {int} controlData.query._offset The offset of the search result window
         * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
         * @see OpenIZ.Ims.get
         * @see OpenIZModel.Concept
         * @example
         * OpenIZ.Concept.findConceptAsync({
         *      query: { "mnemonic":"Female" },
         *      continueWith: function(result) { // do something with result },
         *      onException: function(ex) { // handle exception }
         *  });
         */
        findConceptAsync: function (controlData) {

            OpenIZ.Ims.get({
                resource: "Concept",
                query: controlData.query,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
        /**
         * @summary Perform a search of concept sets asynchronously
         * @memberof OpenIZ.Concept
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The query filters to apply to the search
         * @param {int} controlData.query._count The limit of results to return from the ims
         * @param {int} controlData.query._offset The offset of the search result window
         * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
         * @see OpenIZ.Ims.get
         * @see OpenIZModel.ConceptSet
         * @example
         * OpenIZ.Concept.findConceptSetAsync({
         *      query: { "member.mnemonic":"Female" },
         *      continueWith: function(result) { // do something with result },
         *      onException: function(ex) { // handle exception }
         *  });
         */
        findConceptSetAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "ConceptSet",
                query: controlData.query,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        }
    },

    /**
     * @summary Represents a series of functions for submitting {@link OpenIZModel.Bundle} instances
     * @static
     * @class
     * @memberof OpenIZ
     */
    Bundle: {
        /**
         * @summary Insert the bundle asynchronously
         * @memberof OpenIZ.Bundle
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         */
        insertAsync: function (controlData) {
            OpenIZ.Ims.post({
                resource: "Bundle",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                data: controlData.data,
                state: controlData.state,
                finally: controlData.finally,
                synchronous: controlData.synchronous
            });
        }
    },
    /**
     * @summary Represents a series of functions related to {@link OpenIZModel.Patient} instances
     * @static
     * @class
     * @memberof OpenIZ
     */
    Patient: {
        /**
         * @summary Perform a search of patients asynchronously
         * @memberof OpenIZ.Patient
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The query filters to apply to the search
         * @param {int} controlData.query._count The limit of results to return from the ims
         * @param {int} controlData.query._offset The offset of the search result window
         * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
         * @see OpenIZ.Ims.get
         * @see OpenIZModel.Patient
         */
        findAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "Patient",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                query: controlData.query,
                state: controlData.state
            });
        },
        /**
         * @summary Asynchronously insert a patient object into the IMS
         * @memberof OpenIZ.Patient
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.data The patient data to be inserted into the IMS
         * @see OpenIZ.Ims.post
         * @see OpenIZModel.Patient
         */
        insertAsync: function (controlData) {
            OpenIZ.Ims.post({
                resource: "Patient",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                data: controlData.data,
                state: controlData.state
            });
        },
        /**
         * @summary Asynchronously updates a patient object in the IMS
         * @memberof OpenIZ.Patient
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.data The patient data to be inserted into the IMS
         * @param {uuid} controlData.id The identifier of the patient that is to be updated
         * @see OpenIZ.Ims.put
         * @see OpenIZModel.Patient
         */
        updateAsync: function (controlData) {
            OpenIZ.Ims.put({
                resource: "Patient",
                data: controlData.data,
                id: controlData.id,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                versionId: controlData.versionId,
                finally: controlData.finally,
                state: controlData.state
            });
        },
        /**
         * @summary Asynchronously deletes a patient object in the IMS
         * @memberof OpenIZ.Patient
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {uuid} controlData.id The identifier of the patient that is to be updated
         * @see OpenIZ.Ims.delete
         * @see OpenIZModel.Patient
         */
        obsoleteAsync: function (controlData) {
            OpenIZ.Ims.delete({
                resource: "Patient",
                id: controlData.id,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
        /**
         * @summary Performs a patient get asynchronously
         * @memberof OpenIZ.Patient
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {uuid} controlData.id The identifier of the patient that is to be updated
         * @see OpenIZ.Ims.get
         * @see OpenIZModel.Patient
         */
        getAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "Patient",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                query: {
                    _id: controlData.id || controlData.patientId,
                    _viewModel: "full"
                },
                state: controlData.state
            });
        },
        /**
          * @summary Downloads a patient asynchronously
         * @memberof OpenIZ.Patient
         * @method
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {uuid} controlData.id The identifier of the patient that is to be downloaded
         * @see OpenIZ.Ims.get
         * @see OpenIZModel.Patient
         */
        downloadAsync: function (controlData) {
            OpenIZ.Util.simpleGet("/__ims/Patient.Download", {
                query: { _id: controlData.id },
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        }
    },


    /**
     * @summary Provides a series of utility functions for interacting with {@link OpenIZModel.Place} instances
     * @static
     * @class
     * @memberof OpenIZ
     */
    Place: {
        /**
         * @summary Perform an asynchronous search on the place resource
         * @method
         * @memberof OpenIZ.Place
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The query filters to apply to the search
         * @param {int} controlData.query._count The limit of results to return from the ims
         * @param {int} controlData.query._offset The offset of the search result window
         * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
         * @see OpenIZModel.Place
         */
        findAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "Place",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                query: controlData.query,
                state: controlData.state,
                synchronous: controlData.synchronous
            });
        },
        /**
         * @summary Bind a place filter to a select box
         * @param {HTMLElement} target The element to be bound to
         * @param {object} filter Of the objects to be shown (additional to be added to the current name filter)
         * @method
         * @memberof OpenIZ.Place
         */
        bindSelect: function (target, filter) {
            $(target).select2({
                ajax: {
                    url: "/__ims/Place",
                    dataType: 'json',
                    delay: 500,
                    method: "GET",
                    data: function (params) {
                        filter["name.component.value"] = "~" + params.term;
                        filter["_count"] = 5;
                        filter["_offset"] = 0;
                        return filter;
                    },
                    processResults: function (data, params) {

                        params.page = params.page || 0;

                        return {
                            results: data.item,
                            pagination: {
                                more: data.offset + data.count < data.total
                            }
                        };
                    },
                    cache: true
                },
                escapeMarkup: function (markup) { return markup; }, // Format normally
                minimumInputLength: 4,
                templateSelection: function (place) {
                    if (place.name != null)
                        return "<span class='glyphicon glyphicon-map-marker'></span> " + place.name.OfficialRecord.component.$other.value;
                    else
                        return "<span class='glyphicon glyphicon-map-marker'></span> " + place.text;
                },
                templateResult: function (place) {
                    if (place.text != null)
                        return place.text;
                    return "<div class='label label-default'>" +
                        place.typeConceptModel.name[OpenIZ.Localization.getLocale()] + "</div> " + place.name.OfficialRecord.component.$other.value;
                }
            });
        }
    },
    /**
     * @static
     * @class
     * @summary Provides functions for managing {@link OpenIZModel.Provider} objects on the IMS
     * @memberof OpenIZ
     */
    Provider: {
        /**
         * @summary Perform an asynchronous search on the provider resource
         * @method
         * @memberof OpenIZ.Provider
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {object} controlData.query The query filters to apply to the search
         * @param {int} controlData.query._count The limit of results to return from the ims
         * @param {int} controlData.query._offset The offset of the search result window
         * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
         */
        findProviderAsync: function (controlData) {
            OpenIZ.Ims.get({
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                resource: 'Provider',
                query: controlData.query,
                state: controlData.state
            });
        },
        /**
         * @summary Bind a provider filter to a select box
         * @param {HTMLElement} target The element to be bound to
         * @param {object} filter Of the objects to be shown (additional to be added to the current name filter)
         * @method
         * @memberof OpenIZ.Provider
         */
        bindSelect: function (target, filter) {
            $(target).select2({
                ajax: {
                    url: "/__ims/Provider",
                    dataType: 'json',
                    delay: 500,
                    method: "GET",
                    data: function (params) {
                        filter["name.component.value"] = "~" + params.term;
                        filter["_count"] = 5;
                        filter["_offset"] = 0;
                        return filter;
                    },
                    processResults: function (data, params) {

                        params.page = params.page || 0;

                        return {
                            results: data.item,
                            pagination: {
                                more: data.offset + data.count < data.total
                            }
                        };
                    },
                    cache: true
                },
                escapeMarkup: function (markup) { return markup; }, // Format normally
                minimumInputLength: 1,
                templateSelection: function (provider) {
                    if (provider.text != null)
                        return "<span class='glyphicon glyphicon-user'></span> " + provider.text;
                    return "<span class='glyphicon glyphicon-user'></span> " + OpenIZ.Util.renderName(provider.name.OfficialRecord);
                },
                templateResult: function (provider) {
                    if (provider.text != null)
                        return provider.text;
                    return "<span class='glyphicon glyphicon-user'></span> " + OpenIZ.Util.renderName(provider.name.OfficialRecord);
                }
            });

        }
    },
    /**
     * @summary Entity class for interacting with {@link OpenIZModel.Entity} instances and derivatives
     * @see OpenIZModel.Entity
     * @see OpenIZModel.Person
     * @see OpenIZModel.Place
     * @see OpenIZModel.Material
     * @see OpenIZModel.ManufacturedMaterial
     * @see OpenIZModel.Patient
     * @see OpenIZModel.UserEntity
     * @see OpenIZModel.Provider
     * @static
     * @class
     * @memberof OpenIZ
     */
    Entity: {
        /**
         * @summary Get an empty entity template object asynchronously. See the OpenIZ documentation for more information about templates
         * @method
         * @memberof OpenIZ.Entity
         * @param {object} controlData The data which controls the asynchronous operation.
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.templateId The identifier of the template which to retrieve. Templates should be registered with the application manifest
         * @example
         * OpenIZ.CarePlan.getEntityTemplateAsync({
         *      templateId: "Entity.Patient.Baby", 
         *      continueWith: function(template) {
         *          $scope.act = template;
         *      }
         * });
         */
        getEntityTemplateAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "Entity/Template",
                query: { "templateId": controlData.templateId },
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
    },
    /**
    * @summary The configuration property is used to segregate the functions related to configuration of the main OpenIZ system including realm, updating configuration, etc.
     * @static
     * @class
    * @memberof OpenIZ
    * @property {object} $configuration Cached configuration used for synchronous calls
    */
    Configuration: {
        $configuration: null,
        /** 
         * @summary Get the configuration file in an async manner
         * @method
         * @memberof OpenIZ.Configuration
         * @param {Object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @return {Object} The complete configuration object
         */
        getConfigurationAsync: function (controlData) {
            OpenIZ.Util.simpleGet('/__config/all', {
                continueWith: function (data) {
                    OpenIZ.Configuration.$configuration = data;
                    controlData.continueWith(data, controlData.state);
                },
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });

        },
        /**
         * @summary Gets the specified appplication setting
         * @method
         * @param {String} key The key of the application setting to retrieve
         * @return {String} The setting value
         */
        getApplicationSetting: function (key) {
            try {
                for (var s in OpenIZ.Configuration.$configuration.application.setting)
                    if (OpenIZ.Configuration.$configuration.application.setting[s].key == key)
                        return OpenIZ.Configuration.$configuration.application.setting[s].value;
                return null;
            }
            catch (e) {
                throw new OpenIZModel.Exception("Exception", e.message, e.detail, e);
            }
        },
        /**
         * @summary Gets the specified appplication setting. This method does not save the setting, merely sets it in the local configuration
         * @method
         * @param {String} key The key of the application setting to retrieve
         * @return {String} The setting value
         */
        setApplicationSetting: function (key, value) {
            try {
                for (var s in OpenIZ.Configuration.$configuration.application.setting)
                    if (OpenIZ.Configuration.$configuration.application.setting[s].key == key) {
                        OpenIZ.Configuration.$configuration.application.setting[s].value = value;
                        return;
                    }
                OpenIZ.Configuration.$configuration.application.setting.push({ key: key, value: value });
            }
            catch (e) {
                throw new OpenIZModel.Exception("Exception", e.message, e.detail, e);
            }
        },
        /**
        * @summary Gets the current realm to which the client is connected
        * @memberof OpenIZ.Configuration
        * @method
       */
        getRealm: function () {
            try {
                return OpenIZ.Configuration.$configuration.realmName;
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
        * @summary Gets the specified OpenIZ configuration section name. 
        * @returns A JSON object representing the configuration data for the particular section
        * @param {string} sectionName The name of the section which should be retrieved.
        * @memberof OpenIZ.Configuration
        * @method
        */
        getSection: function (sectionName) {
            try {
                return OpenIZ.Configuration.$configuration[sectionName];
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
        * @summary Joins the current client to the specified realm. The backing container is responsible for performing all steps in terms of generating a client certificate, sending it to the realm auth service, etc.
        * @returns True if the specified realm was joined successfully
        * @param {object} controlData An object containing search, offset, count and callback data
        * @param {string} controlData.address The address root of the realm. Example: demo.openiz.org
        * @param {string} controlData.deviceName A unique name for the device which is being joined
        * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
        * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
        * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
        * @memberof OpenIZ.Configuration
        * @method
        */
        joinRealmAsync: function (controlData) {
            try {
                // Perform auth request
                $.ajax(
                 {
                     method: 'POST',
                     url: '/__config/realm',
                     data: {
                         realmUri: controlData.domain,
                         deviceName: controlData.deviceName,
                         force: controlData.force,
                         enableTrace: controlData.enableTrace,
                         enableSSL: controlData.enableSSL,
                         port: controlData.port
                     },
                     dataType: "json",
                     contentType: 'application/x-www-urlform-encoded',
                     success: function (xhr, data) {
                         if (data != null && data.error !== undefined)
                             controlData.onException(new OpenIZModel.Exception(data.type, data.error, null, data.caused_by), controlData.state
                             );
                         else if (data != null) {
                             OpenIZ.Configuration.$configuration = xhr;
                             controlData.continueWith(xhr, controlData.state);
                         }
                         else if (controlData.onException != null)
                             controlData.onException(new OpenIZModel.Exception("Exception", "err_general",
                                 data,
                                 null
                             ), controlData.state);

                         if (controlData.finally !== undefined)
                             controlData.finally(controlData.state);
                     },
                     error: function (data) {
                         var error = data.responseJSON;
                         if (error != null && error.error !== undefined) // config error
                             controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                     error.error_description,
                                     error.caused_by
                                 ), controlData.state);

                         else if (controlData.onException != null) // unknown error
                             controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                     data,
                                     null
                                 ), controlData.state);
                         if (controlData.finally !== undefined)
                             controlData.finally(controlData.state);

                     }
                 });
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
        * @summary Instructs the current client container to leave the currently configured realm
        * @memberof OpenIZ.Configuration
        * @method
        * @returns True if the realm was successfully left.
        */
        leaveRealm: function () {
            try {
                if (!confirm('You are about to leave the realm ' + OpenIZ.Configuration.$configuration.realmName + '. Doing so will force the OpenIZ back into an initial configuration mode. Are you sure you want to do this?'))
                    return false;
                return OpenIZConfigurationService.LeaveRealm();
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
        * @summary Instructs the container to save the specified configuration object to the local configuration file/source.
        * @param {object} controlData An object containing search, offset, count and callback data
        * @param {string} controlData.address The address root of the realm. Example: demo.openiz.org
        * @param {string} controlData.deviceName A unique name for the device which is being joined
        * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
        * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
        * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
        * @memberof OpenIZ.Configuration
        * @method
        * @returns true if the save operation was successful
        */
        saveAsync: function (controlData) {
            try {
                // Perform auth request
                $.ajax(
                 {
                     method: 'POST',
                     url: '/__config/all',
                     data: JSON.stringify(controlData.data),
                     dataType: "json",
                     contentType: 'application/json',
                     success: function (xhr, data) {
                         if (data != null && data.error !== undefined)
                             controlData.onException(new OpenIZModel.Exception(data.type, data.error, null, data.caused_by), controlData.state
                             );
                         else if (data != null) {
                             OpenIZ.Configuration.$configuration = xhr;
                             controlData.continueWith(xhr, controlData.state);
                         }
                         else
                             controlData.onException(new OpenIZModel.Exception("Exception", "err_general",
                                 data,
                                 null
                             ), controlData.state);

                         if (controlData.finally !== undefined)
                             controlData.finally(controlData.state);
                     },
                     error: function (data) {
                         var error = data.responseJSON;
                         if (error != null && error.error !== undefined) // config error
                             controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                     error.error_description,
                                     error.caused_by
                                 ), controlData.state);

                         else // unknown error
                             controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                     data,
                                     null
                                 ), controlData.state);
                         if (controlData.finally !== undefined)
                             controlData.finally(controlData.state);

                     }
                 });
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
        * @summary Get applet specific key/value pair configuration parameters which are currently set for the application
        * @memberof OpenIZ.Configuration
        * @method
        * @param {object} controlData An object containing async control data
        * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
        * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
        * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
        * @param {String} controlData.appletId The identifier of the applet from which the settings should be retrieved
        * @returns A key/value pair representing the applet settings
        */
        getAppletSettingsAsync: function (controlData) {
            OpenIZ.Util.simpleGet("/__config/user", {
                query: { "_id": controlData.appletId },
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
        /**
        * @summary Saves the applet specific settings in a key/value pair format to the configuration store
        * @memberof OpenIZ.Configuration
        * @method
        * @param {object} controlData An object containing async control data
        * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
        * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
        * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
        * @param {String} controlData.appletId The applet identification for which the settings apply
        * @param {Object} controlData.settings A key/value pair JSON object of the settings
        * @returns True if the settings save was successful
        */
        saveAppletSettingsAsync: function (controlData) {
            OpenIZ.Util.simplePost("/__config/user?_id=" + controlData.appletId, {
                data: controlData.settings,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
        /**
        * @summary Get local user preference strings in a key/value pair JSON object
        * @memberof OpenIZ.Configuration
        * @param {object} controlData An object containing async control data
        * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
        * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
        * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
        * @method
        */
        getUserPreferencesAsync: function (controlData) {
            OpenIZ.Util.simpleGet("/__config/user", controlData);
        },
        /**
        * @summary Save the user preferences in the key/value pair format
        * @memberof OpenIZ.Configuration
        * @method
        * @memberof OpenIZ.Configuration
        * @param {object} controlData An object containing search, offset, count and callback data
        * @param {object} controlData.data An object containing control data
        * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
        * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
        * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
        * @returns true if the save was successful
        */
        saveUserPreferencesAsync: function (controlData) {
            OpenIZ.Util.simplePost("/__config/user", controlData);
        }
    },
    /**
     * @static
     * @class
     * @summary Provides utility functions for interacting with {@link OpenIZModel.Material} instances
     * @memberOf OpenIZ
     */
    Material:
        {
            /** 
             * @deprecated
             * @summary Deprecated, use getMaterialAsync
             */
            findMaterialAsync: function (controlData) {
                return OpenIZ.Material.getMaterialAsync(controlData);
            },
            /**
             * @summary Get manufactured materials from the IMS 
             * @param {object} controlData An object containing search, offset, count and callback data
             * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
             * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
             * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
             * @param {object} controlData.query The query filters to apply to the search
             * @param {int} controlData.query._count The limit of results to return from the ims
             * @param {int} controlData.query._offset The offset of the search result window
             * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
             * @memberof OpenIZ.ManufacturedMaterial
             * @method
             */
            getMaterialAsync: function (controlData) {
                var query = controlData.query || {};
                if (controlData instanceof String)
                    query += "&classConcept=" + OpenIZModel.EntityClassKeys.Material;
                else
                    query.classConcept = OpenIZModel.EntityClassKeys.Material;
                OpenIZ.Ims.get({
                    resource: "Material",
                    continueWith: controlData.continueWith,
                    onException: controlData.onException,
                    query: query,
                    finally: controlData.finally,
                    state: controlData.state
                })
            }
        },
    /**
     * @static
     * @class
     * @summary Provides utility functions for interacting with {@link OpenIZModel.ManufacturedMaterial} instances
     * @memberOf OpenIZ
     */
    ManufacturedMaterial:
        {
            /**
             * @deprecated
             * @see OpenIZ.ManufacturedMaterial.getManufacturedMaterialsAsync
             */
            getManufacturedMaterials: function (controlData) {
                OpenIZ.ManufacturedMaterial.getManufacturedMaterialAsync(controlData);
            },
            /**
             * @summary Get manufactured materials from the IMS 
             * @param {object} controlData An object containing search, offset, count and callback data
             * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
             * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
             * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
             * @param {object} controlData.query The query filters to apply to the search
             * @param {int} controlData.query._count The limit of results to return from the ims
             * @param {int} controlData.query._offset The offset of the search result window
             * @param {uuid} controlData.query._id The identifier of the object to retrieve from the IMS (performs a get rather than a query)
             * @memberof OpenIZ.ManufacturedMaterial
             * @method
             */
            getManufacturedMaterialAsync: function (controlData) {
                OpenIZ.Ims.get({
                    resource: "ManufacturedMaterial",
                    continueWith: controlData.continueWith,
                    onException: controlData.onException,
                    query: controlData.query,
                    state: controlData.state
                })
            }
        },
    /**
     * @static
     * @class
     * @summary Provides utilities for interacting with {@link OpenIZModel.UserEntity} classes
     * @memberof OpenIZ
     */
    UserEntity: {
        /**
         * @summary Updates the specified user entity in the IMS in an asynchronous manner
         * @param {object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {OpenIZModel.UserEntity} controlData.data The user entity which is to be updated in the IMS service
         * @memberof OpenIZ.UserEntity
         * @method
         */
        updateAsync: function (controlData) {
            OpenIZ.Ims.put({
                resource: "UserEntity",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                data: controlData.data,
                state: controlData.state
            });
        }
    },
    /**
     * @static
     * @class
     * @summary Provides utilities for interacting with the queues in OpenIZ
     * @memberof OpenIZ
     */
    Queue: {
        /**
         * @summary Represents a list of names of queue objects
         * @enum
         * @static
         * @memberof OpenIZ
         */
        QueueNames: {
            InboundQueue: "inbound",
            OutboundQueue: "outbound",
            DeadLetterQueue: "dead",
            AdminQueue: "admin"
        },
        /** 
         * @summary Forces a synchronization
         * @param {object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {String} controlData.queueName The name of the queue to retrieve
         * @memberof OpenIZ.Queue
         * @method
         */
        forceResyncAsync: function (controlData) {
            OpenIZ.Util.simplePost("/__app/queue", {
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
        /** 
         * @summary Retrieves a specified queue object
         * @param {object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {String} controlData.queueName The name of the queue to retrieve
         * @memberof OpenIZ.Queue
         * @method
         */
        getQueueAsync: function (controlData) {
            OpenIZ.Util.simpleGet("/__app/queue", {
                query: { _queue: controlData.queueName, id: "!null", _id: controlData.id },
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
        /** 
         * @summary Re-queues a specified queue object
         * @param {object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {String} controlData.queueId The ID of the item to re-queue
         * @memberof OpenIZ.Queue
         * @method
         */
        requeueDeadAsync: function (controlData) {
            OpenIZ.Util.simplePut("/__app/queue", {
                query: "_id=" + controlData.queueId,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
        /** 
         * @summary Deletes a specified queue object
         * @param {object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {String} controlData.queueName The name of the queue to delete from
         * @param {String} controlData.queueId The ID of the queue item to delete
         * @memberof OpenIZ.Queue
         * @method
         */
        deleteQueueAsync: function (controlData) {
            OpenIZ.Util.simpleDelete("/__app/queue", {
                query: "_id=" + controlData.queueId + "&_queue=" + controlData.queueName,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        },
        /** 
         * @summary Re-queues the specified objects from the database back into the queue
         * @param {object} controlData An object containing search, offset, count and callback data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {Date} controlData.from The lower date bound on which to queue data
         * @param {Date} controlData.to The upper date bound on which to queue data
         * @memberof OpenIZ.Queue
         * @method
         */
        resubmitAsync: function (controlData) {
            OpenIZ.Util.simplePut("/__app/data/sync", {
                query: "creationTime=<" + OpenIZ.Util.toDateInputString(controlData.to || new Date()) + "T23:59:59" + "&creationTime=>" + OpenIZ.Util.toDateInputString(controlData.from || new Date()) + "T00:00:00",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                state: controlData.state
            });
        }
    }
};

OpenIZ.UserInterface.patientController = new OpenIZ.UserInterface.PatientControllerPrototype();

// No caching
$.ajaxSetup({
    cache: false,
    beforeSend: function (data, settings) {
        if (OpenIZ.Authentication.$elevationCredentials.$enabled) {
            data.setRequestHeader("Authorization", "BASIC " +
                btoa(OpenIZ.Authentication.$elevationCredentials.userName + ":" + OpenIZ.Authentication.$elevationCredentials.password));
        }
        if (!OpenIZ.App.magic)
            OpenIZ.App.magic = OpenIZApplicationService.GetMagic();
        data.setRequestHeader("X-OIZMagic", OpenIZ.App.magic);
    },
    converters: {
        "text json": function (data) {
            return $.parseJSON(data, true);
        }
    }
});

// Handles error conditions related to expiry
$(document).ajaxError(function (e, data, setting, err) {
    if ((data.status == 401 || data.status == 403)) {
        if (OpenIZ.Authentication.$session && OpenIZ.Authentication.$elevationCredentials.continueWith == undefined && (OpenIZ.Authentication.$session && OpenIZ.Authentication.$session.exp < new Date() || document.cookie == "") &&
            window.location.hash != "/corelogin") {
            console.error("Unauthorized Access:> " + e);
            OpenIZ.Authentication.$sessionExpiredHandler();
        }
        else if (OpenIZ.Authentication.$elevationCredentials.continueWith) // The session is active
            OpenIZ.Authentication.showElevationDialog();
    }
    else
        console.warn(new OpenIZModel.Exception("Exception", "err_request", err, null));
});

// Parameters
(window.onpopstate = function () {
    var match,
        pl = /\+/g,  // Regex for replacing addition symbol with a space
        search = /([^&=]+)=?([^&]*)/g,
        decode = function (s) { return decodeURIComponent(s.replace(pl, " ")); },
        query = window.location.search.substring(1);

    OpenIZ.urlParams = {};
    while (match = search.exec(query))
        OpenIZ.urlParams[decode(match[1])] = decode(match[2]);
})();

/**
 * @method
 * @memberof Date
 * @summary Get the week of the year
 */
Date.prototype.getWeek = function () {
    var oneJan = this.getFirstDayOfYear();
    return Math.ceil((((this - oneJan) / 86400000) + oneJan.getDay() + 1) / 7);
}

/**
 * @method
 * @memberof Date
 * @summary Get the week of the year
 */
Date.prototype.getUTC = function () {
    return new Date(this.toUTCString());
}

/** 
 * @method
 * @memberof Date
 * @summary Get the first day of the year
 */
Date.prototype.getFirstDayOfYear = function () {
    return new Date(this.getFullYear(), 0, 1);
}

/**
 * @method 
 * @memberof Date
 * @summary Get the first day of the following week
 */
Date.prototype.nextMonday = function () {
    var retVal = this.getFirstDayOfYear();
    retVal.setDate(retVal.getDate() + (new Date().getWeek() * 7));
    return retVal;
}


/**
 * @summary Gets the date on the next day
 * @method
 * @memberof Date
 * @param {Number} days The number of days to add
 */
Date.prototype.addDays = function (days) {
    var retVal = new Date(this.getFullYear(), this.getMonth(), this.getDate(), this.getHours(), this.getMinutes(), this.getSeconds(), this.getMilliseconds());
    retVal.setDate(retVal.getDate() + days);
    return retVal;
}


/**
 * @summary Adds the specified number of seconds to the date
 * @method
 * @memberof Date
 * @param {Number} seconds The number of seconds to add
 */
Date.prototype.addSeconds = function (seconds) {
    var retVal = new Date(this.getFullYear(), this.getMonth(), this.getDate(), this.getHours(), this.getMinutes(), this.getSeconds(), this.getMilliseconds());
    retVal.setSeconds(retVal.getSeconds() + seconds);
    return retVal;
}

/**
 * @summary Add the specified seconds to the date
 * @method
 * @memberof Date
 * @param {Number} seconds The number of seconds to add
 */
Date.prototype.addSeconds = function (seconds) {
    var retVal = new Date(this.getFullYear(), this.getMonth(), this.getDate(), this.getHours(), this.getMinutes(), this.getSeconds(), this.getMilliseconds());
    retVal.setSeconds(retVal.getSeconds() + seconds);
    return retVal;
}

/**
 * @summary Truncates the specified date
 * @method
 * @memberof Date
 */
Date.prototype.trunc = function () {
    var retVal = new Date(this.getFullYear(), this.getMonth(), this.getDate(), this.getHours(), this.getMinutes(), this.getSeconds(), this.getMilliseconds());
    retVal.setSeconds(0);
    retVal.setMinutes(0);
    retVal.setHours(0);
    retVal.setMilliseconds(0);
    return retVal;
}

/**
 * @summary Gets the date on the next day
 * @memberof Date
 * @method
 */
Date.prototype.tomorrow = function () {
    return this.addDays(1);
}

/** 
 * @summary Truncates date to day
 * @memberof Date
 * @method
 */
Date.prototype.trunc = function () {
    var retVal = new Date(this.getFullYear(), this.getMonth(), this.getDate(), this.getHours(), this.getMinutes(), this.getSeconds(), this.getMilliseconds());
    retVal.setSeconds(0);
    retVal.setHours(0);
    retVal.setMinutes(0);
    retVal.setMilliseconds(0);
    return retVal;
}

/**
 * @summary Gets the date on the previous day
 * @method
 * @memberof Date
 */
Date.prototype.yesterday = function () {
    return this.addDays(-1);
}

/** 
 * @summary Last Week Day
 * @memberof Date
 * @method
 * @param {int} year The year for the date to get the end of week
 * @param {int} month The month for which to gather tha last date
 */
Date.prototype.lastWeekDay = function (month, year) {
    if (!day && !month && !year) {
        var date = new Date();
        year = date.getYear();
        month = date.getMonth();
    }

    var lastDay = new Date(year, month + 1, 0);
    switch (lastDay.getDay()) {
        case 0:
            lastDay.setDate(lastDay.getDate() - 2);
            break;
        case 6:
            lastDay.setDate(lastDay.getDate() - 1);
            break;
    }
    return lastDay;
}

/** 
 * @summary Decodes a hex string
 * @method
 */
String.prototype.hexDecode = function () {
    return this.replace(/([0-9A-Fa-f]{2})/g, function (i, a) {
        return String.fromCharCode(parseInt(a, 16));
    });
}

/** 
 * @summary Encodes a hex string
 * @method
 */
String.prototype.hexEncode = function () {
    var hex, i;

    var result = "";
    for (i = 0; i < this.length; i++) {
        hex = this.charCodeAt(i).toString(16);
        result += ("0" + hex).slice(-2);
    }

    return result
}

/** 
 * @summary Turns a string into a byte array
 * @method
 */
String.prototype.toByteArray = function () {
    var result = [];
    for (var i = 0; i < this.length; i++) {
        result.push(this.charCodeAt(i));
    }

    return result
}


/** Android 4.4 Hacks **/
if (!String.prototype.startsWith)
    String.prototype.startsWith = function (start) {
        return this.indexOf(start) == 0;
    };