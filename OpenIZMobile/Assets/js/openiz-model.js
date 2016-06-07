/// <reference path="openiz.js"/>

/**
 * OpenIZ wrapper binding
 *
 * The purpose of this file is to provide a convenient JavaScript wrapper around the OpenIZ host container for applets.
 * This file should be changed whenever you are writing an Applet container which will host applets.
 */

/**
 * @summary Namespace for OpenIZ model wrapper JAvaScript classes
 * @class
 */
var OpenIZModel = new function () {

    /**
     * @summary Represents utilities for dealing with bundles
     * @param {Object} bundleData The IMSI formatted bundle
     * @class
     */
    this.Bundle = function (bundleData) {

        var _self = this;

        /** 
         * @property {String} Identifier of the entry which is the first object for the bundle
         */
        this.entry = bundleData.entry;

        /**
         * @property {Object} The collection of items contained within the bundle
         */
        this.items = [];
        for (var itm in bundleData.item)
        {
            switch(itm["$type"])
            {
                case "SecurityUser":
                    this.items.push(new OpenIZModel.SecurityUser(itm));
                    break;
                case "Patient":
                    this.items.push(new OpenIZModel.Patient(itm));
                    break;
            }
        }
        
        /**
         * @summary Gets the item with specified id from the specified bundle
         * @param {Object} bundle The bundle in which to search for the id
         * @param {String} id The identifier of the entry
         * @param {Function} defaultFn The function to be called if no match is found
         */
        this.getItem = function (id, defaultFn) {
            for (var itm in _self.items)
                if (itm.id == id)
                    return itm;
            return defaultFn(id);
        };

        /**
         * @summary Gets the entry object in the bundle
         * @param {Object} bundle The bundle from which to extract the entry object
         */
        this.getEntry = function()
        {
            if (_self.entry == null) return null;
            for (var itm in _self.item)
                if (itm.id == _self.entry)
                    return itm;
            
            // Throw error
            throw new OpenIZModel.Exception("Entry object not found in bundle", null, null);
        }

        /**
         * @summary Represent this bundle as an IMSI bundle
         * @returns {Object} The IMSI formatted bundle
         */
        this.toImsi = function()
        {
            var retVal = {
                "$type": "Bundle",
                entry: _self.entry,
                item: []
            };

            // Represent object as IMSI
            for(var itm in _self.items)
                retVal.item.push(itm.toImsi());

            return retVal;
        }
    };

    /**
     * @summary Represents OpenIZ session data, rather, the session that is currently in play.
     * @class
     * @constructor
     * @param {Object} sessionData The IMSI formatted session data
     * @property {Object} identity The identity of the authenticated user
     * @property {String} method The method of authentication, either local, oauth, or basic
     * @property {Date} expires The date on which the session will expire
     * @property {Date} issued The date on which the session was issued
     * @property {String} refresh_token The token which can be used as a refresh
     * @property {String} jwt Contains the JWT token assets (name, etc.) from the session
     */
    this.Session = function (sessionData) {

        this.identity = {
            name: sessionData.username,
            roles: sessionData.roles
        };
        this.method = sessionData.method;
        this.expires = sessionData.exp;
        this.issued = sessionData.nbf;
        this.refresh_token = sessionData.refresh_token;
        this.jwt = sessionData.jwt;

        /**
         * @summary Returns whether the current session is expired
         */
        this.isExpired = function () {
            return new Date() > this.expires;
        };
        /**
         * @summary The abandon function is used to abandon the current session
         */
        this.abandon = function () {
            return OpenIZ.Authentication.abandonSession();
        };
        /**
         * @summary The refresh function is used to refresh the current session 
         */
        this.refresh = function () {
            return OpenIZ.Authentication.refreshSession();
        };
    };

    /**
     * @summary Represents a security user (user information)
     * @class Security User model
     * @constructor
     * @param {Object} securityUserData the IMSI formatted security user data
    * @property {String} id The unique identifier of the user
    * @property {String} email The email of the user
    * @property {Bool} emailConfirmed Indicates whether the email is confirmed
    * @property {Number} invalidLoginAttempts Number of invalid login attempts
    * @property {Date} lockout When set, indicates the earliest time the user can log in again
    * @property {String} userName The user name of the user
    * @property {String} photo The photograph of the user
    * @property {Object} entity The entity (userEntity) of the user
    * @property {Date} lastLoginTime The last login time
    * @property {String} phoneNumber The phone number of the user
    * @property {Bool} phoneNumberConfirmed Whether the phone number is confirmed
     */
    this.SecurityUser = function (securityUserData) {

        var _self = this;
        this.id = securityUserData.id;
        this.email = securityUserData.email;
        this.emailConfirmed = securityUserData.emailConfirmed;
        this.invalidLoginAttempts = securityUserData.invalidLoginAttempts
        this.lockout = securityUserData.lockout;
        this.userName = securityUserData.userName;
        this.photo = securityUserData.photo;
        this.entity = new OpenIZModel.UserEntity(securityUserData.entity);
        this.lastLoginTime = securityUserData.lastLoginTime;
        this.phoneNumber = securityUserData.phoneNumber;
        this.phoneNumberConfirmed = securityUserData.phoneNumberConfirmed;

        /**
         * @summary Insert security user into the back-end
         */
        this.insert = function () {
           return OpenIZ.Security.insertUser(_self);
        }

        /**
         * @summary Saves the current security user object in the back-end
         */
        this.save = function () {
            return OpenIZ.Security.updateUser(_self);
        };

        /**
         * @summary Obsoletes the specified security user
         */
        this.obsolete = function () {
            return OpenIZ.Security.obsoleteUser(_self);
        };

    };

    /**
     * @summary This class represents data related to a complex name or address
     * @class A complex name with use and multiple parts
     * @constructor
     * @param {Object} valueData The IMSI formatted GenericComponentData wrapper having use, component [{ type / value }] format
     * @property {Object} use The prescribed use of the componentized value
     * @property {Object} components The components with type/value of the component representing the value
     */
    this.ComponentizedValue = function(valueData)
    {

        var _self = this;

        // Use
        this.use = OpenIZ.Concept.getConcept(valueData.use);

        // Components
        this.components = [];
        for (var comp in valueData.component)
            this.components.push(
            {
                type: OpenIZ.concept.getConcept(comp.type),
                value: comp.value
            });

        /**
         * @summary Represent this as an IMSI component
         */
        this.toImsi = function() {
            var retVal = {
                use: _self.use.id,
                component: []
            };
            for (var c in _self.components)
                retVal.component.push({ type: c.type.id, value: c.value });
            return retVal;
        }

    }

    /**
     * @summary This class represents data related to a patient
     * @class The model patient class
     * @constructor
     * @param {Object} patientData The IMSI formatted patient data
    * @property {String} id The unique identifier of the patient
    * @property {String} versionId The version identifier of the patient
    * @property {Date} deceasedDate The date that this patient bacame deceased
    * @property {String} deceasedDatePrecision The precision of the deceased date (if unknown)
    * @property {Number} multipleBirthOrder The order that this patient was in a multiple birth
    * @property {String} gender The gender of the patient
    * @property {Date} dateOfBirth The date of birth of the patient
    * @property {String} dateOfBirthPrecision The precision of the date of birth if not exact
    * @property {Object} statusConcept The status of the patient
    * @property {Object} typeConcept The type concept
    * @property {Object} identifiers All identifiers related to the patient
    * @property {Object} relationship All Relationships that the patient holds
    * @property {Object} languages Languages of communication the patient can be contacted in
    * @property {Object} telecoms Telecommunications addresses for the patient
    * @property {Object} extensions Extensions to the core patient object
    * @property {Object} names The series of names which the patient uses
    * @property {Object} addresses The series of contact addresses for the patient
    * @property {Object} notes Additional textual notes about the patient
    * @property {Object} tags A series of key/value pairs which are used to tag the patient record
    * @property {Object} participations The series of acts that the patient participates in

     */
    this.Patient = function (patientData) {

        // Self reference
        var _self = this;

        this.id = patientData.id;
        this.versionId = patientData.versionId;
        this.deceasedDate = patientData.deceasedDate;
        this.deceasedDatePrecision = patientData.deceasedDatePrecision;
        this.multipleBirthOrder = patientData.multipleBirthOrder;
        this.gender = OpenIZ.Concept.getConcept(patientData.genderConcept);
        this.dateOfBirth = patientData.dateOfBirth;
        this.dateOfBirthPrecision = patientData.dateOfBirthPrecision;
        this.statusConcept = OpenIZ.Concept.getConcept(patientData.statusConcept);
        this.typeConcept = OpenIZ.Concept.getConcept(patientData.typeConcept);
        this.identifiers = patientData.identifier;

        // Map relationships
        this.relationships = [];
        for (var rel in patientData.relationship)
        {
            this.relationships.push(
                {
                    relationshipType: OpenIZ.Concept.getConcept(rel.relationshipType),
                    target : OpenIZ.Entity.get(rel.target)
                });
        }

        this.languages = patientData.language;

        // Map telecoms
        this.telecoms = [];
        for (var tel in patientData.telecom)
            this.telecoms.push({
                use: OpenIZ.Concept.getConcept(tel.use),
                value: tel.value
            });

        this.extensions = patientData.extension;

        // Map names
        this.names = [];
        for (var nam in patientData.name) {
            this.names.push(new OpenIZModel.ComponentizedValue(nam));
        }

        // Map addresses
        this.addresses = [];
        for (var add in patientData.address) {
            this.addresses.push(new OpenIZModel.ComponentizedValue(add))
        }

        // Map notes
        this.notes = [];
        for (var nt in patientData.note)
            this.notes.push({ author: OpenIZ.Entity.get(nt.author), text: nt.text });

        this.tags = patientData.tag;
            
        // Map participations
        this.participations = [];
        for (var ptcpt in patientData.participation)
            this.participations.push({
                role: OpenIZ.Concept.getConcept(ptcpt.participationRole),
                act: OpenIZ.Act.get(ptcpt.source)
            });

        /**
         * @summary Inserts this patient into the IMS database
         */
        this.insert = function () {
            return OpenIZ.Patient.insert(_self);
        };

        /**
         * @summary Update the patient in the IMS database
         */
        this.save = function () {
            return OpenIZ.Patient.update(_self);
        }

        /**
         * @summary Obsolete this patient in the database
         */
        this.obsolete = function () {
            return OpenIZ.Patient.obsolete(_self.key);
        }

        /**
         * Represent as IMSI
         */
        this.toImsi = function () {
            var retVal = {
                "$type": "Patient"
            };

            // TODO: Implement this.

            return retVal;
        }
    };

    /**
     * @class
     * @summary Represents a simple exception class
     */
    this.Exception = function(message, detail, cause)
    {
        this._self = this;
        this.message = message;
        this.details = detail;
        this.caused_by = cause;
        this.toString = function () {
            return String.format("{message} ({details}) \r\n Caused By: {caused_by}", _self);
        }
    }
};
