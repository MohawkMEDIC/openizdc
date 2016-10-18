/// <reference path="openiz.js"/>

/*
 * Copyright (C) 2015-2016 Mohawk College of Applied Arts and Technology
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
 * Date: 2016-10-18
 */

/**
 * @summary A documented namespace
 * @namespace
 */
var OpenIZModel = OpenIZModel || {

    /**
    * @summary Keys for the setting of a mood for an act.
    * @class
    * @property {String} Appointment: 'C46EEE70-5612-473F-8D24-595EA15C9C39',
    * @property {String} AppointmentRequest: '0395F357-6821-4562-8192-49AC3C94F548',
    * @property {String} Definition: '3B14A426-6337-4F2A-B83B-E6BE7DBCD5A5',
    * @property {String} EventOccurrence: 'EC74541F-87C4-4327-A4B9-97F325501747',
    * @property {String} Goal: '13925967-E748-4DD6-B562-1E1DA3DDFB06',
    * @property {String} Intent: '099BCC5E-8E2F-4D50-B509-9F9D5BBEB58E',
    * @property {String} Promise: 'B389DEDF-BE61-456B-AA70-786E1A5A69E0',
    * @property {String} Propose: 'ACF7BAF2-221F-4BC2-8116-CEB5165BE079',
    * @property {String} Request: 'E658CA72-3B6A-4099-AB6E-7CF6861A5B61'
    */
    ActMoodKeys:
    {
        Appointment: 'C46EEE70-5612-473F-8D24-595EA15C9C39',
        AppointmentRequest: '0395F357-6821-4562-8192-49AC3C94F548',
        Definition: '3B14A426-6337-4F2A-B83B-E6BE7DBCD5A5',
        EventOccurrence: 'EC74541F-87C4-4327-A4B9-97F325501747',
        Goal: '13925967-E748-4DD6-B562-1E1DA3DDFB06',
        Intent: '099BCC5E-8E2F-4D50-B509-9F9D5BBEB58E',
        Promise: 'B389DEDF-BE61-456B-AA70-786E1A5A69E0',
        Propose: 'ACF7BAF2-221F-4BC2-8116-CEB5165BE079',
        Request: 'E658CA72-3B6A-4099-AB6E-7CF6861A5B61'
    },
    /**
    * @summary Address use component keys
    * @class
    */
    AddressComponentKeys:
    {
        BuildingNumberSuffix: "B2DBF05C-584D-46DB-8CBF-026A6EA30D81",
        PostBox: "2047F216-F41E-4CFB-A024-05D4D3DE52F5",
        UnitIdentifier: "908C09DF-81FE-45AC-9233-0881A278A401",
        AddressLine: "4F342D28-8850-4DAF-8BCA-0B44A255F7ED",
        DeliveryAddressLine: "F6139B21-3A36-4A3F-B498-0C661F06DF59",
        Precinct: "ACAFE0F2-E209-43BB-8633-3665FD7C90BA",
        CensusTract: "4B3A347C-28FA-4560-A1A9-3795C9DB3D3B",
        DeliveryModeIdentifier: "08BD6027-47EB-43DE-8454-59B7A5D00A3E",
        DeliveryInstallationArea: "EC9D5AB8-3BE1-448F-9346-6A08253F9DEA",
        DeliveryMode: "12608636-910D-4BAC-B849-7F999DE20332",
        BuildingNumber: "F3C86E99-8AFC-4947-9DD8-86412A34B1C7",
        Delimiter: "4C6B9519-A493-44A9-80E6-32D85109B04B",
        County: "D9489D56-DDAC-4596-B5C6-8F41D73D8DC5",
        PostalCode: "78A47122-F9BF-450F-A93F-90A103C5F1E8",
        CareOf: "8C89A89E-08C5-4374-87F9-ADB3C9261DF6",
        StreetName: "0432D671-ABC3-4249-872C-AFD5274C2298",
        StreetType: "121953F6-0465-41DE-8F7A-B0E08204C771",
        StreetAddressLine: "F69DCFA8-DF18-403B-9217-C59680BAD99E",
        UnitDesignator: "B18E71CB-203C-4640-83F0-CC86DEBBBBC0",
        Country: "48B2FFB3-07DB-47BA-AD73-FC8FB8502471",
        StreetNameBase: "37C7DBC8-4AC6-464A-AF65-D65FCBA60238",
        Direction: "1F678716-AB8F-4856-9F76-D82FE3165C22",
        City: "05B85461-578B-4988-BCA6-E3E94BE9DB76",
        State: "8CF4B0B0-84E5-4122-85FE-6AFA8240C218",
        DeliveryInstallationType: "684FB800-145C-47C5-98C5-E7AA53802B69",
        AdditionalLocator: "D2312B8E-BDFB-4012-9397-F14336F8D206",
        DeliveryInstallationQualifier: "78FB6EED-6549-4F22-AB3E-F3696DA050BC",
        BuildingNumberNumeric: "3258B4D6-E4DC-43E6-9F29-FD8423A2AE12"
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
    NameUseKeys:
    {
        License: "48075D19-7B29-4CA5-9C73-0CBD31248446",
        Alphabetic: "71D1C07C-6EE6-4240-8A95-19F96583512E",
        Religious: "15207687-5290-4672-A7DF-2880A23DCBB5",
        Artist: "4A7BF199-F33B-42F9-8B99-32433EA67BD7",
        Phonetic: "2B085D38-3308-4664-9F89-48D8EF4DABA7",
        Indigenous: "A3FB2A05-5EBE-47AE-AFD0-4C1B22336090",
        Soundex: "E5794E3B-3025-436F-9417-5886FEEAD55A",
        Assigned: "A87A6D21-2CA6-4AEA-88F3-6135CCEB58D1",
        Search: "87964BFF-E442-481D-9749-69B2A84A1FBE",
        Ideographic: "09000479-4672-44F8-BB4A-72FB25F7356A",
        Pseudonym: "C31564EF-CA8D-4528-85A8-88245FCEF344",
        MaidenName: "0674C1C8-963A-4658-AFF9-8CDCD308FA68",
        Legal: "EFFE122D-8D30-491D-805D-addcb4466c35",
        OfficialRecord: "1EC9583A-B019-4BAA-B856-B99CAF368656",
        Syllabic: "B4CA3BF0-A7FC-44F3-87D5-E126BEDA93FF",
        Anonymous: "95E6843A-26FF-4046-B6F4-EB440D4B85F7"
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
    StatusConceptKeys: {
        New: "C34FCBF1-E0FE-4989-90FD-0DC49E1B9685",
        Obsolete: "BDEF5F90-5497-4F26-956C-8F818CCE2BD2",
        Nullfied: "CD4AA3C4-02D5-4CC9-9088-EF8F31E321C5",
        Active: "C8064CBD-FA06-4530-B430-1A52F1530C27"
    },
    // OpenIZ.Core.Model.BaseEntityData, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @abstract
     * @extends OpenIZModel.IdentifiedData
     * @summary             Represents the root of all model classes in the OpenIZ Core            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.BaseEntityData} copyData Copy constructor (if present)
     */
    BaseEntityData: function (copyData) {
        if (copyData) {
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // BaseEntityData 
    // OpenIZ.Core.Model.Association`1, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @abstract
     * @extends OpenIZModel.IdentifiedData
     * @summary             Represents a bse class for bound relational data            
     * @property {date} modifiedOn            Get the modification date            
     * @property {uuid} source            Gets or sets the source entity's key (where the relationship is FROM)            
     * @property {OpenIZModel.IdentifiedData} sourceModel [Delay loaded from source],             The entity that this relationship targets            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.Association} copyData Copy constructor (if present)
     */
    Association: function (copyData) {
        if (copyData) {
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // Association 
    // OpenIZ.Core.Model.IdentifiedData, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @abstract
     * @summary             Represents data that is identified by a key            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {date} modifiedOn            Gets or sets the modified on time            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.IdentifiedData} copyData Copy constructor (if present)
     */
    IdentifiedData: function (copyData) {
        if (copyData) {
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.modifiedOn = copyData.modifiedOn;
            this.etag = copyData.etag;
        }
    },  // IdentifiedData 
    // OpenIZ.Core.Model.NonVersionedEntityData, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.BaseEntityData
     * @summary             Updateable entity data which is not versioned            
     * @property {string} updatedTime            Gets or sets the creation time in XML format            
     * @property {date} modifiedOn            Gets the time this item was modified            
     * @property {uuid} updatedBy            Gets or sets the created by identifier            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.NonVersionedEntityData} copyData Copy constructor (if present)
     */
    NonVersionedEntityData: function (copyData) {
        if (copyData) {
            this.updatedTime = copyData.updatedTime;
            this.modifiedOn = copyData.modifiedOn;
            this.updatedBy = copyData.updatedBy;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // NonVersionedEntityData 
    // OpenIZ.Core.Model.VersionedAssociation`1, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @abstract
     * @extends OpenIZModel.Association
     * @summary             Represents a relational class which is bound on a version boundary            
     * @property {number} effectiveVersionSequence            Gets or sets the effective version of this type            
     * @property {number} obsoleteVersionSequence            Gets or sets the obsoleted version identifier            
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.VersionedEntityData} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.VersionedAssociation} copyData Copy constructor (if present)
     */
    VersionedAssociation: function (copyData) {
        if (copyData) {
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // VersionedAssociation 
    // OpenIZ.Core.Model.VersionedEntityData`1, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @abstract
     * @extends OpenIZModel.BaseEntityData
     * @summary             Represents versioned based data, that is base data which has versions            
     * @property {string} etag            Override the ETag            
     * @property {uuid} previousVersion            Gets or sets the previous version key            
     * @property {OpenIZModel.VersionedEntityData} previousVersionModel [Delay loaded from previousVersion],             Gets or sets the previous version            
     * @property {uuid} version            Gets or sets the key which represents the version of the entity            
     * @property {number} sequence            The sequence number of the version (for ordering)            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.VersionedEntityData} copyData Copy constructor (if present)
     */
    VersionedEntityData: function (copyData) {
        if (copyData) {
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // VersionedEntityData 
    // OpenIZ.Core.Model.Security.SecurityApplication, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.NonVersionedEntityData
     * @summary             Represents a security application            
     * @property {string} applicationSecret            Gets or sets the application secret used for authenticating the application            
     * @property {string} name            Gets or sets the name of the security device/user/role/devie            
     * @property {string} updatedTime            Gets or sets the creation time in XML format            
     * @property {date} modifiedOn            Gets the time this item was modified            
     * @property {uuid} updatedBy            Gets or sets the created by identifier            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.SecurityApplication} copyData Copy constructor (if present)
     */
    SecurityApplication: function (copyData) {
        if (copyData) {
            this.applicationSecret = copyData.applicationSecret;
            this.name = copyData.name;
            this.updatedTime = copyData.updatedTime;
            this.modifiedOn = copyData.modifiedOn;
            this.updatedBy = copyData.updatedBy;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // SecurityApplication 
    // OpenIZ.Core.Model.Security.SecurityDevice, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.NonVersionedEntityData
     * @summary             Represents a security device            
     * @property {string} deviceSecret            Gets or sets the device secret            
     * @property {string} name            Gets or sets the name of the security device/user/role/devie            
     * @property {string} updatedTime            Gets or sets the creation time in XML format            
     * @property {date} modifiedOn            Gets the time this item was modified            
     * @property {uuid} updatedBy            Gets or sets the created by identifier            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.SecurityDevice} copyData Copy constructor (if present)
     */
    SecurityDevice: function (copyData) {
        if (copyData) {
            this.deviceSecret = copyData.deviceSecret;
            this.name = copyData.name;
            this.updatedTime = copyData.updatedTime;
            this.modifiedOn = copyData.modifiedOn;
            this.updatedBy = copyData.updatedBy;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // SecurityDevice 
    // OpenIZ.Core.Model.Security.SecurityEntity, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.NonVersionedEntityData
     * @summary             Security Entity base class            
     * @property {string} updatedTime            Gets or sets the creation time in XML format            
     * @property {date} modifiedOn            Gets the time this item was modified            
     * @property {uuid} updatedBy            Gets or sets the created by identifier            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.NonVersionedEntityData} copyData Copy constructor (if present)
     */
    NonVersionedEntityData: function (copyData) {
        if (copyData) {
            this.updatedTime = copyData.updatedTime;
            this.modifiedOn = copyData.modifiedOn;
            this.updatedBy = copyData.updatedBy;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // NonVersionedEntityData 
    // OpenIZ.Core.Model.Security.SecurityPolicy, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.BaseEntityData
     * @summary             Represents a simply security policy            
     * @property {string} handler            Gets or sets the handler which may handle this policy            
     * @property {string} name            Gets or sets the name of the policy            
     * @property {string} oid            Gets or sets the universal ID            
     * @property {bool} isPublic            Whether the property is public            
     * @property {bool} canOverride            Whether the policy can be elevated over            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.SecurityPolicy} copyData Copy constructor (if present)
     */
    SecurityPolicy: function (copyData) {
        if (copyData) {
            this.handler = copyData.handler;
            this.name = copyData.name;
            this.oid = copyData.oid;
            this.isPublic = copyData.isPublic;
            this.canOverride = copyData.canOverride;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // SecurityPolicy 
    // OpenIZ.Core.Model.Security.SecurityPolicyInstance, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Association
     * @summary             Represents a security policy instance            
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.NonVersionedEntityData} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.SecurityPolicyInstance} copyData Copy constructor (if present)
     */
    SecurityPolicyInstance: function (copyData) {
        if (copyData) {
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // SecurityPolicyInstance 
    // OpenIZ.Core.Model.Security.SecurityRole, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.NonVersionedEntityData
     * @summary             Security role            
     * @property {string} name            Gets or sets the name of the security role            
     * @property {string} description            Description of the role            
     * @property {string} updatedTime            Gets or sets the creation time in XML format            
     * @property {date} modifiedOn            Gets the time this item was modified            
     * @property {uuid} updatedBy            Gets or sets the created by identifier            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.NonVersionedEntityData} copyData Copy constructor (if present)
     */
    NonVersionedEntityData: function (copyData) {
        if (copyData) {
            this.name = copyData.name;
            this.description = copyData.description;
            this.updatedTime = copyData.updatedTime;
            this.modifiedOn = copyData.modifiedOn;
            this.updatedBy = copyData.updatedBy;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // NonVersionedEntityData 
    // OpenIZ.Core.Model.Security.SecurityUser, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.NonVersionedEntityData
     * @summary             Security user represents a user for the purpose of security             
     * @property {string} email            Gets or sets the email address of the user            
     * @property {bool} emailConfirmed            Gets or sets whether the email address is confirmed            
     * @property {number} invalidLoginAttempts            Gets or sets the number of invalid login attempts by the user            
     * @property {string} lockout            Gets or sets the creation time in XML format            
     * @property {string} passwordHash            Gets or sets whether the password hash is enabled            
     * @property {string} securityStamp            Gets or sets whether the security has is enabled            
     * @property {bool} twoFactorEnabled            Gets or sets whether two factor authentication is required            
     * @property {string} userName            Gets or sets the logical user name ofthe user            
     * @property {bytea} photo            Gets or sets the binary representation of the user's photo            
     * @property {string} lastLoginTime            Gets or sets the creation time in XML format            
     * @property {string} phoneNumber            Gets or sets the patient's phone number            
     * @property {bool} phoneNumberConfirmed            Gets or sets whether the phone number was confirmed            
     * @property {uuid} userClass            Gets or sets the user class key            
     * @property {string} etag            Gets the etag            
     * @property {string} updatedTime            Gets or sets the creation time in XML format            
     * @property {date} modifiedOn            Gets the time this item was modified            
     * @property {uuid} updatedBy            Gets or sets the created by identifier            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.SecurityUser} copyData Copy constructor (if present)
     */
    SecurityUser: function (copyData) {
        if (copyData) {
            this.email = copyData.email;
            this.emailConfirmed = copyData.emailConfirmed;
            this.invalidLoginAttempts = copyData.invalidLoginAttempts;
            this.lockout = copyData.lockout;
            this.passwordHash = copyData.passwordHash;
            this.securityStamp = copyData.securityStamp;
            this.twoFactorEnabled = copyData.twoFactorEnabled;
            this.userName = copyData.userName;
            this.photo = copyData.photo;
            this.lastLoginTime = copyData.lastLoginTime;
            this.phoneNumber = copyData.phoneNumber;
            this.phoneNumberConfirmed = copyData.phoneNumberConfirmed;
            this.userClass = copyData.userClass;
            this.etag = copyData.etag;
            this.updatedTime = copyData.updatedTime;
            this.modifiedOn = copyData.modifiedOn;
            this.updatedBy = copyData.updatedBy;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // SecurityUser 
    // OpenIZ.Core.Model.Roles.Patient, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Person
     * @summary             Represents an entity which is a patient            
     * @property {date} deceasedDate            Gets or sets the date the patient was deceased            
     * @property {DatePrecision} deceasedDatePrecision            Gets or sets the precision of the date of deceased            
     * @property {number} multipleBirthOrder            Gets or sets the multiple birth order of the patient             
     * @property {uuid} genderConcept            Gets or sets the gender concept key            
     * @property {OpenIZModel.Concept} genderConceptModel [Delay loaded from genderConcept],             Gets or sets the gender concept            
     * @property {date} dateOfBirth            Gets or sets the person's date of birth            
     * @property {DatePrecision} dateOfBirthPrecision            Gets or sets the precision ofthe date of birth            
     * @property {OpenIZModel.PersonLanguageCommunication} language            Gets the person's languages of communication            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} determinerConcept            Determiner concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} creationAct            Creation act reference            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.EntityIdentifier} identifier            Gets the identifiers associated with this entity            
     * @property {OpenIZModel.EntityRelationship} relationship            Gets a list of all associated entities for this entity            
     * @property {OpenIZModel.EntityTelecomAddress} telecom            Gets a list of all telecommunications addresses associated with the entity            
     * @property {OpenIZModel.EntityExtension} extension            Gets a list of all extensions associated with the entity            
     * @property {OpenIZModel.EntityName} name            Gets a list of all names associated with the entity            
     * @property {OpenIZModel.EntityAddress} address            Gets a list of all addresses associated with the entity            
     * @property {OpenIZModel.EntityNote} note            Gets a list of all notes associated with the entity            
     * @property {OpenIZModel.EntityTag} tag            Gets a list of all tags associated with the entity            
     * @property {OpenIZModel.ActParticipation} participation            Gets the acts in which this entity participates            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Entity} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.Patient} copyData Copy constructor (if present)
     */
    Patient: function (copyData) {
        if (copyData) {
            this.deceasedDate = copyData.deceasedDate;
            this.deceasedDatePrecision = copyData.deceasedDatePrecision;
            this.multipleBirthOrder = copyData.multipleBirthOrder;
            this.genderConcept = copyData.genderConcept;
            this.genderConceptModel = copyData.genderConceptModel;
            this.dateOfBirth = copyData.dateOfBirth;
            this.dateOfBirthPrecision = copyData.dateOfBirthPrecision;
            this.language = copyData.language;
            this.template = copyData.template;
            this.classConcept = copyData.classConcept;
            this.determinerConcept = copyData.determinerConcept;
            this.statusConcept = copyData.statusConcept;
            this.creationAct = copyData.creationAct;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.determinerConceptModel = copyData.determinerConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.creationActModel = copyData.creationActModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.telecom = copyData.telecom;
            this.extension = copyData.extension;
            this.name = copyData.name;
            this.address = copyData.address;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // Patient 
    // OpenIZ.Core.Model.Roles.Provider, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Person
     * @summary             Represents a provider role of a person            
     * @property {uuid} providerSpecialty            Gets or sets the provider specialty key            
     * @property {OpenIZModel.Concept} providerSpecialtyModel [Delay loaded from providerSpecialty],             Gets or sets the provider specialty            
     * @property {date} dateOfBirth            Gets or sets the person's date of birth            
     * @property {DatePrecision} dateOfBirthPrecision            Gets or sets the precision ofthe date of birth            
     * @property {OpenIZModel.PersonLanguageCommunication} language            Gets the person's languages of communication            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} determinerConcept            Determiner concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} creationAct            Creation act reference            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.EntityIdentifier} identifier            Gets the identifiers associated with this entity            
     * @property {OpenIZModel.EntityRelationship} relationship            Gets a list of all associated entities for this entity            
     * @property {OpenIZModel.EntityTelecomAddress} telecom            Gets a list of all telecommunications addresses associated with the entity            
     * @property {OpenIZModel.EntityExtension} extension            Gets a list of all extensions associated with the entity            
     * @property {OpenIZModel.EntityName} name            Gets a list of all names associated with the entity            
     * @property {OpenIZModel.EntityAddress} address            Gets a list of all addresses associated with the entity            
     * @property {OpenIZModel.EntityNote} note            Gets a list of all notes associated with the entity            
     * @property {OpenIZModel.EntityTag} tag            Gets a list of all tags associated with the entity            
     * @property {OpenIZModel.ActParticipation} participation            Gets the acts in which this entity participates            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Entity} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.Provider} copyData Copy constructor (if present)
     */
    Provider: function (copyData) {
        if (copyData) {
            this.providerSpecialty = copyData.providerSpecialty;
            this.providerSpecialtyModel = copyData.providerSpecialtyModel;
            this.dateOfBirth = copyData.dateOfBirth;
            this.dateOfBirthPrecision = copyData.dateOfBirthPrecision;
            this.language = copyData.language;
            this.template = copyData.template;
            this.classConcept = copyData.classConcept;
            this.determinerConcept = copyData.determinerConcept;
            this.statusConcept = copyData.statusConcept;
            this.creationAct = copyData.creationAct;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.determinerConceptModel = copyData.determinerConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.creationActModel = copyData.creationActModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.telecom = copyData.telecom;
            this.extension = copyData.extension;
            this.name = copyData.name;
            this.address = copyData.address;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // Provider 
    // OpenIZ.Core.Model.Entities.UserEntity, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Person
     * @summary             Represents a user entity            
     * @property {uuid} securityUser            Gets or sets the security user key            
     * @property {OpenIZModel.SecurityUser} securityUserModel [Delay loaded from securityUser],             Gets or sets the security user key            
     * @property {date} dateOfBirth            Gets or sets the person's date of birth            
     * @property {DatePrecision} dateOfBirthPrecision            Gets or sets the precision ofthe date of birth            
     * @property {OpenIZModel.PersonLanguageCommunication} language            Gets the person's languages of communication            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} determinerConcept            Determiner concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} creationAct            Creation act reference            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.EntityIdentifier} identifier            Gets the identifiers associated with this entity            
     * @property {OpenIZModel.EntityRelationship} relationship            Gets a list of all associated entities for this entity            
     * @property {OpenIZModel.EntityTelecomAddress} telecom            Gets a list of all telecommunications addresses associated with the entity            
     * @property {OpenIZModel.EntityExtension} extension            Gets a list of all extensions associated with the entity            
     * @property {OpenIZModel.EntityName} name            Gets a list of all names associated with the entity            
     * @property {OpenIZModel.EntityAddress} address            Gets a list of all addresses associated with the entity            
     * @property {OpenIZModel.EntityNote} note            Gets a list of all notes associated with the entity            
     * @property {OpenIZModel.EntityTag} tag            Gets a list of all tags associated with the entity            
     * @property {OpenIZModel.ActParticipation} participation            Gets the acts in which this entity participates            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Entity} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.UserEntity} copyData Copy constructor (if present)
     */
    UserEntity: function (copyData) {
        if (copyData) {
            this.securityUser = copyData.securityUser;
            this.securityUserModel = copyData.securityUserModel;
            this.dateOfBirth = copyData.dateOfBirth;
            this.dateOfBirthPrecision = copyData.dateOfBirthPrecision;
            this.language = copyData.language;
            this.template = copyData.template;
            this.classConcept = copyData.classConcept;
            this.determinerConcept = copyData.determinerConcept;
            this.statusConcept = copyData.statusConcept;
            this.creationAct = copyData.creationAct;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.determinerConceptModel = copyData.determinerConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.creationActModel = copyData.creationActModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.telecom = copyData.telecom;
            this.extension = copyData.extension;
            this.name = copyData.name;
            this.address = copyData.address;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // UserEntity 
    // OpenIZ.Core.Model.Entities.ApplicationEntity, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Entity
     * @summary             An associative entity which links a SecurityApplication to an Entity            
     * @property {uuid} securityApplication            Gets or sets the security application            
     * @property {OpenIZModel.SecurityApplication} securityApplicationModel [Delay loaded from securityApplication],             Gets or sets the security application            
     * @property {string} softwareName            Gets or sets the name of the software            
     * @property {string} versionName            Gets or sets the version of the software            
     * @property {string} vendorName            Gets or sets the vendoer name of the software            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} determinerConcept            Determiner concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} creationAct            Creation act reference            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.EntityIdentifier} identifier            Gets the identifiers associated with this entity            
     * @property {OpenIZModel.EntityRelationship} relationship            Gets a list of all associated entities for this entity            
     * @property {OpenIZModel.EntityTelecomAddress} telecom            Gets a list of all telecommunications addresses associated with the entity            
     * @property {OpenIZModel.EntityExtension} extension            Gets a list of all extensions associated with the entity            
     * @property {OpenIZModel.EntityName} name            Gets a list of all names associated with the entity            
     * @property {OpenIZModel.EntityAddress} address            Gets a list of all addresses associated with the entity            
     * @property {OpenIZModel.EntityNote} note            Gets a list of all notes associated with the entity            
     * @property {OpenIZModel.EntityTag} tag            Gets a list of all tags associated with the entity            
     * @property {OpenIZModel.ActParticipation} participation            Gets the acts in which this entity participates            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Entity} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.ApplicationEntity} copyData Copy constructor (if present)
     */
    ApplicationEntity: function (copyData) {
        if (copyData) {
            this.securityApplication = copyData.securityApplication;
            this.securityApplicationModel = copyData.securityApplicationModel;
            this.softwareName = copyData.softwareName;
            this.versionName = copyData.versionName;
            this.vendorName = copyData.vendorName;
            this.template = copyData.template;
            this.classConcept = copyData.classConcept;
            this.determinerConcept = copyData.determinerConcept;
            this.statusConcept = copyData.statusConcept;
            this.creationAct = copyData.creationAct;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.determinerConceptModel = copyData.determinerConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.creationActModel = copyData.creationActModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.telecom = copyData.telecom;
            this.extension = copyData.extension;
            this.name = copyData.name;
            this.address = copyData.address;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // ApplicationEntity 
    // OpenIZ.Core.Model.Entities.DeviceEntity, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Entity
     * @summary             Represents a device entity            
     * @property {uuid} securityDevice            Gets or sets the security device key            
     * @property {OpenIZModel.SecurityDevice} securityDeviceModel [Delay loaded from securityDevice],             Gets or sets the security device            
     * @property {string} manufacturerModelName            Gets or sets the manufacturer model name            
     * @property {string} operatingSystemName            Gets or sets the operating system name            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} determinerConcept            Determiner concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} creationAct            Creation act reference            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.EntityIdentifier} identifier            Gets the identifiers associated with this entity            
     * @property {OpenIZModel.EntityRelationship} relationship            Gets a list of all associated entities for this entity            
     * @property {OpenIZModel.EntityTelecomAddress} telecom            Gets a list of all telecommunications addresses associated with the entity            
     * @property {OpenIZModel.EntityExtension} extension            Gets a list of all extensions associated with the entity            
     * @property {OpenIZModel.EntityName} name            Gets a list of all names associated with the entity            
     * @property {OpenIZModel.EntityAddress} address            Gets a list of all addresses associated with the entity            
     * @property {OpenIZModel.EntityNote} note            Gets a list of all notes associated with the entity            
     * @property {OpenIZModel.EntityTag} tag            Gets a list of all tags associated with the entity            
     * @property {OpenIZModel.ActParticipation} participation            Gets the acts in which this entity participates            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Entity} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.DeviceEntity} copyData Copy constructor (if present)
     */
    DeviceEntity: function (copyData) {
        if (copyData) {
            this.securityDevice = copyData.securityDevice;
            this.securityDeviceModel = copyData.securityDeviceModel;
            this.manufacturerModelName = copyData.manufacturerModelName;
            this.operatingSystemName = copyData.operatingSystemName;
            this.template = copyData.template;
            this.classConcept = copyData.classConcept;
            this.determinerConcept = copyData.determinerConcept;
            this.statusConcept = copyData.statusConcept;
            this.creationAct = copyData.creationAct;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.determinerConceptModel = copyData.determinerConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.creationActModel = copyData.creationActModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.telecom = copyData.telecom;
            this.extension = copyData.extension;
            this.name = copyData.name;
            this.address = copyData.address;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // DeviceEntity 
    // OpenIZ.Core.Model.Entities.Entity, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedEntityData
     * @summary             Represents the base of all entities            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} determinerConcept            Determiner concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} creationAct            Creation act reference            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.EntityIdentifier} identifier            Gets the identifiers associated with this entity            
     * @property {OpenIZModel.EntityRelationship} relationship            Gets a list of all associated entities for this entity            
     * @property {OpenIZModel.EntityTelecomAddress} telecom            Gets a list of all telecommunications addresses associated with the entity            
     * @property {OpenIZModel.EntityExtension} extension            Gets a list of all extensions associated with the entity            
     * @property {OpenIZModel.EntityName} name            Gets a list of all names associated with the entity            
     * @property {OpenIZModel.EntityAddress} address            Gets a list of all addresses associated with the entity            
     * @property {OpenIZModel.EntityNote} note            Gets a list of all notes associated with the entity            
     * @property {OpenIZModel.EntityTag} tag            Gets a list of all tags associated with the entity            
     * @property {OpenIZModel.ActParticipation} participation            Gets the acts in which this entity participates            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Entity} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.Entity} copyData Copy constructor (if present)
     */
    Entity: function (copyData) {
        if (copyData) {
            this.template = copyData.template;
            this.classConcept = copyData.classConcept;
            this.determinerConcept = copyData.determinerConcept;
            this.statusConcept = copyData.statusConcept;
            this.creationAct = copyData.creationAct;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.determinerConceptModel = copyData.determinerConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.creationActModel = copyData.creationActModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.telecom = copyData.telecom;
            this.extension = copyData.extension;
            this.name = copyData.name;
            this.address = copyData.address;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // Entity 
    // OpenIZ.Core.Model.Entities.EntityAddress, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Entity address            
     * @property {uuid} use            Gets or sets the address use key            
     * @property {OpenIZModel.Concept} useModel [Delay loaded from use],             Gets or sets the address use            
     * @property {OpenIZModel.AddressComponent} component            Gets or sets the component types            
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.EntityAddress} copyData Copy constructor (if present)
     */
    EntityAddress: function (copyData) {
        if (copyData) {
            this.use = copyData.use;
            this.useModel = copyData.useModel;
            this.component = copyData.component;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // EntityAddress 
    // OpenIZ.Core.Model.Entities.EntityAddressComponent, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.GenericComponentValues
     * @summary             A single address component            
     * @property {uuid} type
     * @property {OpenIZModel.Concept} typeModel [Delay loaded from type], 
     * @property {string} value
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.EntityAddress} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.AddressComponent} copyData Copy constructor (if present)
     */
    AddressComponent: function (copyData) {
        if (copyData) {
            this.type = copyData.type;
            this.typeModel = copyData.typeModel;
            this.value = copyData.value;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // AddressComponent 
    // OpenIZ.Core.Model.Entities.EntityName, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Represents a name for an entity            
     * @property {uuid} use            Gets or sets the name use key            
     * @property {OpenIZModel.Concept} useModel [Delay loaded from use],             Gets or sets the name use            
     * @property {OpenIZModel.EntityNameComponent} component            Gets or sets the component types            
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.EntityName} copyData Copy constructor (if present)
     */
    EntityName: function (copyData) {
        if (copyData) {
            this.use = copyData.use;
            this.useModel = copyData.useModel;
            this.component = copyData.component;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // EntityName 
    // OpenIZ.Core.Model.Entities.EntityNameComponent, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.GenericComponentValues
     * @summary             Represents a name component which is bound to a name            
     * @property {string} phoneticCode            Gets or sets the phonetic code of the reference term            
     * @property {uuid} phoneticAlgorithm            Gets or sets the identifier of the phonetic code            
     * @property {OpenIZModel.PhoneticAlgorithm} phoneticAlgorithmModel [Delay loaded from phoneticAlgorithm],             Gets or sets the phonetic algorithm            
     * @property {uuid} type
     * @property {OpenIZModel.Concept} typeModel [Delay loaded from type], 
     * @property {string} value
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.EntityName} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.EntityNameComponent} copyData Copy constructor (if present)
     */
    EntityNameComponent: function (copyData) {
        if (copyData) {
            this.phoneticCode = copyData.phoneticCode;
            this.phoneticAlgorithm = copyData.phoneticAlgorithm;
            this.phoneticAlgorithmModel = copyData.phoneticAlgorithmModel;
            this.type = copyData.type;
            this.typeModel = copyData.typeModel;
            this.value = copyData.value;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // EntityNameComponent 
    // OpenIZ.Core.Model.Entities.EntityRelationship, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Represents an association between two entities            
     * @property {uuid} target            The target of the association            
     * @property {uuid} holder            The entity that this relationship targets            
     * @property {OpenIZModel.Entity} holderModel [Delay loaded from holder],             The entity that this relationship targets            
     * @property {OpenIZModel.Entity} targetModel [Delay loaded from target],             Target entity reference            
     * @property {uuid} relationshipType            Association type key            
     * @property {bool} inversionInd            The inversion indicator            
     * @property {OpenIZModel.Concept} relationshipTypeModel [Delay loaded from relationshipType],             Gets or sets the association type            
     * @property {number} quantity            Represents the quantity of target in source            
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.EntityRelationship} copyData Copy constructor (if present)
     */
    EntityRelationship: function (copyData) {
        if (copyData) {
            this.target = copyData.target;
            this.holder = copyData.holder;
            this.holderModel = copyData.holderModel;
            this.targetModel = copyData.targetModel;
            this.relationshipType = copyData.relationshipType;
            this.inversionInd = copyData.inversionInd;
            this.relationshipTypeModel = copyData.relationshipTypeModel;
            this.quantity = copyData.quantity;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // EntityRelationship 
    // OpenIZ.Core.Model.Entities.EntityTelecomAddress, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Represents an entity telecom address            
     * @property {uuid} use            Gets or sets the name use key            
     * @property {OpenIZModel.Concept} useModel [Delay loaded from use],             Gets or sets the name use            
     * @property {string} value            Gets or sets the value of the telecom address            
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.EntityTelecomAddress} copyData Copy constructor (if present)
     */
    EntityTelecomAddress: function (copyData) {
        if (copyData) {
            this.use = copyData.use;
            this.useModel = copyData.useModel;
            this.value = copyData.value;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // EntityTelecomAddress 
    // OpenIZ.Core.Model.Entities.GenericComponentValues`1, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @abstract
     * @extends OpenIZModel.Association
     * @summary             A generic class representing components of a larger item (i.e. address, name, etc);            
     * @property {uuid} type            Component type key            
     * @property {OpenIZModel.Concept} typeModel [Delay loaded from type],             Gets or sets the type of address component            
     * @property {string} value            Gets or sets the value of the name component            
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.IdentifiedData} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.GenericComponentValues} copyData Copy constructor (if present)
     */
    GenericComponentValues: function (copyData) {
        if (copyData) {
            this.type = copyData.type;
            this.typeModel = copyData.typeModel;
            this.value = copyData.value;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // GenericComponentValues 
    // OpenIZ.Core.Model.Entities.ManufacturedMaterial, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Material
     * @summary             Manufactured material            
     * @property {string} lotNumber            Gets or sets the lot number of the manufactured material            
     * @property {number} quantity            The base quantity of the object in the units. This differs from quantity on the relationship            which is a /per ...             
     * @property {uuid} formConcept            Gets or sets the form concept's key            
     * @property {uuid} quantityConcept            Gets or sets the quantity concept ref            
     * @property {OpenIZModel.Concept} formConceptModel [Delay loaded from formConcept],             Gets or sets the concept which dictates the form of the material (solid, liquid, capsule, injection, etc.)            
     * @property {OpenIZModel.Concept} quantityConceptModel [Delay loaded from quantityConcept],             Gets or sets the concept which dictates the unit of measure for a single instance of this entity            
     * @property {date} expiryDate            Gets or sets the expiry date of the material            
     * @property {bool} isAdministrative            True if the material is simply administrative            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} determinerConcept            Determiner concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} creationAct            Creation act reference            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.EntityIdentifier} identifier            Gets the identifiers associated with this entity            
     * @property {OpenIZModel.EntityRelationship} relationship            Gets a list of all associated entities for this entity            
     * @property {OpenIZModel.EntityTelecomAddress} telecom            Gets a list of all telecommunications addresses associated with the entity            
     * @property {OpenIZModel.EntityExtension} extension            Gets a list of all extensions associated with the entity            
     * @property {OpenIZModel.EntityName} name            Gets a list of all names associated with the entity            
     * @property {OpenIZModel.EntityAddress} address            Gets a list of all addresses associated with the entity            
     * @property {OpenIZModel.EntityNote} note            Gets a list of all notes associated with the entity            
     * @property {OpenIZModel.EntityTag} tag            Gets a list of all tags associated with the entity            
     * @property {OpenIZModel.ActParticipation} participation            Gets the acts in which this entity participates            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Entity} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.ManufacturedMaterial} copyData Copy constructor (if present)
     */
    ManufacturedMaterial: function (copyData) {
        if (copyData) {
            this.lotNumber = copyData.lotNumber;
            this.quantity = copyData.quantity;
            this.formConcept = copyData.formConcept;
            this.quantityConcept = copyData.quantityConcept;
            this.formConceptModel = copyData.formConceptModel;
            this.quantityConceptModel = copyData.quantityConceptModel;
            this.expiryDate = copyData.expiryDate;
            this.isAdministrative = copyData.isAdministrative;
            this.template = copyData.template;
            this.classConcept = copyData.classConcept;
            this.determinerConcept = copyData.determinerConcept;
            this.statusConcept = copyData.statusConcept;
            this.creationAct = copyData.creationAct;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.determinerConceptModel = copyData.determinerConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.creationActModel = copyData.creationActModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.telecom = copyData.telecom;
            this.extension = copyData.extension;
            this.name = copyData.name;
            this.address = copyData.address;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // ManufacturedMaterial 
    // OpenIZ.Core.Model.Entities.Material, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Entity
     * @summary             Represents a material             
     * @property {number} quantity            The base quantity of the object in the units. This differs from quantity on the relationship            which is a /per ...             
     * @property {uuid} formConcept            Gets or sets the form concept's key            
     * @property {uuid} quantityConcept            Gets or sets the quantity concept ref            
     * @property {OpenIZModel.Concept} formConceptModel [Delay loaded from formConcept],             Gets or sets the concept which dictates the form of the material (solid, liquid, capsule, injection, etc.)            
     * @property {OpenIZModel.Concept} quantityConceptModel [Delay loaded from quantityConcept],             Gets or sets the concept which dictates the unit of measure for a single instance of this entity            
     * @property {date} expiryDate            Gets or sets the expiry date of the material            
     * @property {bool} isAdministrative            True if the material is simply administrative            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} determinerConcept            Determiner concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} creationAct            Creation act reference            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.EntityIdentifier} identifier            Gets the identifiers associated with this entity            
     * @property {OpenIZModel.EntityRelationship} relationship            Gets a list of all associated entities for this entity            
     * @property {OpenIZModel.EntityTelecomAddress} telecom            Gets a list of all telecommunications addresses associated with the entity            
     * @property {OpenIZModel.EntityExtension} extension            Gets a list of all extensions associated with the entity            
     * @property {OpenIZModel.EntityName} name            Gets a list of all names associated with the entity            
     * @property {OpenIZModel.EntityAddress} address            Gets a list of all addresses associated with the entity            
     * @property {OpenIZModel.EntityNote} note            Gets a list of all notes associated with the entity            
     * @property {OpenIZModel.EntityTag} tag            Gets a list of all tags associated with the entity            
     * @property {OpenIZModel.ActParticipation} participation            Gets the acts in which this entity participates            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Entity} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.Material} copyData Copy constructor (if present)
     */
    Material: function (copyData) {
        if (copyData) {
            this.quantity = copyData.quantity;
            this.formConcept = copyData.formConcept;
            this.quantityConcept = copyData.quantityConcept;
            this.formConceptModel = copyData.formConceptModel;
            this.quantityConceptModel = copyData.quantityConceptModel;
            this.expiryDate = copyData.expiryDate;
            this.isAdministrative = copyData.isAdministrative;
            this.template = copyData.template;
            this.classConcept = copyData.classConcept;
            this.determinerConcept = copyData.determinerConcept;
            this.statusConcept = copyData.statusConcept;
            this.creationAct = copyData.creationAct;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.determinerConceptModel = copyData.determinerConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.creationActModel = copyData.creationActModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.telecom = copyData.telecom;
            this.extension = copyData.extension;
            this.name = copyData.name;
            this.address = copyData.address;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // Material 
    // OpenIZ.Core.Model.Entities.Organization, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Entity
     * @summary             Organization entity            
     * @property {uuid} industryConcept            Gets or sets the industry concept key            
     * @property {OpenIZModel.Concept} industryConceptModel [Delay loaded from industryConcept],             Gets or sets the industry in which the organization operates            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} determinerConcept            Determiner concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} creationAct            Creation act reference            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.EntityIdentifier} identifier            Gets the identifiers associated with this entity            
     * @property {OpenIZModel.EntityRelationship} relationship            Gets a list of all associated entities for this entity            
     * @property {OpenIZModel.EntityTelecomAddress} telecom            Gets a list of all telecommunications addresses associated with the entity            
     * @property {OpenIZModel.EntityExtension} extension            Gets a list of all extensions associated with the entity            
     * @property {OpenIZModel.EntityName} name            Gets a list of all names associated with the entity            
     * @property {OpenIZModel.EntityAddress} address            Gets a list of all addresses associated with the entity            
     * @property {OpenIZModel.EntityNote} note            Gets a list of all notes associated with the entity            
     * @property {OpenIZModel.EntityTag} tag            Gets a list of all tags associated with the entity            
     * @property {OpenIZModel.ActParticipation} participation            Gets the acts in which this entity participates            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Entity} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.Organization} copyData Copy constructor (if present)
     */
    Organization: function (copyData) {
        if (copyData) {
            this.industryConcept = copyData.industryConcept;
            this.industryConceptModel = copyData.industryConceptModel;
            this.template = copyData.template;
            this.classConcept = copyData.classConcept;
            this.determinerConcept = copyData.determinerConcept;
            this.statusConcept = copyData.statusConcept;
            this.creationAct = copyData.creationAct;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.determinerConceptModel = copyData.determinerConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.creationActModel = copyData.creationActModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.telecom = copyData.telecom;
            this.extension = copyData.extension;
            this.name = copyData.name;
            this.address = copyData.address;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // Organization 
    // OpenIZ.Core.Model.Entities.Person, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Entity
     * @summary             Represents an entity which is a person            
     * @property {date} dateOfBirth            Gets or sets the person's date of birth            
     * @property {DatePrecision} dateOfBirthPrecision            Gets or sets the precision ofthe date of birth            
     * @property {OpenIZModel.PersonLanguageCommunication} language            Gets the person's languages of communication            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} determinerConcept            Determiner concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} creationAct            Creation act reference            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.EntityIdentifier} identifier            Gets the identifiers associated with this entity            
     * @property {OpenIZModel.EntityRelationship} relationship            Gets a list of all associated entities for this entity            
     * @property {OpenIZModel.EntityTelecomAddress} telecom            Gets a list of all telecommunications addresses associated with the entity            
     * @property {OpenIZModel.EntityExtension} extension            Gets a list of all extensions associated with the entity            
     * @property {OpenIZModel.EntityName} name            Gets a list of all names associated with the entity            
     * @property {OpenIZModel.EntityAddress} address            Gets a list of all addresses associated with the entity            
     * @property {OpenIZModel.EntityNote} note            Gets a list of all notes associated with the entity            
     * @property {OpenIZModel.EntityTag} tag            Gets a list of all tags associated with the entity            
     * @property {OpenIZModel.ActParticipation} participation            Gets the acts in which this entity participates            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Entity} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.Person} copyData Copy constructor (if present)
     */
    Person: function (copyData) {
        if (copyData) {
            this.dateOfBirth = copyData.dateOfBirth;
            this.dateOfBirthPrecision = copyData.dateOfBirthPrecision;
            this.language = copyData.language;
            this.template = copyData.template;
            this.classConcept = copyData.classConcept;
            this.determinerConcept = copyData.determinerConcept;
            this.statusConcept = copyData.statusConcept;
            this.creationAct = copyData.creationAct;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.determinerConceptModel = copyData.determinerConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.creationActModel = copyData.creationActModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.telecom = copyData.telecom;
            this.extension = copyData.extension;
            this.name = copyData.name;
            this.address = copyData.address;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // Person 
    // OpenIZ.Core.Model.Entities.PersonLanguageCommunication, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Represents a single preferred communication method for the entity            
     * @property {string} languageCode            Gets or sets the language code            
     * @property {bool} isPreferred            Gets or set the user's preference indicator            
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.PersonLanguageCommunication} copyData Copy constructor (if present)
     */
    PersonLanguageCommunication: function (copyData) {
        if (copyData) {
            this.languageCode = copyData.languageCode;
            this.isPreferred = copyData.isPreferred;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // PersonLanguageCommunication 
    // OpenIZ.Core.Model.Entities.Place, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Entity
     * @summary             An entity which is a place where healthcare services are delivered            
     * @property {uuid} classConcept            Gets or sets the class concept key            
     * @property {bool} isMobile            True if location is mobile            
     * @property {Double} lat            Gets or sets the latitude            
     * @property {Double} lng            Gets or sets the longitude            
     * @property {OpenIZModel.PlaceService} service            Gets the services            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {uuid} determinerConcept            Determiner concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} creationAct            Creation act reference            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.EntityIdentifier} identifier            Gets the identifiers associated with this entity            
     * @property {OpenIZModel.EntityRelationship} relationship            Gets a list of all associated entities for this entity            
     * @property {OpenIZModel.EntityTelecomAddress} telecom            Gets a list of all telecommunications addresses associated with the entity            
     * @property {OpenIZModel.EntityExtension} extension            Gets a list of all extensions associated with the entity            
     * @property {OpenIZModel.EntityName} name            Gets a list of all names associated with the entity            
     * @property {OpenIZModel.EntityAddress} address            Gets a list of all addresses associated with the entity            
     * @property {OpenIZModel.EntityNote} note            Gets a list of all notes associated with the entity            
     * @property {OpenIZModel.EntityTag} tag            Gets a list of all tags associated with the entity            
     * @property {OpenIZModel.ActParticipation} participation            Gets the acts in which this entity participates            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Entity} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.Place} copyData Copy constructor (if present)
     */
    Place: function (copyData) {
        if (copyData) {
            this.classConcept = copyData.classConcept;
            this.isMobile = copyData.isMobile;
            this.lat = copyData.lat;
            this.lng = copyData.lng;
            this.service = copyData.service;
            this.template = copyData.template;
            this.determinerConcept = copyData.determinerConcept;
            this.statusConcept = copyData.statusConcept;
            this.creationAct = copyData.creationAct;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.determinerConceptModel = copyData.determinerConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.creationActModel = copyData.creationActModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.telecom = copyData.telecom;
            this.extension = copyData.extension;
            this.name = copyData.name;
            this.address = copyData.address;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // Place 
    // OpenIZ.Core.Model.Entities.PlaceService, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Represents a service for a place            
     * @property {Object} serviceSchedule            The schedule that the service is offered            
     * @property {uuid} serviceConcept            Gets or sets the service concept key            
     * @property {OpenIZModel.Concept} serviceConceptModel [Delay loaded from serviceConcept],             Gets or sets the service concept            
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.PlaceService} copyData Copy constructor (if present)
     */
    PlaceService: function (copyData) {
        if (copyData) {
            this.serviceSchedule = copyData.serviceSchedule;
            this.serviceConcept = copyData.serviceConcept;
            this.serviceConceptModel = copyData.serviceConceptModel;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // PlaceService 
    // OpenIZ.Core.Model.DataTypes.AssigningAuthority, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.BaseEntityData
     * @summary             Represents a model class which is an assigning authority            
     * @property {string} name            Gets or sets the name of the assigning authority            
     * @property {string} domainName            Gets or sets the domain name of the assigning authority            
     * @property {string} description            Gets or sets the description of the assigning authority            
     * @property {string} oid            Gets or sets the oid of the assigning authority            
     * @property {string} url            The URL of the assigning authority            
     * @property {uuid} scope            Represents scopes to which the authority is bound            
     * @property {uuid} assigningDevice            Assigning device identifier            
     * @property {OpenIZModel.Concept} scopeModel [Delay loaded from scope],             Gets concept sets to which this concept is a member            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.AssigningAuthority} copyData Copy constructor (if present)
     */
    AssigningAuthority: function (copyData) {
        if (copyData) {
            this.name = copyData.name;
            this.domainName = copyData.domainName;
            this.description = copyData.description;
            this.oid = copyData.oid;
            this.url = copyData.url;
            this.scope = copyData.scope;
            this.assigningDevice = copyData.assigningDevice;
            this.scopeModel = copyData.scopeModel;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // AssigningAuthority 
    // OpenIZ.Core.Model.DataTypes.CodeSystem, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.NonVersionedEntityData
     * @summary             Represents a code system which is a collection of reference terms            
     * @property {string} name            Gets or sets the name of the code system            
     * @property {string} oid            Gets or sets the Oid of the code system            
     * @property {string} authority            Gets or sets the authority of the code system            
     * @property {string} obsoletionReason            Gets or sets the obsoletion reason of the code system            
     * @property {string} url            Gets or sets the URL of the code system            
     * @property {string} version            Gets or sets the version text of the code system            
     * @property {string} description            Gets or sets the human description            
     * @property {string} updatedTime            Gets or sets the creation time in XML format            
     * @property {date} modifiedOn            Gets the time this item was modified            
     * @property {uuid} updatedBy            Gets or sets the created by identifier            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.CodeSystem} copyData Copy constructor (if present)
     */
    CodeSystem: function (copyData) {
        if (copyData) {
            this.name = copyData.name;
            this.oid = copyData.oid;
            this.authority = copyData.authority;
            this.obsoletionReason = copyData.obsoletionReason;
            this.url = copyData.url;
            this.version = copyData.version;
            this.description = copyData.description;
            this.updatedTime = copyData.updatedTime;
            this.modifiedOn = copyData.modifiedOn;
            this.updatedBy = copyData.updatedBy;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // CodeSystem 
    // OpenIZ.Core.Model.DataTypes.Concept, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedEntityData
     * @summary             A class representing a generic concept used in the OpenIZ datamodel            
     * @property {bool} isReadonly            Gets or sets an indicator which dictates whether the concept is a system concept            
     * @property {string} mnemonic            Gets or sets the unchanging mnemonic for the concept            
     * @property {uuid} statusConcept            Gets or sets the status concept key            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Gets or sets the status of the concept            
     * @property {OpenIZModel.ConceptRelationship} relationship            Gets a list of concept relationships            
     * @property {uuid} conceptClass            Gets or sets the class identifier            
     * @property {OpenIZModel.ConceptClass} conceptClassModel [Delay loaded from conceptClass],             Gets or sets the classification of the concept            
     * @property {OpenIZModel.ConceptReferenceTerm} referenceTerm            Gets a list of concept reference terms            
     * @property {OpenIZModel.ConceptName} name            Gets the concept names            
     * @property {uuid} conceptSet            Concept sets as identifiers for XML purposes only            
     * @property {OpenIZModel.ConceptSet} conceptSetModel [Delay loaded from conceptSet],             Gets concept sets to which this concept is a member            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Concept} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.Concept} copyData Copy constructor (if present)
     */
    Concept: function (copyData) {
        if (copyData) {
            this.isReadonly = copyData.isReadonly;
            this.mnemonic = copyData.mnemonic;
            this.statusConcept = copyData.statusConcept;
            this.statusConceptModel = copyData.statusConceptModel;
            this.relationship = copyData.relationship;
            this.conceptClass = copyData.conceptClass;
            this.conceptClassModel = copyData.conceptClassModel;
            this.referenceTerm = copyData.referenceTerm;
            this.name = copyData.name;
            this.conceptSet = copyData.conceptSet;
            this.conceptSetModel = copyData.conceptSetModel;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // Concept 
    // OpenIZ.Core.Model.DataTypes.ConceptClass, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.NonVersionedEntityData
     * @summary             Identifies a classification for a concept            
     * @property {string} name            Gets or sets the name of the concept class            
     * @property {string} mnemonic            Gets or sets the mnemonic            
     * @property {string} updatedTime            Gets or sets the creation time in XML format            
     * @property {date} modifiedOn            Gets the time this item was modified            
     * @property {uuid} updatedBy            Gets or sets the created by identifier            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ConceptClass} copyData Copy constructor (if present)
     */
    ConceptClass: function (copyData) {
        if (copyData) {
            this.name = copyData.name;
            this.mnemonic = copyData.mnemonic;
            this.updatedTime = copyData.updatedTime;
            this.modifiedOn = copyData.modifiedOn;
            this.updatedBy = copyData.updatedBy;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ConceptClass 
    // OpenIZ.Core.Model.DataTypes.ConceptName, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Represents a name (human name) that a concept may have            
     * @property {string} language            Gets or sets the language code of the object            
     * @property {string} value            Gets or sets the name of the reference term            
     * @property {string} phoneticCode            Gets or sets the phonetic code of the reference term            
     * @property {uuid} phoneticAlgorithm            Gets or sets the identifier of the phonetic code            
     * @property {OpenIZModel.PhoneticAlgorithm} phoneticAlgorithmModel [Delay loaded from phoneticAlgorithm],             Gets or sets the phonetic algorithm            
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Concept} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ConceptName} copyData Copy constructor (if present)
     */
    ConceptName: function (copyData) {
        if (copyData) {
            this.language = copyData.language;
            this.value = copyData.value;
            this.phoneticCode = copyData.phoneticCode;
            this.phoneticAlgorithm = copyData.phoneticAlgorithm;
            this.phoneticAlgorithmModel = copyData.phoneticAlgorithmModel;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ConceptName 
    // OpenIZ.Core.Model.DataTypes.ConceptReferenceTerm, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Represents a reference term relationship between a concept and reference term            
     * @property {uuid} term            Gets or sets the reference term identifier            
     * @property {OpenIZModel.ReferenceTerm} termModel [Delay loaded from term],             Gets or set the reference term            
     * @property {uuid} relationshipType            Gets or sets the relationship type identifier            
     * @property {OpenIZModel.ConceptRelationshipType} relationshipTypeModel [Delay loaded from relationshipType],             Gets or sets the relationship type            
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Concept} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ConceptReferenceTerm} copyData Copy constructor (if present)
     */
    ConceptReferenceTerm: function (copyData) {
        if (copyData) {
            this.term = copyData.term;
            this.termModel = copyData.termModel;
            this.relationshipType = copyData.relationshipType;
            this.relationshipTypeModel = copyData.relationshipTypeModel;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ConceptReferenceTerm 
    // OpenIZ.Core.Model.DataTypes.ConceptRelationship, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Represents a relationship between two concepts            
     * @property {uuid} targetConcept            Gets or sets the target concept identifier            
     * @property {OpenIZModel.Concept} targetConceptModel [Delay loaded from targetConcept],             Gets or sets the target concept            
     * @property {uuid} relationshipType            Relationship type            
     * @property {OpenIZModel.ConceptRelationshipType} relationshipTypeModel [Delay loaded from relationshipType],             Gets or sets the relationship type            
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Concept} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ConceptRelationship} copyData Copy constructor (if present)
     */
    ConceptRelationship: function (copyData) {
        if (copyData) {
            this.targetConcept = copyData.targetConcept;
            this.targetConceptModel = copyData.targetConceptModel;
            this.relationshipType = copyData.relationshipType;
            this.relationshipTypeModel = copyData.relationshipTypeModel;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ConceptRelationship 
    // OpenIZ.Core.Model.DataTypes.ConceptRelationshipType, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.NonVersionedEntityData
     * @summary             Concept relationship type            
     * @property {string} name            Gets or sets the name of the relationship            
     * @property {string} mnemonic            The invariant of the relationship type            
     * @property {string} updatedTime            Gets or sets the creation time in XML format            
     * @property {date} modifiedOn            Gets the time this item was modified            
     * @property {uuid} updatedBy            Gets or sets the created by identifier            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ConceptRelationshipType} copyData Copy constructor (if present)
     */
    ConceptRelationshipType: function (copyData) {
        if (copyData) {
            this.name = copyData.name;
            this.mnemonic = copyData.mnemonic;
            this.updatedTime = copyData.updatedTime;
            this.modifiedOn = copyData.modifiedOn;
            this.updatedBy = copyData.updatedBy;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ConceptRelationshipType 
    // OpenIZ.Core.Model.DataTypes.ConceptSet, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.BaseEntityData
     * @summary             Represents set of concepts            
     * @property {string} name            Gets or sets the name of the concept set            
     * @property {string} mnemonic            Gets or sets the mnemonic for the concept set (used for convenient lookup)            
     * @property {string} oid            Gets or sets the oid of the concept set            
     * @property {string} url            Gets or sets the url of the concept set            
     * @property {uuid} concept            Concepts as identifiers for XML purposes only            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ConceptSet} copyData Copy constructor (if present)
     */
    ConceptSet: function (copyData) {
        if (copyData) {
            this.name = copyData.name;
            this.mnemonic = copyData.mnemonic;
            this.oid = copyData.oid;
            this.url = copyData.url;
            this.concept = copyData.concept;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ConceptSet 
    // OpenIZ.Core.Model.DataTypes.Extension`1, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @abstract
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Represents a base entity extension            
     * @property {bytea} value            Gets or sets the value of the extension            
     * @property {OpenIZModel.ExtensionType} extensionType            Gets or sets the extension type            
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.VersionedEntityData} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.Extension} copyData Copy constructor (if present)
     */
    Extension: function (copyData) {
        if (copyData) {
            this.value = copyData.value;
            this.extensionType = copyData.extensionType;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // Extension 
    // OpenIZ.Core.Model.DataTypes.EntityExtension, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Extension
     * @summary             Extension bound to entity            
     * @property {bytea} value
     * @property {OpenIZModel.ExtensionType} extensionType
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.EntityExtension} copyData Copy constructor (if present)
     */
    EntityExtension: function (copyData) {
        if (copyData) {
            this.value = copyData.value;
            this.extensionType = copyData.extensionType;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // EntityExtension 
    // OpenIZ.Core.Model.DataTypes.ActExtension, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Extension
     * @summary             Act extension            
     * @property {bytea} value
     * @property {OpenIZModel.ExtensionType} extensionType
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Act} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ActExtension} copyData Copy constructor (if present)
     */
    ActExtension: function (copyData) {
        if (copyData) {
            this.value = copyData.value;
            this.extensionType = copyData.extensionType;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ActExtension 
    // OpenIZ.Core.Model.DataTypes.ExtensionType, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.NonVersionedEntityData
     * @summary             Instructions on how an extensionshould be handled            
     * @property {string} handlerClass            Gets or sets the description            
     * @property {string} name            Gets or sets the description            
     * @property {string} updatedTime            Gets or sets the creation time in XML format            
     * @property {date} modifiedOn            Gets the time this item was modified            
     * @property {uuid} updatedBy            Gets or sets the created by identifier            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ExtensionType} copyData Copy constructor (if present)
     */
    ExtensionType: function (copyData) {
        if (copyData) {
            this.handlerClass = copyData.handlerClass;
            this.name = copyData.name;
            this.updatedTime = copyData.updatedTime;
            this.modifiedOn = copyData.modifiedOn;
            this.updatedBy = copyData.updatedBy;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ExtensionType 
    // OpenIZ.Core.Model.DataTypes.EntityIdentifier, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.IdentifierBase
     * @summary             Entity identifiers            
     * @property {string} value
     * @property {OpenIZModel.IdentifierType} type
     * @property {OpenIZModel.AssigningAuthority} authority
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.EntityIdentifier} copyData Copy constructor (if present)
     */
    EntityIdentifier: function (copyData) {
        if (copyData) {
            this.value = copyData.value;
            this.type = copyData.type;
            this.authority = copyData.authority;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // EntityIdentifier 
    // OpenIZ.Core.Model.DataTypes.ActIdentifier, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.IdentifierBase
     * @summary             Act identifier            
     * @property {string} value
     * @property {OpenIZModel.IdentifierType} type
     * @property {OpenIZModel.AssigningAuthority} authority
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Act} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ActIdentifier} copyData Copy constructor (if present)
     */
    ActIdentifier: function (copyData) {
        if (copyData) {
            this.value = copyData.value;
            this.type = copyData.type;
            this.authority = copyData.authority;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ActIdentifier 
    // OpenIZ.Core.Model.DataTypes.IdentifierBase`1, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @abstract
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Represents an external assigned identifier            
     * @property {string} value            Gets or sets the value of the identifier            
     * @property {OpenIZModel.IdentifierType} type            Gets or sets the identifier type            
     * @property {OpenIZModel.AssigningAuthority} authority            Gets or sets the assigning authority             
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.VersionedEntityData} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.IdentifierBase} copyData Copy constructor (if present)
     */
    IdentifierBase: function (copyData) {
        if (copyData) {
            this.value = copyData.value;
            this.type = copyData.type;
            this.authority = copyData.authority;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // IdentifierBase 
    // OpenIZ.Core.Model.DataTypes.IdentifierType, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.BaseEntityData
     * @summary             Represents a basic information class which classifies the use of an identifier            
     * @property {uuid} scopeConcept            Gets or sets the id of the scope concept            
     * @property {uuid} typeConcept            Gets or sets the concept which identifies the type            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept            
     * @property {OpenIZModel.Concept} scopeConceptModel [Delay loaded from scopeConcept],             Gets the scope of the identifier            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.IdentifierType} copyData Copy constructor (if present)
     */
    IdentifierType: function (copyData) {
        if (copyData) {
            this.scopeConcept = copyData.scopeConcept;
            this.typeConcept = copyData.typeConcept;
            this.typeConceptModel = copyData.typeConceptModel;
            this.scopeConceptModel = copyData.scopeConceptModel;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // IdentifierType 
    // OpenIZ.Core.Model.DataTypes.Note`1, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @abstract
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Generic note class            
     * @property {string} text            Gets or sets the note text            
     * @property {uuid} author            Gets or sets the author key            
     * @property {OpenIZModel.Entity} authorModel [Delay loaded from author],             Gets or sets the author entity            
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.VersionedEntityData} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.Note} copyData Copy constructor (if present)
     */
    Note: function (copyData) {
        if (copyData) {
            this.text = copyData.text;
            this.author = copyData.author;
            this.authorModel = copyData.authorModel;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // Note 
    // OpenIZ.Core.Model.DataTypes.EntityNote, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Note
     * @summary             Represents a note attached to an entity            
     * @property {string} text
     * @property {uuid} author
     * @property {OpenIZModel.Entity} authorModel [Delay loaded from author], 
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.EntityNote} copyData Copy constructor (if present)
     */
    EntityNote: function (copyData) {
        if (copyData) {
            this.text = copyData.text;
            this.author = copyData.author;
            this.authorModel = copyData.authorModel;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // EntityNote 
    // OpenIZ.Core.Model.DataTypes.ActNote, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Note
     * @summary             Represents a note attached to an entity            
     * @property {string} text
     * @property {uuid} author
     * @property {OpenIZModel.Entity} authorModel [Delay loaded from author], 
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Act} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ActNote} copyData Copy constructor (if present)
     */
    ActNote: function (copyData) {
        if (copyData) {
            this.text = copyData.text;
            this.author = copyData.author;
            this.authorModel = copyData.authorModel;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ActNote 
    // OpenIZ.Core.Model.DataTypes.PhoneticAlgorithm, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.NonVersionedEntityData
     * @summary             Represents a phonetic algorithm record in the model            
     * @property {string} name            Gets the name of the phonetic algorithm            
     * @property {string} handler            Gets the handler (or generator) for the phonetic algorithm            
     * @property {string} updatedTime            Gets or sets the creation time in XML format            
     * @property {date} modifiedOn            Gets the time this item was modified            
     * @property {uuid} updatedBy            Gets or sets the created by identifier            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.PhoneticAlgorithm} copyData Copy constructor (if present)
     */
    PhoneticAlgorithm: function (copyData) {
        if (copyData) {
            this.name = copyData.name;
            this.handler = copyData.handler;
            this.updatedTime = copyData.updatedTime;
            this.modifiedOn = copyData.modifiedOn;
            this.updatedBy = copyData.updatedBy;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // PhoneticAlgorithm 
    // OpenIZ.Core.Model.DataTypes.ReferenceTerm, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.NonVersionedEntityData
     * @summary             Represents a basic reference term            
     * @property {string} mnemonic            Gets or sets the mnemonic for the reference term            
     * @property {OpenIZModel.CodeSystem} codeSystemModel [Delay loaded from codeSystem],             Gets or sets the code system             
     * @property {uuid} codeSystem            Gets or sets the code system identifier            
     * @property {OpenIZModel.ReferenceTermName} name            Gets display names associated with the reference term            
     * @property {string} updatedTime            Gets or sets the creation time in XML format            
     * @property {date} modifiedOn            Gets the time this item was modified            
     * @property {uuid} updatedBy            Gets or sets the created by identifier            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ReferenceTerm} copyData Copy constructor (if present)
     */
    ReferenceTerm: function (copyData) {
        if (copyData) {
            this.mnemonic = copyData.mnemonic;
            this.codeSystemModel = copyData.codeSystemModel;
            this.codeSystem = copyData.codeSystem;
            this.name = copyData.name;
            this.updatedTime = copyData.updatedTime;
            this.modifiedOn = copyData.modifiedOn;
            this.updatedBy = copyData.updatedBy;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ReferenceTerm 
    // OpenIZ.Core.Model.DataTypes.ReferenceTermName, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.BaseEntityData
     * @summary             Display name of a code system or reference term            
     * @property {string} language            Gets or sets the language code of the object            
     * @property {string} value            Gets or sets the name of the reference term            
     * @property {string} phoneticCode            Gets or sets the phonetic code of the reference term            
     * @property {uuid} phoneticAlgorithm            Gets or sets the identifier of the phonetic code            
     * @property {OpenIZModel.PhoneticAlgorithm} phoneticAlgorithmModel [Delay loaded from phoneticAlgorithm],             Gets or sets the phonetic algorithm            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ReferenceTermName} copyData Copy constructor (if present)
     */
    ReferenceTermName: function (copyData) {
        if (copyData) {
            this.language = copyData.language;
            this.value = copyData.value;
            this.phoneticCode = copyData.phoneticCode;
            this.phoneticAlgorithm = copyData.phoneticAlgorithm;
            this.phoneticAlgorithmModel = copyData.phoneticAlgorithmModel;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ReferenceTermName 
    // OpenIZ.Core.Model.DataTypes.Tag`1, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @abstract
     * @extends OpenIZModel.BaseEntityData
     * @summary             Represents the base class for tags            
     * @property {string} key            Gets or sets the key of the tag            
     * @property {string} value            Gets or sets the value of the tag            
     * @property {uuid} source            Gets or sets the source entity's key (where the relationship is FROM)            
     * @property {OpenIZModel.IdentifiedData} sourceModel [Delay loaded from source],             The entity that this relationship targets            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.Tag} copyData Copy constructor (if present)
     */
    Tag: function (copyData) {
        if (copyData) {
            this.key = copyData.key;
            this.value = copyData.value;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // Tag 
    // OpenIZ.Core.Model.DataTypes.EntityTag, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Tag
     * @summary             Represents a tag associated with an entity            
     * @property {string} key
     * @property {string} value
     * @property {uuid} source
     * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.EntityTag} copyData Copy constructor (if present)
     */
    EntityTag: function (copyData) {
        if (copyData) {
            this.key = copyData.key;
            this.value = copyData.value;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // EntityTag 
    // OpenIZ.Core.Model.DataTypes.ActTag, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Tag
     * @summary             Represents a tag on an act            
     * @property {string} key
     * @property {string} value
     * @property {uuid} source
     * @property {OpenIZModel.Act} sourceModel [Delay loaded from source], 
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ActTag} copyData Copy constructor (if present)
     */
    ActTag: function (copyData) {
        if (copyData) {
            this.key = copyData.key;
            this.value = copyData.value;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ActTag 
    // OpenIZ.Core.Model.DataTypes.TemplateDefinition, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.NonVersionedEntityData
     * @summary             Represents a template definition            
     * @property {string} mnemonic            Gets or sets the mnemonic            
     * @property {string} name            Gets or set the name             
     * @property {string} oid            Gets or sets the oid of the concept set            
     * @property {string} description            Gets or sets the description            
     * @property {string} updatedTime            Gets or sets the creation time in XML format            
     * @property {date} modifiedOn            Gets the time this item was modified            
     * @property {uuid} updatedBy            Gets or sets the created by identifier            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.TemplateDefinition} copyData Copy constructor (if present)
     */
    TemplateDefinition: function (copyData) {
        if (copyData) {
            this.mnemonic = copyData.mnemonic;
            this.name = copyData.name;
            this.oid = copyData.oid;
            this.description = copyData.description;
            this.updatedTime = copyData.updatedTime;
            this.modifiedOn = copyData.modifiedOn;
            this.updatedBy = copyData.updatedBy;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // TemplateDefinition 
    // OpenIZ.Core.Model.Collection.Bundle, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.IdentifiedData
     * @summary             Represents a collection of model items             
     * @property {date} modifiedOn            Gets the time the bundle was modified            
     * @property {OpenIZModel.IdentifiedData} item            Gets or sets items in the bundle            
     * @property {uuid} entry            Entry into the bundle            
     * @property {number} offset            Gets or sets the count in this bundle            
     * @property {number} count            Gets or sets the count in this bundle            
     * @property {number} totalResults            Gets or sets the total results            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.Bundle} copyData Copy constructor (if present)
     */
    Bundle: function (copyData) {
        if (copyData) {
            this.modifiedOn = copyData.modifiedOn;
            this.item = copyData.item;
            this.entry = copyData.entry;
            this.offset = copyData.offset;
            this.count = copyData.count;
            this.totalResults = copyData.totalResults;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // Bundle 
    // OpenIZ.Core.Model.Acts.Act, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedEntityData
     * @summary             Represents the base class for an act            
     * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {string} actTime            Gets or sets the creation time in XML format            
     * @property {string} startTime            Gets or sets the creation time in XML format            
     * @property {string} stopTime            Gets or sets the creation time in XML format            
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} moodConcept            Mood concept            
     * @property {uuid} reasonConcept            Reason concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.ActIdentifier} identifier            Gets the identifiers associated with this act            
     * @property {OpenIZModel.ActRelationship} relationship            Gets a list of all associated acts for this act            
     * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
     * @property {OpenIZModel.ActExtension} extension            Gets a list of all extensions associated with the act            
     * @property {OpenIZModel.ActNote} note            Gets a list of all notes associated with the act            
     * @property {OpenIZModel.ActTag} tag            Gets a list of all tags associated with the act            
     * @property {OpenIZModel.ActParticipation} participation            Participations            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Act} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.Act} copyData Copy constructor (if present)
     */
    Act: function (copyData) {
        if (copyData) {
            this.isNegated = copyData.isNegated;
            this.template = copyData.template;
            this.actTime = copyData.actTime;
            this.startTime = copyData.startTime;
            this.stopTime = copyData.stopTime;
            this.classConcept = copyData.classConcept;
            this.moodConcept = copyData.moodConcept;
            this.reasonConcept = copyData.reasonConcept;
            this.statusConcept = copyData.statusConcept;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.moodConceptModel = copyData.moodConceptModel;
            this.reasonConceptModel = copyData.reasonConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.policy = copyData.policy;
            this.extension = copyData.extension;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // Act 
    // OpenIZ.Core.Model.Acts.ActParticipation, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Associates an entity which participates in an act            
     * @property {uuid} player            Gets or sets the target entity reference            
     * @property {uuid} participationRole            Gets or sets the participation role key            
     * @property {OpenIZModel.Entity} playerModel [Delay loaded from player],             Gets or sets the entity which participated in the act            
     * @property {OpenIZModel.Concept} participationRoleModel [Delay loaded from participationRole],             Gets or sets the role that the entity played in participating in the act            
     * @property {uuid} act            The entity that this relationship targets            
     * @property {OpenIZModel.Act} actModel [Delay loaded from act],             The entity that this relationship targets            
     * @property {number} quantity            Gets or sets the quantity of player in the act            
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Act} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ActParticipation} copyData Copy constructor (if present)
     */
    ActParticipation: function (copyData) {
        if (copyData) {
            this.player = copyData.player;
            this.participationRole = copyData.participationRole;
            this.playerModel = copyData.playerModel;
            this.participationRoleModel = copyData.participationRoleModel;
            this.act = copyData.act;
            this.actModel = copyData.actModel;
            this.quantity = copyData.quantity;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ActParticipation 
    // OpenIZ.Core.Model.Acts.ActProtocol, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Represents information related to the clinical protocol to which an act is a member of            
     * @property {uuid} protocol            Gets or sets the protocol  to which this act belongs            
     * @property {OpenIZModel.Protocol} protocolModel [Delay loaded from protocol],             Gets or sets the protocol data related to the protocol            
     * @property {string} state            Represents any state data related to the act / protocol link            
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Act} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ActProtocol} copyData Copy constructor (if present)
     */
    ActProtocol: function (copyData) {
        if (copyData) {
            this.protocol = copyData.protocol;
            this.protocolModel = copyData.protocolModel;
            this.state = copyData.state;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ActProtocol 
    // OpenIZ.Core.Model.Acts.ActRelationship, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.VersionedAssociation
     * @summary             Act relationships            
     * @property {uuid} target            The target of the association            
     * @property {OpenIZModel.Act} targetModel [Delay loaded from target],             Target act reference            
     * @property {uuid} relationshipType            Association type key            
     * @property {OpenIZModel.Concept} relationshipTypeModel [Delay loaded from relationshipType],             Gets or sets the association type            
     * @property {number} effectiveVersionSequence
     * @property {number} obsoleteVersionSequence
     * @property {date} modifiedOn
     * @property {uuid} source
     * @property {OpenIZModel.Act} sourceModel [Delay loaded from source], 
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.ActRelationship} copyData Copy constructor (if present)
     */
    ActRelationship: function (copyData) {
        if (copyData) {
            this.target = copyData.target;
            this.targetModel = copyData.targetModel;
            this.relationshipType = copyData.relationshipType;
            this.relationshipTypeModel = copyData.relationshipTypeModel;
            this.effectiveVersionSequence = copyData.effectiveVersionSequence;
            this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
            this.modifiedOn = copyData.modifiedOn;
            this.source = copyData.source;
            this.sourceModel = copyData.sourceModel;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // ActRelationship 
    // OpenIZ.Core.Model.Acts.ControlAct, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Act
     * @summary             Represents an act which indicates why data was created/changed            
     * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {string} actTime            Gets or sets the creation time in XML format            
     * @property {string} startTime            Gets or sets the creation time in XML format            
     * @property {string} stopTime            Gets or sets the creation time in XML format            
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} moodConcept            Mood concept            
     * @property {uuid} reasonConcept            Reason concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.ActIdentifier} identifier            Gets the identifiers associated with this act            
     * @property {OpenIZModel.ActRelationship} relationship            Gets a list of all associated acts for this act            
     * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
     * @property {OpenIZModel.ActExtension} extension            Gets a list of all extensions associated with the act            
     * @property {OpenIZModel.ActNote} note            Gets a list of all notes associated with the act            
     * @property {OpenIZModel.ActTag} tag            Gets a list of all tags associated with the act            
     * @property {OpenIZModel.ActParticipation} participation            Participations            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Act} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.ControlAct} copyData Copy constructor (if present)
     */
    ControlAct: function (copyData) {
        if (copyData) {
            this.isNegated = copyData.isNegated;
            this.template = copyData.template;
            this.actTime = copyData.actTime;
            this.startTime = copyData.startTime;
            this.stopTime = copyData.stopTime;
            this.classConcept = copyData.classConcept;
            this.moodConcept = copyData.moodConcept;
            this.reasonConcept = copyData.reasonConcept;
            this.statusConcept = copyData.statusConcept;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.moodConceptModel = copyData.moodConceptModel;
            this.reasonConceptModel = copyData.reasonConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.policy = copyData.policy;
            this.extension = copyData.extension;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // ControlAct 
    // OpenIZ.Core.Model.Acts.Observation, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @abstract
     * @extends OpenIZModel.Act
     * @summary             Represents a class which is an observation            
     * @property {uuid} interpretationConcept            Gets or sets the interpretation concept            
     * @property {OpenIZModel.Concept} interpretationConceptModel [Delay loaded from interpretationConcept],             Gets or sets the concept which indicates the interpretation of the observtion            
     * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {string} actTime            Gets or sets the creation time in XML format            
     * @property {string} startTime            Gets or sets the creation time in XML format            
     * @property {string} stopTime            Gets or sets the creation time in XML format            
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} moodConcept            Mood concept            
     * @property {uuid} reasonConcept            Reason concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.ActIdentifier} identifier            Gets the identifiers associated with this act            
     * @property {OpenIZModel.ActRelationship} relationship            Gets a list of all associated acts for this act            
     * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
     * @property {OpenIZModel.ActExtension} extension            Gets a list of all extensions associated with the act            
     * @property {OpenIZModel.ActNote} note            Gets a list of all notes associated with the act            
     * @property {OpenIZModel.ActTag} tag            Gets a list of all tags associated with the act            
     * @property {OpenIZModel.ActParticipation} participation            Participations            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Act} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.Observation} copyData Copy constructor (if present)
     */
    Observation: function (copyData) {
        if (copyData) {
            this.interpretationConcept = copyData.interpretationConcept;
            this.interpretationConceptModel = copyData.interpretationConceptModel;
            this.isNegated = copyData.isNegated;
            this.template = copyData.template;
            this.actTime = copyData.actTime;
            this.startTime = copyData.startTime;
            this.stopTime = copyData.stopTime;
            this.classConcept = copyData.classConcept;
            this.moodConcept = copyData.moodConcept;
            this.reasonConcept = copyData.reasonConcept;
            this.statusConcept = copyData.statusConcept;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.moodConceptModel = copyData.moodConceptModel;
            this.reasonConceptModel = copyData.reasonConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.policy = copyData.policy;
            this.extension = copyData.extension;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // Observation 
    // OpenIZ.Core.Model.Acts.QuantityObservation, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Observation
     * @summary             Represents an observation that contains a quantity            
     * @property {number} value            Gets or sets the observed quantity            
     * @property {uuid} unitOfMeasure            Gets or sets the key of the uom concept            
     * @property {OpenIZModel.Concept} unitOfMeasureModel [Delay loaded from unitOfMeasure],             Gets or sets the unit of measure            
     * @property {uuid} interpretationConcept            Gets or sets the interpretation concept            
     * @property {OpenIZModel.Concept} interpretationConceptModel [Delay loaded from interpretationConcept],             Gets or sets the concept which indicates the interpretation of the observtion            
     * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {string} actTime            Gets or sets the creation time in XML format            
     * @property {string} startTime            Gets or sets the creation time in XML format            
     * @property {string} stopTime            Gets or sets the creation time in XML format            
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} moodConcept            Mood concept            
     * @property {uuid} reasonConcept            Reason concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.ActIdentifier} identifier            Gets the identifiers associated with this act            
     * @property {OpenIZModel.ActRelationship} relationship            Gets a list of all associated acts for this act            
     * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
     * @property {OpenIZModel.ActExtension} extension            Gets a list of all extensions associated with the act            
     * @property {OpenIZModel.ActNote} note            Gets a list of all notes associated with the act            
     * @property {OpenIZModel.ActTag} tag            Gets a list of all tags associated with the act            
     * @property {OpenIZModel.ActParticipation} participation            Participations            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Act} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.QuantityObservation} copyData Copy constructor (if present)
     */
    QuantityObservation: function (copyData) {
        if (copyData) {
            this.value = copyData.value;
            this.unitOfMeasure = copyData.unitOfMeasure;
            this.unitOfMeasureModel = copyData.unitOfMeasureModel;
            this.interpretationConcept = copyData.interpretationConcept;
            this.interpretationConceptModel = copyData.interpretationConceptModel;
            this.isNegated = copyData.isNegated;
            this.template = copyData.template;
            this.actTime = copyData.actTime;
            this.startTime = copyData.startTime;
            this.stopTime = copyData.stopTime;
            this.classConcept = copyData.classConcept;
            this.moodConcept = copyData.moodConcept;
            this.reasonConcept = copyData.reasonConcept;
            this.statusConcept = copyData.statusConcept;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.moodConceptModel = copyData.moodConceptModel;
            this.reasonConceptModel = copyData.reasonConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.policy = copyData.policy;
            this.extension = copyData.extension;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // QuantityObservation 
    // OpenIZ.Core.Model.Acts.TextObservation, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Observation
     * @summary             Represents an observation with a text value            
     * @property {string} value            Gets or sets the textual value            
     * @property {uuid} interpretationConcept            Gets or sets the interpretation concept            
     * @property {OpenIZModel.Concept} interpretationConceptModel [Delay loaded from interpretationConcept],             Gets or sets the concept which indicates the interpretation of the observtion            
     * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {string} actTime            Gets or sets the creation time in XML format            
     * @property {string} startTime            Gets or sets the creation time in XML format            
     * @property {string} stopTime            Gets or sets the creation time in XML format            
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} moodConcept            Mood concept            
     * @property {uuid} reasonConcept            Reason concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.ActIdentifier} identifier            Gets the identifiers associated with this act            
     * @property {OpenIZModel.ActRelationship} relationship            Gets a list of all associated acts for this act            
     * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
     * @property {OpenIZModel.ActExtension} extension            Gets a list of all extensions associated with the act            
     * @property {OpenIZModel.ActNote} note            Gets a list of all notes associated with the act            
     * @property {OpenIZModel.ActTag} tag            Gets a list of all tags associated with the act            
     * @property {OpenIZModel.ActParticipation} participation            Participations            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Act} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.TextObservation} copyData Copy constructor (if present)
     */
    TextObservation: function (copyData) {
        if (copyData) {
            this.value = copyData.value;
            this.interpretationConcept = copyData.interpretationConcept;
            this.interpretationConceptModel = copyData.interpretationConceptModel;
            this.isNegated = copyData.isNegated;
            this.template = copyData.template;
            this.actTime = copyData.actTime;
            this.startTime = copyData.startTime;
            this.stopTime = copyData.stopTime;
            this.classConcept = copyData.classConcept;
            this.moodConcept = copyData.moodConcept;
            this.reasonConcept = copyData.reasonConcept;
            this.statusConcept = copyData.statusConcept;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.moodConceptModel = copyData.moodConceptModel;
            this.reasonConceptModel = copyData.reasonConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.policy = copyData.policy;
            this.extension = copyData.extension;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // TextObservation 
    // OpenIZ.Core.Model.Acts.CodedObservation, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Observation
     * @summary             Represents an observation with a concept value            
     * @property {uuid} value            Gets or sets the key of the uom concept            
     * @property {OpenIZModel.Concept} valueModel [Delay loaded from value],             Gets or sets the coded value of the observation            
     * @property {uuid} interpretationConcept            Gets or sets the interpretation concept            
     * @property {OpenIZModel.Concept} interpretationConceptModel [Delay loaded from interpretationConcept],             Gets or sets the concept which indicates the interpretation of the observtion            
     * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {string} actTime            Gets or sets the creation time in XML format            
     * @property {string} startTime            Gets or sets the creation time in XML format            
     * @property {string} stopTime            Gets or sets the creation time in XML format            
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} moodConcept            Mood concept            
     * @property {uuid} reasonConcept            Reason concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.ActIdentifier} identifier            Gets the identifiers associated with this act            
     * @property {OpenIZModel.ActRelationship} relationship            Gets a list of all associated acts for this act            
     * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
     * @property {OpenIZModel.ActExtension} extension            Gets a list of all extensions associated with the act            
     * @property {OpenIZModel.ActNote} note            Gets a list of all notes associated with the act            
     * @property {OpenIZModel.ActTag} tag            Gets a list of all tags associated with the act            
     * @property {OpenIZModel.ActParticipation} participation            Participations            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Act} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.CodedObservation} copyData Copy constructor (if present)
     */
    CodedObservation: function (copyData) {
        if (copyData) {
            this.value = copyData.value;
            this.valueModel = copyData.valueModel;
            this.interpretationConcept = copyData.interpretationConcept;
            this.interpretationConceptModel = copyData.interpretationConceptModel;
            this.isNegated = copyData.isNegated;
            this.template = copyData.template;
            this.actTime = copyData.actTime;
            this.startTime = copyData.startTime;
            this.stopTime = copyData.stopTime;
            this.classConcept = copyData.classConcept;
            this.moodConcept = copyData.moodConcept;
            this.reasonConcept = copyData.reasonConcept;
            this.statusConcept = copyData.statusConcept;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.moodConceptModel = copyData.moodConceptModel;
            this.reasonConceptModel = copyData.reasonConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.policy = copyData.policy;
            this.extension = copyData.extension;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // CodedObservation 
    // OpenIZ.Core.Model.Acts.PatientEncounter, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Act
     * @summary             Represents an encounter a patient has with the health system            
     * @property {uuid} dischargeDisposition            Gets or sets the key of discharge disposition            
     * @property {OpenIZModel.Concept} dischargeDispositionModel [Delay loaded from dischargeDisposition],             Gets or sets the discharge disposition (how the patient left the encounter            
     * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {string} actTime            Gets or sets the creation time in XML format            
     * @property {string} startTime            Gets or sets the creation time in XML format            
     * @property {string} stopTime            Gets or sets the creation time in XML format            
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} moodConcept            Mood concept            
     * @property {uuid} reasonConcept            Reason concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.ActIdentifier} identifier            Gets the identifiers associated with this act            
     * @property {OpenIZModel.ActRelationship} relationship            Gets a list of all associated acts for this act            
     * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
     * @property {OpenIZModel.ActExtension} extension            Gets a list of all extensions associated with the act            
     * @property {OpenIZModel.ActNote} note            Gets a list of all notes associated with the act            
     * @property {OpenIZModel.ActTag} tag            Gets a list of all tags associated with the act            
     * @property {OpenIZModel.ActParticipation} participation            Participations            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Act} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.PatientEncounter} copyData Copy constructor (if present)
     */
    PatientEncounter: function (copyData) {
        if (copyData) {
            this.dischargeDisposition = copyData.dischargeDisposition;
            this.dischargeDispositionModel = copyData.dischargeDispositionModel;
            this.isNegated = copyData.isNegated;
            this.template = copyData.template;
            this.actTime = copyData.actTime;
            this.startTime = copyData.startTime;
            this.stopTime = copyData.stopTime;
            this.classConcept = copyData.classConcept;
            this.moodConcept = copyData.moodConcept;
            this.reasonConcept = copyData.reasonConcept;
            this.statusConcept = copyData.statusConcept;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.moodConceptModel = copyData.moodConceptModel;
            this.reasonConceptModel = copyData.reasonConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.policy = copyData.policy;
            this.extension = copyData.extension;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    },  // PatientEncounter 
    // OpenIZ.Core.Model.Acts.Protocol, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.BaseEntityData
     * @summary             Represents the model of a protocol            
     * @property {string} name            Gets or sets the name of the protocol            
     * @property {string} handlerClass            Gets or sets the handler class AQN            
     * @property {bytea} definition            Contains instructions which the handler class can understand            
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @property {string} etag            Gets a tag which changes whenever the object is updated            
     * @param {OpenIZModel.Protocol} copyData Copy constructor (if present)
     */
    Protocol: function (copyData) {
        if (copyData) {
            this.name = copyData.name;
            this.handlerClass = copyData.handlerClass;
            this.definition = copyData.definition;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
            this.etag = copyData.etag;
        }
    },  // Protocol 
    // OpenIZ.Core.Model.Acts.SubstanceAdministration, OpenIZ.Core.Model, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZModel
     * @public
     * @extends OpenIZModel.Act
     * @summary             Represents an act whereby a substance is administered to the patient            
     * @property {uuid} route            Gets or sets the key for route            
     * @property {uuid} doseUnit            Gets or sets the key for dosing unit            
     * @property {OpenIZModel.Concept} routeModel [Delay loaded from route],             Gets or sets a concept which indicates the route of administration (eg: Oral, Injection, etc.)            
     * @property {OpenIZModel.Concept} doseUnitModel [Delay loaded from doseUnit],             Gets or sets a concept which indicates the unit of measure for the dose (eg: 5 mL, 10 mL, 1 drop, etc.)            
     * @property {number} doseQuantity            Gets or sets the amount of substance administered            
     * @property {number} doseSequence            The sequence of the dose (i.e. OPV 0 = 0 , OPV 1 = 1, etc.)            
     * @property {uuid} site            Gets or sets the site            
     * @property {OpenIZModel.Concept} siteModel [Delay loaded from site],             Gets or sets a concept which indicates the site of administration            
     * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
     * @property {OpenIZModel.TemplateDefinition} template            Gets or sets the template identifier             
     * @property {string} actTime            Gets or sets the creation time in XML format            
     * @property {string} startTime            Gets or sets the creation time in XML format            
     * @property {string} stopTime            Gets or sets the creation time in XML format            
     * @property {uuid} classConcept            Class concept            
     * @property {uuid} moodConcept            Mood concept            
     * @property {uuid} reasonConcept            Reason concept            
     * @property {uuid} statusConcept            Status concept id            
     * @property {uuid} typeConcept            Type concept identifier            
     * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
     * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
     * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
     * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
     * @property {OpenIZModel.ActIdentifier} identifier            Gets the identifiers associated with this act            
     * @property {OpenIZModel.ActRelationship} relationship            Gets a list of all associated acts for this act            
     * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
     * @property {OpenIZModel.ActExtension} extension            Gets a list of all extensions associated with the act            
     * @property {OpenIZModel.ActNote} note            Gets a list of all notes associated with the act            
     * @property {OpenIZModel.ActTag} tag            Gets a list of all tags associated with the act            
     * @property {OpenIZModel.ActParticipation} participation            Participations            
     * @property {string} etag
     * @property {uuid} previousVersion
     * @property {OpenIZModel.Act} previousVersionModel [Delay loaded from previousVersion], 
     * @property {uuid} version
     * @property {number} sequence
     * @property {string} creationTime            Gets or sets the creation time in XML format            
     * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
     * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
     * @property {date} modifiedOn            Get the modified on time            
     * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
     * @property {uuid} createdBy            Gets or sets the created by identifier            
     * @property {uuid} obsoletedBy            Gets or sets the obsoleted by identifier            
     * @property {uuid} id            The internal primary key value of the entity            
     * @property {string} $type            Gets the type            
     * @param {OpenIZModel.SubstanceAdministration} copyData Copy constructor (if present)
     */
    SubstanceAdministration: function (copyData) {
        if (copyData) {
            this.route = copyData.route;
            this.doseUnit = copyData.doseUnit;
            this.routeModel = copyData.routeModel;
            this.doseUnitModel = copyData.doseUnitModel;
            this.doseQuantity = copyData.doseQuantity;
            this.doseSequence = copyData.doseSequence;
            this.site = copyData.site;
            this.siteModel = copyData.siteModel;
            this.isNegated = copyData.isNegated;
            this.template = copyData.template;
            this.actTime = copyData.actTime;
            this.startTime = copyData.startTime;
            this.stopTime = copyData.stopTime;
            this.classConcept = copyData.classConcept;
            this.moodConcept = copyData.moodConcept;
            this.reasonConcept = copyData.reasonConcept;
            this.statusConcept = copyData.statusConcept;
            this.typeConcept = copyData.typeConcept;
            this.classConceptModel = copyData.classConceptModel;
            this.moodConceptModel = copyData.moodConceptModel;
            this.reasonConceptModel = copyData.reasonConceptModel;
            this.statusConceptModel = copyData.statusConceptModel;
            this.typeConceptModel = copyData.typeConceptModel;
            this.identifier = copyData.identifier;
            this.relationship = copyData.relationship;
            this.policy = copyData.policy;
            this.extension = copyData.extension;
            this.note = copyData.note;
            this.tag = copyData.tag;
            this.participation = copyData.participation;
            this.etag = copyData.etag;
            this.previousVersion = copyData.previousVersion;
            this.previousVersionModel = copyData.previousVersionModel;
            this.version = copyData.version;
            this.sequence = copyData.sequence;
            this.creationTime = copyData.creationTime;
            this.obsoletionTime = copyData.obsoletionTime;
            this.createdByModel = copyData.createdByModel;
            this.modifiedOn = copyData.modifiedOn;
            this.obsoletedByModel = copyData.obsoletedByModel;
            this.createdBy = copyData.createdBy;
            this.obsoletedBy = copyData.obsoletedBy;
            this.id = copyData.id;
            this.$type = copyData.$type;
        }
    }, // SubstanceAdministration 

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
} // OpenIZModel
