/// <reference path="openiz-model.js"/>

/**
 * @summary OpenIZ Javascript binding class.
 *
 * @description The purpose of this object is to facilitate and organize OpenIZ applet integration with the backing  * OpenIZ container. For example, to allow an applet to get the current on/offline status, or authenticate a user.
 * @class
 * @property {Object} Authentication Represents a static member to access authentication functions such as login, logout, etc.
 * @property {Object} Concept Represents a static member to access concept utility functions
 * @property {Object} Patient Represents static member to access patient utility data
 * @property {Object} Configuration Represents a static member to access configuration functions
 * @property {Object} App Represents a static member to access application utilities such as toasts, closing windows, etc.
 * @property {Object} _session the current session
 * @property {Object} Localization Represents a static member to access localization fields
 * @property {Object} Util Represents a static member to access utility functions
 * @property {Object} urlParams Represents URL parameters passed to the applet
 */
var OpenIZ = new function () {

    this.urlParams = {};
    /**
     * @summary Utility functions
     * @class
     */
    this.UtilClass = function () {
        /**
         * @summary Changes the specified date string into an appropriate ISO string
         * @param {String} date The date to be formatted
         */
        this.toDateInputString = function (date) {
            return date.toISOString().substring(0, 10)
        };
    };

    /** 
    * @summary The authentication section is used to interface with OpenIZ's authentication sub-systems including session management information, etc.
    * @class
    */
    this.AuthenticationUtil = function() {
        /**
        * @summary Performs a login with the authentication service returning the active Session object if applicable
        * @param {String} userName the name of the user to authenticate
        * @param {String} password The user's password
        * @returns A new OpenIZ.Session object with the current session information if successful, null if not
        * @throws An applicable exception for the validation error.
        */
        this.login = function (userName, password) {
            try
            {
                
                var data = OpenIZSessionService.Login(userName, password);

                if (data == null)
                    return null;
                else if (data.lastIndexOf("err", 0) == 0)
                    throw new OpenIZModel.Exception(OpenIZ.Localization.getString(data), null, null);
                else
                {
                    var pData = JSON.parse(data);
                    if (pData != null && pData.error !== undefined)
                        throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_" + pData.error),
                            pData.error_description, 
                            null
                        );
                    else if (data != null)
                        return new OpenIZModel.Session(data);
                    else
                        return null;
                    
                }
            }
            catch(ex)
            {
                console.warn(ex);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_login"), ex.error, ex);
            }
        };
        /**
        * @summary Sets the password for the specified user to some other password. 
        * Note: You will need to have the ChangePassword policy or be changing the password of the currently 
        * logged in user or else this function will return an error
        * @param {String} userName The name of the user to which the password change applies
        * @param {String} password The password of the user.
        * @returns True if the password was successfully changed, false otherwise
        */
        this.setPassword = function (userName, password) {
            // TODO: Implement
        };
        /**
        * @summary Performs a two-factor authentication login. 
        * Note: In the term of "forgot password" the back-end may require just the SMS short code in tfaSecret
        * @param {String} userName The name of the user being logged in
        * @param {String} password The password of the user being logged in
        * @param {String} tfaSecret The two-factor secret (SMS CODE, E-MAIL Code, etc.)
        * @returns The granted session object if the login was successful
        */
        this.loginEx = function (userName, password, tfaSecret) {
            // TODO: Implement
        };
        /**
        * @summary Registers the specified user data 
        * @param {String} userName The desired user name for the user
        * @param {String} password The password the user desires
        * @param {Object} profileData The data (instance of OpenIZUser class) which contains the user's data
        * @returns The user profile
        */
        this.register = function (userName, password, profileData) {
            // TODO: Implement
        };
        /**
        * @summary Gets the current session from the client host
        * @returns An instance of Session representing the current session
        */
        this.getSession = function () {
            try {
                var data = OpenIZSessionService.GetSession();
                if (data != null)
                    return new OpenIZModel.Session(JSON.parse(data));
                return null;
            }
            catch (ex) {
                console.error(ex);
                return null;
            }
        };
        /**
         * @summary Destroys the current session
         */
        this.abandonSession = function () {
            try {
                OpenIZSessionService.Abandon();
                return true;
            }
            catch (ex) {
                console.error(ex);
                return false;
            }
        };
        /** 
         * @summary Refreshes the current session so that the token remains valid
         * @returns The newly created session
         */
        this.refreshSession = function () {
            try {
                if (OpenIZ.Authentication.getSession() == null)
                    throw new OpenIZModel.Exception(
                        OpenIZ.Localization.getString("err_no_session"),
                        OpenIZ.Localization.getString("err_no_session_detail"),
                        null
                    );
                // Refresh the specified session data
                var data = OpenIZSessionService.Refresh();

                if (data == null)
                    return null;
                else if (data.lastIndexOf("err", 0) == 0)
                    throw new OpenIZModel.Exception(OpenIZ.Localization.getString(data), null, null);
                else {
                    alert(data);
                    var pData = JSON.parse(data);
                    if (pData != null && pData.error !== undefined)
                        throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_" + pData.error),
                            pData.error_description,
                            null);
                    else if (data != null)
                        return new OpenIZModel.Session(data);
                    else
                        return null;

                }
            }
            catch (ex) {
                console.warn(ex);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_refresh_session"), ex.message, ex);
            }
        };
    };

    /**
    * @summary Represents application specific functions for interoperating with the mobile application itself
    * @class
    */
    this.AppUtil = function() {
        /**
         * @summary Uses the device camera to scan a barcode from the device
         * @returns The value of the barcode detected by the scanner
         */
        this.scanBarcode = function () {
            try {
                var value = OpenIZApplicationService.BarcodeScan();
                console.log('Barcode scan complete. Data: ' + value);
                return value;
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_scan_barcode"), e.message, e);
            }
        };
        /**
         *@summary  Navigate backward, even if the back functionality crosses applets
         */
        this.back = function () {
            try {
                var value = OpenIZApplicationService.Back();
            }
            catch (e) {
                console.error(e);
            }
        };
        /**
         * @summary Closes the applet and kills the Android view
         */
        this.close = function () {
            try {
                var value = OpenIZApplicationService.Close();
            }
            catch (e) {
                console.error(e);
            }
        };
        /**
         * @summary Displays a TOAST on the user's screen
         * @param {String} text The text of the toast to be shown
         */
        this.toast = function (text) {
            try {
                var value = OpenIZApplicationService.ShowToast(text);
            }
            catch (e) {
                console.error(e);
            }
        };
        /**
         * @summary Navigates to the specified applet passing any context variables to it via "context"
         * @param {String} appletId The ID of the applet to be navigated
         * @param {Object} context Any context variables to be passed to the applet
         */
        this.navigateApplet = function (appletId, context) {
            try {
                OpenIZApplicationService.Navigate(appletId, JSON.stringify(context));
            }
            catch (e) {
                console.error(e);
            }
        };
    };

    /**
     * @summary Represents functions related to the localization of applets
     * @class
     */
    this.LocalizationUtil = function() {
        /**
         * @summary Gets the specified localized string the current display language from the resources file
         * @param {String} stringId The identifier of the string
         * @returns The specified string
         */
        this.getString = function (stringId) {
            try {
                return OpenIZApplicationService.GetString(stringId);
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_get_string"), e.message, e);
            }
        };
        /**
         * @summary Gets the current user interface locale name
         * @returns The ISO language code of the current UI 
         */
        this.getLocale = function () {
            return OpenIZApplicationService.GetLocale();
        };
        /**
         * @summary Sets the current user interface locale
         * @param {String} lcoale The locale to set the user interface to
         * @returns The locale the user interface is now operating in
         */
        this.setLocale = function (locale) {
            return OpenIZApplicationService.SetLocale(locale);
        };
    };

    /**
     * @summary Represents functions related to the concept dictionary
     * @class
     */
    this.ConceptUtil = function() {
        /**
         * @summary Gets the concept by identifier
         * @param {String} conceptId The identifier of the concept to retrieve
         * @returns The retrieved concept
         */
        this.getConcept = function(conceptId)
        {
            try {
                var data = OpenIZConceptService.GetConcept(conceptId);

                if (data == null)
                    return null;
                else
                    return new OpenIZModel.Concept(JSON.parse(data));
            }
            catch(e)
            {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_get_concept"), e.message, e);

            }
        };
        /**
         * @summary Gets the specified values of concepts from the concept set.
         * @param {String} setName The name of the concept set to retrieve (Ex: AdministrativeGender)
         * @returns A list of {OpenIZModel.Cocnept} objects which represent the concepts
         */
        this.getConceptSet = function (setName) {
            try {
                var data = OpenIZConceptService.GetConceptSet(setName);

                if (data == null)
                    return null;
                else {
                    var results = JSON.parse(data);
                    var retVal = [];
                    for (var r in results)
                        retVal.push(new OpenIZModel.ConceptSet(r));
                    return results;
                }
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_get_concept_set"), e.message, e);


            }
        };
    };
    


    /**
     * @summary Represents a series of functions related to patients
     * @class
     */
    this.PatientUtil = function() {

        /**
         * @summary Query the OpenIZ data store for patients matching the specified query string. The query string should be
         * an IMS query format string like name[L].part[FAM].value=Smith&name[L].part[GIV].value=John
         * @param {String} searchString The IMSI search string to be searched
         * @returns An array of {OpenIZModel.Patient} classes which represent the search results.
         */
        this.search = function (searchString) {
            try {
                var results = JSON.parse(OpenIZPatientService.Find(searchString));

                // Convert the IMSI patient data to a nicer javascript format
                var retVal = [];
                for (var i = 0; i < results.length; i++)
                    retVal.push(new OpenIZModel.Patient(result[i]));

                return retVal;
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_search_patient"), e.message, e);

            }
        };
        /**
         * @summary Register a patient in the IMS system returning the registered patient data
         * @param {Object} patient The patient to be insterted
         * @throw Exception if the patient is already registered
         * @returns The registered patient data
         */
        this.insert = function (patient) {
            try
            {
                if (typeof (patient) != "Patient")
                    throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_invalid_argument"), typeof(patient), null);

                var imsiJson = JSON.stringify(patient.toImsi());
                // log the imsi
                console.info(imsiJson);
                return new OpenIZModel.Patient(JSON.parse(OpenIZPatientService.Insert(imsiJson)));
            }
            catch(e)
            {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_insert_patient"), e.message, e);

            }
        };
        /**
         * @summary Updates the specified patient instance with the specified data
         * @param {Object} patient The patient to be updated, including their primary identifier key
         * @throw Exception if the patient does not exist
         * @returns The updated patient data
         */
        this.update = function (patient) {
            try
            {
                if (typeof (patient) != "Patient")
                    throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_invalid_argument"), typeof(patient), null);
                else if (patient.key == null)
                    throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_update_no_key"), typeof(patient), null);

                var imsiJson = JSON.stringify(patient.toImsi());
                console.info(imsiJson);
                return new OpenIZModel.Patient(JSON.parse(OpenIZPatientService.Update(imsiJson)));
            }
            catch(e)
            {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_update_patient"), e.message, e);

            }
        };
        /**
         * @summary Obsoletes the specified patient instance in the IMS data
         * @param {String} patientId The unique identifier of the patient to be obsoleted
         * @throw Exception if the patient does not exist
         * @returns The obsoleted patient instance
         */
        this.obsolete = function (patientId) {
            try
            {
                return new OpenIZModel.Patient(JSON.parse(OpenIZPatientService.Obsolete(patientId)));
            }
            catch(e)
            {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_obsolete_patient"), e.message, e);
            }
        };
        /** 
         * @summary Retrieves the specified patient instance from the IMS datastore
         * @param {String} patientId The unique identifier of the patient to be retrieved
         * @returns The retrieved patient instance if exists, null if not found
         */
        this.get = function (patientId) {
            try {
                var results = this.search("key=" + patientId);
                if (results.length == 0)
                    return null;
                else
                    return results[0];
            }
            catch (e) {
                console.error(e);
                throw new OpenIZModel.Exception(OpenIZ.Localization.getString("err_get_patient"), e.message, e);
            }
        };
    };

    /**
    * @summary The configuration property is used to segregate the functions related to configuration of the main OpenIZ system including realm, updating configuration, etc.
    * @class
    */
    this.ConfigurationUtil = function() {
        /**
        * @summary Gets the current realm to which the client is connected
        */
        this.getRealm = function () {
            return OpenIZ.Configuration.getSection("SecurityConfigurationSection").domain;
        };
        /**
        * @summary Gets the specified OpenIZ configuration section name. 
        * @returns A JSON object representing the configuration data for the particular section
        * @param {Object} sectionName The name of the section which should be retrieved.
        */
        this.getSection = function (sectionName) {
            try {
                return JSON.parse(OpenIZConfigurationService.GetSection(sectionName));
            }
            catch (e) {
                console.error(e);
            }
        };
        /**
        * @summary Joins the current client to the specified realm. The backing container is responsible for performing all steps in terms of generating a client certificate, sending it to the realm auth service, etc.
        * @returns True if the specified realm was joined successfully
        * @param {String} address The address root of the realm. Example: demo.openiz.org
        * @param {String} deviceName A unique name for the device which is being joined
        */
        this.joinRealm = function (address, deviceName) {
            try {
                return OpenIZConfigurationService.JoinRealm(address, deviceName);
            }
            catch (e) {
                console.error(e);
            }
        };
        /**
        * @summary Instructs the current client container to leave the currently configured realm
        * @returns True if the realm was successfully left.
        */
        this.leaveRealm = function () {
            try {
                if (!confirm('You are about to leave the realm ' + configuration.realm.address + '. Doing so will force the OpenIZ back into an initial configuration mode. Are you sure you want to do this?'))
                    return false;
                return OpenIZConfigurationService.LeaveRealm();
            }
            catch (e) {
                console.error(e);
            }
        };
        /**
        * @summary Instructs the container to save the specified configuration object to the local configuration file/source.
        * @description The format of the configuration parameter is a JSON object in format:
        *  {
        *      retention: "all|local|none", // The retention policy of all data in realm, local data only to tablet, or no offline data
        *      authentication: "online|offline", // The authentication policy of either must be online or offline allowed
        *      subscribe: [ // Represents subscription queries which should be used to determine what should be in the outbox
        *          "Patient?id=xif", 
        *      ],
        *      enableForgotPassword: true|false, // Whether users can reset their password
        *  }
        *
        * @param {Object} configuraton The configuration object to be saved for global configuration
        * @returns true if the save operation was successful
        */
        this.save = function (configuration) {
            try {
                if (OpenIZConfigurationService.Save(JSON.stringify(configuration))) {
                    OpenIZ.App.toast("Changes will take effect when OpenIZ is restarted");
                    return true;
                }
                else
                    OpenIZ.App.toast("Saving configuration failed");
                return false;
            }
            catch (e) {
                console.error(e);
            }
        };
        /**
        * @summary Get applet specific key/value pair configuration parameters which are currently set for the application
        * @param {String} appletId The identifier of the applet from which the settings should be retrieved
        * @returns A key/value pair representing the applet settings
        */
        this.getAppletSettings =function (appletId) {
            // TODO: Implement
        };
        /**
        * @summary Saves the applet specific settings in a key/value pair format to the configuration store
        * @param {String} appletId The applet identification for which the settings apply
        * @param {Object} settings A key/value pair JSON object of the settings
        * @returns True if the settings save was successful
        */
        this.saveAppletSettings =function (appletId, settings) {
            // TODO: Implement
        };
        /**
        * @summary Get local user preference strings in a key/value pair JSON object
        * @returns The user preferences of the current user
        */
        this.getUserPreferences = function () {
            // TODO: Implement
        };
        /**
        * @summary Save the user preferences in the key/value pair format
        * @param {Object} preferences The user preferences for the current user which should be saved
        * @returns true if the save was successful
        */
        this.saveUserPreferences = function (preferences) {
            // TODO: Implement
        };
    };

    this.Util = new OpenIZ.UtilClass();
    this.AppUtil = new OpenIZ.AppUtil();
    this.Authentication = new OpenIZ.AuthenticationUtil();
    this.Concept = new OpenIZ.ConceptUtil();
    this.Configuration = new OpenIZ.ConfigurationUtil();
    this.Localization = new OpenIZ.LocalizationUtil();
    this.Patient = new OpenIZ.PatientUtil();
    this._session = this.Authentication.getSession();

};


