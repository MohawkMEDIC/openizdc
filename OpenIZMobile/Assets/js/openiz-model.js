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
     * @summary Takes concept and returns the appropriate key for the concept
     */
    this.getObjectKey = function(concept)
    {
        if (typeof (concept) == "String")
            return concept;
        else
            return concept.id;
    }

    /**
     * @summary Resolves the specified concept from the bundle or database
     * @param {OpenIZModel.Bundle} bundleContext The context in which the concept may be found
     */
    this.resolveConcept = function(bundleContext, conceptId)
    {
        var retVal = null;
        if (bundleContext != null)
            retVal = bundleContext.getItem(conceptId, function () { return null; });
        if (retVal == null)
            retVal = OpenIZ.Concept.getConcept(conceptId);
        return retVal;
    }

    /**
     * @summary Represents utilities for dealing with bundles
     * @param {Object} bundleData The IMSI formatted bundle
     * @class
     * @property {String} Identifier of the entry which is the first object for the bundle
     * @property {Object} The collection of items contained within the bundle
     * @property {Numeric} offset The offset of the bundle result set
     * @property {Numeric} totalResults The total results in the query
     * @property {Numeric} count The number of results in the current bundle
     */
    this.Bundle = function (bundleData) {

        var _self = this;

        this.entry = bundleData.entry;
        this.offset = bundleData.offset;
        this.count = bundleData.count;
        this.totalResults = bundleData.totalResults;

        this.items = [];
        for (var itm in bundleData.item)
        {
            switch(itm["$type"])
            {
                case "SecurityUser":
                    this.items.push(new OpenIZModel.SecurityUser(itm, this));
                    break;
                case "Patient":
                    this.items.push(new OpenIZModel.Patient(itm, this));
                    break;
                case "Concept":
                    this.items.push(new OpenIZModel.Concept(itm, this));
                    break;
                case "ConceptSet":
                    this.items.push(new OpenIZModel.ConceptSet(itm, this));
                    break;
            }
        }
        
        /**
         * @summary Gets the item with specified id from the specified bundle
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
         * @summary Returns the first entry of type 
         * @param {String} type The type of entry to return
         */
        this.first = function(type)
        {
            return _self.all(type);
        }

        /**
         * @summary Returns all of the matching entries of the particular type
         * @param {String} type The type of object to return
         */
        this.all = function(type)
        {
            var retVal = [];
            for (var itm in _self.items)
                if (itm["$type"] == type)
                    retVal.push(itm);
            return retVal;
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
     * @param {OpenIZModel.Bundle} bundleContext The context from which bundle items should be taken

    * @property {String} id The unique identifier of the user
    * @property {String} email The email of the user
    * @property {Bool} emailConfirmed Indicates whether the email is confirmed
    * @property {Number} invalidLoginAttempts Number of invalid login attempts
    * @property {Date} lockout When set, indicates the earliest time the user can log in again
    * @property {String} userName The user name of the user
    * @property {String} photo The photograph of the user
    * @property {OpenIZModel.Entity} entity The entity (userEntity) of the user
    * @property {Date} lastLoginTime The last login time
    * @property {String} phoneNumber The phone number of the user
    * @property {Bool} phoneNumberConfirmed Whether the phone number is confirmed
     */
    this.SecurityUser = function (securityUserData, bundleContext) {

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
     * @summary Represents a set of related concepts
     * @class
     * @constructor
     * @param {Object} valueData The IMSI formatted concept set data
     * @param {OpenIZModel.Bundle} bundleContext The context from which bundle items should be taken
     * @property {String} id The unique identifier for the concept set
     * @property {String} name The name of the concept set
     * @property {String} mnemonic The mnemonic of the concept set
     * @property {String} oid The OID of the concept set
     * @property {String} url The url of the concept set
     * @property {Object} members The members ({Concept} instances of the set)
     */
    this.ConceptSet = function(valueData, bundleContext) {

        var _self = this;
        this.id = valueData.id;
        this.name = valueData.name;
        this.mnemonic = valueData.mnemonic;
        this.oid = valueData.oid;
        this.url = valueData.url;

        // Map set members
        this.members = [];
        for (var mem in valueData.concept)
            this.members.push(OpenIZModel.resolveConcept(bundleContext, mem));

        /**
         * @summary Represents the concept set as a simple key/value pair for use in a select drop-down
         * @param {String} lang The language to fetch display names for
         */
        this.toSelectModel = function (lang) {
            var retVal = [];
            for (var mem in _self.members)
                retVal.push(mem.toSelectModel(lang));
            return retVal;
        };

        /**
         * @summary Represent this concept set as a IMSI object
         */
        this.toImsi = function () {
            var retVal = {
                "$type": "ConceptSet",
                "id": _self.id,
                "name": _self.name,
                "mnemonic": _self.mnemonic,
                "oid": _self.oid,
                "url": _self.url,
                "concept": []
            };

            for (var mem in _self.members)
                retVal.concept.push(OpenIZModel.getObjectKey(mem));
            return retVal;

        };

    }

    /**
     * @summary This class represents a javascript equivalent to a Concept
     * @class 
     * @constructor
     * @param {Object} valueData The IMSI formatted concept data
     * @param {Bundle} bundleContext The context from which bundle items should be taken
     * @property {String} id The identifier of the concept
     * @property {String} versionId The version of the concept
     * @property {Boolean} isSystemConcept Indicates whether the concept is a system (readonly) concept
     * @property {String} mnemonic The mnemonic of the concept
     * @property {Object} status The status concept
     * @property {Object} class The classification of the concept
     * @property {Object} names The names by which the concept is known
     */
    this.Concept = function (valueData, bundleContext) {

        var _self = this;
        this.id = valueData.id;
        this.versionId = valueData.versionId;
        this.isSystemConcept = valueData.isReadonly;
        this.mnemonic = valueData.mnemonic;
        this.status = OpenIZModel.resolveConcept(bundleContext, valueData.statusConcept);
        this.class = valueData.class;

        // Map names
        this.names = [];
        for (var nam in valueData.name)
            this.names.push(
                {
                    lang: nam.language,
                    value: nam.value
                });

        /**
         * @summary Represents this concept as a select option
         */
        this.toSelectModel = function (lang) {
            for (var nam in _self.names)
                if (lang == nam.lang)
                    return { id: _self.id, value: nam.value };
            return { id: _self.id, value: _self.names[0].value };
        }

        /**
         * @summary Represents this concept as an IMSI formatted data object
         */
        this.toImsi = function () {
            var retVal = {
                "$type": "Concept",
                "id": _self.id,
                "versionId": _self.versionId,
                "isReadonly": _self.isSystemConcept,
                "mnemonic": _self.mnemonic,
                "statusConcept": OpenIZModel.getObjectKey(_self.status),
                "class": _self.class,
                "name": []
            };

            if (_self.names != null)
                for (var nam in _self.names)
                    retVal.name.push({
                        "language": nam.lang,
                        "value": nam.value
                    });

            return retVal;
        }
    };

    /**
     * @summary This class represents data related to a complex name or address
     * @class A complex name with use and multiple parts
     * @constructor
     * @param {Object} valueData The IMSI formatted GenericComponentData wrapper having use, component [{ type / value }] format
     * @property {OpenIZModel.Concept} use The prescribed use of the componentized value
     * @property {Object} components The components with type/value of the component representing the value
     */
    this.ComponentizedValue = function(valueData)
    {

        var _self = this;

        // Use
        this.use = OpenIZModel.resolveConcept(bundleContext, valueData.use);

        // Components
        this.components = [];
        for (var comp in valueData.component)
            this.components.push(
            {
                type: OpenIZModel.resolveConcept(bundleContext, comp.type),
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
     * @param {Bundle} bundleContext The context from which bundle items should be taken
    * @property {String} id The unique identifier of the patient
    * @property {String} versionId The version identifier of the patient
    * @property {Date} deceasedDate The date that this patient bacame deceased
    * @property {String} deceasedDatePrecision The precision of the deceased date (if unknown)
    * @property {Number} multipleBirthOrder The order that this patient was in a multiple birth
    * @property {String} gender The gender of the patient
    * @property {Date} dateOfBirth The date of birth of the patient
    * @property {String} dateOfBirthPrecision The precision of the date of birth if not exact
    * @property {OpenIZModel.Concept} status The status of the patient
    * @property {OpenIZModel.Concept} type The type concept
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
    this.Patient = function (patientData, bundleContext) {

        // Self reference
        var _self = this;
        this.id = patientData.id;
        this.versionId = patientData.versionId;
        this.deceasedDate = patientData.deceasedDate;
        this.deceasedDatePrecision = patientData.deceasedDatePrecision;
        this.multipleBirthOrder = patientData.multipleBirthOrder;
        this.gender = OpenIZModel.resolveConcept(bundleContext, patientData.genderConcept);
        this.dateOfBirth = patientData.dateOfBirth;
        this.dateOfBirthPrecision = patientData.dateOfBirthPrecision;
        this.status = OpenIZModel.resolveConcept(bundleContext, patientData.statusConcept);
        this.type = OpenIZModel.resolveConcept(bundleContext, patientData.typeConcept);
        this.identifiers = patientData.identifier;

        // Map relationships
        this.relationships = [];
        for (var rel in patientData.relationship)
        {
            this.relationships.push(
                {
                    relationshipType: OpenIZModel.resolveConcept(bundleContext, rel.relationshipType),
                    target : OpenIZ.Entity.get(rel.target)
                });
        }

        this.languages = patientData.language;

        // Map telecoms
        this.telecoms = [];
        for (var tel in patientData.telecom)
            this.telecoms.push({
                use: OpenIZModel.resolveConcept(bundleContext, tel.use),
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
                role: OpenIZModel.resolveConcept(bundleContext, ptcpt.participationRole),
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
         * @summary Represent as IMSI
         */
        this.toImsi = function () {
            var retVal = {
                "$type": "Patient",
                "id" : _self.id,
                "deceasedDate" : _self.deceasedDate,
                "multipleBirthOrder": _self.multipleBirthOrder,
                "deceasedDatePrecision": _self.deceasedDatePrecision,
                "dateOfBirth": _self.dateOfBirth,
                "identifier": _self.identifiers,
                "genderConcept": OpenIZModel.getObjectKey(_self.gender),
                "statusConcept": OpenIZModel.getObjectKey(_self.status),
                "typeConcept": OpenIZModel.getObjectKey(_self.type),
                "dateOfBirth": _self.dateOfBirth,
                "dateOfBirthPrecision": _self.dateOfBirthPrecision,
                "relationship": [],
                "language": _self.languages,
                "telecom": [],
                "extension": _self.extensions,
                "name": [],
                "address": [],
                "note": [],
                "tag": _self.tags,
                "participation" : []
            };

            // Map relationships
            if (_self.relationships != null)
                for (var rel in _self.relationship) {
                    retVal.relationship.push(
                        {
                            relationshipType: OpenIZModel.getObjectKey(rel.relationshipType),
                            target: OpenIZModel.getObjectKey(rel.target)
                        });
                }

            // Map telecoms
            if(_self.telecoms != null)
                for (var tel in _self.telecoms)
                    retVal.telecom.push({
                        use: OpenIZModel.getObjectKey(tel.use),
                        value: tel.value
                    });

            // Map names
            if(_self.names != null)
                for (var nam in _self.names) {
                    retVal.name.push(nam.toImsi());
                }

            // Map addresses
            if(_self.addresses != null)
                for (var add in _self.addresses) {
                    retVal.address.push(add.toImsi());
                }

            // Map notes
            if(_self.notes != null)
                for (var nt in _self.notes)
                    retVal.note.push(
                        {
                            author: OpenIZModel.getObjectKey(nt.author),
                            text: nt.text
                        });

            // Map participations
            if(_self.participations != null)
                for (var ptcpt in _self.participations)
                    retVal.participation.push({
                        participationRole: OpenIZModel.getObjectKey(ptcpt.role),
                        source: OpenIZModel.getObjectKey(ptcpt.act)
                    });

            return retVal;
        }
    };

    /**
     * @class
     * @summary Represents a simple exception class
     */
    this.Exception = function(message, detail, cause)
    {
        _self = this;

        this.message = message;
        this.details = detail;
        this.caused_by = cause;

    }
};
