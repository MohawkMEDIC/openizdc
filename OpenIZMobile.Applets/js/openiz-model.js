/// <reference path="openiz.js"/>

/**
 * OpenIZ wrapper binding
 *
 * The purpose of this file is to provide a convenient JavaScript wrapper around the OpenIZ host container for applets.
 * This file should be changed whenever you are writing an Applet container which will host applets.
 */

/**
 * @summary Namespace for OpenIZ model wrapper JAvaScript classes
 * @property {String} EmptyGuid An empty GUID
 * @namespace
 */
var OpenIZModel = OpenIZModel || {

    // Empty guid
    EmptyGuid : "00000000-0000-0000-0000-000000000000",

    /**
     * @summary Keys for setting the use of an address
     * @class
    * @property {String} Direct D0DB6EDB-6CDC-4671-8BC2-00F1C808E188
    * @property {String} Ideographic 09000479-4672-44F8-BB4A-72FB25F7356A
    * @property {String} WorkPlace EAA6F08E-BB8E-4457-9DC0-3A1555FADF5C
    * @property {String} PostalAddress 7246E98D-20C6-4AE6-85AD-4AA09649FEB7
    * @property {String} VacationHome 5D69534C-4597-4D11-BB98-56A9918F5238
    * @property {String} Alphabetic 71D1C07C-6EE6-4240-8A95-19F96583512E
    * @property {String} Soundex E5794E3B-3025-436F-9417-5886FEEAD55A
    * @property {String} PhysicalVisit 5724A9B6-24B6-43B7-8075-7A0D61FCB814
    * @property {String} BadAddress F3132FC0-AADD-40B7-B875-961C40695389
    * @property {String} TemporaryAddress CEF6EA31-A097-4F59-8723-A38C727C6597
    * @property {String} Syllabic B4CA3BF0-A7FC-44F3-87D5-E126BEDA93FF
    * @property {String} Phonetic 2B085D38-3308-4664-9F89-48D8EF4DABA7
    * @property {String} HomeAddress 493C3E9D-4F65-4E4D-9582-C9008F4F2EB4
    * @property {String} PrimaryHome C4FAAFD8-FC90-4330-8B4B-E4E64C86B87B
    * @property {String} Public EC35EA7C-55D2-4619-A56B-F7A986412F7F
     */
    AddressUseKeys : {
        Direct : 'D0DB6EDB-6CDC-4671-8BC2-00F1C808E188',
        Ideographic : '09000479-4672-44F8-BB4A-72FB25F7356A',
        WorkPlace : 'EAA6F08E-BB8E-4457-9DC0-3A1555FADF5C',
        PostalAddress : '7246E98D-20C6-4AE6-85AD-4AA09649FEB7',
        VacationHome : '5D69534C-4597-4D11-BB98-56A9918F5238',
        Alphabetic : '71D1C07C-6EE6-4240-8A95-19F96583512E',
        Soundex : 'E5794E3B-3025-436F-9417-5886FEEAD55A',
        PhysicalVisit : '5724A9B6-24B6-43B7-8075-7A0D61FCB814',
        BadAddress : 'F3132FC0-AADD-40B7-B875-961C40695389',
        TemporaryAddress : 'CEF6EA31-A097-4F59-8723-A38C727C6597',
        Syllabic : 'B4CA3BF0-A7FC-44F3-87D5-E126BEDA93FF',
        Phonetic : '2B085D38-3308-4664-9F89-48D8EF4DABA7',
        HomeAddress : '493C3E9D-4F65-4E4D-9582-C9008F4F2EB4',
        PrimaryHome : 'C4FAAFD8-FC90-4330-8B4B-E4E64C86B87B',
        Public : 'EC35EA7C-55D2-4619-A56B-F7A986412F7F'
    },

    /**
     * @summary Address use component keys
     * @class
     */
    AddressComponentKeys : 
    {
        BuildingNumberSuffix : "B2DBF05C-584D-46DB-8CBF-026A6EA30D81", 
        PostBox : "2047F216-F41E-4CFB-A024-05D4D3DE52F5", 
        UnitIdentifier : "908C09DF-81FE-45AC-9233-0881A278A401", 
        AddressLine : "4F342D28-8850-4DAF-8BCA-0B44A255F7ED", 
        DeliveryAddressLine : "F6139B21-3A36-4A3F-B498-0C661F06DF59", 
        Precinct : "ACAFE0F2-E209-43BB-8633-3665FD7C90BA", 
        CensusTract : "4B3A347C-28FA-4560-A1A9-3795C9DB3D3B", 
        DeliveryModeIdentifier : "08BD6027-47EB-43DE-8454-59B7A5D00A3E", 
        DeliveryInstallationArea : "EC9D5AB8-3BE1-448F-9346-6A08253F9DEA", 
        DeliveryMode : "12608636-910D-4BAC-B849-7F999DE20332", 
        BuildingNumber : "F3C86E99-8AFC-4947-9DD8-86412A34B1C7", 
        Delimiter : "4C6B9519-A493-44A9-80E6-32D85109B04B", 
        County : "D9489D56-DDAC-4596-B5C6-8F41D73D8DC5", 
        PostalCode : "78A47122-F9BF-450F-A93F-90A103C5F1E8", 
        CareOf : "8C89A89E-08C5-4374-87F9-ADB3C9261DF6", 
        StreetName : "0432D671-ABC3-4249-872C-AFD5274C2298", 
        StreetType : "121953F6-0465-41DE-8F7A-B0E08204C771", 
        StreetAddressLine : "F69DCFA8-DF18-403B-9217-C59680BAD99E", 
        UnitDesignator : "B18E71CB-203C-4640-83F0-CC86DEBBBBC0", 
        Country : "48B2FFB3-07DB-47BA-AD73-FC8FB8502471", 
        StreetNameBase : "37C7DBC8-4AC6-464A-AF65-D65FCBA60238", 
        Direction : "1F678716-AB8F-4856-9F76-D82FE3165C22", 
        City : "05B85461-578B-4988-BCA6-E3E94BE9DB76", 
        State : "8CF4B0B0-84E5-4122-85FE-6AFA8240C218", 
        DeliveryInstallationType : "684FB800-145C-47C5-98C5-E7AA53802B69", 
        AdditionalLocator : "D2312B8E-BDFB-4012-9397-F14336F8D206", 
        DeliveryInstallationQualifier : "78FB6EED-6549-4F22-AB3E-F3696DA050BC", 
        BuildingNumberNumeric : "3258B4D6-E4DC-43E6-9F29-FD8423A2AE12"
    },

    /**
     * @summary Code system keys
     * @class
    * @property {String} ISO6391 ISO-639-1
    * @property {String} ICD10CM ICD-10 CM
    * @property {String} UCUM Universal Codes for Units of Measure
    * @property {String} ICD9 International Classification of Disease v9
    * @property {String} LOINC Logical Observation Identifiers Names and Codes
    * @property {String} ICD10 International Classification of Disease v10
    * @property {String} ISO6392 ISO 639-2
    * @property {String} SNOMEDCT Systemized Nomenclature of Medicine clinical terms
    * @property {String} CVX HL7 / CDC Common Vaccine Codes
    */
    CodeSystemKeys : 
    {
        ISO6391 : "EB04FE20-BBBC-4C70-9EEF-045BC4F70982", 
        ICD10CM : "ED9742E5-FA5B-4644-9FB5-2F935ED08B1E", 
        UCUM : "4853A702-FFF3-4EFB-8DD7-54AACCA53664", 
        ICD9 : "51EA1E1B-EDC0-455A-A72B-9076860E284D", 
        LOINC : "08C59397-706B-456A-AEB1-9E7D5A2ADC94", 
        ICD10 : "F7A5CBD8-5425-415E-8308-D14B94F56917", 
        ISO6392 : "089044EA-DD41-4258-A497-E6247DD364F6", 
        SNOMEDCT : "B3030751-D4DB-420B-B765-E837607820CD", 
        CVX : "EBA4F94A-2CAD-4BB3-ACA7-F4E54EAAC4BD"
    },

    /**
     * @summary Name use keys
     * @class
    * @property {String} License 48075D19-7B29-4CA5-9C73-0CBD31248446
    * @property {String} Alphabetic 71D1C07C-6EE6-4240-8A95-19F96583512E
    * @property {String} Religious 15207687-5290-4672-A7DF-2880A23DCBB5
    * @property {String} Artist 4A7BF199-F33B-42F9-8B99-32433EA67BD7
    * @property {String} Phonetic 2B085D38-3308-4664-9F89-48D8EF4DABA7
    * @property {String} Indigenous A3FB2A05-5EBE-47AE-AFD0-4C1B22336090
    * @property {String} Soundex E5794E3B-3025-436F-9417-5886FEEAD55A
    * @property {String} Assigned A87A6D21-2CA6-4AEA-88F3-6135CCEB58D1
    * @property {String} Search 87964BFF-E442-481D-9749-69B2A84A1FBE
    * @property {String} Ideographic 09000479-4672-44F8-BB4A-72FB25F7356A
    * @property {String} Pseudonym C31564EF-CA8D-4528-85A8-88245FCEF344
    * @property {String} MaidenName 0674C1C8-963A-4658-AFF9-8CDCD308FA68
    * @property {String} Legal EFFE122D-8D30-491D-805D-ADDCB4466C35
    * @property {String} OfficialRecord 1EC9583A-B019-4BAA-B856-B99CAF368656
    * @property {String} Syllabic B4CA3BF0-A7FC-44F3-87D5-E126BEDA93FF
    * @property {String} Anonymous 95E6843A-26FF-4046-B6F4-EB440D4B85F7
     */
    NameUseKeys : 
    {
        License : "48075D19-7B29-4CA5-9C73-0CBD31248446", 
        Alphabetic : "71D1C07C-6EE6-4240-8A95-19F96583512E", 
        Religious : "15207687-5290-4672-A7DF-2880A23DCBB5", 
        Artist : "4A7BF199-F33B-42F9-8B99-32433EA67BD7", 
        Phonetic : "2B085D38-3308-4664-9F89-48D8EF4DABA7", 
        Indigenous : "A3FB2A05-5EBE-47AE-AFD0-4C1B22336090", 
        Soundex : "E5794E3B-3025-436F-9417-5886FEEAD55A", 
        Assigned : "A87A6D21-2CA6-4AEA-88F3-6135CCEB58D1", 
        Search : "87964BFF-E442-481D-9749-69B2A84A1FBE", 
        Ideographic : "09000479-4672-44F8-BB4A-72FB25F7356A", 
        Pseudonym : "C31564EF-CA8D-4528-85A8-88245FCEF344", 
        MaidenName : "0674C1C8-963A-4658-AFF9-8CDCD308FA68", 
        Legal: "EFFE122D-8D30-491D-805D-addcb4466c35",
        OfficialRecord : "1EC9583A-B019-4BAA-B856-B99CAF368656", 
        Syllabic : "B4CA3BF0-A7FC-44F3-87D5-E126BEDA93FF", 
        Anonymous : "95E6843A-26FF-4046-B6F4-EB440D4B85F7"
    },

    /**
     * @summary Name component type keys
     * @class
    * @property {String} Title 4386D92A-D81B-4033-B968-01E57E20D5E0
    * @property {String} Family 29B98455-ED61-49F8-A161-2D73363E1DF0
    * @property {String} Delimiter 4C6B9519-A493-44A9-80E6-32D85109B04B
    * @property {String} Prefix A787187B-6BE4-401E-8836-97FC000C5D16
    * @property {String} Given 2F64BDE2-A696-4B0A-9690-B21EBD7E5092
    * @property {String} Suffix 064523DF-BB03-4932-9323-CDF0CC9590BA
     */
    NameComponentKeys : 
    {
        Title : "4386D92A-D81B-4033-B968-01E57E20D5E0", 
        Family : "29B98455-ED61-49F8-A161-2D73363E1DF0", 
        Delimiter : "4C6B9519-A493-44A9-80E6-32D85109B04B", 
        Prefix : "A787187B-6BE4-401E-8836-97FC000C5D16", 
        Given : "2F64BDE2-A696-4B0A-9690-B21EBD7E5092", 
        Suffix: "064523DF-BB03-4932-9323-CDF0CC9590BA"
    },

    /**
     * @summary Status concept keys
     * @class
     * @property {String} 
     * @property {String} New Object is 'new' meaning no business rules have been executed and the object needs review
     * @property {String} Obsolete Object is obsolete (no longer active)
     * @property {String} Nullfied Object was created in error
     * @property {String} Active Object is active
     */
    StatusConceptKeys : {
        New  : "C34FCBF1-E0FE-4989-90FD-0DC49E1B9685",
        Obsolete  :"BDEF5F90-5497-4F26-956C-8F818CCE2BD2",
        Nullfied  :"CD4AA3C4-02D5-4CC9-9088-EF8F31E321C5",
        Active  : "C8064CBD-FA06-4530-B430-1A52F1530C27"
    },

    /**
     * @summary Represents utilities for dealing with bundles
     * @param {Object} bundleData The IMSI formatted bundle
     * @class
     * @constructor
     * @property {String} entry Identifier of the entry which is the first object for the bundle
     * @property {Object} items The collection of items contained within the bundle
     * @property {Numeric} offset The offset of the bundle result set
     * @property {Numeric} totalResults The total results in the query
     * @property {Numeric} count The number of results in the current bundle
     */
    Bundle : function (bundleData) {

        // console.info("Bundle.ctor " + bundleData);
        var _self = this;
        this.$type = "Bundle";

        for (var v in bundleData)
            this[v] = bundleData[v];

        /**
         * @summary Gets the item with specified id from the specified bundle
         * @param {String} id The identifier of the entry
         */
        this.getItem = function (id) {
            // console.info("Bundle.getItem " + id);
            for (var i in _self.item) {
                var itm = _self.item[i];
                if (itm.id == id)
                    return itm;
            }
            return null;
        };

        /**
         * @summary Gets the entry object in the bundle
         */
        this.getEntry = function () {
            // console.info("Bundle.getEntry");
            if (_self.entry == null) return null;
            for (var i in _self.item) {
                var itm = _self.item[i];
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
            // console.info("Bundle.first " + type);
            for (var i in _self.item) {
                var itm = _self.item[i];
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
            // console.info("Bundle.all " + type);

            var retVal = new OpenIZModel.Bundle({
                offset: _self.offset,
                count: _self.count,
                totalResults : _self.totalResults
            });

            for (var i in _self.item) {
                var itm = _self.item[i];
                if (itm.$type == type)
                    retVal.item.push(itm);
            }
            return retVal;
        };

        /**
         * @summary Merge this bundle with another
         * @param {OpenIZModel#Bundle} otherBundle The other bundle to merge
         */
        this.merge = function (otherBundle) {
            for (var i in otherBundle.item) {
                var itm = otherBundle.item[i];
                if (_self.getItem(itm.id) == null)
                    _self.items.push(itm);
            }
            // console.info("Bundle now contains " + _self.items.length + " items");
        };

        
    },

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
    Session : function (sessionData) {

        // console.info("Session.ctor " + sessionData);

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
    },

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
    * @property {OpenIZModel#Entity} entity The entity (userEntity) of the user
    * @property {Date} lastLoginTime The last login time
    * @property {String} phoneNumber The phone number of the user
    * @property {Bool} phoneNumberConfirmed Whether the phone number is confirmed
     */
    SecurityUser : function (securityUserData) {

        // console.info("SecurityUser.ctor " + securityUserData);

        var _self = this;
        this.$type = "SecurityUser";

        for (var v in securityUserData)
            this[v] = securityUserData[v];

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


    },

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
    ConceptSet : function(valueData) {

        // console.info("ConceptSet.ctor " + valueData);
        var _self = this;
        this.$type = "ConceptSet";

        for (var v in bundleData)
            this[v] = bundleData[v];


    },

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
    Concept : function (valueData) {

        // console.info("Concept.ctor " + valueData);
        var _self = this;
        this.$type = "Concept";

        for (var v in bundleData)
            this[v] = bundleData[v];
    },
    /**
     * @summary This class represents data related to a patient
     * @class The model patient class
     * @constructor
     * @param {Object} patientData The IMSI formatted patient data (see: http://openiz.org/artifacts/0.01pre/imsi)
    * @property {String} id The unique identifier of the patient
    * @property {String} versionId The version identifier of the patient
    * @property {Date} deceasedDate The date that this patient bacame deceased
    * @property {String} deceasedDatePrecision The precision of the deceased date (if unknown)
    * @property {Number} multipleBirthOrder The order that this patient was in a multiple birth
    * @property {String} gender The gender of the patient
    * @property {Date} dateOfBirth The date of birth of the patient
    * @property {String} dateOfBirthPrecision The precision of the date of birth if not exact
    * @property {OpenIZModel#Concept} status The status of the patient
    * @property {OpenIZModel#Concept} type The type concept
    * @property {Object} identifiers All identifiers related to the patient
    * @property {Object} relationship All Relationships that the patient holds
    * @property {Object} languages Languages of communication the patient can be contacted in
    * @property {Object} telecoms Telecommunications addresses for the patient
    * @property {Object} extensions Extensions to the core patient object
    * @property {OpenIZModel#ComponentizedValue} names The series of names which the patient uses
    * @property {OpenIZModel#ComponentizedValue} addresses The series of contact addresses for the patient
    * @property {Object} notes Additional textual notes about the patient
    * @property {Object} tags A series of key/value pairs which are used to tag the patient record
    * @property {Object} participations The series of acts that the patient participates in
     */
    Patient : function (patientData) {

        // console.info("Patient.ctor " + patientData);
        
        // Self reference
        var _self = this;
        this.$type = "Patient";
        for (var v in patientData)
            this[v] = patientData[v];

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
        };

        /**
         * @summary Obsolete this patient in the database
         */
        this.obsolete = function () {
            return OpenIZ.Patient.obsolete(_self.key);
        };

    },

    /**
     * @class
     * @summary Represents a simple exception class
     * @constructor
     * @property {String} message Informational message about the exception
     * @property {Object} details Any detail / diagnostic information
     * @property {OpenIZModel#Exception} caused_by The cause of the exception
     * @param {String} message Informational message about the exception
     * @param {Object} detail Any detail / diagnostic information
     * @param {OpenIZModel#Exception} cause The cause of the exception
     */
    Exception : function (message, detail, cause) {
        _self = this;

        this.message = message;
        this.details = detail;
        this.caused_by = cause;

    }
    

};

