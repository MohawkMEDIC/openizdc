/// <reference path="openiz-model.js"/>
/// <reference path="~/lib/select2.min.js"/>

/**
 * @version 0.6.12 (Dalhouse)
 * @copyright (C) 2015-2016, Mohawk College of Applied Arts and Technology
 * @license Apache 2.0
 */
/*
 * Copyright 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-7-18
 */


/// <reference path="~/lib/jquery.min.js"/>
// SHIM
var OpenIZApplicationService = window.OpenIZApplicationService || {};
var OpenIZSessionService = window.OpenIZSessionService || {};

/**
 * @callback OpenIZ~continueWith
 * @summary The function call which is called whenever an asynchronous operation completes successfully
 * @param {Object} data The result data from the asynchronous callback
 */
/**
 * @callback OpenIZ~onException
 * @summary The exception handling callback whenever an asynchronous operation does not complete successfully
 * @param {OpenIZModel#Exception} exception The exception which was thrown as a result of the operation.
 */
/**
 * @callback OpenIZ~finally
 * @summary The callback which is always executed from an asynchronous call regardless of whehter the operation was successful or not
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
     * @summary Provides operations for managing {@link OpenIZModel.Act} instances.
     * @memberof OpenIZ
     * @static
     * @class
     */
    Act: {
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
          * @see {OpenIZ.IMS.get}
          */
        findAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "Act",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                query: controlData.query
            });
        },
        /**
         * @summary Performs a patient insert asynchronously
         * @memberof OpenIZ.Act
         * @method
         * @see {OpenIZ.Ims.post}
         * @param {object} controlData The data which controls the asynchronous process
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {OpenIZModel.Act} controlData.data The data which is to be inserted on the IMS
         * @example
         * OpenIZ.Act.insertAsync({
         *      data: new OpenIZModel.Act(...),
         *      continueWith: function(result) { // Do something with result },
         *      onException: function(ex) { // Handle exception }
         * });
         */
        insertAsync: function (controlData) {
            OpenIZ.Ims.post({
                resource: "Act",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                data: controlData.data
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
                query: { "templateId": controlData.templateId },
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally
            });
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

                    if (controlData.continueWith !== undefined)
                        controlData.continueWith(xhr);

                    if (controlData.finally !== undefined)
                        controlData.finally();
                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === null)
                        console.error(error);
                    else if (error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                null
                            ));

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ));

                    if (controlData.finally !== undefined)
                        controlData.finally();
                }
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

                    if (controlData.continueWith !== undefined)
                        controlData.continueWith(xhr);

                    if (controlData.finally !== undefined)
                        controlData.finally();
                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === null)
                        console.error(error);
                    else if (error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                null
                            ));

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ));

                    if (controlData.finally !== undefined)
                        controlData.finally();
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
                    if (controlData.continueWith !== undefined)
                        controlData.continueWith(xhr);

                    if (controlData.finally !== undefined)
                        controlData.finally();
                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === undefined)
                        console.error(error);
                    else if (error != undefined && error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                null
                            ));
                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ));

                    // Do finally
                    if (controlData.finally !== undefined)
                        controlData.finally();

                }
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
                url: "/__ims/" + controlData.resource,
                data: { "_id": controlData.id },
                accept: 'application/json',
                contentType: 'application/json',
                success: function (xhr, data) {
                    if (controlData.continueWith !== undefined)
                        controlData.continueWith(xhr);

                    if (controlData.finally !== undefined)
                        controlData.finally();
                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === undefined)
                        console.error(error);
                    else if (error != undefined && error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                null
                            ));
                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ));

                    // Do finally
                    if (controlData.finally !== undefined)
                        controlData.finally();

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
     * @summary Utility functions which assist in the writing of other OpenIZ functions
     * @static
     * @class
     * @memberof OpenIZ
     */
    Util: {
        /**
         * @summary Perform a simple post of JSON data to the backend
         * @method
         * @memberof
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
                        controlData.continueWith(xhr);

                    if (controlData.finally !== undefined)
                        controlData.finally();
                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === null)
                        console.error(error);
                    else if (error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                null
                            ));

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ));

                    if (controlData.finally !== undefined)
                        controlData.finally();
                }
            });
        },
        /**
         * @summary Perform a simple get not necessarily against the IMS
         * @method
         * @memberof OpenIZ.Util   
         * @param {object} controlData The control data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} url The URL from which to retrieve 
         * @param {object} controlData.query The query to be included
         */
        simpleGet: function (url, controlData) {
            controlData.onException = controlData.onException || OpenIZ.Util.logException;
            // Perform auth request
            $.getJSON(url, controlData.query, function (data) {

                if (data != null && data.error !== undefined)
                    controlData.onException(new OpenIZModel.Exception(data.type, data.error),
                        data.error_description,
                        null
                    );
                else if (data != null) {
                    controlData.continueWith(data);
                }
                else
                    controlData.onException(new OpenIZModel.Exception("Exception", "err_general",
                        data,
                        null
                    ));
            }).error(function (data) {
                var error = data.responseJSON;
                if (error != null && error.error !== undefined) //  error
                    controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                            error.error_description,
                            null
                        ));

                else // unknown error
                    controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                            data,
                            null
                        ));
            }).always(function () {
                if (controlData.finally !== undefined)
                    controlData.finally();
            });
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
                entity.address !== undefined ? (entity.address.Direct || entity.address.HomeAddress) :
                (entity.Direct || entity.HomeAddress);
            var retVal = "";
            if (address.component.AdditionalLocator)
                retVal += address.component.AdditionalLocator + ", ";
            if (address.component.StreetAddressLine)
                retVal += address.component.StreetAddressLine + ", ";
            if (address.component.City)
                retVal += address.component.City + ", ";
            if (address.component.County != null)
                retVal += address.component.County + ", ";
            if (address.component.State != null)
                retVal += address.component.State + ", ";
            if (address.component.Country != null)
                retVal += address.component.Country + ", ";
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
         * @summary Log an exception to the console
         * @method
         * @memberof OpenIZ.Util
         * @param {OpenIZModel.Exception} e The exception to be logged to the console
         */
        logException: function (e) {
            console.warn(e);
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
            else
                return entityName;
        },
        /**
         * @summary Changes the specified date string into an appropriate ISO string
         * @memberof OpenIZ.Util
         * @method
         * @param {String} date The date to be formatted
         * @return {string} A DATE as an ISO String only
         */
        toDateInputString: function (date) {
            return date.toISOString().substring(0, 10);
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
                    controlData.continueWith(syncFn());
                }
                catch (ex) {
                    if (controlData.onException === undefined)
                        console.error(ex);
                    else
                        controlData.onException(ex);
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
        $session : null,
        /**
         * @summary Credentials to use for elevation in lieu of the current session
         */
        $elevationCredentials: {},
        /**
         * @summary Show an elevation dialog
         * @method
         * @memberof OpenIZ.Authentication
         */
        showElevationDialog : function() {
            $("#authenticationDialog").modal('show');
        },
        /** 
         * @summary Hide the elevation dialog
         * @method
         * @memberof OpenIZ.Authentication
         */
        hideElevationDialog : function() {
            $("#authenticationDialog").modal('hide');
        },
        /**
         * @summary Perform a login operation asynchronously
         * @memberof OpenIZ.Authentication
         * @method
         * @see {OpenIZ.Util.simpleGet}
         * @param {string} controlData.userName The username to use when authenticating
         * @param {string} controlData.password The password of the user to authenticate with
         * @param {string} controlData.scope The scope to append to the OAUTH request
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
                     grant_type: 'password',
                     scope: controlData.scope
                 },
                 dataType: "json",
                 contentType: 'application/x-www-urlform-encoded',
                 success: function (xhr, data) {
                     if (data != null && data.error !== undefined)
                         controlData.onException(new OpenIZModel.Exception(data.type, data.error),
                             data.error_description,
                             null
                         );
                     else if (data != null) {
                         controlData.continueWith(data);
                         OpenIZ.Authentication.$session = data;
                     }
                     else
                         controlData.onException(new OpenIZModel.Exception("Exception", "err_general",
                             data,
                             null
                         ));

                     if (controlData.finally !== undefined)
                         controlData.finally();
                 },
                 error: function (data) {
                     var error = data.responseJSON;
                     if (error != null && error.error !== undefined) // oauth 2 error
                         controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                 error.error_description,
                                 null
                             ));

                     else // unknown error
                         controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                 data,
                                 null
                             ));
                     if (controlData.finally !== undefined)
                         controlData.finally();

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
                success: function (xhr, data) {
                    if (data != null && data.error !== undefined)
                        controlData.onException(new OpenIZModel.Exception(data.type, data.error),
                            data.error_description,
                            null
                        );
                    else if (data != null)
                        controlData.continueWith(data);
                    else
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general",
                            data,
                            null
                        ));

                    if (controlData.finally !== undefined)
                        controlData.finally();
                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (error != null && error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                null
                            ));

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ));
                    if (controlData.finally !== undefined)
                        controlData.finally();

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
            OpenIZ.Util.simpleGet('/__auth/get_session', controlData);
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
                success: function (xhr, data)
                {
                    controlData.continueWith(data);

                    if (controlData.finally !== undefined)
                    {
                        controlData.finally();
                    }
                },
                error: function (data)
                {
                    var error = data.responseJSON;

                    if (error != null && error.error !== undefined)
                    {
                        // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error, error.error_description, null));
                    }
                    else
                    {
                        // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error, data, null));
                    }

                    if (controlData.finally !== undefined)
                    {
                        controlData.finally();
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
        * @see {OpenIZ.Ims.get}
        */
        getUserAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "UserEntity",
                query: controlData.query,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally
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
         * @summary Interprets the observation, setting the interpretationConcept property of the observation
         * @param {OpenIZModel.Observation} obs The observation which is to be interpretation
         * @param {string} ruleSet The rule set to be applied for the clinical decision
         * @memberof OpenIZ.CarePlan
         * @method
         */
        interpretObservation: function (obs, ruleSet) {
            obs.interpretationConcept = '41d42abf-17ad-4144-bf97-ec3fd907f57d';
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
         * @param {OpenIZModel.Patient} controlData.data The seed data which should be passed to the forecasting engine in order to calculate the plan
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
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
                url += "&startTime=<" + controlData.onDate.toISOString() + "&stopTime=>" + controlData.onDate.toISOString();
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
                    controlData.continueWith(xhr);
                    if (controlData.finally !== undefined)
                        controlData.finally();
                },
                error: function (data) {
                    var error = data.responseJSON;

                    if (controlData.onException !== undefined) {
                        if (error.error !== undefined) // error
                        {
                            controlData.onException(new OpenIZModel.Exception(error.type, error.error, error.error_description, null));
                        }
                        else {
                            // unknown error
                            controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error, data, null));
                        }
                    }
                    if (controlData.finally !== undefined) {
                        controlData.finally();
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
        getLogInfoAsync: function(controlData) {
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
         * @see {OpenIZModel.ApplicationInfo}
         */
        getInfoAsync: function (controlData) {
            OpenIZ.Util.simpleGet('/__app/info', controlData);
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
         * @summary Indicates whether the status dialog is shown
         * @memberof OpenIZ.App
         */
        statusShown: false,
        /**
         * @summary Resolves the specified template
         * @param {string} templateId The identifier of the template to resolve
         * @return {string} The URL of the form template which is to be used to capture data for the specified template
         */
        resolveTemplate: function (templateId) {
            return OpenIZApplicationService.GetTemplateForm(templateId);
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
         * @summary Show the alert panel
         * @param {String} textStr The text on the alert panel to show
         * @method
         * @memberof OpenIZ.App
         */
        showWait: function (textStr) {

            if (textStr != null)
                $("#waitModalText").text(textStr);
            else
                setTimeout(OpenIZ.App.updateStatus, 6000);

            if (!OpenIZ.App.statusShown) {
                $('#waitModal').modal({ show: true, backdrop: 'static' });
                OpenIZ.App.statusShown = true;
            }
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
        hideWait: function () {
            OpenIZ.App.statusShown = false;
            $('#waitModal').modal('hide');
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
                        controlData.continueWith(xhr);

                    if (controlData.finally !== undefined)
                        controlData.finally();
                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === null)
                        console.error(error);
                    else if (error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                error.error_description,
                                null
                            ));

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                data,
                                null
                            ));

                    if (controlData.finally !== undefined)
                        controlData.finally();
                }
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
                return OpenIZApplicationService.GetString(stringId);
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
            return (navigator.language || navigator.userLanguage).substring(0, 2);
        },
        /**
         * @summary Sets the current user interface locale
         * @memberof OpenIZ.Localization
         * @method
         * @param {String} lcoale The locale to set the user interface to
         * @returns The locale the user interface is now operating in
         */
        setLocale: function (locale) {
            return OpenIZApplicationService.SetLocale(locale);
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
         * @see {OpenIZ.IMS.get}
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
                finally: controlData.finally
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
         * @see {OpenIZ.IMS.get}
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
                finally: controlData.finally
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
                data: controlData.data
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
         * @see {OpenIZ.IMS.get}
         * @see OpenIZModel.Patient
         */
        findAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "Patient",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                query: controlData.query
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
         * @see {OpenIZ.IMS.post}
         * @see OpenIZModel.Patient
         */
        insertAsync: function (controlData) {
            OpenIZ.Ims.post({
                resource: "Patient",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                data: controlData.data
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
         * @see {OpenIZ.IMS.put}
         * @see OpenIZModel.Patient
         */
        updateAsync: function (controlData) {
            OpenIZ.Ims.put({
                resource: "Patient",
                data: controlData.data,
                id: controlData.id,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally
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
         * @see {OpenIZ.IMS.delete}
         * @see OpenIZModel.Patient
         */
        obsoleteAsync: function (controlData) {
            OpenIZ.Ims.delete({
                resource: "Patient",
                id: controlData.id,
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally
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
         * @see {OpenIZ.IMS.get}
         * @see OpenIZModel.Patient
         */
        getAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "Patient",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                finally: controlData.finally,
                query: {
                    _id: controlData.patientId
                }
            });
        },
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
                query: controlData.query
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
                query: controlData.query
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
                finally: controlData.finally
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
                    controlData.continueWith(data);
                },
                onException: controlData.onException,
                finally: controlData.finally
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
                         force: controlData.force
                     },
                     dataType: "json",
                     contentType: 'application/x-www-urlform-encoded',
                     success: function (xhr, data) {
                         if (data != null && data.error !== undefined)
                             controlData.onException(new OpenIZModel.Exception(data.type, data.error),
                                 data.error_description,
                                 null
                             );
                         else if (data != null) {
                             OpenIZ.Configuration.$configuration = xhr;
                             controlData.continueWith(xhr);
                         }
                         else if(controlData.onException != null)
                             controlData.onException(new OpenIZModel.Exception("Exception", "err_general",
                                 data,
                                 null
                             ));

                         if (controlData.finally !== undefined)
                             controlData.finally();
                     },
                     error: function (data) {
                         var error = data.responseJSON;
                         if (error != null && error.error !== undefined) // config error
                             controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                     error.error_description,
                                     null
                                 ));

                         else if(controlData.onException != null) // unknown error
                             controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                     data,
                                     null
                                 ));
                         if (controlData.finally !== undefined)
                             controlData.finally();

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
                if (!confirm('You are about to leave the realm ' + configuration.realm.address + '. Doing so will force the OpenIZ back into an initial configuration mode. Are you sure you want to do this?'))
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
                             controlData.onException(new OpenIZModel.Exception(data.type, data.error),
                                 data.error_description,
                                 null
                             );
                         else if (data != null) {
                             OpenIZ.Configuration.$configuration = xhr;
                             controlData.continueWith(xhr);
                         }
                         else
                             controlData.onException(new OpenIZModel.Exception("Exception", "err_general",
                                 data,
                                 null
                             ));

                         if (controlData.finally !== undefined)
                             controlData.finally();
                     },
                     error: function (data) {
                         var error = data.responseJSON;
                         if (error != null && error.error !== undefined) // config error
                             controlData.onException(new OpenIZModel.Exception(error.type, error.error,
                                     error.error_description,
                                     null
                                 ));

                         else // unknown error
                             controlData.onException(new OpenIZModel.Exception("Exception", "err_general" + error,
                                     data,
                                     null
                                 ));
                         if (controlData.finally !== undefined)
                             controlData.finally();

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
        * @param {String} appletId The identifier of the applet from which the settings should be retrieved
        * @returns A key/value pair representing the applet settings
        */
        getAppletSettings: function (appletId) {
            // TODO: Implement
        },
        /**
        * @summary Saves the applet specific settings in a key/value pair format to the configuration store
        * @memberof OpenIZ.Configuration
        * @method
        * @param {String} appletId The applet identification for which the settings apply
        * @param {Object} settings A key/value pair JSON object of the settings
        * @returns True if the settings save was successful
        */
        saveAppletSettings: function (appletId, settings) {
            // TODO: Implement
        },
        /**
        * @summary Get local user preference strings in a key/value pair JSON object
        * @memberof OpenIZ.Configuration
        * @method
        * @returns The user preferences of the current user
        */
        getUserPreferences: function () {
            // TODO: Implement
        },
        /**
        * @summary Save the user preferences in the key/value pair format
        * @memberof OpenIZ.Configuration
        * @method
        * @param {Object} preferences The user preferences for the current user which should be saved
        * @returns true if the save was successful
        */
        saveUserPreferences: function (preferences) {
            // TODO: Implement
        }
    },
    /**
     * @static
     * @class
     * @summary Provides utility functions for interacting with {@link OpenIZModel.ManufacturedMaterial} instances
     * @memberOf OpenIZ.ManufacturedMaterial
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
                query: controlData.query
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
                data: controlData.data
            });
        }
    }
};
/**
 * @summary Current Locale
 */
//OpenIZ.locale = OpenIZ.Localization.getLocale();
// No caching
$.ajaxSetup({
    cache: false, converters: {
        'text json': $.parseJSON
    },
    beforeSend: function (data, settings) {
        if (OpenIZ.Authentication.$elevationCredentials.$enabled) {
            data.setRequestHeader("Authorization", "BASIC " +
                btoa(OpenIZ.Authentication.$elevationCredentials.userName + ":" + OpenIZ.Authentication.$elevationCredentials.password));
        }
        data.setRequestHeader("X-OIZMagic", OpenIZSessionService.GetMagic());
    }
});

$(document).ajaxError(function (e, data, setting, err) {
    if ((data.status == 401 || data.status == 403) && OpenIZ.Authentication.$session != null ) {
        OpenIZ.Authentication.showElevationDialog();
    }
    else
        throw new OpenIZModel.Exception("Exception", "err_request", err, null);
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

