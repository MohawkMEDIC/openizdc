/**
 * OpenIZ Javascript binding class.
 *
 * The purpose of this object is to facilitate and organize OpenIZ applet integration with the backing
 * OpenIZ container. For example, to allow an applet to get the current on/offline status, or authenticate
 * a user.
 */
var OpenIZ = new function () {

    /**
    * Represents URL parameters passed to the applet
    */
    this.urlParams = {};

    /**
     * Utility functions
     */
    this.Util = {
        toDateInputString: function (date) {
            return date.toISOString().substring(0, 10)
        }
    };
    /** 
    * The authentication section is used to interface with OpenIZ's authentication sub-systems
    * including session management information, etc.
    */
    this.Authentication = {
        /**
        * Performs a login with the authentication service returning the active Session object
        * if applicable
        * @param {String} userName the name of the user to authenticate
        * @param {String} password The user's password
        * @return A new OpenIZ.Session object with the current session information if successful, null if not
        * @throws An applicable exception for the validation error.
        */
        login: function (userName, password) {
            try
            {
                var data = OpenIZSessionService.Login(userName, password);

                if (data == null)
                    return null;
                else if (data.lastIndexOf("err", 0) == 0)
                    throw OpenIZ.Localization.getString(data);
                else
                {
                    var pData = JSON.parse(data);
                    if (pData != null && pData.error !== undefined)
                        throw {
                            error: OpenIZ.Localization.getString("err_" + pData.error),
                            description: pData.error_description
                        };
                    else if (data != null)
                        return new OpenIZModel.Session(data);
                    else
                        return null;

                }
            }
            catch(ex)
            {
                console.warn(ex);
                throw ex;
            }
        },
        /**
        * Sets the password for the specified user to some other password. 
        * Note: You will need to have the ChangePassword policy or be changing the password of the currently 
        * logged in user or else this function will return an error
        * @param {String} userName The name of the user to which the password change applies
        * @param {String} password The password of the user.
        * @return True if the password was successfully changed, false otherwise
        */
        setPassword: function (userName, password) {
            // TODO: Implement
        },
        /**
        * Performs a two-factor authentication login. 
        * Note: In the term of "forgot password" the back-end may require just the SMS short code in tfaSecret
        * @param {String} userName The name of the user being logged in
        * @param {String} password The password of the user being logged in
        * @param {String} tfaSecret The two-factor secret (SMS CODE, E-MAIL Code, etc.)
        * @return The granted session object if the login was successful
        */
        loginEx: function (userName, password, tfaSecret) {
            // TODO: Implement
        },
        /**
        * Registers the specified user data 
        * @param {String} userName The desired user name for the user
        * @param {String} password The password the user desires
        * @param {Object} profileData The data (instance of OpenIZUser class) which contains the user's data
        * @return The user profile
        */
        register: function (userName, password, profileData) {
            // TODO: Implement
        },
        /**
        * Gets the current session from the client host
        * @return An instance of Session representing the current session
        */
        getSession: function () {
            try
            {
                var data = OpenIZSessionService.GetSession();
                if (data != null)
                    return new OpenIZModel.Session(JSON.parse(data));
                return null;
            }
            catch(ex)
            {
                console.error(ex);
                return null;
            }
        },
        /**
         * Destroys the current session
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
         * Refreshes the current session so that the token remains valid
         * @return The newly created session
         */
        refreshSession: function () {
            try
            {
                if (OpenIZ.Authentication.getSession() == null)
                    throw {
                        error: OpenIZ.Localization.getString("err_no_session"),
                        description: OpenIZ.Localization.getString("err_no_session_detail")
                    };
                // Refresh the specified session data
                var data = OpenIZSessionService.Refresh();

                if (data == null)
                    return null;
                else if (data.lastIndexOf("err", 0) == 0)
                    throw OpenIZ.Localization.getString(data);
                else {
                    alert(data);
                    var pData = JSON.parse(data);
                    if (pData != null && pData.error !== undefined)
                        throw {
                            error: OpenIZ.Localization.getString("err_" + pData.error),
                            description: pData.error_description
                        };
                    else if (data != null)
                        return new OpenIZModel.Session(data);
                    else
                        return null;

                }
            }
            catch(ex)
            {
                console.warn(ex);
                throw ex;
            }
        }
    };

    /**
    * Represents application specific functions for interoperating with the mobile application itself
    */
    this.App = {
        /**
         * Uses the device camera to scan a barcode from the device
         * @return The value of the barcode detected by the scanner
         */
        scanBarcode: function () {
            try {
                var value = OpenIZApplicationService.BarcodeScan();
                console.log('Barcode scan complete. Data: ' + value);
                return value;
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
         * Navigate backward, even if the back functionality crosses applets
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
         * Closes the applet and kills the Android view
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
         * Displays a TOAST on the user's screen
         * @param {String} text The text of the toast to be shown
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
         * Navigates to the specified applet passing any context variables to it via "context"
         * @param {String} appletId The ID of the applet to be navigated
         * @param {Object} context Any context variables to be passed to the applet
         */
        navigateApplet: function (appletId, context) {
            try {
                OpenIZApplicationService.Navigate(appletId, JSON.stringify(context));
            }
            catch (e) {
                console.error(e);
            }
        }
    };

    /**
     * Represents functions related to the localization of applets
     */
    this.Localization = {
        /**
         * Gets the specified localized string the current display language from the resources file
         * @param {String} stringId The identifier of the string
         * @return The specified string
         */
        getString: function (stringId) {
            try {
                return OpenIZApplicationService.GetString(stringId);
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
         * Gets the current user interface locale name
         * @return The ISO language code of the current UI 
         */
        getLocale: function () {
            // TODO: Implement
        },
        /**
         * Sets the current user interface locale
         * @param {String} lcoale The locale to set the user interface to
         * @return The locale the user interface is now operating in
         */
        setLocale: function (locale) {
            // TODO: Implement
        },
        /** 
         * Gets the specified string in the specified locale
         * @param {String} stringId The identifier of the string resource
         * @param {String} localeId The two digit ISO language code
         * @return The display string in the specified locale
         */
        getStringEx : function(stringId, localeId) {
            // TODO: Implement
        }
    };

    /**
     * Represents functions related to the concept dictionary
     */
    this.Concept = {
        /**
         * Gets the specified values of concepts from the concept set.
         * @param {String} setName The name of the concept set to retrieve (Ex: AdministrativeGender)
         * @return A list of {OpenIZModel.Cocnept} objects which represent the concepts
         */
        getConceptSet: function (setName) {
            try {
                var results = JSON.parse(OpenIZConceptService.GetConceptSet(setName));
                return results;
            }
            catch (e) {
                console.error(e);
            }
        }
    };
    
    /**
     * Represents a series of functions related to patients
     */
    this.Patient = {

        /**
         * Query the OpenIZ data store for patients matching the specified query string. The query string should be
         * an IMS query format string like name[L].part[FAM].value=Smith&name[L].part[GIV].value=John
         * @param {String} searchString The IMSI search string to be searched
         * @return An array of {OpenIZModel.Patient} classes which represent the search results.
         */
        search: function (searchString) {
            try {
                var results = JSON.parse(OpenIZPatientService.Find(formData));
                return results;
            }
            catch (e) {
                console.error(e);
            }
        }
    };

    /**
    * The configuration property is used to segregate the functions related to configuration of the main OpenIZ
    * system including realm, updating configuration, etc.
    */
    this.Configuration = {
        /**
        * Gets the current realm to which the client is connected
        */
        getRealm: function () {
            return OpenIZ.Configuration.getSection("SecurityConfigurationSection").domain;
        },
        /**
        * Gets the specified OpenIZ configuration section name. 
        * @return A JSON object representing the configuration data for the particular section
        * @param {Object} sectionName The name of the section which should be retrieved.
        */
        getSection: function (sectionName) {
            try {
                return JSON.parse(OpenIZConfigurationService.GetSection(sectionName));
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
        * Joins the current client to the specified realm. The backing container is responsible for performing
        * all steps in terms of generating a client certificate, sending it to the realm auth service, etc.
        * @return True if the specified realm was joined successfully
        * @param {String} address The address root of the realm. Example: demo.openiz.org
        * @param {String} deviceName A unique name for the device which is being joined
        */
        joinRealm: function (address, deviceName) {
            try {
                return OpenIZConfigurationService.JoinRealm(address, deviceName);
            }
            catch (e) {
                console.error(e);
            }
        },
        /**
        * Instructs the current client container to leave the currently configured realm
        * @return True if the realm was successfully left.
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
        * Instructs the container to save the specified configuration object to the local configuration file/source.
        * The format of the configuration parameter is a JSON object in format:
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
        * @return true if the save operation was successful
        */
        save: function (configuration) {
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
        },
        /**
        * Get applet specific key/value pair configuration parameters which are currently set for the application
        * @param {String} appletId The identifier of the applet from which the settings should be retrieved
        * @return A key/value pair representing the applet settings
        */
        getAppletSettings: function (appletId) {
            // TODO: Implement
        },
        /**
        * Saves the applet specific settings in a key/value pair format to the configuration store
        * @param {String} appletId The applet identification for which the settings apply
        * @param {Object} settings A key/value pair JSON object of the settings
        * @return True if the settings save was successful
        */
        saveAppletSettings: function (appletId, settings) {
            // TODO: Implement
        },
        /**
        * Get local user preference strings in a key/value pair JSON object
        * @return The user preferences of the current user
        */
        getUserPreferences: function () {
            // TODO: Implement
        },
        /**
        * Save the user preferences in the key/value pair format
        * @param {Object} preferences The user preferences for the current user which should be saved
        * @return true if the save was successful
        */
        saveUserPreferences: function (preferences) {
            // TODO: Implement
        }
    }

    /**
        * Get the current session
        */
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

