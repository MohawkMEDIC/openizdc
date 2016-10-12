/// <reference path="openiz-model.js"/>
/// <reference path="~/lib/select2.min.js"/>

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
var OpenIZApplicationService = window.OpenIZApplicationService || {
    GetLocale: function () { return 'en'; },
    GetStrings: function () { return '[]'; },
    GetTemplateForm: function (templateId) {
        switch (templateId) {
            case "Act.Observation.Weight":
                return "/org.openiz.core/views/templates/act.observation.weight.html";
            case "Act.SubstanceAdmin.Immunization":
                return "/org.openiz.core/views/templates/act.substanceadmin.immunization.html";
            default:
                return "../templates/" + templateId + ".html";

        }
    }
};

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
     * @summary Provides operations for managing acts.
     * @class
     */
    Act: {

        /**
         * @summary Performs a patient insert asynchronously
         * @memberof OpenIZ.Act
         * @method
         * @see {OpenIZ.Util.this.startTaskAsync}
         * @param {Object} controlData The data which is used to control the call
         * @example
         * OpenIZ.Act.insertAsync({
         *      patient: new OpenIZModel.Act(...),
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
        * @summary Get an empty template object asynchronously
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
     * @class
     */
    Ims: {
        /**
         * @summary Post data to the IMS
         * @param {Object} control Control data for the async method
         * @param {String} control.resource The resource to post
         * @param {Function} control.continueWith A callback method to be called when the operation completes successfully
         * @param {Function} control.onException A callback method to be called when the operation throws an exception
         * @param {Object} data The post data (IMSI resource) to be posted
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
                        controlData.onException(new OpenIZModel.Exception(error.error,
                                error.error_description,
                                null
                            ));

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("err_general" + error,
                                data,
                                null
                            ));

                    if (controlData.finally !== undefined)
                        controlData.finally();
                }
            });
        },
        /**
        * @summary Post data to the IMS
        * @param {Object} control Control data for the async method
        * @param {String} control.resource The resource to post
        * @param {Function} control.continueWith A callback method to be called when the operation completes successfully
        * @param {Function} control.onException A callback method to be called when the operation throws an exception
        * @param {Object} data The post data (IMSI resource) to be posted
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
                        controlData.onException(new OpenIZModel.Exception(error.error,
                                error.error_description,
                                null
                            ));

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("err_general" + error,
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
         * @param {Object} control Control data for the async method
         * @param {String} control.resource The resource to post
         * @param {Function} control.continueWith A callback method to be called when the operation completes successfully
         * @param {Function} control.onException A callback method to be called when the operation throws an exception
         * @param {Object} query The query data (IMSI resource) to be posted
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
                        controlData.onException(new OpenIZModel.Exception(error.error,
                                error.error_description,
                                null
                            ));
                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("err_general" + error,
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
     * @class
     */

    Stock: {
        calculateQPeriod: function (qYear, pSupply) {
            return qYear / 12 * pSupply;
        },
        calculateSReserve: function (qPeriod, reservePercent) {
            return qPeriod*reservePercent;
        },
        calculateSMax: function (qPeriod, sReserve) {
            return qPeriod + sReserve;
        },
        calculateSReorder: function (sReserve, qPeriod, lTime, pSupply) {
            return sReserve + qPeriod*lTime/pSupply;
        },
        calculateQOrder: function (sMax, sAvailable, qPeriod, lTime, pSupply) {
            return sMax - sAvailable + qPeriod * lTime/pSupply;
        },
        calculateQNeeded: function (sStart, qRecieved, sEnd, sLost) {
            return (sStart+qRecieved) - (sEnd+sLost);
        },
        calculateAll: function(qYear, pSupply, reservePercent, lTime, sAvailable) {
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
     * @summary Utility functions
     * @class
     */
    Util: {
        /**
         * @summary Perform a simple get
         * @method
         * @memberof OpenIZ.Util
         */
        simpleGet : function(url, controlData) {
            controlData.onException = controlData.onException || OpenIZ.Util.logException;
            // Perform auth request
            $.getJSON(url, null, function (data) {

                if (data != null && data.error !== undefined)
                    controlData.onException(new OpenIZModel.Exception(data.error),
                        data.error_description,
                        null
                    );
                else if (data != null) {
                    controlData.continueWith(data);
                }
                else
                    controlData.onException(new OpenIZModel.Exception("err_general",
                        data,
                        null
                    ));
            }).error(function (data) {
                var error = data.responseJSON;
                if (error != null && error.error !== undefined) //  error
                    controlData.onException(new OpenIZModel.Exception(error.error,
                            error.error_description,
                            null
                        ));

                else // unknown error
                    controlData.onException(new OpenIZModel.Exception("err_general" + error,
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
         * @summary Render act
         */
        renderAct : function(act) {
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
         */
        logException: function (e) {
            console.warn(e);
        },
        /** 
         * @summary Render the manufactured material
         */
        renderManufacturedMaterial : function(scope) {
            var name = OpenIZ.Util.renderName(scope.name.OfficialRecord || scope.name.Assigned);
            return name + "[LN#: " + scope.lotNumber + "]";
        },
        /** 
         * @summary Renders the person
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
    * @class
    */
    Authentication: {

        /**
         * @summary Perform a login operation asynchronously
         * @memberof OpenIZ.Authentication
         * @method
         * @see {OpenIZ.Util.this.startTaskAsync}
         * @param {Object} controlData Data which controls the task
         * @see {OpenIZ.Util.this.startTaskAsync}
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
                     grant_type: 'password'
                 },
                 dataType: "json",
                 contentType: 'application/x-www-urlform-encoded',
                 success: function (xhr, data) {
                     if (data != null && data.error !== undefined)
                         controlData.onException(new OpenIZModel.Exception(data.error),
                             data.error_description,
                             null
                         );
                     else if (data != null)
                         controlData.continueWith(data);
                     else
                         controlData.onException(new OpenIZModel.Exception("err_general",
                             data,
                             null
                         ));

                     if (controlData.finally !== undefined)
                         controlData.finally();
                 },
                 error: function (data) {
                     var error = data.responseJSON;
                     if (error != null && error.error !== undefined) // oauth 2 error
                         controlData.onException(new OpenIZModel.Exception(error.error,
                                 error.error_description,
                                 null
                             ));

                     else // unknown error
                         controlData.onException(new OpenIZModel.Exception("err_general" + error,
                                 data,
                                 null
                             ));
                     if (controlData.finally !== undefined)
                         controlData.finally();

                 }
             });
        },
        /**
         * @summary Set password asynchronously
         * @memberof OpenIZ.Authentication
         * @method
         * @param {Object} controlData The control data for the task
         * @see {OpenIZ.Util.this.startTaskAsync}
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
                        controlData.onException(new OpenIZModel.Exception(data.error),
                            data.error_description,
                            null
                        );
                    else if (data != null)
                        controlData.continueWith(data);
                    else
                        controlData.onException(new OpenIZModel.Exception("err_general",
                            data,
                            null
                        ));

                    if (controlData.finally !== undefined)
                        controlData.finally();
                },
                error: function (data) {
                    var error = data.responseJSON;
                    if (error != null && error.error !== undefined) // oauth 2 error
                        controlData.onException(new OpenIZModel.Exception(error.error,
                                error.error_description,
                                null
                            ));

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("err_general" + error,
                                data,
                                null
                            ));
                    if (controlData.finally !== undefined)
                        controlData.finally();

                }
            });
        },
        /**
        * @summary Gets the current session from the client host
        * @memberof OpenIZ.Authentication
        * @method
        * @returns An instance of Session representing the current session
        */
        getSessionAsync: function (controlData) {
            controlData.onException = controlData.onException || OpenIZ.Util.logException;
            // Perform auth request
            $.getJSON('/__auth/get_session', null, function (data) {
                if (data != null && data.error !== undefined)
                    controlData.onException(new OpenIZModel.Exception(data.error),
                        data.error_description,
                        null
                    );
                else if (data != null)
                    controlData.continueWith(data);
                else
                    controlData.onException(new OpenIZModel.Exception("err_general",
                        data,
                        null
                    ));
            }).error(function (data) {
                var error = data.responseJSON;
                if (error != null && error.error !== undefined) // oauth 2 error
                    controlData.onException(new OpenIZModel.Exception(error.error,
                            error.error_description,
                            null
                        ));

                else // unknown error
                    controlData.onException(new OpenIZModel.Exception("err_general" + error,
                            data,
                            null
                        ));
            }).always(function () {
                if (controlData.finally !== undefined)
                    controlData.finally();
            });

        },
        /**
         * @summary Destroys the current session
         * @memberof OpenIZ.Authentication
         * @method
         */
        abandonSession: function () {
            try {
                OpenIZSessionService.Abandon();
                return true;
            }
            catch (ex) {
                console.error(ex);
                return false;
            }
        },
        /** 
         * @summary Refreshes the current session asynchronously
         * @memberof OpenIZ.Authentication
         * @method
         * @param {Object} controlData Task control data
         * @see {OpenIZ.Util.this.startTaskAsync}
         * @example
         * OpenIZ.Authentication.refreshSessionAsync({
         *      continueWith: function(r) { // do something with result },
         *      onException: function(ex) { // handle exception }
         * });
         */
        refreshSessionAsync: function (controlData) {
        },

        /**
        * @summary Finds a user by username.
        * @memberof OpenIZ.Authentication
        * @method
        */
        getUserAsync: function(controlData) {
            $.ajax(
            {
                method: "POST",
                url: "/__auth/get_user",
                data: controlData.data,
                contentType: "application/json; charset=UTF-8",
                success: function (xhr, data)
                {
                    if (data != null && data.error !== undefined)
                    {
                        controlData.onException(new OpenIZModel.Exception(data.error), data.error_description, null);
                    }
                    else if (data != null)
                    {
                        controlData.continueWith(data);
                    }
                    else
                    {
                        controlData.onException(new OpenIZModel.Exception("err_general", data, null));
                    }

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
                        controlData.onException(new OpenIZModel.Exception(error.error, error.error_description, null));
                    }
                    else
                    {
                        // unknown error
                        controlData.onException(new OpenIZModel.Exception("err_general" + error, data, null));
                    }

                    if (controlData.finally !== undefined)
                    {
                        controlData.finally();
                    }

                }
            });
        },
    },
    /** 
     * @summary Represents functions for interacting with the protocol service
     * @class
     */
    CarePlan: {
        /**
         * @summary Interprets the observation 
         */
        interpretObservation : function(obs, ruleSet) {
            obs.interpretationConcept = '41d42abf-17ad-4144-bf97-ec3fd907f57d';
        },
        /**
         * @summary Generate a care plan for the specified patient
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
            if (controlData.moodConcept !== undefined)
                url += "&moodConcept=" + controlData.moodConcept;
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

                    if (controlData.onException !== undefined)
                    {
                        if (error.error !== undefined) // error
                        {
                            controlData.onException(new OpenIZModel.Exception(error.error, error.error_description, null));
                        }
                        else
                        {
                            // unknown error
                            controlData.onException(new OpenIZModel.Exception("err_general" + error, data, null));
                        }
                    }
                    if (controlData.finally !== undefined)
                    {
                        controlData.finally();
                    }
                }
            });
        },
        /**
         * @summary Get an empty template object asynchronously
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
        /**
         * @summary Get an empty template object asynchronously
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
    * @summary Represents application specific functions for interoperating with the mobile application itself
    * @class
    */
    App: {
        /**
         * @summary Create new guid
         */
        newGuid: function () {
            return OpenIZApplicationService.NewGuid();
        },
        /**
         * @summary Get application information data using typical async information parameters
         */
        getInfoAsync : function(controlData) {
            $.getJSON('/__app/info', null, function (e) { controlData.continueWith(e); if (controlData.finally !== undefined) controlData.finally(); }).
                fail(function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === undefined || error == null)
                        console.error(error);
                    else if (error.error !== undefined) // structured error
                        controlData.onException(new OpenIZModel.Exception(error.error,
                                error.error_description,
                                null
                            ));
                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("err_general" + error,
                                data,
                                null
                            ));

                    if (controlData.finally !== undefined)
                        controlData.finally();
                });
        },
        /**
         * @summary Get the online state of the application
         */
        getOnlineState: function () {
            return OpenIZApplicationService.GetOnlineState();
        },
        /**
         * @summary Indicates whether the status dialog is shown
         */
        statusShown: false,
        /**
         * @summary Resolves the specified template
         */
        resolveTemplate: function (templateId) {
            return OpenIZApplicationService.GetTemplateForm(templateId);
        },
        /**
         * @summary Update the status of the 
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
         */
        networkAvailable: function () {
            try { return OpenIZApplicationService.GetNetworkState(); }
            catch (e) { return false; }
        },
        /**
         * @summary Hide the waiting panel
         */
        hideWait: function () {
            OpenIZ.App.statusShown = false;
            $('#waitModal').modal('hide');
        },
        /**
         * @summary Gets the specified service implementation in memory
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
         */
        getVersion: function () {
            return OpenIZApplicationService.GetVersion();
        },
        /**
         * @summary Get a list of all log files
         */
        getLogFiles: function () {
            return JSON.parse(OpenIZApplicationService.GetLogFiles());
        },
        /**
         * @summary Get a list of all log files
         */
        sendLog: function (logId) {
            OpenIZApplicationService.SendLog(logId);
        },
        /**
         * @summary Gets the currebt asset title
         * @memberof OpenIZ.App
         * @method
         * @returns {String} The title of the applet
         */
        getCurrentAssetTitle: function () {
            return $(document).find("title").text();
        },
        /**
         * @summary Get the application menu items that the user has access to
         * @memberof OpenIZ.App
         * @method
         * @returns {Object} Representing the menu items the user has access to
         */
        getMenus: function () {
            try {

                var data = OpenIZApplicationService.GetMenus();
                if (data == null)
                    return {};
                else if (data.lastIndexOf("err", 0) == 0)
                    throw data;
                else
                    return JSON.parse(data)
            }
            catch (e) {
                console.error(e);
                //throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_get_menus"), e.message, e);
            }
        },
        /**  
         * @summary Get the menus async
         * @method
         * @memberof OpenIZ.App
         * @returns {Object} Representing menu items the current user has access to
         */
        getMenusAsync: function(controlData) {
            OpenIZ.Util.simpleGet("/__app/menu", controlData);

        },
        /**
         * @summary Uses the device camera to scan a barcode from the device
         * @memberof OpenIZ.App
         * @method
         * @returns The value of the barcode detected by the scanner
         */
        scanBarcode: function () {
            try {
                var value = OpenIZApplicationService.BarcodeScan();
                console.log('Barcode scan complete. Data: ' + value);
                return value;
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_scan_barcode"), e.message, e);
            }
        },
        /**
         *@summary  Navigate backward, even if the back functionality crosses applets
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
         * @param {Object} queryData The alert query to pass
         */
        getAlertsAsync: function (controlData) {
            // Perform auth request
            $.getJSON('/__app/alerts', controlData.query, function (e) { controlData.continueWith(e); }).
                fail(function (data) {
                    var error = data.responseJSON;
                    if (controlData.onException === undefined || error == null)
                        console.error(error);
                    else if (error.error !== undefined) // structured error
                        controlData.onException(new OpenIZModel.Exception(error.error,
                                error.error_description,
                                null
                            ));
                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("err_general" + error,
                                data,
                                null
                            ));
                });
        },
        /**
         * @summary Send an asynchronous alert
         * @param {Object} alertData The alert data
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
                        controlData.onException(new OpenIZModel.Exception(error.error,
                                error.error_description,
                                null
                            ));

                    else // unknown error
                        controlData.onException(new OpenIZModel.Exception("err_general" + error,
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
     * @class
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
                throw new OpenIZModel.Exception("Error getting string list", e.message, e);
            }

        }
    },

    /**
     * @summary Represents functions related to the concept dictionary
     * @class
     */
    Concept: {
        /**
         * @summary Perform a search asynchronously
         * @memberof OpenIZ.Concept
         * @method
         * @param {Object} searchData An object containing search, offset, count and callback data
         * @see {OpenIZ.Util.this.startTaskAsync}
         * @example
         * OpenIZ.Concept.findConceptAsync({
         *      query: "mnemonic=Female&_expand=name",
         *      offset: 0,
         *      count: 0,
         *      continueWith: function(result) { // do something with result },
         *      onException: function(ex) { // handle exception }
         *  });
         */
        findConceptAsync: function (controlData) {

            // Perform async operation
            OpenIZ.Util.startTaskAsync(function () {
                var result = {};
                if (controlData.query != "")
                    return OpenIZ.Concept.findConcept(controlData.query, controlData.offset, controlData.count);
                return result;
            }, controlData);
        },
        /**
         * @summary Searches the concept source 
         * @memberof OpenIZ.Concept
         * @method
         * @param {String} imsiQuery The IMSI formatted query
         * @param {Numeric} offset The offset of the search result set
         * @param {Numeric} count The total requested numer in the result set
         * @param {Boolean} returnBundle When true, return a bundle
         * @returns {OpenIZModel.Bundle} The matching bundle containing the results of the query
         */
        findConcept: function (imsiQuery, offset, count) {
            try {
                var data = OpenIZConceptService.SearchConcept(imsiQuery, offset, count);

                if (data == null)
                    return null;
                else {
                    var retVal = new OpenIZModel.Bundle(JSON.parse(data));
                    return retVal;
                }
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_get_concept"), e.message, e);

            }
        },
        /**
         * @summary Finds the specified concept in an asynchronous manner
         * @memberof OpenIZ.Concept
         * @method
         * @param {Object} controlData The data to control the function
         * @see {OpenIZ.Util.this.startTaskAsync}
         * @example
         *  OpenIZ.Concept.findConceptSetAsync({
         *      query: "mnemonic=ActClass",
         *      offset: 0,
         *      count: -1,
         *      continueWith: function(d) { // Do something with result },
         *      onException: function(ex) { // handle exception }
         * });
         */
        findConceptSetAsync: function (controlData) {
            OpenIZ.Util.startTaskAsync(function () {
                if (controlData.query != "")
                    return OpenIZ.Concept.findConceptSet(controlData.query, controlData.offset, controlData.count);
                return {};
            }, controlData);
        },
        /**
         * @summary Searches the specified values of concepts from the concept set.
         * @memberof OpenIZ.Concept
         * @method
         * @param {String} setMnemonic The name of the concept set to retrieve (Ex: AdministrativeGender)
         * @returns {OpenIZModel.Bundle} The matching bundle containing the results of the query
         * @param {Numeric} offset The offset of the search result set
         * @param {Numeric} count The total requested numer in the result set
         */
        findConceptSet: function (imsiQuery, offset, count) {
            try {
                var data = OpenIZConceptService.SearchConceptSet(imsiQuery, offset, count);
                if (data == null || data.lastIndexOf("err", 0) == 0)
                    throw new OpenIZModel.Exception(data, null, null);
                else {
                    var results = JSON.parse(data);

                    var retVal = new OpenIZModel.Bundle(results);
                    return retVal;
                }
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_get_concept_set"), e.message, e);
            }
        },
        /**
         * @summary Gets the specified concept in an asynchronouns manner
         * @memberof OpenIZ.Concept
         * @method
         * @param {Object} controlData The control data for the retrieve
         * @see {OpenIZ.Util.this.startTaskAsync}
         * @example
         * OpenIZ.Concept.getConceptAsync({
         *      id: "<<UUID>>",
         *      continueWith: function(result) { // Do something with result },
         *      onException: function(ex) { // Do something on exception }
         * });
         */
        getConceptAsync: function (controlData) {
            OpenIZ.Util.startTaskAsync(function () {
                return OpenIZ.Concept.getConcept(controlData.id);
            }, controlData);
        },
        /**
         * @summary Gets the specified concept with the specified identifier
         * @memberof OpenIZ.Concept
         * @method
         * @param {String} conceptId The identifier of the concept to retreive
         * @returns {OpenIZModel.Concept} The concept which has the specified identifier
         */
        getConcept: function (conceptId) {
            try {
                var results = OpenIZ.Concept.findConcept("_expand=name&_expand=classConcept&_expand=statusConcept&id=" + conceptId, 0, 1);

                if (results.length == 0)
                    return null;
                else
                    return results.first("Concept");
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_get_concept"), e.message, e);
            }
        }
    },

    /**
     * @summary Represents a series of functions for submitting bundles
     * @class
     */
    Bundle: {
        /**
         * @summary Insert the bundle asynchronously
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
     * @summary Represents a series of functions related to patients
     * @class
     */
    Patient: {
        /**
         * @summary Perform a patient search asynchronously
         * @memberof OpenIZ.Patient
         * @method
         * @see {OpenIZ.Util.this.startTaskAsync}
         * @param {Object} controlData The data to control the query
         * @example
         * OpenIZ.Patient.findAsync({
         *      query: "name[Legal].component[Family].value=Smith&name[Legal].component[Given].value=John",
         *      offset: 0,
         *      count: -1,
         *      continueWith: function(result) { // Do something with result },
         *      onException: function(ex) { // Do something on exception }
         * });
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
         * @summary Query the OpenIZ data store for patients matching the specified query string. The query string should be
         * an IMS query format string like name[L].part[FAM].value=Smith&name[L].part[GIV].value=John
         * @memberof OpenIZ.Patient
         * @method
         * @param {String} searchString The IMSI search string to be searched
         * @param {Numeric} offset The offset of the search result set
         * @param {Numeric} count The total requested numer in the result set
         * @returns {OpenIZModel.Bundle} A bundle of {OpenIZModel.Patient} classes which represent the search results.
         */
        find: function (searchString, offset, count) {
            try {
                var data = OpenIZPatientService.Search(searchString, offset, count);
                if (data.lastIndexOf("err", 0) == 0)
                    throw new OpenIZModel.Exception(data, null, null);
                else {
                    var results = JSON.parse(data);

                    // Convert the IMSI patient data to a nicer javascript format
                    return new OpenIZModel.Bundle(results);
                }
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_search_patient"), e.message, e);

            }
        },
        /**
         * @summary Performs a patient insert asynchronously
         * @memberof OpenIZ.Patient
         * @method
         * @see {OpenIZ.Util.this.startTaskAsync}
         * @param {Object} controlData The data which is used to control the call
         * @example
         * OpenIZ.Patient.insertAsync({
         *      patient: new OpenIZModel.Patient(...),
         *      continueWith: function(result) { // Do something with result },
         *      onException: function(ex) { // Handle exception }
         * });
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
         * @summary Register a patient in the IMS system returning the registered patient data
         * @memberof OpenIZ.Patient
         * @method
         * @param {OpenIZModel.Patient} patient The patient to be insterted
         * @throw Exception if the patient is already registered
         * @returns {OpenIZModel.Patient} The registered patient data
         */
        insert: function (patient) {
            try {
                if (patient["$type"] != "Patient")
                    throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_invalid_argument"), typeof (patient), null);
                var imsiJson = JSON.stringify(patient);
                var data = OpenIZPatientService.Insert(imsiJson);

                if (data.lastIndexOf("err", 0) == 0)
                    throw new OpenIZModel.Exception(OpenIZ.Localization.getString(data));
                else
                    return new OpenIZModel.Patient(JSON.parse(data));
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_insert_patient"), e.message, e);
            }
        },
        /**
         * @summary Performs a patient update asynchronously
         * @memberof OpenIZ.Patient
         * @method
         * @see {OpenIZ.Util.this.startTaskAsync}
         * @param {Object} controlData The data which is used to control the call
         * @example
         * OpenIZ.Patient.updateAsync({
         *      patient: new OpenIZModel.Patient(...),
         *      continueWith: function(result) { // Do something with result },
         *      onException: function(ex) { // Handle exception }
         * });
         */
        updateAsync: function (controlData) {
            OpenIZ.Util.startTaskAsync(function () {
                return OpenIZ.Patient.update(controlData.patient);
            }, controlData);
        },
        /**
         * @summary Updates the specified patient instance with the specified data
         * @memberof OpenIZ.Patient
         * @method
         * @param {OpenIZModel.Patient} patient The patient to be updated, including their primary identifier key
         * @throw Exception if the patient does not exist
         * @returns {OpenIZModel.Patient} The updated patient data
         */
        update: function (patient) {
            try {
                if (patient["$type"] != "Patient")
                    throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_invalid_argument"), typeof (patient), null);
                else if (patient.id == null)
                    throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_update_no_key"), typeof (patient), null);

                var imsiJson = JSON.stringify(patient);
                return new OpenIZModel.Patient(JSON.parse(OpenIZPatientService.Update(imsiJson)));
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_update_patient"), e.message, e);

            }
        },
        /**
         * @summary Performs a patient update asynchronously
         * @memberof OpenIZ.Patient
         * @method
         * @see {OpenIZ.Util.this.startTaskAsync}
         * @param {Object} controlData The data which is used to control the call
         * @example
         * OpenIZ.Patient.obsoleteAsync({
         *      patientId: <<GUID>>, 
         *      continueWith: function(result) { // Do something with result },
         *      onException: function(ex) { // Handle exception }
         * });
         */
        obsoleteAsync: function (controlData) {
            OpenIZ.Util.startTaskAsync(function () {
                return OpenIZ.Patient.obsolete(controlData.patientId);
            }, controlData);
        },
        /**
         * @summary Obsoletes the specified patient instance in the IMS data
         * @memberof OpenIZ.Patient
         * @method
         * @param {String} patientId The unique identifier of the patient to be obsoleted
         * @throw Exception if the patient does not exist
         * @returns The obsoleted patient instance
         */
        obsolete: function (patientId) {
            try {
                return new OpenIZModel.Patient(JSON.parse(OpenIZPatientService.Obsolete(patientId)));
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_obsolete_patient"), e.message, e);
            }
        },
        /**
         * @summary Performs a patient get asynchronously
         * @param {Object} controlData The data which is used to control the call
         * @memberof OpenIZ.Patient
         * @see {OpenIZ.Util.this.startTaskAsync}
         * @method
         * @example
         * OpenIZ.Patient.getAsync({
         *      patientId:<<GUID>>,
         *      continueWith: function(result) { // Do something with result },
         *      onException: function(ex) { // Handle exception }
         * });
         */
        getAsync: function (controlData) {
            OpenIZ.Ims.get({
                resource: "Patient",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                query: {
                    id: controlData.patientId
                }
            });
        },
        /** 
         * @summary Retrieves the specified patient instance from the IMS datastore
         * @memberof OpenIZ.Patient
         * @method
         * @param {String} patientId The unique identifier of the patient to be retrieved
         * @returns {OpenIZModel.Patient} The retrieved patient instance if exists, null if not found
         */
        get: function (patientId) {
            try {
                var results = OpenIZ.Patient.find("_all=true&id=" + patientId, 0, 1);

                if (results.length == 0)
                    return null;
                else
                    return results.first("Patient");
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_get_patient"), e.message, e);
            }
        }
    },


    /**
     * @summary Place functions
     */
    Place: {
        /**
         * @summary Find places async
         */
        findAsync : function(controlData) {
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
         * @param {Element} target The element to be bound to
         * @param {String} filter The filter to show (to be added to the current name filter)
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
    Provider: {
        /**
         * @summmary Find the provider asynchronously
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
         * @summary Bind a Person filter to a select box
         * @param {Element} target The element to be bound to
         * @param {String} filter The filter to show (to be added to the current name filter)
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
    * @summary The configuration property is used to segregate the functions related to configuration of the main OpenIZ system including realm, updating configuration, etc.
    * @class
    */
    Configuration: {
        $configuration : null,
        /** 
         * @summary Get the configuration file in an async manner
         * @method
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
                throw new OpenIZModel.Exception(e.message, e.detail, e);
            }
        },
        /**
         * @summary Gets the specified appplication setting
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
                throw new OpenIZModel.Exception(e.message, e.detail, e);
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
        * @param {Object} sectionName The name of the section which should be retrieved.
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
        * @param {String} address The address root of the realm. Example: demo.openiz.org
        * @param {String} deviceName A unique name for the device which is being joined
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
                         deviceName: controlData.deviceName
                     },
                     dataType: "json",
                     contentType: 'application/x-www-urlform-encoded',
                     success: function (xhr, data) {
                         if (data != null && data.error !== undefined)
                             controlData.onException(new OpenIZModel.Exception(data.error),
                                 data.error_description,
                                 null
                             );
                         else if (data != null) {
                             OpenIZ.Configuration.$configuration = xhr;
                             controlData.continueWith(xhr);
                         }
                         else
                             controlData.onException(new OpenIZModel.Exception("err_general",
                                 data,
                                 null
                             ));

                         if (controlData.finally !== undefined)
                             controlData.finally();
                     },
                     error: function (data) {
                         var error = data.responseJSON;
                         if (error != null && error.error !== undefined) // config error
                             controlData.onException(new OpenIZModel.Exception(error.error,
                                     error.error_description,
                                     null
                                 ));

                         else // unknown error
                             controlData.onException(new OpenIZModel.Exception("err_general" + error,
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
        * @memberof OpenIZ.Configuration
        * @method
        * @param {Object} configuration The configuration data
        * @param {String} configuration.retention all|local|none - The retention policy of all data in realm, local data only to tablet, or no offline data
        * @param {String} configuration.authentication  online|offline The authentication policy of either must be online or offline allowed
        * @param {String} configuration.subscribe Represents subscription queries which should be used to determine what should be in the outbox
        * @param {String} configuration.enableForgotPassword true|false Whether users can reset their password
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
                             controlData.onException(new OpenIZModel.Exception(data.error),
                                 data.error_description,
                                 null
                             );
                         else if (data != null) {
                             OpenIZ.Configuration.$configuration = xhr;
                             controlData.continueWith(xhr);
                         }
                         else
                             controlData.onException(new OpenIZModel.Exception("err_general",
                                 data,
                                 null
                             ));

                         if (controlData.finally !== undefined)
                             controlData.finally();
                     },
                     error: function (data) {
                         var error = data.responseJSON;
                         if (error != null && error.error !== undefined) // config error
                             controlData.onException(new OpenIZModel.Exception(error.error,
                                     error.error_description,
                                     null
                                 ));

                         else // unknown error
                             controlData.onException(new OpenIZModel.Exception("err_general" + error,
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
     * @class
     * @summary Manufactured material class
     */
    ManufacturedMaterial:
    {
        getManufacturedMaterials: function (controlData)
        {
            OpenIZ.Ims.get({
                resource: "ManufacturedMaterial",
                continueWith: controlData.continueWith,
                onException: controlData.onException,
                query: controlData.query
            })
        }
    },
    /**
     * @class
     * @summary Security repository class
     */
    UserEntity: {
        /**
         * @method
         * @summary Updates the specified user entity asynchronously
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
    }
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

