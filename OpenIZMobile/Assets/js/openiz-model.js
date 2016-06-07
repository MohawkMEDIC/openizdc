/// <reference path="openiz.js"/>

/**
 * OpenIZ wrapper binding
 *
 * The purpose of this file is to provide a convenient JavaScript wrapper around the OpenIZ host container for applets.
 * This file should be changed whenever you are writing an Applet container which will host applets.
 */

/**
 * @summary Namespace for OpenIZ model wrapper JAvaScript classes
 */
var OpenIZModel = new function () {

    /**
     * @summary Represents utilities for dealing with bundles
     * @param {Object} bundleData The IMSI formatted bundle
     */
    this.Bundle = function (bundleData) {

        this._self = this;
        this.entry = bundleData.entry;

        // Convert items
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
     * @class OpenIZ session information
     * @constructor
     * @param {Object} sessionData The IMSI formatted session data
     */
    this.Session = function (sessionData) {

        /**
         * The information related to the principal
         */
        this.identity = {
            name: sessionData.username,
            roles: sessionData.roles
        };
        /**
         * The method of authentication, either local, oauth, or basic
         */
        this.method = sessionData.method;
        /**
         * The date on which the session will expire
         */
        this.expires = sessionData.exp;
        /**
         * The date on which the session was issued
         */
        this.issued = sessionData.nbf;
        /**
         * The token which can be used as a refresh
         */
        this.refresh_token = sessionData.refresh_token;
        /**
         * Contains the JWT token assets (name, etc.) from the session
         */
        this.jwt = sessionData.jwt;

        /**
         * Returns whether the current session is expired
         */
        this.isExpired = function () {
            return new Date() > this.expires;
        };
        /**
         * The abandon function is used to abandon the current session
         */
        this.abandon = function () {
            return OpenIZ.Authentication.abandonSession();
        };
        /**
         * The refresh function is used to refresh the current session 
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
     */
    this.SecurityUser = function (securityUserData) {

        this._self = this;
        // The unique identifier of the user
        this.id = securityUserData.id;
        // The email of the user
        this.email = securityUserData.email;
        // Indicates whether the email is confirmed
        this.emailConfirmed = securityUserData.emailConfirmed;
        // Number of invalid login attempts
        this.invalidLoginAttempts = securityUserData.invalidLoginAttempts
        // When set, indicates the earliest time the user can log in again
        this.lockout = securityUserData.lockout;
        // The user name of the user
        this.userName = securityUserData.userName;
        // The photograph of the user
        this.photo = securityUserData.photo;
        // The entity (userEntity) of the user
        this.entity = new OpenIZModel.UserEntity(securityUserData.entity);
        // The last login time
        this.lastLoginTime = securityUserData.lastLoginTime;
        // The phone number of the user
        this.phoneNumber = securityUserData.phoneNumber;
        // Whether the phone number is confirmed
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
     */
    this.ComponentizedValue = function(valueData)
    {

        this._self = this;

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
     */
    this.Patient = function (patientData) {

        // Self reference
        this._self = this;

        // The unique identifier of the patient
        this.id = patientData.id;

        // The version identifier of the patient
        this.versionId = patientData.versionId;
        // The date that this patient bacame deceased
        this.deceasedDate = patientData.deceasedDate;
        // The precision of the deceased date (if unknown)
        this.deceasedDatePrecision = patientData.deceasedDatePrecision;
        // The order that this patient was in a multiple birth
        this.multipleBirthOrder = patientData.multipleBirthOrder;
        // The gender of the patient
        this.gender = OpenIZ.Concept.getConcept(patientData.genderConcept);
        // The date of birth of the patient
        this.dateOfBirth = patientData.dateOfBirth;
        // The precision of the date of birth if not exact
        this.dateOfBirthPrecision = patientData.dateOfBirthPrecision;
        // The status of the patient
        this.statusConcept = OpenIZ.Concept.getConcept(patientData.statusConcept);
        // The type concept
        this.typeConcept = OpenIZ.Concept.getConcept(patientData.typeConcept);
        // All identifiers related to the patient
        this.identifiers = patientData.identifier;

        // Relationships
        this.relationships = [];
        for (var rel in patientData.relationship)
        {
            this.relationships.push(
                {
                    relationshipType: OpenIZ.Concept.getConcept(rel.relationshipType),
                    target : OpenIZ.Entity.get(rel.target)
                });
        }

        // Languages of communication
        this.language = patientData.language;

        // Telecommunications 
        this.telecoms = [];
        for (var tel in patientData.telecom)
            this.telecoms.push({
                use: OpenIZ.Concept.getConcept(tel.use),
                value: tel.value
            });

        // Extensions
        this.extensions = patientData.extension;

        // Names
        this.names = [];
        for (var nam in patientData.name) {
            this.names.push(new OpenIZModel.ComponentizedValue(nam));
        }

        // Addresses
        this.addresses = [];
        for (var add in patientData.address) {
            this.addresses.push(new OpenIZModel.ComponentizedValue(add))
        }

        // Notes
        this.notes = [];
        for (var nt in patientData.note)
            this.notes.push({ author: OpenIZ.Entity.get(nt.author), text: nt.text });

        // Tags
        this.tags = patientData.tag;
            
        // Participations
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