// ---- INITIALIZATION CODE -----

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

// Bind datepickers
$(document).ready(function () {

    
    $('input[type="date"]').each(function (k, v) {
        if ($(v).attr('data-max-date')) {
            var date = new Date();
            date.setDate(date.getDate() + parseInt($(v).attr('data-max-date')));
            var maxDate = OpenIZ.Util.toDateInputString(date);
            $(v).attr('max', maxDate);
        }
        if ($(v).attr('data-min-date')) {
            var date = new Date();
            date.setDate(date.getDate() + parseInt($(v).attr('data-min-date')));
            var minDate = OpenIZ.Util.toDateInputString(date)
            $(v).attr('min', minDate);
        }
    });
    $('select[data-openiz-tag="select2"]').each(function (k, v) {

        // TODO: update this
        $(v).select2({
            dropdownAutoWidth: false


        });
    });
})


/** Allows String.Format() style stuff to work */
if (!String.prototype.format) {
    String.prototype.format = function () {
        var str = this.toString();
        if (!arguments.length)
            return str;
        var args = typeof arguments[0],
            args = (("string" == args || "number" == args) ? arguments : arguments[0]);
        for (arg in args)
            str = str.replace(RegExp("\\{" + arg + "\\}", "gi"), args[arg]);
        return str;
    }
}


