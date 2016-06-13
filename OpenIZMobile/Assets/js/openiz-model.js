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
        if (conceptId == undefined || conceptId == null) return null;

        console.info("resolveConcept " + bundleContext + ", " + conceptId);
        var retVal = null;
        if (bundleContext != null)
            retVal = bundleContext.getItem(conceptId);
        else
            retVal = OpenIZCache.getItem(conceptId);
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

        console.info("Bundle.ctor " + bundleData);

        var _self = this;

        this.entry = bundleData.entry;
        this.offset = bundleData.offset;
        this.count = bundleData.count;
        this.totalResults = bundleData.totalResults;
        this.items = [];
        
        /**
         * @summary Gets the item with specified id from the specified bundle
         * @param {String} id The identifier of the entry
         */
        this.getItem = function (id) {
            console.info("Bundle.getItem " + id);
            for (var i in _self.items) {
                var itm = _self.items[i];
                if (itm.id == id)
                    return itm;
            }
            return null;
        };

        /**
         * @summary Gets the entry object in the bundle
         */
        this.getEntry = function () {
            console.info("Bundle.getEntry");
            if (_self.entry == null) return null;
            for (var i in _self.items) {
                var itm = _self.items[i];
                if (itm.id == _self.entry)
                    return itm;
            }
            // Throw error
            throw new OpenIZModel.Exception("Entry object not found in bundle", null, null);
        };

        /**
         * @summary Returns the first entry of type 
         * @param {String} type The type of entry to return
         */
        this.first = function (type) {
            console.info("Bundle.first " + type);
            for (var i in _self.items) {
                var itm = _self.items[i];
                if (itm.$type == type)
                    return itm;
            }
            return null;
        };

        /**
         * @summary Returns all of the matching entries of the particular type
         * @param {String} type The type of object to return
         */
        this.all = function (type) {
            console.info("Bundle.all " + type);

            var retVal = [];
            for (var i in _self.items) {
                var itm = _self.items[i];
                if (itm.$type == type)
                    retVal.push(itm);
            }
            return retVal;
        };

        /**
         * @summary Merge this bundle with another
         * @param {OpenIZModel.Bundle} otherBundle The other bundle to merge
         */
        this.merge = function (otherBundle) {
            for (var i in otherBundle.items) {
                var itm = otherBundle.items[i];
                if (_self.getItem(itm.id) == null)
                    _self.items.push(itm);
            }
            console.info("Bundle now contains " + _self.items.length + " items");
        };

        /**
         * @summary Represents the concept set as a simple key/value pair for use in a select drop-down
         * @param {String} lang The language to fetch display names for
         */
        this.toSelectModel = function (type, lang) {
            console.info("Bundle.toSelectModel " + lang);

            var retVal = [];
            for (var i in _self.items)
                if(_self.items[i].$type == type)
                    retVal.push(_self.items[i].toSelectModel(lang));
            return retVal;
        };

        /**
         * @summary Represent this bundle as an IMSI bundle
         * @returns {Object} The IMSI formatted bundle
         */
        this.toImsi = function () {
            console.info("Bundle.toImsi");

            var retVal = {
                "$type": "Bundle",
                entry: _self.entry,
                item: []
            };

            // Represent object as IMSI
            for (var i in _self.items)
                retVal.item.push(_self.items[i].toImsi());

            return retVal;
        };

        // Initialize items
        for (var i in bundleData.item) {
            var itm = bundleData.item[i];

            console.info("adding " + itm.$type + " [" + itm.id + "]");
            var obj = null;
            switch (itm.$type) {
                case "SecurityUser":
                    obj = new OpenIZModel.SecurityUser(itm);
                    break;
                case "Patient":
                    obj = new OpenIZModel.Patient(itm);
                    break;
                case "Concept":
                    obj = new OpenIZModel.Concept(itm);
                    break;
                case "ConceptSet":
                    obj = new OpenIZModel.ConceptSet(itm);
                    break;
            }

            // Set type
            if (obj != null)
                this.items.push(obj);
        }

        // Initialize items
        for (var i in _self.items)
            _self.items[i].expand(bundleData.item[i], _self);

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

        console.info("Session.ctor " + sessionData);

        this.$type = "Session";
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
    * @property {OpenIZModel.Entity} entity The entity (userEntity) of the user
    * @property {Date} lastLoginTime The last login time
    * @property {String} phoneNumber The phone number of the user
    * @property {Bool} phoneNumberConfirmed Whether the phone number is confirmed
     */
    this.SecurityUser = function (securityUserData) {

        console.info("SecurityUser.ctor " + securityUserData);

        var _self = this;
        this.$type = "SecurityUser";
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
     * @property {String} id The unique identifier for the concept set
     * @property {String} name The name of the concept set
     * @property {String} mnemonic The mnemonic of the concept set
     * @property {String} oid The OID of the concept set
     * @property {String} url The url of the concept set
     * @property {Object} members The members ({Concept} instances of the set)
     */
    this.ConceptSet = function(valueData) {

        console.info("ConceptSet.ctor " + valueData);

        var _self = this;
        this.$type = "ConceptSet";
        this.id = valueData.id;
        this.name = valueData.name;
        this.mnemonic = valueData.mnemonic;
        this.oid = valueData.oid;
        this.url = valueData.url;

        // Map set members
        this.members = [];

        /** 
         * @summary Expand all properties as needed
         */
        this.expand = function (valueData, bundleContext) {
            console.info("ConceptSet.expand");
            for (var i in valueData.concept)
                _self.members.push(OpenIZModel.resolveConcept(bundleContext, valueData.concept[i]));
        }

        /**
         * @summary Represents the concept set as a simple key/value pair for use in a select drop-down
         * @param {String} lang The language to fetch display names for
         */
        this.toSelectModel = function (lang) {
            console.info("ConceptSet.toSelectModel " + lang);

            var retVal = [];
            for (var i in _self.members)
                retVal.push(_self.members[i].toSelectModel(lang));
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

            for (var i in _self.members)
                retVal.concept.push(OpenIZModel.getObjectKey(_self.members[i]));
            return retVal;

        };

    }

    /**
     * @summary This class represents a javascript equivalent to a Concept
     * @class 
     * @constructor
     * @param {Object} valueData The IMSI formatted concept data
     * @property {String} id The identifier of the concept
     * @property {String} versionId The version of the concept
     * @property {Boolean} isSystemConcept Indicates whether the concept is a system (readonly) concept
     * @property {String} mnemonic The mnemonic of the concept
     * @property {Object} status The status concept
     * @property {Object} class The classification of the concept
     * @property {Object} names The names by which the concept is known
     */
    this.Concept = function (valueData) {

        console.info("Concept.ctor " + valueData);

        var _self = this;
        this.$type = "Concept";
        this.id = valueData.id;
        this.versionId = valueData.versionId;
        this.isSystemConcept = valueData.isReadonly;
        this.mnemonic = valueData.mnemonic;
        this.class = valueData.class;
        this.status = {};

        // Map names
        this.names = [];
        for (var i in valueData.name) {
            var nam = valueData.name[i];
            this.names.push(
                {
                    lang: nam.language,
                    value: nam.value
                });
        }

        /**
         * @summary Expand property keys
         */
        this.expand = function (valueData, bundleContext) {
            console.info("Concept.expand " + _self.id);

            _self.status = OpenIZModel.resolveConcept(bundleContext, valueData.statusConcept);
        }

        /**
         * @summary Represents this concept as a select option
         */
        this.toSelectModel = function (lang) {
            console.info("Concept.toSelectModel " + lang);

            for (var i in _self.names) {
                var nam = _self.names[i];
                if (lang == nam.lang)
                    return { id: _self.id, text: nam.value };
            }
            return { id: _self.id, text: _self.mnemonic };
        }

        /**
         * @summary Represents this concept as an IMSI formatted data object
         */
        this.toImsi = function () {

            console.info("Concept.toImsi");

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
                for (var i in _self.names) {
                    var nam = _self.names[i];
                    retVal.name.push({
                        "language": nam.lang,
                        "value": nam.value
                    });
                }
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
        for (var i in valueData.component) {
            var comp = valueData.component[i];
            this.components.push(
            {
                type: OpenIZModel.resolveConcept(bundleContext, comp.type),
                value: comp.value
            });
        }

        /**
         * @summary Represent this as an IMSI component
         */
        this.toImsi = function() {
            var retVal = {
                use: _self.use.id,
                component: []
            };
            for (var c in _self.components) {
                var comp = _self.components[c];

                retVal.component.push({ type: comp.type.id, value: comp.value });
            }
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

        console.info("Patient.ctor " + patientData);

        // Self reference
        var _self = this;
        this.$type = "Patient";
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
        this.relationships = [];
        this.telecoms = [];
        this.languages = patientData.language;
        this.extensions = patientData.extension;
        this.names = [];
        this.notes = [];
        this.addresses = [];
        this.tags = patientData.tag;
        this.participations = [];

        /**
         * @summary Expand any properties on the object
         */
        this.expand = function (valueData, bundleContext) {
            console.info("Patient.expand");

            // Map relationships
            for (var i in patientData.relationship) {
                var rel = patientData.relationship[i];
                _self.relationships.push(
                    {
                        relationshipType: OpenIZModel.resolveConcept(bundleContext, rel.relationshipType),
                        target: OpenIZ.Entity.get(rel.target)
                    });
            }

            // Map telecoms
            for (var i in patientData.telecom) {
                var tel = patientData.telecom[i];
                _self.telecoms.push({
                    use: OpenIZModel.resolveConcept(bundleContext, tel.use),
                    value: tel.value
                });
            }

            // Map names
            for (var i in patientData.name) {
                var nam = patientData.name[i];
                _self.names.push(new OpenIZModel.ComponentizedValue(nam));
            }

            // Map addresses
            for (var i in patientData.address) {
                var add = patientData.address[i];
                _self.addresses.push(new OpenIZModel.ComponentizedValue(add))
            }

            // Map notes
            for (var i in patientData.note) {
                var nt = patientData.note[i];
                _self.notes.push({ author: OpenIZ.Entity.get(nt.author), text: nt.text });
            }

            // Map participations
            for (var i in patientData.participation) {
                var ptcpt = patientData.participation[i];
                _self.participations.push({
                    role: OpenIZModel.resolveConcept(bundleContext, ptcpt.participationRole),
                    act: OpenIZ.Act.get(ptcpt.source)
                });
            }
        };

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
                for (var i in _self.relationships) {
                    var rel = _self.relationships[i];
                    retVal.relationship.push(
                        {
                            relationshipType: OpenIZModel.getObjectKey(rel.relationshipType),
                            target: OpenIZModel.getObjectKey(rel.target)
                        });
                }

            // Map telecoms
            if(_self.telecoms != null)
                for (var i in _self.telecoms) {
                    var tel = _self.telecoms[i];
                    retVal.telecom.push({
                        use: OpenIZModel.getObjectKey(tel.use),
                        value: tel.value
                    });
                }

            // Map names
            if(_self.names != null)
                for (var i in _self.names) {
                    var nam = _self.names[i];
                    retVal.name.push(nam.toImsi());
                }

            // Map addresses
            if(_self.addresses != null)
                for (var i in _self.addresses) {
                    var add = _self.addresses[i];
                    retVal.address.push(add.toImsi());
                }

            // Map notes
            if(_self.notes != null)
                for (var i in _self.notes) {
                    var nt = _self.notes[i];
                    retVal.note.push(
                        {
                            author: OpenIZModel.getObjectKey(nt.author),
                            text: nt.text
                        });
                }

            // Map participations
            if(_self.participations != null)
                for (var i in _self.participations) {
                    var ptcpt = _self.participations[i];
                    retVal.participation.push({
                        participationRole: OpenIZModel.getObjectKey(ptcpt.role),
                        source: OpenIZModel.getObjectKey(ptcpt.act)
                    });
                }

            return retVal;
        }
    };

    /**
     * @class
     * @summary Represents a simple exception class
     */
    this.Exception = function (message, detail, cause) {
        _self = this;

        this.message = message;
        this.details = detail;
        this.caused_by = cause;

    };

};
