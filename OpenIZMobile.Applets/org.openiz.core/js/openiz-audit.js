/// <reference path="openiz.js"/>
/*
 * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-10-30
 */

/**
 * @summary A documented namespace
 * @namespace
 * @property {uuid} EmptyGuid A property which represents an empty UUID
 */
var OpenIZAudit = OpenIZAudit || {
    // MARC.HI.EHRS.SVC.Auditing.Data.AuditableObject, MARC.HI.EHRS.SVC.Auditing.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZAudit
     * @public
     * @summary             Identifies an object that adds context to the audit            
     * @property {string} id            Identifies the object in the event            
     * @property {OpenIZAudit.AuditableObjectType} type            Identifies the type of object being expressed            (see: {@link OpenIZAudit.AuditableObjectType} for values)
     * @property {OpenIZAudit.AuditableObjectRole} role            Identifies the role type of the object            (see: {@link OpenIZAudit.AuditableObjectRole} for values)
     * @property {OpenIZAudit.AuditableObjectLifecycle} lifecycle            Identifies where in the lifecycle of the object this object is currently within            (see: {@link OpenIZAudit.AuditableObjectLifecycle} for values)
     * @property {OpenIZAudit.AuditableObjectIdType} idType            Identifies the type of identifier supplied            (see: {@link OpenIZAudit.AuditableObjectIdType} for values)
     * @property {OpenIZAudit.AuditCode} customCode            Custom id type code            
     * @property {string} queryData            Data associated with the object            
     * @property {string} name            Data associated with the object            
     * @property {ObjectDataExtension} dictionary            Additional object data            
     * @param {OpenIZAudit.AuditableObject} copyData Copy constructor (if present)
     */
    AuditableObject: function (copyData) {
        this.$type = 'AuditableObject';
        if (copyData) {
            this.dictionary = copyData.dictionary;
            this.name = copyData.name;
            this.queryData = copyData.queryData;
            this.customCode = copyData.customCode;
            this.idType = copyData.idType;
            this.lifecycle = copyData.lifecycle;
            this.role = copyData.role;
            this.type = copyData.type;
            this.id = copyData.id;
        }
    },  // AuditableObject 
    // MARC.HI.EHRS.SVC.Auditing.Data.AuditActorData, MARC.HI.EHRS.SVC.Auditing.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZAudit
     * @public
     * @summary             Data related to actors that participate in the event            
     * @property {string} uid            The unique identifier for the user in the system            
     * @property {string} uname            The name of the user in the system            
     * @property {bool} isReq            True if the user is the primary requestor            
     * @property {string} apId            Identifies the network access point from which the user accessed the system            
     * @property {OpenIZAudit.NetworkAccessPointType} apType            Identifies the type of network access point            (see: {@link OpenIZAudit.NetworkAccessPointType} for values)
     * @property {OpenIZAudit.AuditCode} role            Identifies the role(s) that the actor has played            
     * @property {string} altUid            Alternative user identifier            
     * @param {OpenIZAudit.AuditActorData} copyData Copy constructor (if present)
     */
    AuditActorData: function (copyData) {
        this.$type = 'AuditActorData';
        if (copyData) {
            this.altUid = copyData.altUid;
            this.role = copyData.role;
            this.apType = copyData.apType;
            this.apId = copyData.apId;
            this.isReq = copyData.isReq;
            this.uname = copyData.uname;
            this.uid = copyData.uid;
        }
    },  // AuditActorData 
    // MARC.HI.EHRS.SVC.Auditing.Data.AuditCode, MARC.HI.EHRS.SVC.Auditing.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZAudit
     * @public
     * @summary             Represents an audit             
     * @property {string} code            Gets or sets the code of the code value            
     * @property {string} system            Gets or sets the system in which the code value is drawn            
     * @property {string} systemName            Gets or sets the human readable name of the code sytsem            
     * @property {string} systemVersion            Gets or sets the version of the code system            
     * @property {string} display            Gets or sets the display name            
     * @param {OpenIZAudit.AuditCode} copyData Copy constructor (if present)
     */
    AuditCode: function (copyData) {
        this.$type = 'AuditCode';
        if (copyData) {
            this.display = copyData.display;
            this.systemVersion = copyData.systemVersion;
            this.systemName = copyData.systemName;
            this.system = copyData.system;
            this.code = copyData.code;
        }
    },  // AuditCode 
    // MARC.HI.EHRS.SVC.Auditing.Data.AuditData, MARC.HI.EHRS.SVC.Auditing.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZAudit
     * @public
     * @summary             Represents data related to an audit event            
     * @property {date} timestamp            Event timestamp            
     * @property {OpenIZAudit.ActionType} action            Identifies the action code            (see: {@link OpenIZAudit.ActionType} for values)
     * @property {OpenIZAudit.OutcomeIndicator} outcome            Identifies the outcome of the event            (see: {@link OpenIZAudit.OutcomeIndicator} for values)
     * @property {OpenIZAudit.EventIdentifierType} event            Identifies the event            (see: {@link OpenIZAudit.EventIdentifierType} for values)
     * @property {OpenIZAudit.AuditCode} type            Identifies the type of event            
     * @property {OpenIZAudit.AuditActorData} actor            Represents the actors within the audit event            
     * @property {OpenIZAudit.AuditableObject} object            Represents other objects of interest            
     * @param {OpenIZAudit.AuditData} copyData Copy constructor (if present)
     */
    AuditData: function (copyData) {
        this.$type = 'AuditData';
        if (copyData) {
            this.object = copyData.object;
            this.actor = copyData.actor;
            this.type = copyData.type;
            this.event = copyData.event;
            this.outcome = copyData.outcome;
            this.action = copyData.action;
            this.timestamp = copyData.timestamp;
        }
    },  // AuditData 
    // MARC.HI.EHRS.SVC.Auditing.Data.AuditableObjectType, MARC.HI.EHRS.SVC.Auditing.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null
    /**
     * @enum {uuid}
     * @memberof OpenIZAudit
     * @public
     * @readonly
     * @summary             Identifies the type of auditable objects in the system            
     */
    AuditableObjectType: {
        /** 
         * 
         */
        Person: 'Person',
        /** 
         * 
         */
        SystemObject: 'SystemObject',
        /** 
         * 
         */
        Organization: 'Organization',
        /** 
         * 
         */
        Other: 'Other',
    },  // AuditableObjectType 
    // MARC.HI.EHRS.SVC.Auditing.Data.AuditableObjectRole, MARC.HI.EHRS.SVC.Auditing.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null
    /**
     * @enum {uuid}
     * @memberof OpenIZAudit
     * @public
     * @readonly
     * @summary             Identifies roles of objects in the audit event            
     */
    AuditableObjectRole: {
        /** 
         * 
         */
        Patient: 'Patient',
        /** 
         * 
         */
        Location: 'Location',
        /** 
         * 
         */
        Report: 'Report',
        /** 
         * 
         */
        Resource: 'Resource',
        /** 
         * 
         */
        MasterFile: 'MasterFile',
        /** 
         * 
         */
        User: 'User',
        /** 
         * 
         */
        List: 'List',
        /** 
         * 
         */
        Doctor: 'Doctor',
        /** 
         * 
         */
        Subscriber: 'Subscriber',
        /** 
         * 
         */
        Guarantor: 'Guarantor',
        /** 
         * 
         */
        SecurityUser: 'SecurityUser',
        /** 
         * 
         */
        SecurityGroup: 'SecurityGroup',
        /** 
         * 
         */
        SecurityResource: 'SecurityResource',
        /** 
         * 
         */
        SecurityGranularityDefinition: 'SecurityGranularityDefinition',
        /** 
         * 
         */
        Provider: 'Provider',
        /** 
         * 
         */
        DataDestination: 'DataDestination',
        /** 
         * 
         */
        DataRepository: 'DataRepository',
        /** 
         * 
         */
        Schedule: 'Schedule',
        /** 
         * 
         */
        Customer: 'Customer',
        /** 
         * 
         */
        Job: 'Job',
        /** 
         * 
         */
        JobStream: 'JobStream',
        /** 
         * 
         */
        Table: 'Table',
        /** 
         * 
         */
        RoutingCriteria: 'RoutingCriteria',
        /** 
         * 
         */
        Query: 'Query',
    },  // AuditableObjectRole 
    // MARC.HI.EHRS.SVC.Auditing.Data.AuditableObjectLifecycle, MARC.HI.EHRS.SVC.Auditing.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null
    /**
     * @enum {uuid}
     * @memberof OpenIZAudit
     * @public
     * @readonly
     * @summary             Auditable object lifecycle            
     */
    AuditableObjectLifecycle: {
        /** 
         * 
         */
        Creation: 'Creation',
        /** 
         * 
         */
        Import: 'Import',
        /** 
         * 
         */
        Amendment: 'Amendment',
        /** 
         * 
         */
        Verification: 'Verification',
        /** 
         * 
         */
        Translation: 'Translation',
        /** 
         * 
         */
        Access: 'Access',
        /** 
         * 
         */
        Deidentification: 'Deidentification',
        /** 
         * 
         */
        Aggregation: 'Aggregation',
        /** 
         * 
         */
        Report: 'Report',
        /** 
         * 
         */
        Export: 'Export',
        /** 
         * 
         */
        Disclosure: 'Disclosure',
        /** 
         * 
         */
        ReceiptOfDisclosure: 'ReceiptOfDisclosure',
        /** 
         * 
         */
        Archiving: 'Archiving',
        /** 
         * 
         */
        LogicalDeletion: 'LogicalDeletion',
        /** 
         * 
         */
        PermanentErasure: 'PermanentErasure',
    },  // AuditableObjectLifecycle 
    // MARC.HI.EHRS.SVC.Auditing.Data.AuditableObjectIdType, MARC.HI.EHRS.SVC.Auditing.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null
    /**
     * @enum {uuid}
     * @memberof OpenIZAudit
     * @public
     * @readonly
     * @summary             Classifies the type of identifier that a auditable object may have            
     */
    AuditableObjectIdType: {
        /** 
         * 
         */
        MedicalRecord: 'MedicalRecord',
        /** 
         * 
         */
        PatientNumber: 'PatientNumber',
        /** 
         * 
         */
        EncounterNumber: 'EncounterNumber',
        /** 
         * 
         */
        EnrolleeNumber: 'EnrolleeNumber',
        /** 
         * 
         */
        SocialSecurityNumber: 'SocialSecurityNumber',
        /** 
         * 
         */
        AccountNumber: 'AccountNumber',
        /** 
         * 
         */
        GuarantorNumber: 'GuarantorNumber',
        /** 
         * 
         */
        ReportName: 'ReportName',
        /** 
         * 
         */
        ReportNumber: 'ReportNumber',
        /** 
         * 
         */
        SearchCritereon: 'SearchCritereon',
        /** 
         * 
         */
        UserIdentifier: 'UserIdentifier',
        /** 
         * 
         */
        Uri: 'Uri',
        /** 
         *             Custom code            
         */
        Custom: 'Custom',
    },  // AuditableObjectIdType 
    // MARC.HI.EHRS.SVC.Auditing.Data.NetworkAccessPointType, MARC.HI.EHRS.SVC.Auditing.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null
    /**
     * @enum {uuid}
     * @memberof OpenIZAudit
     * @public
     * @readonly
     * @summary             Represents the type of network access point            
     */
    NetworkAccessPointType: {
        /** 
         *             The identifier is a machine name            
         */
        MachineName: 'MachineName',
        /** 
         *             Identifier is an IP address            
         */
        IPAddress: 'IPAddress',
        /** 
         *             Identifier is a telephone number            
         */
        TelephoneNumber: 'TelephoneNumber',
    },  // NetworkAccessPointType 
    // MARC.HI.EHRS.SVC.Auditing.Data.ActionType, MARC.HI.EHRS.SVC.Auditing.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null
    /**
     * @enum {uuid}
     * @memberof OpenIZAudit
     * @public
     * @readonly
     * @summary             Represents types of actions            
     */
    ActionType: {
        /** 
         *             Data was created in the system            
         */
        Create: 'Create',
        /** 
         *             Data was viewed, printed, displayed, etc...            
         */
        Read: 'Read',
        /** 
         *             Data was revised in the system            
         */
        Update: 'Update',
        /** 
         *             Data was removed from the system            
         */
        Delete: 'Delete',
        /** 
         *             A system, or application function was performed            
         */
        Execute: 'Execute',
    },  // ActionType 
    // MARC.HI.EHRS.SVC.Auditing.Data.OutcomeIndicator, MARC.HI.EHRS.SVC.Auditing.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null
    /**
     * @enum {uuid}
     * @memberof OpenIZAudit
     * @public
     * @readonly
     * @summary             Represents potential outcomes            
     */
    OutcomeIndicator: {
        /** 
         *             Successful operation            
         */
        Success: 'Success',
        /** 
         *             Minor failure, action should be restarted            
         */
        MinorFail: 'MinorFail',
        /** 
         *             Action was terminated            
         */
        SeriousFail: 'SeriousFail',
        /** 
         *             Major failure, action is made unavailable            
         */
        EpicFail: 'EpicFail',
    },  // OutcomeIndicator 
    // MARC.HI.EHRS.SVC.Auditing.Data.EventIdentifierType, MARC.HI.EHRS.SVC.Auditing.Core, Version=1.0.3.0, Culture=neutral, PublicKeyToken=null
    /**
     * @enum {uuid}
     * @memberof OpenIZAudit
     * @public
     * @readonly
     * @summary             Event Identifier            
     */
    EventIdentifierType: {
        /** 
         * 
         */
        ProvisioningEvent: 'ProvisioningEvent',
        /** 
         * 
         */
        MedicationEvent: 'MedicationEvent',
        /** 
         * 
         */
        ResourceAssignment: 'ResourceAssignment',
        /** 
         * 
         */
        CareEpisode: 'CareEpisode',
        /** 
         * 
         */
        CareProtocol: 'CareProtocol',
        /** 
         * 
         */
        ProcedureRecord: 'ProcedureRecord',
        /** 
         * 
         */
        Query: 'Query',
        /** 
         * 
         */
        PatientRecord: 'PatientRecord',
        /** 
         * 
         */
        OrderRecord: 'OrderRecord',
        /** 
         * 
         */
        NetowrkEntry: 'NetowrkEntry',
        /** 
         * 
         */
        Import: 'Import',
        /** 
         * 
         */
        Export: 'Export',
        /** 
         * 
         */
        ApplicationActivity: 'ApplicationActivity',
        /** 
         * 
         */
        SecurityAlert: 'SecurityAlert',
        /** 
         * 
         */
        UserAuthentication: 'UserAuthentication',
        /** 
         * 
         */
        EmergencyOverrideStarted: 'EmergencyOverrideStarted',
        /** 
         * 
         */
        UseOfRestrictedFunction: 'UseOfRestrictedFunction',
        /** 
         * 
         */
        Login: 'Login',
        /** 
         * 
         */
        Logout: 'Logout',
    },  // EventIdentifierType 
    /*
     * Copyright 2015-2018 Mohawk College of Applied Arts and Technology
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
     * Date: 2016-11-3
     */

    // Empty guid
    EmptyGuid: "00000000-0000-0000-0000-000000000000",

    /**
         * @class
         * @summary Represents a simple exception class
         * @constructor
         * @memberof OpenIZModel
         * @property {String} message Informational message about the exception
         * @property {Object} details Any detail / diagnostic information
         * @property {OpenIZModel.Exception} caused_by The cause of the exception
         * @param {String} message Informational message about the exception
         * @param {Object} detail Any detail / diagnostic information
         * @param {OpenIZModel.Exception} cause The cause of the exception
         */
    Exception: function (type, message, detail, cause) {
        _self = this;

        this.type = type;
        this.message = message;
        this.details = detail;
        this.caused_by = cause;

    }
} // OpenIZAudit
