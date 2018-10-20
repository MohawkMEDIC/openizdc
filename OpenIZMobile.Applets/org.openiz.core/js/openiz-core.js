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
 * @summary Execution environment for BRE in Javascript
 * @class
 * @static
 * @namespace
 * @description The BRE normally runs in the server as a series of rules, however bcause some rules may
 * need to be run as local rules for the user interface, this class simulates the BRE back-end service. For
 * example, validation functions and/or pre/post install actions will simulate the execution on the server
 * but won't actually result in the storage
 */
var OpenIZBre = OpenIZBre || {
    /**
     * @summary Issue priority
     * @enum
     * @memberof OpenIZBre
     * @description This enumeration is used to control the priority of a raised business rule issue.
     */
    IssuePriority: {
        /** 
         * The issue is serious and should halt the execution of the business rule. The issue cannot be ignored
         */
        Error: 1,
        /** 
         * The issue is a warning. The user should be alerted to the presence of this rule, however the user may override it
         */
        Warning: 2,
        /** 
         * The issue should be logged for further followup
         */
        Information : 3
    },
    /** 
     * @summary Represents a detected issue which is business rule issue
     * @description Detected issues are used on validation functions to alert the user or the server of issues
     * which might violate clinic rules or simple business logic.
     * @constructor
     * @class
     * @param {string} text The textual content of the issue
     * @param {OpenIZBre.IssuePriority} priority
     * @memberof OpenIZBre
     */
    DetectedIssue: function(text, priority) {
        this.text = text;
        this.priority = priority;
    },
    /**
     * @summary Allows rules to know they're running in the UI rather than the back-end. 
     * @description Rules often use this flag to change the way that they tag objects as "rules already run"
     * or to change the way that certain resources are shown.
     * @memberof OpenIZBre
     */
    $inFrontEnd: true,
    /**
     * @summary Reference sets loaded
     * @private
     */
    $refSets : {},
    /** 
     * @summary Reference set base URLs
     * @private
     */
    $refSetBase : [],
    /**
     * @summary The current list of triggers registered for javascript
     * @memberof OpenIZBre
     * @private
     */
    _triggers: [],
    /**
     * @summary The current list of validators registered for javascript
     * @memberof OpenIZBre
     * @private
     */
    _validators: [],
    /**
     * @method
     * @memberof OpenIZBre
     * @summary Simulates the simplify 
     * @deprecated
     */
    ToViewModel: function (object) { return object; },
    /**
     * @method
     * @memberof OpenIZBre
     * @summary Simulates the expand object method
     * @deprecated
     */
    FromViewModel: function (object) { return object; },
    /**
     * @method
     * @memberof OpenIZBre
     * @summary Adds a trigger event to the triggers collection
     * @description This function adds the specified business rule to the current execution context. The 
     * execution will run the specified callback when the specified trigger occurs on the specified type
     * @param {string} type The type of object being registered
     * @param {string} trigger The name of the BRE trigger to subscribe to
     * @param {function} callback The callback function
     * @example
     * // Before an Observation is inserted, add a tag that says reviewStatus 
     * OpenIZBre.AddBusinessRule("Observation", "BeforeInsert", function(obs) {
     *      // Have we already tagged?
     *      obs.tag = obs.tag || {};
     *      if(!obs.tag["reviewStatus"])
     *          obs.tag["reviewStatus"] = "NeedsApprovalFromDivo";
     *      return obs;
     * });
     */
    AddBusinessRule: function (type, trigger, callback) {
        this._triggers.push({
            type: type,
            trigger: trigger,
            callback: callback
        });
    },
    /**
     * @method
     * @memberof OpenIZBre
     * @summary Adds a validator to the current execution context
     * @description This method will add a validator to the current execution context. A validator is run prior to persistence
     * and is even run in the user interface before submitting the object to do a sanity check on the object. 
     * @param {string} type The type of object being registered
     * @param {function} callback The callback function for validation, this function should return an array of DetectedIssue 
     * @see OpenIZBre.DetectedIssue
     * @example 
     * // Add validator to make sure a completed act doesn't occur in the future
     * OpenIZBre.AddValidator("Act", function(act) {
     *      var issues = [];
     *      if(act.statusCocnept == OpenIZModel.StatusKeys.Complete &&
     *          act.actTime > new Date())
     *          issues.push(new OpenIZBre.DetectedIssue("locale.error.actInTheFuture", OpenIZBre.IssuePriority.Error));
     *      return issues;
     *  });
     * });
     */
    AddValidator: function (type, callback) {
        this._validators.push({
            type: type,
            callback: callback
        });
    },
    /** 
     * @method
     * @memberof OpenIZBre
     * @summary Simulates the rule being executed
     * @description This function can be used by callers to execute the specified trigger for the specified instance. The instance
     * is expected to be a valid class from OpenIZModel namespace
     * @see OpenIZModel
     * @param {string} trigger The trigger execute
     * @param {object} instance The instance to execute the trigger on
     * @example
     * // In my user interface I want to call beforeInsert to get the interpretation of the observation
     * if(myObservation) {
     *      var beforeInsert = OpenIZBre.ExecuteRule("BeforeInsert", myObservation);
     *      alert("Observation was interpreted as " + myObservation.interpretationConcept);
     * }
     */
    ExecuteRule: function (trigger, instance) {
        // Execute the rule
        var retVal = instance;
        for (var t in this._triggers)
            if (this._triggers[t].type == instance.$type && this._triggers[t].trigger == trigger) {
                var triggerResult = this._triggers[t].callback(retVal);
                retVal = triggerResult || retVal;
            }
        return retVal;
    },
    /** 
     * @method
     * @memberof OpenIZBre
     * @summary Performs the validation function on the specified instance
     * @description Calling this method will invoke all registered validators on the specified instance and will return a 
     * collection of DetectedIssues.
     * @see OpenIZBre.DetectedIssue
     * @example
     * if(myObservation) {
     *      var issues = OpenIZBre.Validate(myObservation);
     *      alert("There are " + issues.length + " issues with this observation");
     * }
     */
    Validate: function (instance) {

        // Execute the rule
        var retVal = [];
        for (var t in this._validators)
            if (this._validators[t].type == instance.$type) {
                var issues = this._validators[t].callback(instance);
                for (var i in issues)
                    retVal.push(issues[i]);
            }
        return retVal;

    }
};
/// <Reference path="./openiz.js"/>
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
var OpenIZWarehouse = OpenIZWarehouse || {

    // OpenIZ.Core.Data.Warehouse.DatamartDefinition, OpenIZ.Core.PCL, Version=0.8.0.23380, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZWarehouse
     * @public
     * @summary             Represents a datamart definition which contains the definition of fields for a datamart            
     * @property {uuid} id            Gets or sets the identifier of the data mart            
     * @property {string} name            Gets or sets the name of the data mart            
     * @property {date} creationTime            Gets or sets the time that the data mart was created            
     * @property {OpenIZWarehouse.DatamartSchema} schema            Gets or sets the datamart schema            
     * @param {OpenIZWarehouse.DatamartDefinition} copyData Copy constructor (if present)
     */
    DatamartDefinition: function (copyData) {
        this.$type = 'DatamartDefinition';
        if (copyData) {
            this.schema = copyData.schema;
            this.creationTime = copyData.creationTime;
            this.name = copyData.name;
            this.id = copyData.id;
        }
    },  // DatamartDefinition 
    // OpenIZ.Core.Data.Warehouse.DatamartSchema, OpenIZ.Core.PCL, Version=0.8.0.23380, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZWarehouse
     * @public
     * @summary             Represents a datamart schema which gives hints to the properties to be stored from             a dynamic object            
     * @property {uuid} id            Gets or sets the unique identifier for the schema itself            
     * @property {string} name            Gets or sets the name of the element in the database            
     * @property {OpenIZWarehouse.DatamartSchemaProperty} property            Gets or sets the property names for the schema element            
     * @property {OpenIZWarehouse.DatamartStoredQuery} sqp            Gets or sets the query associated with the schema            
     * @param {OpenIZWarehouse.DatamartSchema} copyData Copy constructor (if present)
     * @example
     * var schema = OpenIZWarehouse.DatamartSchema({
     *      name: "mydatamart",
     *      property: [
     *          new OpenIZWarehouse.DatamartSchemaProperty({
     *            name: "uuid",
     *              type: OpenIZWarehouse.SchemaPropertyAttributes.Uuid,
     *              attributes: OpenIZWarehouse.SchemaPropertyAttributes.Unique | OpenIZWarehouse.SchemaPropertyAttributes.NotNull
     *          }),
     *          new OpenIZWarehouse.DatamartSchemaProperty({
     *              name: "value",
     *              type: OpenIZWarehouse.SchemaPropertyAttributes.String,
     *              attributes: OpenIZWarehouse.SchemaPropertyAttributes.NotNull
     *          })
     *      ],
     *      sqp : [
     *          new OpenIZWarehouse.DatamartStoredQuery({
     *              name : "countbyvalue",
     *              property : [
     *                  new OpenIZWarehouse.DatamartSchemaProperty({ name: "uuid", type: OpenIZWarehouse.SchemaPropertyType.Uuid }),
     *                  new OpenIZWarehouse.DatamartSchemaProperty({ name: "c", type: OpenIZWarehouse.SchemaPropertyType.Integer })
     *              ],
     *              select : [
     *                  new OpenIZWarehouse.DatamartStoredQueryDefinition({
     *                      provider: "sqlite",
     *                      sql: "SELECT value, count(*) as c FROM mymart group by value"
     *                  })
     *              ]
     *          });
     *      ]
     * });
     */
    DatamartSchema: function (copyData) {
        this.$type = 'DatamartSchema';
        if (copyData) {
            this.sqp = copyData.sqp;
            this.property = copyData.property;
            this.name = copyData.name;
            this.id = copyData.id;
        }
    },  // DatamartSchema 
    // OpenIZ.Core.Data.Warehouse.DatamartSchemaProperty, OpenIZ.Core.PCL, Version=0.8.0.23380, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZWarehouse
     * @public
     * @summary             Represents a single property on the data mart schema            
     * @property {uuid} id            Gets or sets the identifier of the warehouse property            
     * @property {string} name            Gets or sets the name of the property            
     * @property {SchemaPropertyType} type            Gets or sets the type of property            
     * @property {SchemaPropertyAttributes} attributes            Gets or sets the attributes associated with the property            
     * @property {OpenIZWarehouse.DatamartSchemaProperty} property            Gets or sets the sub-properties of this property            
     * @param {OpenIZWarehouse.DatamartSchemaProperty} copyData Copy constructor (if present)
     * @example
     * var property = new OpenIZWarehouse.DatamartSchemaProperty({
     *      name: "id",
     *      type: OpenIZWarehouse.SchemaPropertyAttributes.Uuid,
     *      attributes: OpenIZWarehouse.SchemaPropertyAttributes.Unique | OpenIZWarehouse.SchemaPropertyAttributes.NotNull
     * });
     * schema.property.push(property);
     * @see OpenIZWarehouse.DatamartSchema
     */
    DatamartSchemaProperty: function (copyData) {
        this.$type = 'DatamartSchemaProperty';
        if (copyData) {
            this.property = copyData.property;
            this.attributes = copyData.attributes;
            this.type = copyData.type;
            this.name = copyData.name;
            this.id = copyData.id;
        }
    },  // DatamartSchemaProperty 
    // OpenIZ.Core.Data.Warehouse.DatamartStoredQuery, OpenIZ.Core.PCL, Version=0.8.0.23380, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZWarehouse
     * @public
     * @summary             Represents a stored query creation statement            
     * @property {uuid} id            Gets or sets the provider identifier            
     * @property {string} name            Definition of the query            
     * @property {OpenIZWarehouse.DatamartSchemaProperty} property            Gets or sets the property names for the schema element            
     * @property {OpenIZWarehouse.DatamartStoredQueryDefinition} select            Definition of the query            
     * @param {OpenIZWarehouse.DatamartStoredQuery} copyData Copy constructor (if present)
     * @example
     *  var storedQuery = new OpenIZWarehouse.DatamartStoredQuery({
     *      name : "countbyuuid",
     *      property : [
     *          new OpenIZWarehouse.DatamartSchemaProperty({ name: "uuid", type: OpenIZWarehouse.SchemaPropertyType.Uuid }),
     *          new OpenIZWarehouse.DatamartSchemaProperty({ name: "uuid", type: OpenIZWarehouse.SchemaPropertyType.Integer })
     *      ],
     *      select : [
     *          new OpenIZWarehouse.DatamartStoredQueryDefinition({
     *              provider: "sqlite",
     *              sql: "SELECT uuid, count(*) as c FROM mymart"
     *          })
     *      ]
     *  });
     *  schema.sqp.push(storedQuery);
     * @see OpenIZWarehouse.DatamartStoredQueryDefinition
     * @see OpenIZWarehouse.DatamartSchema
     */
    DatamartStoredQuery: function (copyData) {
        this.$type = 'DatamartStoredQuery';
        if (copyData) {
            this.select = copyData.select;
            this.property = copyData.property;
            this.name = copyData.name;
            this.id = copyData.id;
        }
    },  // DatamartStoredQuery 
    // OpenIZ.Core.Data.Warehouse.DatamartStoredQueryDefinition, OpenIZ.Core.PCL, Version=0.8.0.23380, Culture=neutral, PublicKeyToken=null
    /**
     * @class
     * @memberof OpenIZWarehouse
     * @public
     * @summary             Represents the SQL for an actual query            
     * @property {string} provider            Provider identifier            
     * @property {string} sql            The SQL             
     * @param {OpenIZWarehouse.DatamartStoredQueryDefinition} copyData Copy constructor (if present)
     * @example
     *  var definition = new OpenIZWarehouse.DatamartStoredQueryDefinition({
     *      provider: "sqlite",
     *      sql: "SELECT uuid, count(*) as c FROM mymart
     *  });
     *  myStoredQuery.select.push(definition);
     */
    DatamartStoredQueryDefinition: function (copyData) {
        this.$type = 'DatamartStoredQueryDefinition';
        if (copyData) {
            this.sql = copyData.sql;
            this.provider = copyData.provider;
        }
    },  // DatamartStoredQueryDefinition 

    /** 
     * @enum {int}
     * @memberof OpenIZWarehouse
     * @public
     * @readonly
     * @summary Schema property attributes
     */
    SchemaPropertyAttributes: {
        None: 0x0,
        Indexed: 0x1,
        NotNull: 0x2,
        Unique: 0x4
    },

    /** 
     * @enum {int}
     * @memberof OpenIZWarehouse
     * @public
     * @readonly
     * @summary Schema property types
     */
    SchemaPropertyType: {
        String: 0,
        Integer: 1,
        Float: 2,
        Date: 3,
        Boolean: 4,
        Uuid: 5,
        Binary: 6,
        Object: 7
    },

    /**
     * @class 
     * @memberof OpenIZWarehouse
     * @summary Represents the ad-hoc warehouse service wrappers
     * @static
     */
    Adhoc: {
        /** 
         * @memberof OpenIZWarehouse.Adhoc
         * @method
         * @summary Creates a new datamart with the specified schema
         * @param {object} controlData the control data
         * @param {String} controlData.name The name of the datamart
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {OpenIZWarehouse.DatamartSchema} controlData.schema The schema (definition) of the object being stored in the datamart
         * @return {OpenIZWarehouse.DatamartDefinition} The created datamart definition
         */
        createDatamartAsync: function (controlData) {
            throw OpenIZModel.Exception("NotSupportedException", "Creating datamarts is not supported");
        },
        /** 
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Gets a list of all available datamarts
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         */
        getDatamartsAsync: function (controlData) {
            OpenIZ.Util.simpleGet(
                "/__zombo/marts",
                controlData);
        },
        /** 
         * @method
         * @param {object} controlData 
         * @param {string} controlData.name
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Gets a list of all available datamarts
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         */
        getDatamartAsync: function (controlData) {
            OpenIZ.Util.simpleGet(
                "/__zombo/marts",
                {
                    continueWith: controlData.continueWith,
                    onException: controlData.onException,
                    state: controlData.state,
                    finally: controlData.finally,
                    query: { name: controlData.name }
                });
        },
        /**
         * @method
         * @memberof OpenIZWarehouse.Adhoc
          * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
        * @summary Deletes a datamart
         */
        deleteDatamartAsync: function (controlData) {
            throw OpenIZModel.Exception("NotSupportedException", "Deleting datamarts is not supported");

        },
        /**
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @deprecated
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} martId The datamart from which the object should be retrieved
         * @param {string} tupleId The identifier of the tuple in that data mart's schema
         * @return {Object} The data stored in that tuple
         */
        getObjectAsync: function (martId, tupleId) {
            throw OpenIZModel.Exception("NotSupportedException", "Get datamart tuple is not supported");

        },
        /**
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Executes a stored query on the datamart with the specified parameters
         * @param {Object} controlData The control data for the operation
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.martId The datamart from which the object should be retrieved
         * @param {string} controlData.queryName The query to be executed
         * @param {Object} controlData.parameter The object representing query parameters to the mart
         * @return {Object} A list of matching tuple or aggregates
         */
        queryAsync: function (controlData) {

            var query = controlData.parameters;
            query.queryId = controlData.queryId;
            query.martId = controlData.martId;
            OpenIZ.Util.simpleGet(
                "/__zombo/storedQuery",
                {
                    query: query,
                    continueWith: controlData.continueWith,
                    finally: controlData.finally,
                    onException: controlData.onException,
                    state: controlData.state
                });

        },
        /**
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Adds the specified tuple to the datamart
        * @param {Object} controlData The control data for the operation
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.martId The datamart to which the object should be stored
         * @param {Object} controlData.object The object to be stored in the datamart
         * @return {string} The tuple identifier of the object stored
         */
        addAsync: function (controlData) {
            throw OpenIZModel.Exception("NotSupportedException", "Add data to datamarts is not supported");

        },
        /** 
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @summary Removes the specified tuple from the warehouse
        * @param {Object} controlData The control data for the operation
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.martId The datamart from which the object should be removed
         * @param {string} controlData.tupleId The identifier of the tuple to be deleted
         * @return {string} The tuple identifier of the object stored
         */
        removeAsync: function (controlData) {
            throw OpenIZModel.Exception("NotSupportedException", "Remove data from datamarts is not supported");

        },
        /**
         * @method
         * @memberof OpenIZWarehouse.Adhoc
         * @param {object} controlData the control data
         * @param {OpenIZ~continueWith} controlData.continueWith The callback to call when the operation is completed successfully
         * @param {OpenIZ~onException} controlData.onException The callback to call when the operation encounters an exception
         * @param {OpenIZ~finally} controlData.finally The callback of a function to call whenever the operation completes successfully or not
         * @param {string} controlData.martId The datamart from which the object should be retrieved
         * @param {string} controlData.query The query to match control data on
         * @return {Object} The data stored in that tuple
         */
        adhocQueryAsync: function (controlData) {
            var query = controlData.parameters;
            query.martId = controlData.martId;
            OpenIZ.Util.simpleGet(
                "/__zombo/adhocQuery",
                {
                    query: query,
                    continueWith: controlData.continueWith,
                    finally: controlData.finally,
                    onException: controlData.onException,
                    state: controlData.state
                });
        }
    }

}

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
var OpenIZModel = OpenIZModel || {
// OpenIZ.Core.Model.BaseEntityData, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @abstract
 * @extends OpenIZModel.IdentifiedData
 * @summary             Represents the root of all model classes in the OpenIZ Core            
 * @description 
 *             This abstract class is used to encapsulate the key properties of base data elements in the OpenIZ
 *             model, namely it keeps track of which entities created and obsoleted a particular resource and when those
 *             events occurred.
 *             
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.BaseEntityData} copyData Copy constructor (if present)
 */
BaseEntityData : function(copyData) { 
	this.$type = 'BaseEntityData';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	}
},  // BaseEntityData 
// OpenIZ.Core.Model.Association`1, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @param {OpenIZModel.Association} copyData Copy constructor (if present)
 */
Association : function(copyData) { 
	this.$type = 'Association';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	}
},  // Association 
// OpenIZ.Core.Model.IdentifiedData, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @abstract
 * @summary             Represents data that is identified by a key            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @property {date} modifiedOn            Gets or sets the modified on time            
 * @param {OpenIZModel.IdentifiedData} copyData Copy constructor (if present)
 */
IdentifiedData : function(copyData) { 
	this.$type = 'IdentifiedData';
	if(copyData) {
	this.modifiedOn = copyData.modifiedOn;
	this.id = copyData.id;
	}
},  // IdentifiedData 
// OpenIZ.Core.Model.NonVersionedEntityData, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.BaseEntityData
 * @summary             Updateable entity data which is not versioned            
 * @property {string} updatedTime            Gets or sets the creation time in XML format            
 * @property {date} modifiedOn            Gets the time this item was modified            
 * @property {uuid} updatedBy            Gets or sets the created by identifier            
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.NonVersionedEntityData} copyData Copy constructor (if present)
 */
NonVersionedEntityData : function(copyData) { 
	this.$type = 'NonVersionedEntityData';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.updatedBy = copyData.updatedBy;
	this.modifiedOn = copyData.modifiedOn;
	this.updatedTime = copyData.updatedTime;
	}
},  // NonVersionedEntityData 
// OpenIZ.Core.Model.VersionedAssociation`1, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @param {OpenIZModel.VersionedAssociation} copyData Copy constructor (if present)
 */
VersionedAssociation : function(copyData) { 
	this.$type = 'VersionedAssociation';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	}
},  // VersionedAssociation 
// OpenIZ.Core.Model.VersionedEntityData`1, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @abstract
 * @extends OpenIZModel.BaseEntityData
 * @summary             Represents versioned based data, that is base data which has versions            
 * @property {uuid} previousVersion            Gets or sets the previous version key            
 * @property {uuid} version            Gets or sets the key which represents the version of the entity            
 * @property {number} sequence            The sequence number of the version (for ordering)            
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.VersionedEntityData} copyData Copy constructor (if present)
 */
VersionedEntityData : function(copyData) { 
	this.$type = 'VersionedEntityData';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	}
},  // VersionedEntityData 
// OpenIZ.Core.Model.Security.SecurityApplication, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.SecurityEntity
 * @summary             Represents a security application            
 * @property {string} applicationSecret            Gets or sets the application secret used for authenticating the application            
 * @property {string} name            Gets or sets the name of the security device/user/role/device.            
 * @property {string} updatedTime            Gets or sets the creation time in XML format            
 * @property {date} modifiedOn            Gets the time this item was modified            
 * @property {uuid} updatedBy            Gets or sets the created by identifier            
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.SecurityApplication} copyData Copy constructor (if present)
 */
SecurityApplication : function(copyData) { 
	this.$type = 'SecurityApplication';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.updatedBy = copyData.updatedBy;
	this.modifiedOn = copyData.modifiedOn;
	this.updatedTime = copyData.updatedTime;
	this.name = copyData.name;
	this.applicationSecret = copyData.applicationSecret;
	}
},  // SecurityApplication 
// OpenIZ.Core.Model.Security.SecurityDevice, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.SecurityEntity
 * @summary             Represents a security device            
 * @property {string} deviceSecret            Gets or sets the device secret            
 * @property {string} name            Gets or sets the name of the security device/user/role/devie            
 * @property {string} updatedTime            Gets or sets the creation time in XML format            
 * @property {date} modifiedOn            Gets the time this item was modified            
 * @property {uuid} updatedBy            Gets or sets the created by identifier            
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.SecurityDevice} copyData Copy constructor (if present)
 */
SecurityDevice : function(copyData) { 
	this.$type = 'SecurityDevice';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.updatedBy = copyData.updatedBy;
	this.modifiedOn = copyData.modifiedOn;
	this.updatedTime = copyData.updatedTime;
	this.name = copyData.name;
	this.deviceSecret = copyData.deviceSecret;
	}
},  // SecurityDevice 
// OpenIZ.Core.Model.Security.SecurityEntity, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.NonVersionedEntityData
 * @summary             Security Entity base class            
 * @property {string} updatedTime            Gets or sets the creation time in XML format            
 * @property {date} modifiedOn            Gets the time this item was modified            
 * @property {uuid} updatedBy            Gets or sets the created by identifier            
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.SecurityEntity} copyData Copy constructor (if present)
 */
SecurityEntity : function(copyData) { 
	this.$type = 'SecurityEntity';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.updatedBy = copyData.updatedBy;
	this.modifiedOn = copyData.modifiedOn;
	this.updatedTime = copyData.updatedTime;
	}
},  // SecurityEntity 
// OpenIZ.Core.Model.Security.SecurityPolicy, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.SecurityPolicy} copyData Copy constructor (if present)
 */
SecurityPolicy : function(copyData) { 
	this.$type = 'SecurityPolicy';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.canOverride = copyData.canOverride;
	this.isPublic = copyData.isPublic;
	this.oid = copyData.oid;
	this.name = copyData.name;
	this.handler = copyData.handler;
	}
},  // SecurityPolicy 
// OpenIZ.Core.Model.Security.SecurityPolicyInstance, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Association
 * @summary             Represents a security policy instance            
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.SecurityEntity} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.SecurityPolicyInstance} copyData Copy constructor (if present)
 */
SecurityPolicyInstance : function(copyData) { 
	this.$type = 'SecurityPolicyInstance';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	}
},  // SecurityPolicyInstance 
// OpenIZ.Core.Model.Security.SecurityRole, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.SecurityEntity
 * @summary             Security role            
 * @property {string} name            Gets or sets the name of the security role            
 * @property {string} description            Description of the role            
 * @property {string} updatedTime            Gets or sets the creation time in XML format            
 * @property {date} modifiedOn            Gets the time this item was modified            
 * @property {uuid} updatedBy            Gets or sets the created by identifier            
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.SecurityRole} copyData Copy constructor (if present)
 */
SecurityRole : function(copyData) { 
	this.$type = 'SecurityRole';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.updatedBy = copyData.updatedBy;
	this.modifiedOn = copyData.modifiedOn;
	this.updatedTime = copyData.updatedTime;
	this.description = copyData.description;
	this.name = copyData.name;
	}
},  // SecurityRole 
// OpenIZ.Core.Model.Security.SecurityUser, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.SecurityEntity
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
 * @property {uuid} userClass            Gets or sets the user class key            (see: {@link OpenIZModel.UserClassKeys} for values)
 * @property {string} updatedTime            Gets or sets the creation time in XML format            
 * @property {date} modifiedOn            Gets the time this item was modified            
 * @property {uuid} updatedBy            Gets or sets the created by identifier            
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.SecurityUser} copyData Copy constructor (if present)
 */
SecurityUser : function(copyData) { 
	this.$type = 'SecurityUser';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.updatedBy = copyData.updatedBy;
	this.modifiedOn = copyData.modifiedOn;
	this.updatedTime = copyData.updatedTime;
	this.userClass = copyData.userClass;
	this.phoneNumberConfirmed = copyData.phoneNumberConfirmed;
	this.phoneNumber = copyData.phoneNumber;
	this.lastLoginTime = copyData.lastLoginTime;
	this.photo = copyData.photo;
	this.userName = copyData.userName;
	this.twoFactorEnabled = copyData.twoFactorEnabled;
	this.securityStamp = copyData.securityStamp;
	this.passwordHash = copyData.passwordHash;
	this.lockout = copyData.lockout;
	this.invalidLoginAttempts = copyData.invalidLoginAttempts;
	this.emailConfirmed = copyData.emailConfirmed;
	this.email = copyData.email;
	}
},  // SecurityUser 
// OpenIZ.Core.Model.Roles.Patient, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Person
 * @summary             Represents an entity which is a patient            
 * @property {string} deceasedDate            Deceased date XML            
 * @property {OpenIZModel.DatePrecision} deceasedDatePrecision            Gets or sets the precision of the date of deceased            (see: {@link OpenIZModel.DatePrecision} for values)
 * @property {number} multipleBirthOrder            Gets or sets the multiple birth order of the patient             
 * @property {uuid} genderConcept            Gets or sets the gender concept key            
 * @property {OpenIZModel.Concept} genderConceptModel [Delay loaded from genderConcept],             Gets or sets the gender concept            
 * @property {OpenIZModel.DatePrecision} dateOfBirthPrecision            Gets or sets the precision ofthe date of birth            (see: {@link OpenIZModel.DatePrecision} for values)
 * @property {string} dateOfBirth            Gets the date of birth as XML            
 * @property {OpenIZModel.PersonLanguageCommunication} language            Gets the person's languages of communication            
 * @property {object} address            Gets a list of all addresses associated with the entity            
 * @property {OpenIZModel.EntityAddress} address.Alphabetic             Represents an alphabetic address used for matching             
 * @property {OpenIZModel.EntityAddress} address.BadAddress             Represents a bad address, i.e. an address which is old or invalid.            
 * @property {OpenIZModel.EntityAddress} address.Direct             Represents a workplace address that reaches the person directly without intermediaries.            
 * @property {OpenIZModel.EntityAddress} address.HomeAddress             The home address            
 * @property {OpenIZModel.EntityAddress} address.Ideographic             Represents an address expressed in an ideographic manner (example: Kaji)            
 * @property {OpenIZModel.EntityAddress} address.Phonetic             Represents an address expressed as a phonetic spelling of an ideographic address            
 * @property {OpenIZModel.EntityAddress} address.PhysicalVisit             The address is a physical place where visits should occur            
 * @property {OpenIZModel.EntityAddress} address.PostalAddress             The address is a postal address used for the delivery of mail and materials            
 * @property {OpenIZModel.EntityAddress} address.PrimaryHome             Represents a primary address to reach a contact after business hours.            
 * @property {OpenIZModel.EntityAddress} address.Public             Represents an address that is a standard address that may be subject to a switchboard or operator prior to reaching the intended entity.            
 * @property {OpenIZModel.EntityAddress} address.Soundex             Represents an address used for soundex matching purposes.            
 * @property {OpenIZModel.EntityAddress} address.Syllabic             Represents a syllabic address.            
 * @property {OpenIZModel.EntityAddress} address.TemporaryAddress             Represents a temporary address that may be good for visiting or mailing.            
 * @property {OpenIZModel.EntityAddress} address.VacationHome             Represents a vacation home to reach a person while on vacation.            
 * @property {OpenIZModel.EntityAddress} address.WorkPlace             Represents an office address, should be used for business communications            
 * @property {OpenIZModel.EntityAddress} address.$other Unclassified
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.EntityClassKeys} for values)
 * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
 * @property {uuid} creationAct            Creation act reference            
 * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
 * @property {uuid} determinerConcept            Determiner concept            (see: {@link OpenIZModel.DeterminerKeys} for values)
 * @property {object} extension            Gets a list of all extensions associated with the entity            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {object} identifier            Gets the identifiers associated with this entity            
 * @property {OpenIZModel.EntityIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} name            Gets a list of all names associated with the entity            
 * @property {OpenIZModel.EntityName} name.Alphabetic             The name used is an alphabetic representation of the name (ex: romaji in Japanese)            
 * @property {OpenIZModel.EntityName} name.Anonymous             The name is an anonymous name for the object (not the real name but a name used for care delivery)            
 * @property {OpenIZModel.EntityName} name.Artist             The name represents an artist name or stage name            
 * @property {OpenIZModel.EntityName} name.Assigned             The name represents an assigned name (given or bestowed by an authority)            
 * @property {OpenIZModel.EntityName} name.Ideographic             THe name represents an ideographic representation of the name            
 * @property {OpenIZModel.EntityName} name.Indigenous             The name is an indigenous name or tribal name for the patient            
 * @property {OpenIZModel.EntityName} name.Legal             The name represents the current legal name of an object (such as a corporate name)            
 * @property {OpenIZModel.EntityName} name.License             The name represents a name as displayed on a license or known to a license authority            
 * @property {OpenIZModel.EntityName} name.MaidenName             THe name is a maiden name (name of a patient before marriage)            
 * @property {OpenIZModel.EntityName} name.OfficialRecord             The name as it appears on an official record            
 * @property {OpenIZModel.EntityName} name.Phonetic             The name represents a phonetic representation of a name such as a SOUNDEX code            
 * @property {OpenIZModel.EntityName} name.Pseudonym             The name is a pseudonym for the object or an synonym name            
 * @property {OpenIZModel.EntityName} name.Religious             The name is to be used for religious purposes (such as baptismal name)            
 * @property {OpenIZModel.EntityName} name.Search             The name is to be used in the performing of matches only            
 * @property {OpenIZModel.EntityName} name.Soundex             The name represents the computed soundex code of a name            
 * @property {OpenIZModel.EntityName} name.Syllabic             The name represents a syllabic name.            
 * @property {OpenIZModel.EntityName} name.$other Unclassified
 * @property {string} note            Gets a list of all notes associated with the entity            
 * @property {object} participation            Gets the acts in which this entity participates            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {object} relationship            Gets a list of all associated entities for this entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Access             The access            
 * @property {OpenIZModel.EntityRelationship} relationship.ActiveMoiety             The active moiety            
 * @property {OpenIZModel.EntityRelationship} relationship.AdministerableMaterial             The administerable material            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedChild             The adopted child            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedDaughter             The adopted daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedSon             The adopted son            
 * @property {OpenIZModel.EntityRelationship} relationship.Affiliate             The affiliate            
 * @property {OpenIZModel.EntityRelationship} relationship.Agent             The agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Aliquot             The aliquot            
 * @property {OpenIZModel.EntityRelationship} relationship.Assigned             The assigned            
 * @property {OpenIZModel.EntityRelationship} relationship.AssignedEntity             The assigned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Aunt             The aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.Birthplace             The birthplace            
 * @property {OpenIZModel.EntityRelationship} relationship.Brother             The brother            
 * @property {OpenIZModel.EntityRelationship} relationship.Brotherinlaw             The brotherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Caregiver             The caregiver            
 * @property {OpenIZModel.EntityRelationship} relationship.CaseSubject             The case subject            
 * @property {OpenIZModel.EntityRelationship} relationship.Child             The child            
 * @property {OpenIZModel.EntityRelationship} relationship.ChildInlaw             The child inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Citizen             The citizen            
 * @property {OpenIZModel.EntityRelationship} relationship.Claimant             The claimant            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchInvestigator             The clinical research investigator            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchSponsor             The clinical research sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CommissioningParty             The commissioning party            
 * @property {OpenIZModel.EntityRelationship} relationship.Contact             The contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Cousin             The cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.CoverageSponsor             The coverage sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CoveredParty             The covered party            
 * @property {OpenIZModel.EntityRelationship} relationship.Daughter             The daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.DaughterInlaw             The daughter inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.DedicatedServiceDeliveryLocation             The dedicated service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Dependent             The dependent            
 * @property {OpenIZModel.EntityRelationship} relationship.DistributedMaterial             The distributed material            
 * @property {OpenIZModel.EntityRelationship} relationship.DomesticPartner             The domestic partner            
 * @property {OpenIZModel.EntityRelationship} relationship.EmergencyContact             The emergency contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Employee             The employee            
 * @property {OpenIZModel.EntityRelationship} relationship.ExposedEntity             The exposed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.FamilyMember             The family member            
 * @property {OpenIZModel.EntityRelationship} relationship.Father             The father            
 * @property {OpenIZModel.EntityRelationship} relationship.Fatherinlaw             The fatherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterChild             The foster child            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterDaughter             The foster daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterSon             The foster son            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandchild             The grandchild            
 * @property {OpenIZModel.EntityRelationship} relationship.Granddaughter             The granddaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandfather             The grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandmother             The grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandparent             The grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandson             The grandson            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandfather             The great grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandmother             The great grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandparent             The great grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Guarantor             The guarantor            
 * @property {OpenIZModel.EntityRelationship} relationship.GUARD             The guard            
 * @property {OpenIZModel.EntityRelationship} relationship.Guardian             The guardian            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfbrother             The halfbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsibling             The halfsibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsister             The halfsister            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthcareProvider             The healthcare provider            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthChart             The health chart            
 * @property {OpenIZModel.EntityRelationship} relationship.HeldEntity             The held entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Husband             The husband            
 * @property {OpenIZModel.EntityRelationship} relationship.IdentifiedEntity             The identified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.IncidentalServiceDeliveryLocation             The incidental service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Individual             The individual            
 * @property {OpenIZModel.EntityRelationship} relationship.InvestigationSubject             The investigation subject            
 * @property {OpenIZModel.EntityRelationship} relationship.InvoicePayor             The invoice payor            
 * @property {OpenIZModel.EntityRelationship} relationship.Isolate             The isolate            
 * @property {OpenIZModel.EntityRelationship} relationship.LicensedEntity             The licensed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.MaintainedEntity             The maintained entity            
 * @property {OpenIZModel.EntityRelationship} relationship.ManufacturedProduct             The manufactured product            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalAunt             The maternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalCousin             The maternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandfather             The maternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandmother             The maternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandparent             The maternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandfather             The maternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandmother             The maternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandparent             The maternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalUncle             The maternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.MilitaryPerson             The military person            
 * @property {OpenIZModel.EntityRelationship} relationship.Mother             The mother            
 * @property {OpenIZModel.EntityRelationship} relationship.Motherinlaw             The motherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.NamedInsured             The named insured            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalBrother             The natural brother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalChild             The natural child            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalDaughter             The natural daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFather             The natural father            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFatherOfFetus             The natural father of fetus            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalMother             The natural mother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalParent             The natural parent            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSibling             The natural sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSister             The natural sister            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSon             The natural son            
 * @property {OpenIZModel.EntityRelationship} relationship.Nephew             The nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NextOfKin             The next of kin            
 * @property {OpenIZModel.EntityRelationship} relationship.Niece             The niece            
 * @property {OpenIZModel.EntityRelationship} relationship.NieceNephew             The niece nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NotaryPublic             The notary public            
 * @property {OpenIZModel.EntityRelationship} relationship.OwnedEntity             The owned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Parent             The parent            
 * @property {OpenIZModel.EntityRelationship} relationship.ParentInlaw             The parent inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Part             The part            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalAunt             The paternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalCousin             The paternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandfather             The paternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandmother             The paternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandparent             The paternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandfather             The paternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandmother             The paternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandparent             The paternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalUncle             The paternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Patient             The patient            
 * @property {OpenIZModel.EntityRelationship} relationship.Payee             The payee            
 * @property {OpenIZModel.EntityRelationship} relationship.PersonalRelationship             The personal relationship            
 * @property {OpenIZModel.EntityRelationship} relationship.PlaceOfDeath             The place of death            
 * @property {OpenIZModel.EntityRelationship} relationship.PolicyHolder             The policy holder            
 * @property {OpenIZModel.EntityRelationship} relationship.ProgramEligible             The program eligible            
 * @property {OpenIZModel.EntityRelationship} relationship.QualifiedEntity             The qualified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.RegulatedProduct             The regulated product            
 * @property {OpenIZModel.EntityRelationship} relationship.ResearchSubject             The research subject            
 * @property {OpenIZModel.EntityRelationship} relationship.RetailedMaterial             The retailed material            
 * @property {OpenIZModel.EntityRelationship} relationship.Roomate             The roomate            
 * @property {OpenIZModel.EntityRelationship} relationship.ServiceDeliveryLocation             The service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Sibling             The sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.SiblingInlaw             The sibling inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.SignificantOther             The significant other            
 * @property {OpenIZModel.EntityRelationship} relationship.SigningAuthorityOrOfficer             The signing authority or officer            
 * @property {OpenIZModel.EntityRelationship} relationship.Sister             The sister            
 * @property {OpenIZModel.EntityRelationship} relationship.Sisterinlaw             The sisterinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Son             The son            
 * @property {OpenIZModel.EntityRelationship} relationship.SonInlaw             The son inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Specimen             The specimen            
 * @property {OpenIZModel.EntityRelationship} relationship.Spouse             The spouse            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepbrother             The stepbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepChild             The step child            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepdaughter             The stepdaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepfather             The stepfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepmother             The stepmother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepParent             The step parent            
 * @property {OpenIZModel.EntityRelationship} relationship.StepSibling             The step sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepsister             The stepsister            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepson             The stepson            
 * @property {OpenIZModel.EntityRelationship} relationship.Student             The student            
 * @property {OpenIZModel.EntityRelationship} relationship.Subscriber             The subscriber            
 * @property {OpenIZModel.EntityRelationship} relationship.TerritoryOfAuthority             The territory of authority            
 * @property {OpenIZModel.EntityRelationship} relationship.TherapeuticAgent             The therapeutic agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Uncle             The uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Underwriter             The underwriter            
 * @property {OpenIZModel.EntityRelationship} relationship.UsedEntity             The used entity            
 * @property {OpenIZModel.EntityRelationship} relationship.WarrantedProduct             The warranted product            
 * @property {OpenIZModel.EntityRelationship} relationship.Wife             The wife            
 * @property {OpenIZModel.EntityRelationship} relationship.Replaces             The replaces            
 * @property {OpenIZModel.EntityRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {object} tag            Gets a list of all tags associated with the entity            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {object} telecom            Gets a list of all telecommunications addresses associated with the entity            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.AnsweringService             answering service            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.EmergencyContact             Emergency contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.MobileContact             Mobile phone contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Pager             pager            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Public             public (800 number example) contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.TemporaryAddress             temporary contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.WorkPlace             For use in the workplace            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.$other Unclassified
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Patient} copyData Copy constructor (if present)
 */
Patient : function(copyData) { 
	this.$type = 'Patient';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.typeConcept = copyData.typeConcept;
	this.typeConceptModel = copyData.typeConceptModel;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.telecom = copyData.telecom;
	this.tag = copyData.tag;
	this.statusConcept = copyData.statusConcept;
	this.statusConceptModel = copyData.statusConceptModel;
	this.relationship = copyData.relationship;
	this.participation = copyData.participation;
	this.note = copyData.note;
	this.name = copyData.name;
	this.identifier = copyData.identifier;
	this.extension = copyData.extension;
	this.determinerConcept = copyData.determinerConcept;
	this.determinerConceptModel = copyData.determinerConceptModel;
	this.creationAct = copyData.creationAct;
	this.creationActModel = copyData.creationActModel;
	this.classConcept = copyData.classConcept;
	this.classConceptModel = copyData.classConceptModel;
	this.address = copyData.address;
	this.language = copyData.language;
	this.dateOfBirth = copyData.dateOfBirth;
	this.dateOfBirthPrecision = copyData.dateOfBirthPrecision;
	this.genderConceptModel = copyData.genderConceptModel;
	this.genderConcept = copyData.genderConcept;
	this.multipleBirthOrder = copyData.multipleBirthOrder;
	this.deceasedDatePrecision = copyData.deceasedDatePrecision;
	this.deceasedDate = copyData.deceasedDate;
	}
},  // Patient 
// OpenIZ.Core.Model.Roles.Provider, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Person
 * @summary             Represents a provider role of a person            
 * @property {uuid} providerSpecialty            Gets or sets the provider specialty key            
 * @property {OpenIZModel.Concept} providerSpecialtyModel [Delay loaded from providerSpecialty],             Gets or sets the provider specialty            
 * @property {OpenIZModel.DatePrecision} dateOfBirthPrecision            Gets or sets the precision ofthe date of birth            (see: {@link OpenIZModel.DatePrecision} for values)
 * @property {string} dateOfBirth            Gets the date of birth as XML            
 * @property {OpenIZModel.PersonLanguageCommunication} language            Gets the person's languages of communication            
 * @property {object} address            Gets a list of all addresses associated with the entity            
 * @property {OpenIZModel.EntityAddress} address.Alphabetic             Represents an alphabetic address used for matching             
 * @property {OpenIZModel.EntityAddress} address.BadAddress             Represents a bad address, i.e. an address which is old or invalid.            
 * @property {OpenIZModel.EntityAddress} address.Direct             Represents a workplace address that reaches the person directly without intermediaries.            
 * @property {OpenIZModel.EntityAddress} address.HomeAddress             The home address            
 * @property {OpenIZModel.EntityAddress} address.Ideographic             Represents an address expressed in an ideographic manner (example: Kaji)            
 * @property {OpenIZModel.EntityAddress} address.Phonetic             Represents an address expressed as a phonetic spelling of an ideographic address            
 * @property {OpenIZModel.EntityAddress} address.PhysicalVisit             The address is a physical place where visits should occur            
 * @property {OpenIZModel.EntityAddress} address.PostalAddress             The address is a postal address used for the delivery of mail and materials            
 * @property {OpenIZModel.EntityAddress} address.PrimaryHome             Represents a primary address to reach a contact after business hours.            
 * @property {OpenIZModel.EntityAddress} address.Public             Represents an address that is a standard address that may be subject to a switchboard or operator prior to reaching the intended entity.            
 * @property {OpenIZModel.EntityAddress} address.Soundex             Represents an address used for soundex matching purposes.            
 * @property {OpenIZModel.EntityAddress} address.Syllabic             Represents a syllabic address.            
 * @property {OpenIZModel.EntityAddress} address.TemporaryAddress             Represents a temporary address that may be good for visiting or mailing.            
 * @property {OpenIZModel.EntityAddress} address.VacationHome             Represents a vacation home to reach a person while on vacation.            
 * @property {OpenIZModel.EntityAddress} address.WorkPlace             Represents an office address, should be used for business communications            
 * @property {OpenIZModel.EntityAddress} address.$other Unclassified
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.EntityClassKeys} for values)
 * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
 * @property {uuid} creationAct            Creation act reference            
 * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
 * @property {uuid} determinerConcept            Determiner concept            (see: {@link OpenIZModel.DeterminerKeys} for values)
 * @property {object} extension            Gets a list of all extensions associated with the entity            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {object} identifier            Gets the identifiers associated with this entity            
 * @property {OpenIZModel.EntityIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} name            Gets a list of all names associated with the entity            
 * @property {OpenIZModel.EntityName} name.Alphabetic             The name used is an alphabetic representation of the name (ex: romaji in Japanese)            
 * @property {OpenIZModel.EntityName} name.Anonymous             The name is an anonymous name for the object (not the real name but a name used for care delivery)            
 * @property {OpenIZModel.EntityName} name.Artist             The name represents an artist name or stage name            
 * @property {OpenIZModel.EntityName} name.Assigned             The name represents an assigned name (given or bestowed by an authority)            
 * @property {OpenIZModel.EntityName} name.Ideographic             THe name represents an ideographic representation of the name            
 * @property {OpenIZModel.EntityName} name.Indigenous             The name is an indigenous name or tribal name for the patient            
 * @property {OpenIZModel.EntityName} name.Legal             The name represents the current legal name of an object (such as a corporate name)            
 * @property {OpenIZModel.EntityName} name.License             The name represents a name as displayed on a license or known to a license authority            
 * @property {OpenIZModel.EntityName} name.MaidenName             THe name is a maiden name (name of a patient before marriage)            
 * @property {OpenIZModel.EntityName} name.OfficialRecord             The name as it appears on an official record            
 * @property {OpenIZModel.EntityName} name.Phonetic             The name represents a phonetic representation of a name such as a SOUNDEX code            
 * @property {OpenIZModel.EntityName} name.Pseudonym             The name is a pseudonym for the object or an synonym name            
 * @property {OpenIZModel.EntityName} name.Religious             The name is to be used for religious purposes (such as baptismal name)            
 * @property {OpenIZModel.EntityName} name.Search             The name is to be used in the performing of matches only            
 * @property {OpenIZModel.EntityName} name.Soundex             The name represents the computed soundex code of a name            
 * @property {OpenIZModel.EntityName} name.Syllabic             The name represents a syllabic name.            
 * @property {OpenIZModel.EntityName} name.$other Unclassified
 * @property {string} note            Gets a list of all notes associated with the entity            
 * @property {object} participation            Gets the acts in which this entity participates            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {object} relationship            Gets a list of all associated entities for this entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Access             The access            
 * @property {OpenIZModel.EntityRelationship} relationship.ActiveMoiety             The active moiety            
 * @property {OpenIZModel.EntityRelationship} relationship.AdministerableMaterial             The administerable material            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedChild             The adopted child            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedDaughter             The adopted daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedSon             The adopted son            
 * @property {OpenIZModel.EntityRelationship} relationship.Affiliate             The affiliate            
 * @property {OpenIZModel.EntityRelationship} relationship.Agent             The agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Aliquot             The aliquot            
 * @property {OpenIZModel.EntityRelationship} relationship.Assigned             The assigned            
 * @property {OpenIZModel.EntityRelationship} relationship.AssignedEntity             The assigned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Aunt             The aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.Birthplace             The birthplace            
 * @property {OpenIZModel.EntityRelationship} relationship.Brother             The brother            
 * @property {OpenIZModel.EntityRelationship} relationship.Brotherinlaw             The brotherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Caregiver             The caregiver            
 * @property {OpenIZModel.EntityRelationship} relationship.CaseSubject             The case subject            
 * @property {OpenIZModel.EntityRelationship} relationship.Child             The child            
 * @property {OpenIZModel.EntityRelationship} relationship.ChildInlaw             The child inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Citizen             The citizen            
 * @property {OpenIZModel.EntityRelationship} relationship.Claimant             The claimant            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchInvestigator             The clinical research investigator            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchSponsor             The clinical research sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CommissioningParty             The commissioning party            
 * @property {OpenIZModel.EntityRelationship} relationship.Contact             The contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Cousin             The cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.CoverageSponsor             The coverage sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CoveredParty             The covered party            
 * @property {OpenIZModel.EntityRelationship} relationship.Daughter             The daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.DaughterInlaw             The daughter inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.DedicatedServiceDeliveryLocation             The dedicated service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Dependent             The dependent            
 * @property {OpenIZModel.EntityRelationship} relationship.DistributedMaterial             The distributed material            
 * @property {OpenIZModel.EntityRelationship} relationship.DomesticPartner             The domestic partner            
 * @property {OpenIZModel.EntityRelationship} relationship.EmergencyContact             The emergency contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Employee             The employee            
 * @property {OpenIZModel.EntityRelationship} relationship.ExposedEntity             The exposed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.FamilyMember             The family member            
 * @property {OpenIZModel.EntityRelationship} relationship.Father             The father            
 * @property {OpenIZModel.EntityRelationship} relationship.Fatherinlaw             The fatherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterChild             The foster child            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterDaughter             The foster daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterSon             The foster son            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandchild             The grandchild            
 * @property {OpenIZModel.EntityRelationship} relationship.Granddaughter             The granddaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandfather             The grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandmother             The grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandparent             The grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandson             The grandson            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandfather             The great grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandmother             The great grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandparent             The great grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Guarantor             The guarantor            
 * @property {OpenIZModel.EntityRelationship} relationship.GUARD             The guard            
 * @property {OpenIZModel.EntityRelationship} relationship.Guardian             The guardian            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfbrother             The halfbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsibling             The halfsibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsister             The halfsister            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthcareProvider             The healthcare provider            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthChart             The health chart            
 * @property {OpenIZModel.EntityRelationship} relationship.HeldEntity             The held entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Husband             The husband            
 * @property {OpenIZModel.EntityRelationship} relationship.IdentifiedEntity             The identified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.IncidentalServiceDeliveryLocation             The incidental service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Individual             The individual            
 * @property {OpenIZModel.EntityRelationship} relationship.InvestigationSubject             The investigation subject            
 * @property {OpenIZModel.EntityRelationship} relationship.InvoicePayor             The invoice payor            
 * @property {OpenIZModel.EntityRelationship} relationship.Isolate             The isolate            
 * @property {OpenIZModel.EntityRelationship} relationship.LicensedEntity             The licensed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.MaintainedEntity             The maintained entity            
 * @property {OpenIZModel.EntityRelationship} relationship.ManufacturedProduct             The manufactured product            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalAunt             The maternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalCousin             The maternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandfather             The maternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandmother             The maternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandparent             The maternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandfather             The maternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandmother             The maternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandparent             The maternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalUncle             The maternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.MilitaryPerson             The military person            
 * @property {OpenIZModel.EntityRelationship} relationship.Mother             The mother            
 * @property {OpenIZModel.EntityRelationship} relationship.Motherinlaw             The motherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.NamedInsured             The named insured            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalBrother             The natural brother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalChild             The natural child            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalDaughter             The natural daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFather             The natural father            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFatherOfFetus             The natural father of fetus            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalMother             The natural mother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalParent             The natural parent            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSibling             The natural sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSister             The natural sister            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSon             The natural son            
 * @property {OpenIZModel.EntityRelationship} relationship.Nephew             The nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NextOfKin             The next of kin            
 * @property {OpenIZModel.EntityRelationship} relationship.Niece             The niece            
 * @property {OpenIZModel.EntityRelationship} relationship.NieceNephew             The niece nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NotaryPublic             The notary public            
 * @property {OpenIZModel.EntityRelationship} relationship.OwnedEntity             The owned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Parent             The parent            
 * @property {OpenIZModel.EntityRelationship} relationship.ParentInlaw             The parent inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Part             The part            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalAunt             The paternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalCousin             The paternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandfather             The paternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandmother             The paternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandparent             The paternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandfather             The paternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandmother             The paternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandparent             The paternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalUncle             The paternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Patient             The patient            
 * @property {OpenIZModel.EntityRelationship} relationship.Payee             The payee            
 * @property {OpenIZModel.EntityRelationship} relationship.PersonalRelationship             The personal relationship            
 * @property {OpenIZModel.EntityRelationship} relationship.PlaceOfDeath             The place of death            
 * @property {OpenIZModel.EntityRelationship} relationship.PolicyHolder             The policy holder            
 * @property {OpenIZModel.EntityRelationship} relationship.ProgramEligible             The program eligible            
 * @property {OpenIZModel.EntityRelationship} relationship.QualifiedEntity             The qualified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.RegulatedProduct             The regulated product            
 * @property {OpenIZModel.EntityRelationship} relationship.ResearchSubject             The research subject            
 * @property {OpenIZModel.EntityRelationship} relationship.RetailedMaterial             The retailed material            
 * @property {OpenIZModel.EntityRelationship} relationship.Roomate             The roomate            
 * @property {OpenIZModel.EntityRelationship} relationship.ServiceDeliveryLocation             The service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Sibling             The sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.SiblingInlaw             The sibling inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.SignificantOther             The significant other            
 * @property {OpenIZModel.EntityRelationship} relationship.SigningAuthorityOrOfficer             The signing authority or officer            
 * @property {OpenIZModel.EntityRelationship} relationship.Sister             The sister            
 * @property {OpenIZModel.EntityRelationship} relationship.Sisterinlaw             The sisterinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Son             The son            
 * @property {OpenIZModel.EntityRelationship} relationship.SonInlaw             The son inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Specimen             The specimen            
 * @property {OpenIZModel.EntityRelationship} relationship.Spouse             The spouse            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepbrother             The stepbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepChild             The step child            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepdaughter             The stepdaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepfather             The stepfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepmother             The stepmother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepParent             The step parent            
 * @property {OpenIZModel.EntityRelationship} relationship.StepSibling             The step sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepsister             The stepsister            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepson             The stepson            
 * @property {OpenIZModel.EntityRelationship} relationship.Student             The student            
 * @property {OpenIZModel.EntityRelationship} relationship.Subscriber             The subscriber            
 * @property {OpenIZModel.EntityRelationship} relationship.TerritoryOfAuthority             The territory of authority            
 * @property {OpenIZModel.EntityRelationship} relationship.TherapeuticAgent             The therapeutic agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Uncle             The uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Underwriter             The underwriter            
 * @property {OpenIZModel.EntityRelationship} relationship.UsedEntity             The used entity            
 * @property {OpenIZModel.EntityRelationship} relationship.WarrantedProduct             The warranted product            
 * @property {OpenIZModel.EntityRelationship} relationship.Wife             The wife            
 * @property {OpenIZModel.EntityRelationship} relationship.Replaces             The replaces            
 * @property {OpenIZModel.EntityRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {object} tag            Gets a list of all tags associated with the entity            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {object} telecom            Gets a list of all telecommunications addresses associated with the entity            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.AnsweringService             answering service            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.EmergencyContact             Emergency contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.MobileContact             Mobile phone contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Pager             pager            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Public             public (800 number example) contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.TemporaryAddress             temporary contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.WorkPlace             For use in the workplace            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.$other Unclassified
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Provider} copyData Copy constructor (if present)
 */
Provider : function(copyData) { 
	this.$type = 'Provider';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.typeConcept = copyData.typeConcept;
	this.typeConceptModel = copyData.typeConceptModel;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.telecom = copyData.telecom;
	this.tag = copyData.tag;
	this.statusConcept = copyData.statusConcept;
	this.statusConceptModel = copyData.statusConceptModel;
	this.relationship = copyData.relationship;
	this.participation = copyData.participation;
	this.note = copyData.note;
	this.name = copyData.name;
	this.identifier = copyData.identifier;
	this.extension = copyData.extension;
	this.determinerConcept = copyData.determinerConcept;
	this.determinerConceptModel = copyData.determinerConceptModel;
	this.creationAct = copyData.creationAct;
	this.creationActModel = copyData.creationActModel;
	this.classConcept = copyData.classConcept;
	this.classConceptModel = copyData.classConceptModel;
	this.address = copyData.address;
	this.language = copyData.language;
	this.dateOfBirth = copyData.dateOfBirth;
	this.dateOfBirthPrecision = copyData.dateOfBirthPrecision;
	this.providerSpecialtyModel = copyData.providerSpecialtyModel;
	this.providerSpecialty = copyData.providerSpecialty;
	}
},  // Provider 
// OpenIZ.Core.Model.Patch.Patch, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.BaseEntityData
 * @summary             Represents a series of patch instructions             
 * @property {OpenIZModel.PatchTarget} appliesTo            Application version            
 * @property {OpenIZModel.PatchOperation} change            A list of patch operations to be applied to the object            
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Patch} copyData Copy constructor (if present)
 */
Patch : function(copyData) { 
	this.$type = 'Patch';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.change = copyData.change;
	this.appliesTo = copyData.appliesTo;
	}
},  // Patch 
// OpenIZ.Core.Model.Patch.PatchOperation, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @summary             Represents a single patch operation            
 * @property {OpenIZModel.PatchOperationType} op            Gets or sets the operation type            (see: {@link OpenIZModel.PatchOperationType} for values)
 * @property {string} path            Gets or sets the path            
 * @property {Object} value            Get or sets the value            
 * @param {OpenIZModel.PatchOperation} copyData Copy constructor (if present)
 */
PatchOperation : function(copyData) { 
	this.$type = 'PatchOperation';
	if(copyData) {
	this.value = copyData.value;
	this.path = copyData.path;
	this.op = copyData.op;
	}
},  // PatchOperation 
// OpenIZ.Core.Model.Patch.PatchTarget, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @summary             Represents a target of a patch            
 * @property {string} type            Identifies the target type            
 * @property {uuid} id            Gets or sets the key            
 * @property {uuid} version            Gets or sets the key            
 * @property {string} etag            Gets or sets the tag of the item            
 * @param {OpenIZModel.PatchTarget} copyData Copy constructor (if present)
 */
PatchTarget : function(copyData) { 
	this.$type = 'PatchTarget';
	if(copyData) {
	this.etag = copyData.etag;
	this.version = copyData.version;
	this.id = copyData.id;
	this.type = copyData.type;
	}
},  // PatchTarget 
// OpenIZ.Core.Model.Entities.Container, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.ManufacturedMaterial
 * @summary             Represents a container.            
 * @property {number} barrierDeltaQuantity            Gets or sets the barrier delta quantity. The distance from the Point of Reference to the separator material (barrier) within a container.            
 * @property {number} bottomDeltaQuantity            Gets or sets the bottom delta quantity. The distance from the Point of Reference to the outside bottom of the container.            
 * @property {number} capacityQuantity            Gets or sets the capacity quantity. The functional capacity of the container.            
 * @property {OpenIZModel.Concept} capTypeConceptModel [Delay loaded from capTypeConcept],             Gets or sets the cap type concept. The type of container cap consistent with de-capping, piercing or other automated manipulation.            
 * @property {uuid} capTypeConcept            Gets or sets the cap type concept key.            
 * @property {number} diameterQuantity            Gets or sets the diameter quantity. The outside diameter of the container.            
 * @property {number} heightQuantity            Gets or sets the height quantity. The height of the container.            
 * @property {OpenIZModel.Concept} separatorTypeConceptModel [Delay loaded from separatorTypeConcept],             Gets or sets the separator type concept. A material added to a container to facilitate and create a physical separation of specimen components of differing density.            Examples: A gel material added to blood collection tubes that following centrifugation creates a physical barrier between the blood cells and the serum or plasma.            
 * @property {uuid} separatorTypeConcept            Gets or sets the separator type concept key.            
 * @property {string} lotNumber            Gets or sets the lot number of the manufactured material            
 * @property {date} expiryDate            Gets or sets the expiry date of the material            
 * @property {OpenIZModel.Concept} formConceptModel [Delay loaded from formConcept],             Gets or sets the concept which dictates the form of the material (solid, liquid, capsule, injection, etc.)            
 * @property {uuid} formConcept            Gets or sets the form concept's key            
 * @property {bool} isAdministrative            True if the material is simply administrative            
 * @property {number} quantity            The base quantity of the object in the units. This differs from quantity on the relationship            which is a /per ...            
 * @property {OpenIZModel.Concept} quantityConceptModel [Delay loaded from quantityConcept],             Gets or sets the concept which dictates the unit of measure for a single instance of this entity            
 * @property {uuid} quantityConcept            Gets or sets the quantity concept ref            
 * @property {object} address            Gets a list of all addresses associated with the entity            
 * @property {OpenIZModel.EntityAddress} address.Alphabetic             Represents an alphabetic address used for matching             
 * @property {OpenIZModel.EntityAddress} address.BadAddress             Represents a bad address, i.e. an address which is old or invalid.            
 * @property {OpenIZModel.EntityAddress} address.Direct             Represents a workplace address that reaches the person directly without intermediaries.            
 * @property {OpenIZModel.EntityAddress} address.HomeAddress             The home address            
 * @property {OpenIZModel.EntityAddress} address.Ideographic             Represents an address expressed in an ideographic manner (example: Kaji)            
 * @property {OpenIZModel.EntityAddress} address.Phonetic             Represents an address expressed as a phonetic spelling of an ideographic address            
 * @property {OpenIZModel.EntityAddress} address.PhysicalVisit             The address is a physical place where visits should occur            
 * @property {OpenIZModel.EntityAddress} address.PostalAddress             The address is a postal address used for the delivery of mail and materials            
 * @property {OpenIZModel.EntityAddress} address.PrimaryHome             Represents a primary address to reach a contact after business hours.            
 * @property {OpenIZModel.EntityAddress} address.Public             Represents an address that is a standard address that may be subject to a switchboard or operator prior to reaching the intended entity.            
 * @property {OpenIZModel.EntityAddress} address.Soundex             Represents an address used for soundex matching purposes.            
 * @property {OpenIZModel.EntityAddress} address.Syllabic             Represents a syllabic address.            
 * @property {OpenIZModel.EntityAddress} address.TemporaryAddress             Represents a temporary address that may be good for visiting or mailing.            
 * @property {OpenIZModel.EntityAddress} address.VacationHome             Represents a vacation home to reach a person while on vacation.            
 * @property {OpenIZModel.EntityAddress} address.WorkPlace             Represents an office address, should be used for business communications            
 * @property {OpenIZModel.EntityAddress} address.$other Unclassified
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.EntityClassKeys} for values)
 * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
 * @property {uuid} creationAct            Creation act reference            
 * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
 * @property {uuid} determinerConcept            Determiner concept            (see: {@link OpenIZModel.DeterminerKeys} for values)
 * @property {object} extension            Gets a list of all extensions associated with the entity            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {object} identifier            Gets the identifiers associated with this entity            
 * @property {OpenIZModel.EntityIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} name            Gets a list of all names associated with the entity            
 * @property {OpenIZModel.EntityName} name.Alphabetic             The name used is an alphabetic representation of the name (ex: romaji in Japanese)            
 * @property {OpenIZModel.EntityName} name.Anonymous             The name is an anonymous name for the object (not the real name but a name used for care delivery)            
 * @property {OpenIZModel.EntityName} name.Artist             The name represents an artist name or stage name            
 * @property {OpenIZModel.EntityName} name.Assigned             The name represents an assigned name (given or bestowed by an authority)            
 * @property {OpenIZModel.EntityName} name.Ideographic             THe name represents an ideographic representation of the name            
 * @property {OpenIZModel.EntityName} name.Indigenous             The name is an indigenous name or tribal name for the patient            
 * @property {OpenIZModel.EntityName} name.Legal             The name represents the current legal name of an object (such as a corporate name)            
 * @property {OpenIZModel.EntityName} name.License             The name represents a name as displayed on a license or known to a license authority            
 * @property {OpenIZModel.EntityName} name.MaidenName             THe name is a maiden name (name of a patient before marriage)            
 * @property {OpenIZModel.EntityName} name.OfficialRecord             The name as it appears on an official record            
 * @property {OpenIZModel.EntityName} name.Phonetic             The name represents a phonetic representation of a name such as a SOUNDEX code            
 * @property {OpenIZModel.EntityName} name.Pseudonym             The name is a pseudonym for the object or an synonym name            
 * @property {OpenIZModel.EntityName} name.Religious             The name is to be used for religious purposes (such as baptismal name)            
 * @property {OpenIZModel.EntityName} name.Search             The name is to be used in the performing of matches only            
 * @property {OpenIZModel.EntityName} name.Soundex             The name represents the computed soundex code of a name            
 * @property {OpenIZModel.EntityName} name.Syllabic             The name represents a syllabic name.            
 * @property {OpenIZModel.EntityName} name.$other Unclassified
 * @property {string} note            Gets a list of all notes associated with the entity            
 * @property {object} participation            Gets the acts in which this entity participates            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {object} relationship            Gets a list of all associated entities for this entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Access             The access            
 * @property {OpenIZModel.EntityRelationship} relationship.ActiveMoiety             The active moiety            
 * @property {OpenIZModel.EntityRelationship} relationship.AdministerableMaterial             The administerable material            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedChild             The adopted child            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedDaughter             The adopted daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedSon             The adopted son            
 * @property {OpenIZModel.EntityRelationship} relationship.Affiliate             The affiliate            
 * @property {OpenIZModel.EntityRelationship} relationship.Agent             The agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Aliquot             The aliquot            
 * @property {OpenIZModel.EntityRelationship} relationship.Assigned             The assigned            
 * @property {OpenIZModel.EntityRelationship} relationship.AssignedEntity             The assigned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Aunt             The aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.Birthplace             The birthplace            
 * @property {OpenIZModel.EntityRelationship} relationship.Brother             The brother            
 * @property {OpenIZModel.EntityRelationship} relationship.Brotherinlaw             The brotherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Caregiver             The caregiver            
 * @property {OpenIZModel.EntityRelationship} relationship.CaseSubject             The case subject            
 * @property {OpenIZModel.EntityRelationship} relationship.Child             The child            
 * @property {OpenIZModel.EntityRelationship} relationship.ChildInlaw             The child inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Citizen             The citizen            
 * @property {OpenIZModel.EntityRelationship} relationship.Claimant             The claimant            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchInvestigator             The clinical research investigator            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchSponsor             The clinical research sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CommissioningParty             The commissioning party            
 * @property {OpenIZModel.EntityRelationship} relationship.Contact             The contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Cousin             The cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.CoverageSponsor             The coverage sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CoveredParty             The covered party            
 * @property {OpenIZModel.EntityRelationship} relationship.Daughter             The daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.DaughterInlaw             The daughter inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.DedicatedServiceDeliveryLocation             The dedicated service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Dependent             The dependent            
 * @property {OpenIZModel.EntityRelationship} relationship.DistributedMaterial             The distributed material            
 * @property {OpenIZModel.EntityRelationship} relationship.DomesticPartner             The domestic partner            
 * @property {OpenIZModel.EntityRelationship} relationship.EmergencyContact             The emergency contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Employee             The employee            
 * @property {OpenIZModel.EntityRelationship} relationship.ExposedEntity             The exposed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.FamilyMember             The family member            
 * @property {OpenIZModel.EntityRelationship} relationship.Father             The father            
 * @property {OpenIZModel.EntityRelationship} relationship.Fatherinlaw             The fatherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterChild             The foster child            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterDaughter             The foster daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterSon             The foster son            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandchild             The grandchild            
 * @property {OpenIZModel.EntityRelationship} relationship.Granddaughter             The granddaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandfather             The grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandmother             The grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandparent             The grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandson             The grandson            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandfather             The great grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandmother             The great grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandparent             The great grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Guarantor             The guarantor            
 * @property {OpenIZModel.EntityRelationship} relationship.GUARD             The guard            
 * @property {OpenIZModel.EntityRelationship} relationship.Guardian             The guardian            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfbrother             The halfbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsibling             The halfsibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsister             The halfsister            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthcareProvider             The healthcare provider            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthChart             The health chart            
 * @property {OpenIZModel.EntityRelationship} relationship.HeldEntity             The held entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Husband             The husband            
 * @property {OpenIZModel.EntityRelationship} relationship.IdentifiedEntity             The identified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.IncidentalServiceDeliveryLocation             The incidental service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Individual             The individual            
 * @property {OpenIZModel.EntityRelationship} relationship.InvestigationSubject             The investigation subject            
 * @property {OpenIZModel.EntityRelationship} relationship.InvoicePayor             The invoice payor            
 * @property {OpenIZModel.EntityRelationship} relationship.Isolate             The isolate            
 * @property {OpenIZModel.EntityRelationship} relationship.LicensedEntity             The licensed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.MaintainedEntity             The maintained entity            
 * @property {OpenIZModel.EntityRelationship} relationship.ManufacturedProduct             The manufactured product            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalAunt             The maternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalCousin             The maternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandfather             The maternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandmother             The maternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandparent             The maternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandfather             The maternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandmother             The maternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandparent             The maternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalUncle             The maternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.MilitaryPerson             The military person            
 * @property {OpenIZModel.EntityRelationship} relationship.Mother             The mother            
 * @property {OpenIZModel.EntityRelationship} relationship.Motherinlaw             The motherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.NamedInsured             The named insured            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalBrother             The natural brother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalChild             The natural child            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalDaughter             The natural daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFather             The natural father            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFatherOfFetus             The natural father of fetus            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalMother             The natural mother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalParent             The natural parent            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSibling             The natural sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSister             The natural sister            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSon             The natural son            
 * @property {OpenIZModel.EntityRelationship} relationship.Nephew             The nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NextOfKin             The next of kin            
 * @property {OpenIZModel.EntityRelationship} relationship.Niece             The niece            
 * @property {OpenIZModel.EntityRelationship} relationship.NieceNephew             The niece nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NotaryPublic             The notary public            
 * @property {OpenIZModel.EntityRelationship} relationship.OwnedEntity             The owned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Parent             The parent            
 * @property {OpenIZModel.EntityRelationship} relationship.ParentInlaw             The parent inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Part             The part            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalAunt             The paternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalCousin             The paternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandfather             The paternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandmother             The paternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandparent             The paternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandfather             The paternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandmother             The paternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandparent             The paternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalUncle             The paternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Patient             The patient            
 * @property {OpenIZModel.EntityRelationship} relationship.Payee             The payee            
 * @property {OpenIZModel.EntityRelationship} relationship.PersonalRelationship             The personal relationship            
 * @property {OpenIZModel.EntityRelationship} relationship.PlaceOfDeath             The place of death            
 * @property {OpenIZModel.EntityRelationship} relationship.PolicyHolder             The policy holder            
 * @property {OpenIZModel.EntityRelationship} relationship.ProgramEligible             The program eligible            
 * @property {OpenIZModel.EntityRelationship} relationship.QualifiedEntity             The qualified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.RegulatedProduct             The regulated product            
 * @property {OpenIZModel.EntityRelationship} relationship.ResearchSubject             The research subject            
 * @property {OpenIZModel.EntityRelationship} relationship.RetailedMaterial             The retailed material            
 * @property {OpenIZModel.EntityRelationship} relationship.Roomate             The roomate            
 * @property {OpenIZModel.EntityRelationship} relationship.ServiceDeliveryLocation             The service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Sibling             The sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.SiblingInlaw             The sibling inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.SignificantOther             The significant other            
 * @property {OpenIZModel.EntityRelationship} relationship.SigningAuthorityOrOfficer             The signing authority or officer            
 * @property {OpenIZModel.EntityRelationship} relationship.Sister             The sister            
 * @property {OpenIZModel.EntityRelationship} relationship.Sisterinlaw             The sisterinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Son             The son            
 * @property {OpenIZModel.EntityRelationship} relationship.SonInlaw             The son inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Specimen             The specimen            
 * @property {OpenIZModel.EntityRelationship} relationship.Spouse             The spouse            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepbrother             The stepbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepChild             The step child            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepdaughter             The stepdaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepfather             The stepfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepmother             The stepmother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepParent             The step parent            
 * @property {OpenIZModel.EntityRelationship} relationship.StepSibling             The step sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepsister             The stepsister            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepson             The stepson            
 * @property {OpenIZModel.EntityRelationship} relationship.Student             The student            
 * @property {OpenIZModel.EntityRelationship} relationship.Subscriber             The subscriber            
 * @property {OpenIZModel.EntityRelationship} relationship.TerritoryOfAuthority             The territory of authority            
 * @property {OpenIZModel.EntityRelationship} relationship.TherapeuticAgent             The therapeutic agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Uncle             The uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Underwriter             The underwriter            
 * @property {OpenIZModel.EntityRelationship} relationship.UsedEntity             The used entity            
 * @property {OpenIZModel.EntityRelationship} relationship.WarrantedProduct             The warranted product            
 * @property {OpenIZModel.EntityRelationship} relationship.Wife             The wife            
 * @property {OpenIZModel.EntityRelationship} relationship.Replaces             The replaces            
 * @property {OpenIZModel.EntityRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {object} tag            Gets a list of all tags associated with the entity            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {object} telecom            Gets a list of all telecommunications addresses associated with the entity            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.AnsweringService             answering service            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.EmergencyContact             Emergency contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.MobileContact             Mobile phone contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Pager             pager            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Public             public (800 number example) contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.TemporaryAddress             temporary contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.WorkPlace             For use in the workplace            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.$other Unclassified
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Container} copyData Copy constructor (if present)
 */
Container : function(copyData) { 
	this.$type = 'Container';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.typeConcept = copyData.typeConcept;
	this.typeConceptModel = copyData.typeConceptModel;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.telecom = copyData.telecom;
	this.tag = copyData.tag;
	this.statusConcept = copyData.statusConcept;
	this.statusConceptModel = copyData.statusConceptModel;
	this.relationship = copyData.relationship;
	this.participation = copyData.participation;
	this.note = copyData.note;
	this.name = copyData.name;
	this.identifier = copyData.identifier;
	this.extension = copyData.extension;
	this.determinerConcept = copyData.determinerConcept;
	this.determinerConceptModel = copyData.determinerConceptModel;
	this.creationAct = copyData.creationAct;
	this.creationActModel = copyData.creationActModel;
	this.classConcept = copyData.classConcept;
	this.classConceptModel = copyData.classConceptModel;
	this.address = copyData.address;
	this.quantityConcept = copyData.quantityConcept;
	this.quantityConceptModel = copyData.quantityConceptModel;
	this.quantity = copyData.quantity;
	this.isAdministrative = copyData.isAdministrative;
	this.formConcept = copyData.formConcept;
	this.formConceptModel = copyData.formConceptModel;
	this.expiryDate = copyData.expiryDate;
	this.lotNumber = copyData.lotNumber;
	this.separatorTypeConcept = copyData.separatorTypeConcept;
	this.separatorTypeConceptModel = copyData.separatorTypeConceptModel;
	this.heightQuantity = copyData.heightQuantity;
	this.diameterQuantity = copyData.diameterQuantity;
	this.capTypeConcept = copyData.capTypeConcept;
	this.capTypeConceptModel = copyData.capTypeConceptModel;
	this.capacityQuantity = copyData.capacityQuantity;
	this.bottomDeltaQuantity = copyData.bottomDeltaQuantity;
	this.barrierDeltaQuantity = copyData.barrierDeltaQuantity;
	}
},  // Container 
// OpenIZ.Core.Model.Entities.UserEntity, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Person
 * @summary             Represents a user entity            
 * @property {OpenIZModel.SecurityUser} securityUserModel [Delay loaded from securityUser],             Gets or sets the security user key            
 * @property {uuid} securityUser            Gets or sets the security user key            
 * @property {OpenIZModel.DatePrecision} dateOfBirthPrecision            Gets or sets the precision ofthe date of birth            (see: {@link OpenIZModel.DatePrecision} for values)
 * @property {string} dateOfBirth            Gets the date of birth as XML            
 * @property {OpenIZModel.PersonLanguageCommunication} language            Gets the person's languages of communication            
 * @property {object} address            Gets a list of all addresses associated with the entity            
 * @property {OpenIZModel.EntityAddress} address.Alphabetic             Represents an alphabetic address used for matching             
 * @property {OpenIZModel.EntityAddress} address.BadAddress             Represents a bad address, i.e. an address which is old or invalid.            
 * @property {OpenIZModel.EntityAddress} address.Direct             Represents a workplace address that reaches the person directly without intermediaries.            
 * @property {OpenIZModel.EntityAddress} address.HomeAddress             The home address            
 * @property {OpenIZModel.EntityAddress} address.Ideographic             Represents an address expressed in an ideographic manner (example: Kaji)            
 * @property {OpenIZModel.EntityAddress} address.Phonetic             Represents an address expressed as a phonetic spelling of an ideographic address            
 * @property {OpenIZModel.EntityAddress} address.PhysicalVisit             The address is a physical place where visits should occur            
 * @property {OpenIZModel.EntityAddress} address.PostalAddress             The address is a postal address used for the delivery of mail and materials            
 * @property {OpenIZModel.EntityAddress} address.PrimaryHome             Represents a primary address to reach a contact after business hours.            
 * @property {OpenIZModel.EntityAddress} address.Public             Represents an address that is a standard address that may be subject to a switchboard or operator prior to reaching the intended entity.            
 * @property {OpenIZModel.EntityAddress} address.Soundex             Represents an address used for soundex matching purposes.            
 * @property {OpenIZModel.EntityAddress} address.Syllabic             Represents a syllabic address.            
 * @property {OpenIZModel.EntityAddress} address.TemporaryAddress             Represents a temporary address that may be good for visiting or mailing.            
 * @property {OpenIZModel.EntityAddress} address.VacationHome             Represents a vacation home to reach a person while on vacation.            
 * @property {OpenIZModel.EntityAddress} address.WorkPlace             Represents an office address, should be used for business communications            
 * @property {OpenIZModel.EntityAddress} address.$other Unclassified
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.EntityClassKeys} for values)
 * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
 * @property {uuid} creationAct            Creation act reference            
 * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
 * @property {uuid} determinerConcept            Determiner concept            (see: {@link OpenIZModel.DeterminerKeys} for values)
 * @property {object} extension            Gets a list of all extensions associated with the entity            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {object} identifier            Gets the identifiers associated with this entity            
 * @property {OpenIZModel.EntityIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} name            Gets a list of all names associated with the entity            
 * @property {OpenIZModel.EntityName} name.Alphabetic             The name used is an alphabetic representation of the name (ex: romaji in Japanese)            
 * @property {OpenIZModel.EntityName} name.Anonymous             The name is an anonymous name for the object (not the real name but a name used for care delivery)            
 * @property {OpenIZModel.EntityName} name.Artist             The name represents an artist name or stage name            
 * @property {OpenIZModel.EntityName} name.Assigned             The name represents an assigned name (given or bestowed by an authority)            
 * @property {OpenIZModel.EntityName} name.Ideographic             THe name represents an ideographic representation of the name            
 * @property {OpenIZModel.EntityName} name.Indigenous             The name is an indigenous name or tribal name for the patient            
 * @property {OpenIZModel.EntityName} name.Legal             The name represents the current legal name of an object (such as a corporate name)            
 * @property {OpenIZModel.EntityName} name.License             The name represents a name as displayed on a license or known to a license authority            
 * @property {OpenIZModel.EntityName} name.MaidenName             THe name is a maiden name (name of a patient before marriage)            
 * @property {OpenIZModel.EntityName} name.OfficialRecord             The name as it appears on an official record            
 * @property {OpenIZModel.EntityName} name.Phonetic             The name represents a phonetic representation of a name such as a SOUNDEX code            
 * @property {OpenIZModel.EntityName} name.Pseudonym             The name is a pseudonym for the object or an synonym name            
 * @property {OpenIZModel.EntityName} name.Religious             The name is to be used for religious purposes (such as baptismal name)            
 * @property {OpenIZModel.EntityName} name.Search             The name is to be used in the performing of matches only            
 * @property {OpenIZModel.EntityName} name.Soundex             The name represents the computed soundex code of a name            
 * @property {OpenIZModel.EntityName} name.Syllabic             The name represents a syllabic name.            
 * @property {OpenIZModel.EntityName} name.$other Unclassified
 * @property {string} note            Gets a list of all notes associated with the entity            
 * @property {object} participation            Gets the acts in which this entity participates            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {object} relationship            Gets a list of all associated entities for this entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Access             The access            
 * @property {OpenIZModel.EntityRelationship} relationship.ActiveMoiety             The active moiety            
 * @property {OpenIZModel.EntityRelationship} relationship.AdministerableMaterial             The administerable material            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedChild             The adopted child            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedDaughter             The adopted daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedSon             The adopted son            
 * @property {OpenIZModel.EntityRelationship} relationship.Affiliate             The affiliate            
 * @property {OpenIZModel.EntityRelationship} relationship.Agent             The agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Aliquot             The aliquot            
 * @property {OpenIZModel.EntityRelationship} relationship.Assigned             The assigned            
 * @property {OpenIZModel.EntityRelationship} relationship.AssignedEntity             The assigned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Aunt             The aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.Birthplace             The birthplace            
 * @property {OpenIZModel.EntityRelationship} relationship.Brother             The brother            
 * @property {OpenIZModel.EntityRelationship} relationship.Brotherinlaw             The brotherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Caregiver             The caregiver            
 * @property {OpenIZModel.EntityRelationship} relationship.CaseSubject             The case subject            
 * @property {OpenIZModel.EntityRelationship} relationship.Child             The child            
 * @property {OpenIZModel.EntityRelationship} relationship.ChildInlaw             The child inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Citizen             The citizen            
 * @property {OpenIZModel.EntityRelationship} relationship.Claimant             The claimant            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchInvestigator             The clinical research investigator            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchSponsor             The clinical research sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CommissioningParty             The commissioning party            
 * @property {OpenIZModel.EntityRelationship} relationship.Contact             The contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Cousin             The cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.CoverageSponsor             The coverage sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CoveredParty             The covered party            
 * @property {OpenIZModel.EntityRelationship} relationship.Daughter             The daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.DaughterInlaw             The daughter inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.DedicatedServiceDeliveryLocation             The dedicated service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Dependent             The dependent            
 * @property {OpenIZModel.EntityRelationship} relationship.DistributedMaterial             The distributed material            
 * @property {OpenIZModel.EntityRelationship} relationship.DomesticPartner             The domestic partner            
 * @property {OpenIZModel.EntityRelationship} relationship.EmergencyContact             The emergency contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Employee             The employee            
 * @property {OpenIZModel.EntityRelationship} relationship.ExposedEntity             The exposed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.FamilyMember             The family member            
 * @property {OpenIZModel.EntityRelationship} relationship.Father             The father            
 * @property {OpenIZModel.EntityRelationship} relationship.Fatherinlaw             The fatherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterChild             The foster child            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterDaughter             The foster daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterSon             The foster son            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandchild             The grandchild            
 * @property {OpenIZModel.EntityRelationship} relationship.Granddaughter             The granddaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandfather             The grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandmother             The grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandparent             The grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandson             The grandson            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandfather             The great grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandmother             The great grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandparent             The great grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Guarantor             The guarantor            
 * @property {OpenIZModel.EntityRelationship} relationship.GUARD             The guard            
 * @property {OpenIZModel.EntityRelationship} relationship.Guardian             The guardian            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfbrother             The halfbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsibling             The halfsibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsister             The halfsister            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthcareProvider             The healthcare provider            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthChart             The health chart            
 * @property {OpenIZModel.EntityRelationship} relationship.HeldEntity             The held entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Husband             The husband            
 * @property {OpenIZModel.EntityRelationship} relationship.IdentifiedEntity             The identified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.IncidentalServiceDeliveryLocation             The incidental service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Individual             The individual            
 * @property {OpenIZModel.EntityRelationship} relationship.InvestigationSubject             The investigation subject            
 * @property {OpenIZModel.EntityRelationship} relationship.InvoicePayor             The invoice payor            
 * @property {OpenIZModel.EntityRelationship} relationship.Isolate             The isolate            
 * @property {OpenIZModel.EntityRelationship} relationship.LicensedEntity             The licensed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.MaintainedEntity             The maintained entity            
 * @property {OpenIZModel.EntityRelationship} relationship.ManufacturedProduct             The manufactured product            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalAunt             The maternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalCousin             The maternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandfather             The maternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandmother             The maternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandparent             The maternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandfather             The maternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandmother             The maternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandparent             The maternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalUncle             The maternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.MilitaryPerson             The military person            
 * @property {OpenIZModel.EntityRelationship} relationship.Mother             The mother            
 * @property {OpenIZModel.EntityRelationship} relationship.Motherinlaw             The motherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.NamedInsured             The named insured            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalBrother             The natural brother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalChild             The natural child            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalDaughter             The natural daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFather             The natural father            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFatherOfFetus             The natural father of fetus            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalMother             The natural mother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalParent             The natural parent            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSibling             The natural sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSister             The natural sister            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSon             The natural son            
 * @property {OpenIZModel.EntityRelationship} relationship.Nephew             The nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NextOfKin             The next of kin            
 * @property {OpenIZModel.EntityRelationship} relationship.Niece             The niece            
 * @property {OpenIZModel.EntityRelationship} relationship.NieceNephew             The niece nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NotaryPublic             The notary public            
 * @property {OpenIZModel.EntityRelationship} relationship.OwnedEntity             The owned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Parent             The parent            
 * @property {OpenIZModel.EntityRelationship} relationship.ParentInlaw             The parent inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Part             The part            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalAunt             The paternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalCousin             The paternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandfather             The paternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandmother             The paternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandparent             The paternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandfather             The paternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandmother             The paternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandparent             The paternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalUncle             The paternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Patient             The patient            
 * @property {OpenIZModel.EntityRelationship} relationship.Payee             The payee            
 * @property {OpenIZModel.EntityRelationship} relationship.PersonalRelationship             The personal relationship            
 * @property {OpenIZModel.EntityRelationship} relationship.PlaceOfDeath             The place of death            
 * @property {OpenIZModel.EntityRelationship} relationship.PolicyHolder             The policy holder            
 * @property {OpenIZModel.EntityRelationship} relationship.ProgramEligible             The program eligible            
 * @property {OpenIZModel.EntityRelationship} relationship.QualifiedEntity             The qualified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.RegulatedProduct             The regulated product            
 * @property {OpenIZModel.EntityRelationship} relationship.ResearchSubject             The research subject            
 * @property {OpenIZModel.EntityRelationship} relationship.RetailedMaterial             The retailed material            
 * @property {OpenIZModel.EntityRelationship} relationship.Roomate             The roomate            
 * @property {OpenIZModel.EntityRelationship} relationship.ServiceDeliveryLocation             The service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Sibling             The sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.SiblingInlaw             The sibling inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.SignificantOther             The significant other            
 * @property {OpenIZModel.EntityRelationship} relationship.SigningAuthorityOrOfficer             The signing authority or officer            
 * @property {OpenIZModel.EntityRelationship} relationship.Sister             The sister            
 * @property {OpenIZModel.EntityRelationship} relationship.Sisterinlaw             The sisterinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Son             The son            
 * @property {OpenIZModel.EntityRelationship} relationship.SonInlaw             The son inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Specimen             The specimen            
 * @property {OpenIZModel.EntityRelationship} relationship.Spouse             The spouse            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepbrother             The stepbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepChild             The step child            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepdaughter             The stepdaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepfather             The stepfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepmother             The stepmother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepParent             The step parent            
 * @property {OpenIZModel.EntityRelationship} relationship.StepSibling             The step sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepsister             The stepsister            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepson             The stepson            
 * @property {OpenIZModel.EntityRelationship} relationship.Student             The student            
 * @property {OpenIZModel.EntityRelationship} relationship.Subscriber             The subscriber            
 * @property {OpenIZModel.EntityRelationship} relationship.TerritoryOfAuthority             The territory of authority            
 * @property {OpenIZModel.EntityRelationship} relationship.TherapeuticAgent             The therapeutic agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Uncle             The uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Underwriter             The underwriter            
 * @property {OpenIZModel.EntityRelationship} relationship.UsedEntity             The used entity            
 * @property {OpenIZModel.EntityRelationship} relationship.WarrantedProduct             The warranted product            
 * @property {OpenIZModel.EntityRelationship} relationship.Wife             The wife            
 * @property {OpenIZModel.EntityRelationship} relationship.Replaces             The replaces            
 * @property {OpenIZModel.EntityRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {object} tag            Gets a list of all tags associated with the entity            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {object} telecom            Gets a list of all telecommunications addresses associated with the entity            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.AnsweringService             answering service            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.EmergencyContact             Emergency contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.MobileContact             Mobile phone contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Pager             pager            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Public             public (800 number example) contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.TemporaryAddress             temporary contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.WorkPlace             For use in the workplace            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.$other Unclassified
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.UserEntity} copyData Copy constructor (if present)
 */
UserEntity : function(copyData) { 
	this.$type = 'UserEntity';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.typeConcept = copyData.typeConcept;
	this.typeConceptModel = copyData.typeConceptModel;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.telecom = copyData.telecom;
	this.tag = copyData.tag;
	this.statusConcept = copyData.statusConcept;
	this.statusConceptModel = copyData.statusConceptModel;
	this.relationship = copyData.relationship;
	this.participation = copyData.participation;
	this.note = copyData.note;
	this.name = copyData.name;
	this.identifier = copyData.identifier;
	this.extension = copyData.extension;
	this.determinerConcept = copyData.determinerConcept;
	this.determinerConceptModel = copyData.determinerConceptModel;
	this.creationAct = copyData.creationAct;
	this.creationActModel = copyData.creationActModel;
	this.classConcept = copyData.classConcept;
	this.classConceptModel = copyData.classConceptModel;
	this.address = copyData.address;
	this.language = copyData.language;
	this.dateOfBirth = copyData.dateOfBirth;
	this.dateOfBirthPrecision = copyData.dateOfBirthPrecision;
	this.securityUser = copyData.securityUser;
	this.securityUserModel = copyData.securityUserModel;
	}
},  // UserEntity 
// OpenIZ.Core.Model.Entities.ApplicationEntity, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Entity
 * @summary             An associative entity which links a SecurityApplication to an Entity            
 * @property {OpenIZModel.SecurityApplication} securityApplicationModel [Delay loaded from securityApplication],             Gets or sets the security application            
 * @property {uuid} securityApplication            Gets or sets the security application            
 * @property {string} softwareName            Gets or sets the name of the software            
 * @property {string} vendorName            Gets or sets the vendoer name of the software            
 * @property {string} versionName            Gets or sets the version of the software            
 * @property {object} address            Gets a list of all addresses associated with the entity            
 * @property {OpenIZModel.EntityAddress} address.Alphabetic             Represents an alphabetic address used for matching             
 * @property {OpenIZModel.EntityAddress} address.BadAddress             Represents a bad address, i.e. an address which is old or invalid.            
 * @property {OpenIZModel.EntityAddress} address.Direct             Represents a workplace address that reaches the person directly without intermediaries.            
 * @property {OpenIZModel.EntityAddress} address.HomeAddress             The home address            
 * @property {OpenIZModel.EntityAddress} address.Ideographic             Represents an address expressed in an ideographic manner (example: Kaji)            
 * @property {OpenIZModel.EntityAddress} address.Phonetic             Represents an address expressed as a phonetic spelling of an ideographic address            
 * @property {OpenIZModel.EntityAddress} address.PhysicalVisit             The address is a physical place where visits should occur            
 * @property {OpenIZModel.EntityAddress} address.PostalAddress             The address is a postal address used for the delivery of mail and materials            
 * @property {OpenIZModel.EntityAddress} address.PrimaryHome             Represents a primary address to reach a contact after business hours.            
 * @property {OpenIZModel.EntityAddress} address.Public             Represents an address that is a standard address that may be subject to a switchboard or operator prior to reaching the intended entity.            
 * @property {OpenIZModel.EntityAddress} address.Soundex             Represents an address used for soundex matching purposes.            
 * @property {OpenIZModel.EntityAddress} address.Syllabic             Represents a syllabic address.            
 * @property {OpenIZModel.EntityAddress} address.TemporaryAddress             Represents a temporary address that may be good for visiting or mailing.            
 * @property {OpenIZModel.EntityAddress} address.VacationHome             Represents a vacation home to reach a person while on vacation.            
 * @property {OpenIZModel.EntityAddress} address.WorkPlace             Represents an office address, should be used for business communications            
 * @property {OpenIZModel.EntityAddress} address.$other Unclassified
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.EntityClassKeys} for values)
 * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
 * @property {uuid} creationAct            Creation act reference            
 * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
 * @property {uuid} determinerConcept            Determiner concept            (see: {@link OpenIZModel.DeterminerKeys} for values)
 * @property {object} extension            Gets a list of all extensions associated with the entity            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {object} identifier            Gets the identifiers associated with this entity            
 * @property {OpenIZModel.EntityIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} name            Gets a list of all names associated with the entity            
 * @property {OpenIZModel.EntityName} name.Alphabetic             The name used is an alphabetic representation of the name (ex: romaji in Japanese)            
 * @property {OpenIZModel.EntityName} name.Anonymous             The name is an anonymous name for the object (not the real name but a name used for care delivery)            
 * @property {OpenIZModel.EntityName} name.Artist             The name represents an artist name or stage name            
 * @property {OpenIZModel.EntityName} name.Assigned             The name represents an assigned name (given or bestowed by an authority)            
 * @property {OpenIZModel.EntityName} name.Ideographic             THe name represents an ideographic representation of the name            
 * @property {OpenIZModel.EntityName} name.Indigenous             The name is an indigenous name or tribal name for the patient            
 * @property {OpenIZModel.EntityName} name.Legal             The name represents the current legal name of an object (such as a corporate name)            
 * @property {OpenIZModel.EntityName} name.License             The name represents a name as displayed on a license or known to a license authority            
 * @property {OpenIZModel.EntityName} name.MaidenName             THe name is a maiden name (name of a patient before marriage)            
 * @property {OpenIZModel.EntityName} name.OfficialRecord             The name as it appears on an official record            
 * @property {OpenIZModel.EntityName} name.Phonetic             The name represents a phonetic representation of a name such as a SOUNDEX code            
 * @property {OpenIZModel.EntityName} name.Pseudonym             The name is a pseudonym for the object or an synonym name            
 * @property {OpenIZModel.EntityName} name.Religious             The name is to be used for religious purposes (such as baptismal name)            
 * @property {OpenIZModel.EntityName} name.Search             The name is to be used in the performing of matches only            
 * @property {OpenIZModel.EntityName} name.Soundex             The name represents the computed soundex code of a name            
 * @property {OpenIZModel.EntityName} name.Syllabic             The name represents a syllabic name.            
 * @property {OpenIZModel.EntityName} name.$other Unclassified
 * @property {string} note            Gets a list of all notes associated with the entity            
 * @property {object} participation            Gets the acts in which this entity participates            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {object} relationship            Gets a list of all associated entities for this entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Access             The access            
 * @property {OpenIZModel.EntityRelationship} relationship.ActiveMoiety             The active moiety            
 * @property {OpenIZModel.EntityRelationship} relationship.AdministerableMaterial             The administerable material            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedChild             The adopted child            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedDaughter             The adopted daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedSon             The adopted son            
 * @property {OpenIZModel.EntityRelationship} relationship.Affiliate             The affiliate            
 * @property {OpenIZModel.EntityRelationship} relationship.Agent             The agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Aliquot             The aliquot            
 * @property {OpenIZModel.EntityRelationship} relationship.Assigned             The assigned            
 * @property {OpenIZModel.EntityRelationship} relationship.AssignedEntity             The assigned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Aunt             The aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.Birthplace             The birthplace            
 * @property {OpenIZModel.EntityRelationship} relationship.Brother             The brother            
 * @property {OpenIZModel.EntityRelationship} relationship.Brotherinlaw             The brotherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Caregiver             The caregiver            
 * @property {OpenIZModel.EntityRelationship} relationship.CaseSubject             The case subject            
 * @property {OpenIZModel.EntityRelationship} relationship.Child             The child            
 * @property {OpenIZModel.EntityRelationship} relationship.ChildInlaw             The child inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Citizen             The citizen            
 * @property {OpenIZModel.EntityRelationship} relationship.Claimant             The claimant            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchInvestigator             The clinical research investigator            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchSponsor             The clinical research sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CommissioningParty             The commissioning party            
 * @property {OpenIZModel.EntityRelationship} relationship.Contact             The contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Cousin             The cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.CoverageSponsor             The coverage sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CoveredParty             The covered party            
 * @property {OpenIZModel.EntityRelationship} relationship.Daughter             The daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.DaughterInlaw             The daughter inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.DedicatedServiceDeliveryLocation             The dedicated service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Dependent             The dependent            
 * @property {OpenIZModel.EntityRelationship} relationship.DistributedMaterial             The distributed material            
 * @property {OpenIZModel.EntityRelationship} relationship.DomesticPartner             The domestic partner            
 * @property {OpenIZModel.EntityRelationship} relationship.EmergencyContact             The emergency contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Employee             The employee            
 * @property {OpenIZModel.EntityRelationship} relationship.ExposedEntity             The exposed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.FamilyMember             The family member            
 * @property {OpenIZModel.EntityRelationship} relationship.Father             The father            
 * @property {OpenIZModel.EntityRelationship} relationship.Fatherinlaw             The fatherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterChild             The foster child            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterDaughter             The foster daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterSon             The foster son            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandchild             The grandchild            
 * @property {OpenIZModel.EntityRelationship} relationship.Granddaughter             The granddaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandfather             The grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandmother             The grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandparent             The grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandson             The grandson            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandfather             The great grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandmother             The great grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandparent             The great grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Guarantor             The guarantor            
 * @property {OpenIZModel.EntityRelationship} relationship.GUARD             The guard            
 * @property {OpenIZModel.EntityRelationship} relationship.Guardian             The guardian            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfbrother             The halfbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsibling             The halfsibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsister             The halfsister            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthcareProvider             The healthcare provider            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthChart             The health chart            
 * @property {OpenIZModel.EntityRelationship} relationship.HeldEntity             The held entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Husband             The husband            
 * @property {OpenIZModel.EntityRelationship} relationship.IdentifiedEntity             The identified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.IncidentalServiceDeliveryLocation             The incidental service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Individual             The individual            
 * @property {OpenIZModel.EntityRelationship} relationship.InvestigationSubject             The investigation subject            
 * @property {OpenIZModel.EntityRelationship} relationship.InvoicePayor             The invoice payor            
 * @property {OpenIZModel.EntityRelationship} relationship.Isolate             The isolate            
 * @property {OpenIZModel.EntityRelationship} relationship.LicensedEntity             The licensed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.MaintainedEntity             The maintained entity            
 * @property {OpenIZModel.EntityRelationship} relationship.ManufacturedProduct             The manufactured product            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalAunt             The maternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalCousin             The maternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandfather             The maternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandmother             The maternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandparent             The maternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandfather             The maternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandmother             The maternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandparent             The maternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalUncle             The maternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.MilitaryPerson             The military person            
 * @property {OpenIZModel.EntityRelationship} relationship.Mother             The mother            
 * @property {OpenIZModel.EntityRelationship} relationship.Motherinlaw             The motherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.NamedInsured             The named insured            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalBrother             The natural brother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalChild             The natural child            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalDaughter             The natural daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFather             The natural father            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFatherOfFetus             The natural father of fetus            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalMother             The natural mother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalParent             The natural parent            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSibling             The natural sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSister             The natural sister            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSon             The natural son            
 * @property {OpenIZModel.EntityRelationship} relationship.Nephew             The nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NextOfKin             The next of kin            
 * @property {OpenIZModel.EntityRelationship} relationship.Niece             The niece            
 * @property {OpenIZModel.EntityRelationship} relationship.NieceNephew             The niece nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NotaryPublic             The notary public            
 * @property {OpenIZModel.EntityRelationship} relationship.OwnedEntity             The owned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Parent             The parent            
 * @property {OpenIZModel.EntityRelationship} relationship.ParentInlaw             The parent inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Part             The part            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalAunt             The paternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalCousin             The paternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandfather             The paternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandmother             The paternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandparent             The paternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandfather             The paternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandmother             The paternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandparent             The paternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalUncle             The paternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Patient             The patient            
 * @property {OpenIZModel.EntityRelationship} relationship.Payee             The payee            
 * @property {OpenIZModel.EntityRelationship} relationship.PersonalRelationship             The personal relationship            
 * @property {OpenIZModel.EntityRelationship} relationship.PlaceOfDeath             The place of death            
 * @property {OpenIZModel.EntityRelationship} relationship.PolicyHolder             The policy holder            
 * @property {OpenIZModel.EntityRelationship} relationship.ProgramEligible             The program eligible            
 * @property {OpenIZModel.EntityRelationship} relationship.QualifiedEntity             The qualified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.RegulatedProduct             The regulated product            
 * @property {OpenIZModel.EntityRelationship} relationship.ResearchSubject             The research subject            
 * @property {OpenIZModel.EntityRelationship} relationship.RetailedMaterial             The retailed material            
 * @property {OpenIZModel.EntityRelationship} relationship.Roomate             The roomate            
 * @property {OpenIZModel.EntityRelationship} relationship.ServiceDeliveryLocation             The service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Sibling             The sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.SiblingInlaw             The sibling inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.SignificantOther             The significant other            
 * @property {OpenIZModel.EntityRelationship} relationship.SigningAuthorityOrOfficer             The signing authority or officer            
 * @property {OpenIZModel.EntityRelationship} relationship.Sister             The sister            
 * @property {OpenIZModel.EntityRelationship} relationship.Sisterinlaw             The sisterinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Son             The son            
 * @property {OpenIZModel.EntityRelationship} relationship.SonInlaw             The son inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Specimen             The specimen            
 * @property {OpenIZModel.EntityRelationship} relationship.Spouse             The spouse            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepbrother             The stepbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepChild             The step child            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepdaughter             The stepdaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepfather             The stepfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepmother             The stepmother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepParent             The step parent            
 * @property {OpenIZModel.EntityRelationship} relationship.StepSibling             The step sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepsister             The stepsister            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepson             The stepson            
 * @property {OpenIZModel.EntityRelationship} relationship.Student             The student            
 * @property {OpenIZModel.EntityRelationship} relationship.Subscriber             The subscriber            
 * @property {OpenIZModel.EntityRelationship} relationship.TerritoryOfAuthority             The territory of authority            
 * @property {OpenIZModel.EntityRelationship} relationship.TherapeuticAgent             The therapeutic agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Uncle             The uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Underwriter             The underwriter            
 * @property {OpenIZModel.EntityRelationship} relationship.UsedEntity             The used entity            
 * @property {OpenIZModel.EntityRelationship} relationship.WarrantedProduct             The warranted product            
 * @property {OpenIZModel.EntityRelationship} relationship.Wife             The wife            
 * @property {OpenIZModel.EntityRelationship} relationship.Replaces             The replaces            
 * @property {OpenIZModel.EntityRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {object} tag            Gets a list of all tags associated with the entity            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {object} telecom            Gets a list of all telecommunications addresses associated with the entity            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.AnsweringService             answering service            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.EmergencyContact             Emergency contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.MobileContact             Mobile phone contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Pager             pager            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Public             public (800 number example) contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.TemporaryAddress             temporary contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.WorkPlace             For use in the workplace            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.$other Unclassified
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ApplicationEntity} copyData Copy constructor (if present)
 */
ApplicationEntity : function(copyData) { 
	this.$type = 'ApplicationEntity';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.typeConcept = copyData.typeConcept;
	this.typeConceptModel = copyData.typeConceptModel;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.telecom = copyData.telecom;
	this.tag = copyData.tag;
	this.statusConcept = copyData.statusConcept;
	this.statusConceptModel = copyData.statusConceptModel;
	this.relationship = copyData.relationship;
	this.participation = copyData.participation;
	this.note = copyData.note;
	this.name = copyData.name;
	this.identifier = copyData.identifier;
	this.extension = copyData.extension;
	this.determinerConcept = copyData.determinerConcept;
	this.determinerConceptModel = copyData.determinerConceptModel;
	this.creationAct = copyData.creationAct;
	this.creationActModel = copyData.creationActModel;
	this.classConcept = copyData.classConcept;
	this.classConceptModel = copyData.classConceptModel;
	this.address = copyData.address;
	this.versionName = copyData.versionName;
	this.vendorName = copyData.vendorName;
	this.softwareName = copyData.softwareName;
	this.securityApplication = copyData.securityApplication;
	this.securityApplicationModel = copyData.securityApplicationModel;
	}
},  // ApplicationEntity 
// OpenIZ.Core.Model.Entities.DeviceEntity, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Entity
 * @summary             Represents a device entity            
 * @property {string} manufacturerModelName            Gets or sets the manufacturer model name            
 * @property {string} operatingSystemName            Gets or sets the operating system name            
 * @property {OpenIZModel.SecurityDevice} securityDeviceModel [Delay loaded from securityDevice],             Gets or sets the security device            
 * @property {uuid} securityDevice            Gets or sets the security device key            
 * @property {object} address            Gets a list of all addresses associated with the entity            
 * @property {OpenIZModel.EntityAddress} address.Alphabetic             Represents an alphabetic address used for matching             
 * @property {OpenIZModel.EntityAddress} address.BadAddress             Represents a bad address, i.e. an address which is old or invalid.            
 * @property {OpenIZModel.EntityAddress} address.Direct             Represents a workplace address that reaches the person directly without intermediaries.            
 * @property {OpenIZModel.EntityAddress} address.HomeAddress             The home address            
 * @property {OpenIZModel.EntityAddress} address.Ideographic             Represents an address expressed in an ideographic manner (example: Kaji)            
 * @property {OpenIZModel.EntityAddress} address.Phonetic             Represents an address expressed as a phonetic spelling of an ideographic address            
 * @property {OpenIZModel.EntityAddress} address.PhysicalVisit             The address is a physical place where visits should occur            
 * @property {OpenIZModel.EntityAddress} address.PostalAddress             The address is a postal address used for the delivery of mail and materials            
 * @property {OpenIZModel.EntityAddress} address.PrimaryHome             Represents a primary address to reach a contact after business hours.            
 * @property {OpenIZModel.EntityAddress} address.Public             Represents an address that is a standard address that may be subject to a switchboard or operator prior to reaching the intended entity.            
 * @property {OpenIZModel.EntityAddress} address.Soundex             Represents an address used for soundex matching purposes.            
 * @property {OpenIZModel.EntityAddress} address.Syllabic             Represents a syllabic address.            
 * @property {OpenIZModel.EntityAddress} address.TemporaryAddress             Represents a temporary address that may be good for visiting or mailing.            
 * @property {OpenIZModel.EntityAddress} address.VacationHome             Represents a vacation home to reach a person while on vacation.            
 * @property {OpenIZModel.EntityAddress} address.WorkPlace             Represents an office address, should be used for business communications            
 * @property {OpenIZModel.EntityAddress} address.$other Unclassified
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.EntityClassKeys} for values)
 * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
 * @property {uuid} creationAct            Creation act reference            
 * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
 * @property {uuid} determinerConcept            Determiner concept            (see: {@link OpenIZModel.DeterminerKeys} for values)
 * @property {object} extension            Gets a list of all extensions associated with the entity            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {object} identifier            Gets the identifiers associated with this entity            
 * @property {OpenIZModel.EntityIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} name            Gets a list of all names associated with the entity            
 * @property {OpenIZModel.EntityName} name.Alphabetic             The name used is an alphabetic representation of the name (ex: romaji in Japanese)            
 * @property {OpenIZModel.EntityName} name.Anonymous             The name is an anonymous name for the object (not the real name but a name used for care delivery)            
 * @property {OpenIZModel.EntityName} name.Artist             The name represents an artist name or stage name            
 * @property {OpenIZModel.EntityName} name.Assigned             The name represents an assigned name (given or bestowed by an authority)            
 * @property {OpenIZModel.EntityName} name.Ideographic             THe name represents an ideographic representation of the name            
 * @property {OpenIZModel.EntityName} name.Indigenous             The name is an indigenous name or tribal name for the patient            
 * @property {OpenIZModel.EntityName} name.Legal             The name represents the current legal name of an object (such as a corporate name)            
 * @property {OpenIZModel.EntityName} name.License             The name represents a name as displayed on a license or known to a license authority            
 * @property {OpenIZModel.EntityName} name.MaidenName             THe name is a maiden name (name of a patient before marriage)            
 * @property {OpenIZModel.EntityName} name.OfficialRecord             The name as it appears on an official record            
 * @property {OpenIZModel.EntityName} name.Phonetic             The name represents a phonetic representation of a name such as a SOUNDEX code            
 * @property {OpenIZModel.EntityName} name.Pseudonym             The name is a pseudonym for the object or an synonym name            
 * @property {OpenIZModel.EntityName} name.Religious             The name is to be used for religious purposes (such as baptismal name)            
 * @property {OpenIZModel.EntityName} name.Search             The name is to be used in the performing of matches only            
 * @property {OpenIZModel.EntityName} name.Soundex             The name represents the computed soundex code of a name            
 * @property {OpenIZModel.EntityName} name.Syllabic             The name represents a syllabic name.            
 * @property {OpenIZModel.EntityName} name.$other Unclassified
 * @property {string} note            Gets a list of all notes associated with the entity            
 * @property {object} participation            Gets the acts in which this entity participates            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {object} relationship            Gets a list of all associated entities for this entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Access             The access            
 * @property {OpenIZModel.EntityRelationship} relationship.ActiveMoiety             The active moiety            
 * @property {OpenIZModel.EntityRelationship} relationship.AdministerableMaterial             The administerable material            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedChild             The adopted child            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedDaughter             The adopted daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedSon             The adopted son            
 * @property {OpenIZModel.EntityRelationship} relationship.Affiliate             The affiliate            
 * @property {OpenIZModel.EntityRelationship} relationship.Agent             The agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Aliquot             The aliquot            
 * @property {OpenIZModel.EntityRelationship} relationship.Assigned             The assigned            
 * @property {OpenIZModel.EntityRelationship} relationship.AssignedEntity             The assigned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Aunt             The aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.Birthplace             The birthplace            
 * @property {OpenIZModel.EntityRelationship} relationship.Brother             The brother            
 * @property {OpenIZModel.EntityRelationship} relationship.Brotherinlaw             The brotherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Caregiver             The caregiver            
 * @property {OpenIZModel.EntityRelationship} relationship.CaseSubject             The case subject            
 * @property {OpenIZModel.EntityRelationship} relationship.Child             The child            
 * @property {OpenIZModel.EntityRelationship} relationship.ChildInlaw             The child inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Citizen             The citizen            
 * @property {OpenIZModel.EntityRelationship} relationship.Claimant             The claimant            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchInvestigator             The clinical research investigator            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchSponsor             The clinical research sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CommissioningParty             The commissioning party            
 * @property {OpenIZModel.EntityRelationship} relationship.Contact             The contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Cousin             The cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.CoverageSponsor             The coverage sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CoveredParty             The covered party            
 * @property {OpenIZModel.EntityRelationship} relationship.Daughter             The daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.DaughterInlaw             The daughter inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.DedicatedServiceDeliveryLocation             The dedicated service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Dependent             The dependent            
 * @property {OpenIZModel.EntityRelationship} relationship.DistributedMaterial             The distributed material            
 * @property {OpenIZModel.EntityRelationship} relationship.DomesticPartner             The domestic partner            
 * @property {OpenIZModel.EntityRelationship} relationship.EmergencyContact             The emergency contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Employee             The employee            
 * @property {OpenIZModel.EntityRelationship} relationship.ExposedEntity             The exposed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.FamilyMember             The family member            
 * @property {OpenIZModel.EntityRelationship} relationship.Father             The father            
 * @property {OpenIZModel.EntityRelationship} relationship.Fatherinlaw             The fatherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterChild             The foster child            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterDaughter             The foster daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterSon             The foster son            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandchild             The grandchild            
 * @property {OpenIZModel.EntityRelationship} relationship.Granddaughter             The granddaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandfather             The grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandmother             The grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandparent             The grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandson             The grandson            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandfather             The great grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandmother             The great grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandparent             The great grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Guarantor             The guarantor            
 * @property {OpenIZModel.EntityRelationship} relationship.GUARD             The guard            
 * @property {OpenIZModel.EntityRelationship} relationship.Guardian             The guardian            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfbrother             The halfbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsibling             The halfsibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsister             The halfsister            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthcareProvider             The healthcare provider            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthChart             The health chart            
 * @property {OpenIZModel.EntityRelationship} relationship.HeldEntity             The held entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Husband             The husband            
 * @property {OpenIZModel.EntityRelationship} relationship.IdentifiedEntity             The identified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.IncidentalServiceDeliveryLocation             The incidental service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Individual             The individual            
 * @property {OpenIZModel.EntityRelationship} relationship.InvestigationSubject             The investigation subject            
 * @property {OpenIZModel.EntityRelationship} relationship.InvoicePayor             The invoice payor            
 * @property {OpenIZModel.EntityRelationship} relationship.Isolate             The isolate            
 * @property {OpenIZModel.EntityRelationship} relationship.LicensedEntity             The licensed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.MaintainedEntity             The maintained entity            
 * @property {OpenIZModel.EntityRelationship} relationship.ManufacturedProduct             The manufactured product            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalAunt             The maternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalCousin             The maternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandfather             The maternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandmother             The maternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandparent             The maternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandfather             The maternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandmother             The maternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandparent             The maternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalUncle             The maternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.MilitaryPerson             The military person            
 * @property {OpenIZModel.EntityRelationship} relationship.Mother             The mother            
 * @property {OpenIZModel.EntityRelationship} relationship.Motherinlaw             The motherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.NamedInsured             The named insured            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalBrother             The natural brother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalChild             The natural child            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalDaughter             The natural daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFather             The natural father            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFatherOfFetus             The natural father of fetus            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalMother             The natural mother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalParent             The natural parent            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSibling             The natural sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSister             The natural sister            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSon             The natural son            
 * @property {OpenIZModel.EntityRelationship} relationship.Nephew             The nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NextOfKin             The next of kin            
 * @property {OpenIZModel.EntityRelationship} relationship.Niece             The niece            
 * @property {OpenIZModel.EntityRelationship} relationship.NieceNephew             The niece nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NotaryPublic             The notary public            
 * @property {OpenIZModel.EntityRelationship} relationship.OwnedEntity             The owned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Parent             The parent            
 * @property {OpenIZModel.EntityRelationship} relationship.ParentInlaw             The parent inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Part             The part            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalAunt             The paternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalCousin             The paternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandfather             The paternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandmother             The paternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandparent             The paternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandfather             The paternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandmother             The paternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandparent             The paternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalUncle             The paternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Patient             The patient            
 * @property {OpenIZModel.EntityRelationship} relationship.Payee             The payee            
 * @property {OpenIZModel.EntityRelationship} relationship.PersonalRelationship             The personal relationship            
 * @property {OpenIZModel.EntityRelationship} relationship.PlaceOfDeath             The place of death            
 * @property {OpenIZModel.EntityRelationship} relationship.PolicyHolder             The policy holder            
 * @property {OpenIZModel.EntityRelationship} relationship.ProgramEligible             The program eligible            
 * @property {OpenIZModel.EntityRelationship} relationship.QualifiedEntity             The qualified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.RegulatedProduct             The regulated product            
 * @property {OpenIZModel.EntityRelationship} relationship.ResearchSubject             The research subject            
 * @property {OpenIZModel.EntityRelationship} relationship.RetailedMaterial             The retailed material            
 * @property {OpenIZModel.EntityRelationship} relationship.Roomate             The roomate            
 * @property {OpenIZModel.EntityRelationship} relationship.ServiceDeliveryLocation             The service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Sibling             The sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.SiblingInlaw             The sibling inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.SignificantOther             The significant other            
 * @property {OpenIZModel.EntityRelationship} relationship.SigningAuthorityOrOfficer             The signing authority or officer            
 * @property {OpenIZModel.EntityRelationship} relationship.Sister             The sister            
 * @property {OpenIZModel.EntityRelationship} relationship.Sisterinlaw             The sisterinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Son             The son            
 * @property {OpenIZModel.EntityRelationship} relationship.SonInlaw             The son inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Specimen             The specimen            
 * @property {OpenIZModel.EntityRelationship} relationship.Spouse             The spouse            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepbrother             The stepbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepChild             The step child            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepdaughter             The stepdaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepfather             The stepfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepmother             The stepmother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepParent             The step parent            
 * @property {OpenIZModel.EntityRelationship} relationship.StepSibling             The step sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepsister             The stepsister            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepson             The stepson            
 * @property {OpenIZModel.EntityRelationship} relationship.Student             The student            
 * @property {OpenIZModel.EntityRelationship} relationship.Subscriber             The subscriber            
 * @property {OpenIZModel.EntityRelationship} relationship.TerritoryOfAuthority             The territory of authority            
 * @property {OpenIZModel.EntityRelationship} relationship.TherapeuticAgent             The therapeutic agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Uncle             The uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Underwriter             The underwriter            
 * @property {OpenIZModel.EntityRelationship} relationship.UsedEntity             The used entity            
 * @property {OpenIZModel.EntityRelationship} relationship.WarrantedProduct             The warranted product            
 * @property {OpenIZModel.EntityRelationship} relationship.Wife             The wife            
 * @property {OpenIZModel.EntityRelationship} relationship.Replaces             The replaces            
 * @property {OpenIZModel.EntityRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {object} tag            Gets a list of all tags associated with the entity            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {object} telecom            Gets a list of all telecommunications addresses associated with the entity            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.AnsweringService             answering service            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.EmergencyContact             Emergency contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.MobileContact             Mobile phone contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Pager             pager            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Public             public (800 number example) contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.TemporaryAddress             temporary contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.WorkPlace             For use in the workplace            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.$other Unclassified
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.DeviceEntity} copyData Copy constructor (if present)
 */
DeviceEntity : function(copyData) { 
	this.$type = 'DeviceEntity';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.typeConcept = copyData.typeConcept;
	this.typeConceptModel = copyData.typeConceptModel;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.telecom = copyData.telecom;
	this.tag = copyData.tag;
	this.statusConcept = copyData.statusConcept;
	this.statusConceptModel = copyData.statusConceptModel;
	this.relationship = copyData.relationship;
	this.participation = copyData.participation;
	this.note = copyData.note;
	this.name = copyData.name;
	this.identifier = copyData.identifier;
	this.extension = copyData.extension;
	this.determinerConcept = copyData.determinerConcept;
	this.determinerConceptModel = copyData.determinerConceptModel;
	this.creationAct = copyData.creationAct;
	this.creationActModel = copyData.creationActModel;
	this.classConcept = copyData.classConcept;
	this.classConceptModel = copyData.classConceptModel;
	this.address = copyData.address;
	this.securityDevice = copyData.securityDevice;
	this.securityDeviceModel = copyData.securityDeviceModel;
	this.operatingSystemName = copyData.operatingSystemName;
	this.manufacturerModelName = copyData.manufacturerModelName;
	}
},  // DeviceEntity 
// OpenIZ.Core.Model.Entities.Entity, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.VersionedEntityData
 * @summary             Represents the base of all entities            
 * @property {object} address            Gets a list of all addresses associated with the entity            
 * @property {OpenIZModel.EntityAddress} address.Alphabetic             Represents an alphabetic address used for matching             
 * @property {OpenIZModel.EntityAddress} address.BadAddress             Represents a bad address, i.e. an address which is old or invalid.            
 * @property {OpenIZModel.EntityAddress} address.Direct             Represents a workplace address that reaches the person directly without intermediaries.            
 * @property {OpenIZModel.EntityAddress} address.HomeAddress             The home address            
 * @property {OpenIZModel.EntityAddress} address.Ideographic             Represents an address expressed in an ideographic manner (example: Kaji)            
 * @property {OpenIZModel.EntityAddress} address.Phonetic             Represents an address expressed as a phonetic spelling of an ideographic address            
 * @property {OpenIZModel.EntityAddress} address.PhysicalVisit             The address is a physical place where visits should occur            
 * @property {OpenIZModel.EntityAddress} address.PostalAddress             The address is a postal address used for the delivery of mail and materials            
 * @property {OpenIZModel.EntityAddress} address.PrimaryHome             Represents a primary address to reach a contact after business hours.            
 * @property {OpenIZModel.EntityAddress} address.Public             Represents an address that is a standard address that may be subject to a switchboard or operator prior to reaching the intended entity.            
 * @property {OpenIZModel.EntityAddress} address.Soundex             Represents an address used for soundex matching purposes.            
 * @property {OpenIZModel.EntityAddress} address.Syllabic             Represents a syllabic address.            
 * @property {OpenIZModel.EntityAddress} address.TemporaryAddress             Represents a temporary address that may be good for visiting or mailing.            
 * @property {OpenIZModel.EntityAddress} address.VacationHome             Represents a vacation home to reach a person while on vacation.            
 * @property {OpenIZModel.EntityAddress} address.WorkPlace             Represents an office address, should be used for business communications            
 * @property {OpenIZModel.EntityAddress} address.$other Unclassified
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.EntityClassKeys} for values)
 * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
 * @property {uuid} creationAct            Creation act reference            
 * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
 * @property {uuid} determinerConcept            Determiner concept            (see: {@link OpenIZModel.DeterminerKeys} for values)
 * @property {object} extension            Gets a list of all extensions associated with the entity            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {object} identifier            Gets the identifiers associated with this entity            
 * @property {OpenIZModel.EntityIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} name            Gets a list of all names associated with the entity            
 * @property {OpenIZModel.EntityName} name.Alphabetic             The name used is an alphabetic representation of the name (ex: romaji in Japanese)            
 * @property {OpenIZModel.EntityName} name.Anonymous             The name is an anonymous name for the object (not the real name but a name used for care delivery)            
 * @property {OpenIZModel.EntityName} name.Artist             The name represents an artist name or stage name            
 * @property {OpenIZModel.EntityName} name.Assigned             The name represents an assigned name (given or bestowed by an authority)            
 * @property {OpenIZModel.EntityName} name.Ideographic             THe name represents an ideographic representation of the name            
 * @property {OpenIZModel.EntityName} name.Indigenous             The name is an indigenous name or tribal name for the patient            
 * @property {OpenIZModel.EntityName} name.Legal             The name represents the current legal name of an object (such as a corporate name)            
 * @property {OpenIZModel.EntityName} name.License             The name represents a name as displayed on a license or known to a license authority            
 * @property {OpenIZModel.EntityName} name.MaidenName             THe name is a maiden name (name of a patient before marriage)            
 * @property {OpenIZModel.EntityName} name.OfficialRecord             The name as it appears on an official record            
 * @property {OpenIZModel.EntityName} name.Phonetic             The name represents a phonetic representation of a name such as a SOUNDEX code            
 * @property {OpenIZModel.EntityName} name.Pseudonym             The name is a pseudonym for the object or an synonym name            
 * @property {OpenIZModel.EntityName} name.Religious             The name is to be used for religious purposes (such as baptismal name)            
 * @property {OpenIZModel.EntityName} name.Search             The name is to be used in the performing of matches only            
 * @property {OpenIZModel.EntityName} name.Soundex             The name represents the computed soundex code of a name            
 * @property {OpenIZModel.EntityName} name.Syllabic             The name represents a syllabic name.            
 * @property {OpenIZModel.EntityName} name.$other Unclassified
 * @property {string} note            Gets a list of all notes associated with the entity            
 * @property {object} participation            Gets the acts in which this entity participates            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {object} relationship            Gets a list of all associated entities for this entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Access             The access            
 * @property {OpenIZModel.EntityRelationship} relationship.ActiveMoiety             The active moiety            
 * @property {OpenIZModel.EntityRelationship} relationship.AdministerableMaterial             The administerable material            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedChild             The adopted child            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedDaughter             The adopted daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedSon             The adopted son            
 * @property {OpenIZModel.EntityRelationship} relationship.Affiliate             The affiliate            
 * @property {OpenIZModel.EntityRelationship} relationship.Agent             The agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Aliquot             The aliquot            
 * @property {OpenIZModel.EntityRelationship} relationship.Assigned             The assigned            
 * @property {OpenIZModel.EntityRelationship} relationship.AssignedEntity             The assigned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Aunt             The aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.Birthplace             The birthplace            
 * @property {OpenIZModel.EntityRelationship} relationship.Brother             The brother            
 * @property {OpenIZModel.EntityRelationship} relationship.Brotherinlaw             The brotherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Caregiver             The caregiver            
 * @property {OpenIZModel.EntityRelationship} relationship.CaseSubject             The case subject            
 * @property {OpenIZModel.EntityRelationship} relationship.Child             The child            
 * @property {OpenIZModel.EntityRelationship} relationship.ChildInlaw             The child inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Citizen             The citizen            
 * @property {OpenIZModel.EntityRelationship} relationship.Claimant             The claimant            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchInvestigator             The clinical research investigator            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchSponsor             The clinical research sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CommissioningParty             The commissioning party            
 * @property {OpenIZModel.EntityRelationship} relationship.Contact             The contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Cousin             The cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.CoverageSponsor             The coverage sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CoveredParty             The covered party            
 * @property {OpenIZModel.EntityRelationship} relationship.Daughter             The daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.DaughterInlaw             The daughter inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.DedicatedServiceDeliveryLocation             The dedicated service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Dependent             The dependent            
 * @property {OpenIZModel.EntityRelationship} relationship.DistributedMaterial             The distributed material            
 * @property {OpenIZModel.EntityRelationship} relationship.DomesticPartner             The domestic partner            
 * @property {OpenIZModel.EntityRelationship} relationship.EmergencyContact             The emergency contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Employee             The employee            
 * @property {OpenIZModel.EntityRelationship} relationship.ExposedEntity             The exposed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.FamilyMember             The family member            
 * @property {OpenIZModel.EntityRelationship} relationship.Father             The father            
 * @property {OpenIZModel.EntityRelationship} relationship.Fatherinlaw             The fatherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterChild             The foster child            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterDaughter             The foster daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterSon             The foster son            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandchild             The grandchild            
 * @property {OpenIZModel.EntityRelationship} relationship.Granddaughter             The granddaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandfather             The grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandmother             The grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandparent             The grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandson             The grandson            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandfather             The great grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandmother             The great grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandparent             The great grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Guarantor             The guarantor            
 * @property {OpenIZModel.EntityRelationship} relationship.GUARD             The guard            
 * @property {OpenIZModel.EntityRelationship} relationship.Guardian             The guardian            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfbrother             The halfbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsibling             The halfsibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsister             The halfsister            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthcareProvider             The healthcare provider            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthChart             The health chart            
 * @property {OpenIZModel.EntityRelationship} relationship.HeldEntity             The held entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Husband             The husband            
 * @property {OpenIZModel.EntityRelationship} relationship.IdentifiedEntity             The identified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.IncidentalServiceDeliveryLocation             The incidental service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Individual             The individual            
 * @property {OpenIZModel.EntityRelationship} relationship.InvestigationSubject             The investigation subject            
 * @property {OpenIZModel.EntityRelationship} relationship.InvoicePayor             The invoice payor            
 * @property {OpenIZModel.EntityRelationship} relationship.Isolate             The isolate            
 * @property {OpenIZModel.EntityRelationship} relationship.LicensedEntity             The licensed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.MaintainedEntity             The maintained entity            
 * @property {OpenIZModel.EntityRelationship} relationship.ManufacturedProduct             The manufactured product            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalAunt             The maternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalCousin             The maternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandfather             The maternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandmother             The maternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandparent             The maternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandfather             The maternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandmother             The maternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandparent             The maternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalUncle             The maternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.MilitaryPerson             The military person            
 * @property {OpenIZModel.EntityRelationship} relationship.Mother             The mother            
 * @property {OpenIZModel.EntityRelationship} relationship.Motherinlaw             The motherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.NamedInsured             The named insured            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalBrother             The natural brother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalChild             The natural child            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalDaughter             The natural daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFather             The natural father            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFatherOfFetus             The natural father of fetus            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalMother             The natural mother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalParent             The natural parent            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSibling             The natural sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSister             The natural sister            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSon             The natural son            
 * @property {OpenIZModel.EntityRelationship} relationship.Nephew             The nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NextOfKin             The next of kin            
 * @property {OpenIZModel.EntityRelationship} relationship.Niece             The niece            
 * @property {OpenIZModel.EntityRelationship} relationship.NieceNephew             The niece nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NotaryPublic             The notary public            
 * @property {OpenIZModel.EntityRelationship} relationship.OwnedEntity             The owned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Parent             The parent            
 * @property {OpenIZModel.EntityRelationship} relationship.ParentInlaw             The parent inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Part             The part            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalAunt             The paternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalCousin             The paternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandfather             The paternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandmother             The paternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandparent             The paternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandfather             The paternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandmother             The paternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandparent             The paternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalUncle             The paternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Patient             The patient            
 * @property {OpenIZModel.EntityRelationship} relationship.Payee             The payee            
 * @property {OpenIZModel.EntityRelationship} relationship.PersonalRelationship             The personal relationship            
 * @property {OpenIZModel.EntityRelationship} relationship.PlaceOfDeath             The place of death            
 * @property {OpenIZModel.EntityRelationship} relationship.PolicyHolder             The policy holder            
 * @property {OpenIZModel.EntityRelationship} relationship.ProgramEligible             The program eligible            
 * @property {OpenIZModel.EntityRelationship} relationship.QualifiedEntity             The qualified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.RegulatedProduct             The regulated product            
 * @property {OpenIZModel.EntityRelationship} relationship.ResearchSubject             The research subject            
 * @property {OpenIZModel.EntityRelationship} relationship.RetailedMaterial             The retailed material            
 * @property {OpenIZModel.EntityRelationship} relationship.Roomate             The roomate            
 * @property {OpenIZModel.EntityRelationship} relationship.ServiceDeliveryLocation             The service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Sibling             The sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.SiblingInlaw             The sibling inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.SignificantOther             The significant other            
 * @property {OpenIZModel.EntityRelationship} relationship.SigningAuthorityOrOfficer             The signing authority or officer            
 * @property {OpenIZModel.EntityRelationship} relationship.Sister             The sister            
 * @property {OpenIZModel.EntityRelationship} relationship.Sisterinlaw             The sisterinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Son             The son            
 * @property {OpenIZModel.EntityRelationship} relationship.SonInlaw             The son inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Specimen             The specimen            
 * @property {OpenIZModel.EntityRelationship} relationship.Spouse             The spouse            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepbrother             The stepbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepChild             The step child            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepdaughter             The stepdaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepfather             The stepfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepmother             The stepmother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepParent             The step parent            
 * @property {OpenIZModel.EntityRelationship} relationship.StepSibling             The step sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepsister             The stepsister            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepson             The stepson            
 * @property {OpenIZModel.EntityRelationship} relationship.Student             The student            
 * @property {OpenIZModel.EntityRelationship} relationship.Subscriber             The subscriber            
 * @property {OpenIZModel.EntityRelationship} relationship.TerritoryOfAuthority             The territory of authority            
 * @property {OpenIZModel.EntityRelationship} relationship.TherapeuticAgent             The therapeutic agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Uncle             The uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Underwriter             The underwriter            
 * @property {OpenIZModel.EntityRelationship} relationship.UsedEntity             The used entity            
 * @property {OpenIZModel.EntityRelationship} relationship.WarrantedProduct             The warranted product            
 * @property {OpenIZModel.EntityRelationship} relationship.Wife             The wife            
 * @property {OpenIZModel.EntityRelationship} relationship.Replaces             The replaces            
 * @property {OpenIZModel.EntityRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {object} tag            Gets a list of all tags associated with the entity            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {object} telecom            Gets a list of all telecommunications addresses associated with the entity            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.AnsweringService             answering service            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.EmergencyContact             Emergency contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.MobileContact             Mobile phone contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Pager             pager            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Public             public (800 number example) contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.TemporaryAddress             temporary contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.WorkPlace             For use in the workplace            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.$other Unclassified
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Entity} copyData Copy constructor (if present)
 */
Entity : function(copyData) { 
	this.$type = 'Entity';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.typeConcept = copyData.typeConcept;
	this.typeConceptModel = copyData.typeConceptModel;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.telecom = copyData.telecom;
	this.tag = copyData.tag;
	this.statusConcept = copyData.statusConcept;
	this.statusConceptModel = copyData.statusConceptModel;
	this.relationship = copyData.relationship;
	this.participation = copyData.participation;
	this.note = copyData.note;
	this.name = copyData.name;
	this.identifier = copyData.identifier;
	this.extension = copyData.extension;
	this.determinerConcept = copyData.determinerConcept;
	this.determinerConceptModel = copyData.determinerConceptModel;
	this.creationAct = copyData.creationAct;
	this.creationActModel = copyData.creationActModel;
	this.classConcept = copyData.classConcept;
	this.classConceptModel = copyData.classConceptModel;
	this.address = copyData.address;
	}
},  // Entity 
// OpenIZ.Core.Model.Entities.EntityAddress, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.VersionedAssociation
 * @summary             Entity address            
 * @property {OpenIZModel.Concept} useModel [Delay loaded from use],             Gets or sets the address use            
 * @property {uuid} use            Gets or sets the address use key            (see: {@link OpenIZModel.AddressUseKeys} for values)
 * @property {object} component            Gets or sets the component types            
 * @property {string} component.AdditionalLocator             An additional locator (example: Beside the red barn).            
 * @property {string} component.AddressLine             An address line as would appear on an address (example: 123 Main Street West)            
 * @property {string} component.BuildingNumber             Identifies a particular building on a street (example: A23 Building)            
 * @property {string} component.BuildingNumberNumeric             Identifies a numeric identifier for a building (example: 123)            
 * @property {string} component.BuildingNumberSuffix             Identifies a suffix to the building number (example: 123 *SECTOR 7*)            
 * @property {string} component.CareOf             Identifies the person where deliveries should be care-of (example: c/o Bob Smith)            
 * @property {string} component.CensusTract             The census tract which is used for political counting of the census            
 * @property {string} component.City             The town or city (example: Toronto)            
 * @property {string} component.Country             The country in which the address resides (example: Canada)            
 * @property {string} component.County             The county or sub-division of a sub-national unit (example: Clark County)            
 * @property {string} component.Delimiter             Represents a meaningless delimiter such as dash, or newline            
 * @property {string} component.DeliveryAddressLine             Represents an address line to be used for delivery rather than physical location (example: Loading Dock #4)            
 * @property {string} component.DeliveryInstallationArea             Represents the area where the delivery should take place            
 * @property {string} component.DeliveryInstallationQualifier             The delivery installation qualifier.            
 * @property {string} component.DeliveryInstallationType             The delivery installation type.            
 * @property {string} component.DeliveryMode             The delivery mode.            
 * @property {string} component.DeliveryModeIdentifier             The delivery mode identifier.            
 * @property {string} component.Direction             Represents a directory such as north, south, east, or west            
 * @property {string} component.PostalCode             A codified adminsitrative unit used to locate the address (zip code or postal code)            
 * @property {string} component.PostBox             Represents a PO box where delivery of mail should take place            
 * @property {string} component.Precinct             Represents a precinct or sub-division of a city such as a burrogh            
 * @property {string} component.State             Represents a state or province, or a sub-division of a national boundary            
 * @property {string} component.StreetAddressLine             Represents a physical street delivery line (example: 123 Main Street West)            
 * @property {string} component.StreetName             Represents the name portion of a street address (example: Main St.)            
 * @property {string} component.StreetNameBase             The street name base portion of a street address (Example: Main)            
 * @property {string} component.StreetType             The street type (example: Street, Road, Hwy)            
 * @property {string} component.UnitDesignator             Identifies the type of unit (example: Suite, Apartment, Unit)            
 * @property {string} component.UnitIdentifier             The identifier of the unit (example: 820)            
 * @property {string} component.$other Unclassified
 * @property {number} effectiveVersionSequence
 * @property {number} obsoleteVersionSequence
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.EntityAddress} copyData Copy constructor (if present)
 */
EntityAddress : function(copyData) { 
	this.$type = 'EntityAddress';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.component = copyData.component;
	this.use = copyData.use;
	this.useModel = copyData.useModel;
	}
},  // EntityAddress 
// OpenIZ.Core.Model.Entities.EntityAddressComponent, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.GenericComponentValues
 * @summary             A single address component            
 * @property {uuid} type            Gets or sets the component type key            (see: {@link OpenIZModel.AddressComponentKeys} for values)
 * @property {OpenIZModel.Concept} typeModel [Delay loaded from type], 
 * @property {string} value
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.EntityAddress} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.AddressComponent} copyData Copy constructor (if present)
 */
AddressComponent : function(copyData) { 
	this.$type = 'AddressComponent';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.value = copyData.value;
	this.typeModel = copyData.typeModel;
	this.type = copyData.type;
	}
},  // AddressComponent 
// OpenIZ.Core.Model.Entities.EntityName, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.VersionedAssociation
 * @summary             Represents a name for an entity            
 * @property {object} component
 * @property {string} component.Delimiter             The name component represents a delimeter in a name such as hyphen or space            
 * @property {string} component.Family             The name component represents the surname            
 * @property {string} component.Given             The name component represents the given name            
 * @property {string} component.Prefix             The name component represents the prefix such as Von or Van            
 * @property {string} component.Suffix             The name component represents a suffix such as III or Esq.            
 * @property {string} component.Title             The name component represents a formal title like Mr, Dr, Capt.            
 * @property {string} component.$other Unclassified
 * @property {OpenIZModel.Concept} useModel [Delay loaded from use],             Gets or sets the name use            
 * @property {uuid} use            Gets or sets the name use key            (see: {@link OpenIZModel.NameUseKeys} for values)
 * @property {number} effectiveVersionSequence
 * @property {number} obsoleteVersionSequence
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.EntityName} copyData Copy constructor (if present)
 */
EntityName : function(copyData) { 
	this.$type = 'EntityName';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.use = copyData.use;
	this.useModel = copyData.useModel;
	this.component = copyData.component;
	}
},  // EntityName 
// OpenIZ.Core.Model.Entities.EntityNameComponent, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.GenericComponentValues
 * @summary             Represents a name component which is bound to a name            
 * @property {uuid} type            Gets or sets the component type key            (see: {@link OpenIZModel.NameComponentKeys} for values)
 * @property {OpenIZModel.PhoneticAlgorithm} phoneticAlgorithmModel [Delay loaded from phoneticAlgorithm],             Gets or sets the phonetic algorithm            
 * @property {uuid} phoneticAlgorithm            Gets or sets the identifier of the phonetic code            (see: {@link OpenIZModel.PhoneticAlgorithmKeys} for values)
 * @property {string} phoneticCode            Gets or sets the phonetic code of the reference term            
 * @property {OpenIZModel.Concept} typeModel [Delay loaded from type], 
 * @property {string} value
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.EntityName} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.EntityNameComponent} copyData Copy constructor (if present)
 */
EntityNameComponent : function(copyData) { 
	this.$type = 'EntityNameComponent';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.value = copyData.value;
	this.typeModel = copyData.typeModel;
	this.phoneticCode = copyData.phoneticCode;
	this.phoneticAlgorithm = copyData.phoneticAlgorithm;
	this.phoneticAlgorithmModel = copyData.phoneticAlgorithmModel;
	this.type = copyData.type;
	}
},  // EntityNameComponent 
// OpenIZ.Core.Model.Entities.EntityRelationship, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.VersionedAssociation
 * @summary             Represents an association between two entities            
 * @property {OpenIZModel.Entity} holderModel [Delay loaded from holder],             The entity that this relationship targets            
 * @property {uuid} holder            The entity that this relationship targets            
 * @property {bool} inversionInd            The inversion indicator            
 * @property {number} quantity            Represents the quantity of target in source            
 * @property {OpenIZModel.Concept} relationshipTypeModel [Delay loaded from relationshipType],             Gets or sets the association type            
 * @property {uuid} relationshipType            Association type key            (see: {@link OpenIZModel.EntityRelationshipTypeKeys} for values)
 * @property {OpenIZModel.Entity} targetModel [Delay loaded from target],             Target entity reference            
 * @property {uuid} target            The target of the association            
 * @property {number} effectiveVersionSequence
 * @property {number} obsoleteVersionSequence
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.EntityRelationship} copyData Copy constructor (if present)
 */
EntityRelationship : function(copyData) { 
	this.$type = 'EntityRelationship';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.target = copyData.target;
	this.targetModel = copyData.targetModel;
	this.relationshipType = copyData.relationshipType;
	this.relationshipTypeModel = copyData.relationshipTypeModel;
	this.quantity = copyData.quantity;
	this.inversionInd = copyData.inversionInd;
	this.holder = copyData.holder;
	this.holderModel = copyData.holderModel;
	}
},  // EntityRelationship 
// OpenIZ.Core.Model.Entities.EntityTelecomAddress, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.VersionedAssociation
 * @summary             Represents an entity telecom address            
 * @property {OpenIZModel.Concept} useModel [Delay loaded from use],             Gets or sets the name use            
 * @property {uuid} use            Gets or sets the name use key            (see: {@link OpenIZModel.TelecomAddressUseKeys} for values)
 * @property {string} value            Gets or sets the value of the telecom address            
 * @property {number} effectiveVersionSequence
 * @property {number} obsoleteVersionSequence
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.EntityTelecomAddress} copyData Copy constructor (if present)
 */
EntityTelecomAddress : function(copyData) { 
	this.$type = 'EntityTelecomAddress';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.value = copyData.value;
	this.use = copyData.use;
	this.useModel = copyData.useModel;
	}
},  // EntityTelecomAddress 
// OpenIZ.Core.Model.Entities.GenericComponentValues`1, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @abstract
 * @extends OpenIZModel.Association
 * @summary             A generic class representing components of a larger item (i.e. address, name, etc);            
 * @property {OpenIZModel.Concept} typeModel [Delay loaded from type],             Gets or sets the type of address component            
 * @property {uuid} type            Component type key            
 * @property {string} value            Gets or sets the value of the name component            
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.IdentifiedData} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.GenericComponentValues} copyData Copy constructor (if present)
 */
GenericComponentValues : function(copyData) { 
	this.$type = 'GenericComponentValues';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.value = copyData.value;
	this.type = copyData.type;
	this.typeModel = copyData.typeModel;
	}
},  // GenericComponentValues 
// OpenIZ.Core.Model.Entities.ManufacturedMaterial, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Material
 * @summary             Manufactured material            
 * @property {string} lotNumber            Gets or sets the lot number of the manufactured material            
 * @property {date} expiryDate            Gets or sets the expiry date of the material            
 * @property {OpenIZModel.Concept} formConceptModel [Delay loaded from formConcept],             Gets or sets the concept which dictates the form of the material (solid, liquid, capsule, injection, etc.)            
 * @property {uuid} formConcept            Gets or sets the form concept's key            
 * @property {bool} isAdministrative            True if the material is simply administrative            
 * @property {number} quantity            The base quantity of the object in the units. This differs from quantity on the relationship            which is a /per ...            
 * @property {OpenIZModel.Concept} quantityConceptModel [Delay loaded from quantityConcept],             Gets or sets the concept which dictates the unit of measure for a single instance of this entity            
 * @property {uuid} quantityConcept            Gets or sets the quantity concept ref            
 * @property {object} address            Gets a list of all addresses associated with the entity            
 * @property {OpenIZModel.EntityAddress} address.Alphabetic             Represents an alphabetic address used for matching             
 * @property {OpenIZModel.EntityAddress} address.BadAddress             Represents a bad address, i.e. an address which is old or invalid.            
 * @property {OpenIZModel.EntityAddress} address.Direct             Represents a workplace address that reaches the person directly without intermediaries.            
 * @property {OpenIZModel.EntityAddress} address.HomeAddress             The home address            
 * @property {OpenIZModel.EntityAddress} address.Ideographic             Represents an address expressed in an ideographic manner (example: Kaji)            
 * @property {OpenIZModel.EntityAddress} address.Phonetic             Represents an address expressed as a phonetic spelling of an ideographic address            
 * @property {OpenIZModel.EntityAddress} address.PhysicalVisit             The address is a physical place where visits should occur            
 * @property {OpenIZModel.EntityAddress} address.PostalAddress             The address is a postal address used for the delivery of mail and materials            
 * @property {OpenIZModel.EntityAddress} address.PrimaryHome             Represents a primary address to reach a contact after business hours.            
 * @property {OpenIZModel.EntityAddress} address.Public             Represents an address that is a standard address that may be subject to a switchboard or operator prior to reaching the intended entity.            
 * @property {OpenIZModel.EntityAddress} address.Soundex             Represents an address used for soundex matching purposes.            
 * @property {OpenIZModel.EntityAddress} address.Syllabic             Represents a syllabic address.            
 * @property {OpenIZModel.EntityAddress} address.TemporaryAddress             Represents a temporary address that may be good for visiting or mailing.            
 * @property {OpenIZModel.EntityAddress} address.VacationHome             Represents a vacation home to reach a person while on vacation.            
 * @property {OpenIZModel.EntityAddress} address.WorkPlace             Represents an office address, should be used for business communications            
 * @property {OpenIZModel.EntityAddress} address.$other Unclassified
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.EntityClassKeys} for values)
 * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
 * @property {uuid} creationAct            Creation act reference            
 * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
 * @property {uuid} determinerConcept            Determiner concept            (see: {@link OpenIZModel.DeterminerKeys} for values)
 * @property {object} extension            Gets a list of all extensions associated with the entity            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {object} identifier            Gets the identifiers associated with this entity            
 * @property {OpenIZModel.EntityIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} name            Gets a list of all names associated with the entity            
 * @property {OpenIZModel.EntityName} name.Alphabetic             The name used is an alphabetic representation of the name (ex: romaji in Japanese)            
 * @property {OpenIZModel.EntityName} name.Anonymous             The name is an anonymous name for the object (not the real name but a name used for care delivery)            
 * @property {OpenIZModel.EntityName} name.Artist             The name represents an artist name or stage name            
 * @property {OpenIZModel.EntityName} name.Assigned             The name represents an assigned name (given or bestowed by an authority)            
 * @property {OpenIZModel.EntityName} name.Ideographic             THe name represents an ideographic representation of the name            
 * @property {OpenIZModel.EntityName} name.Indigenous             The name is an indigenous name or tribal name for the patient            
 * @property {OpenIZModel.EntityName} name.Legal             The name represents the current legal name of an object (such as a corporate name)            
 * @property {OpenIZModel.EntityName} name.License             The name represents a name as displayed on a license or known to a license authority            
 * @property {OpenIZModel.EntityName} name.MaidenName             THe name is a maiden name (name of a patient before marriage)            
 * @property {OpenIZModel.EntityName} name.OfficialRecord             The name as it appears on an official record            
 * @property {OpenIZModel.EntityName} name.Phonetic             The name represents a phonetic representation of a name such as a SOUNDEX code            
 * @property {OpenIZModel.EntityName} name.Pseudonym             The name is a pseudonym for the object or an synonym name            
 * @property {OpenIZModel.EntityName} name.Religious             The name is to be used for religious purposes (such as baptismal name)            
 * @property {OpenIZModel.EntityName} name.Search             The name is to be used in the performing of matches only            
 * @property {OpenIZModel.EntityName} name.Soundex             The name represents the computed soundex code of a name            
 * @property {OpenIZModel.EntityName} name.Syllabic             The name represents a syllabic name.            
 * @property {OpenIZModel.EntityName} name.$other Unclassified
 * @property {string} note            Gets a list of all notes associated with the entity            
 * @property {object} participation            Gets the acts in which this entity participates            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {object} relationship            Gets a list of all associated entities for this entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Access             The access            
 * @property {OpenIZModel.EntityRelationship} relationship.ActiveMoiety             The active moiety            
 * @property {OpenIZModel.EntityRelationship} relationship.AdministerableMaterial             The administerable material            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedChild             The adopted child            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedDaughter             The adopted daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedSon             The adopted son            
 * @property {OpenIZModel.EntityRelationship} relationship.Affiliate             The affiliate            
 * @property {OpenIZModel.EntityRelationship} relationship.Agent             The agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Aliquot             The aliquot            
 * @property {OpenIZModel.EntityRelationship} relationship.Assigned             The assigned            
 * @property {OpenIZModel.EntityRelationship} relationship.AssignedEntity             The assigned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Aunt             The aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.Birthplace             The birthplace            
 * @property {OpenIZModel.EntityRelationship} relationship.Brother             The brother            
 * @property {OpenIZModel.EntityRelationship} relationship.Brotherinlaw             The brotherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Caregiver             The caregiver            
 * @property {OpenIZModel.EntityRelationship} relationship.CaseSubject             The case subject            
 * @property {OpenIZModel.EntityRelationship} relationship.Child             The child            
 * @property {OpenIZModel.EntityRelationship} relationship.ChildInlaw             The child inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Citizen             The citizen            
 * @property {OpenIZModel.EntityRelationship} relationship.Claimant             The claimant            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchInvestigator             The clinical research investigator            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchSponsor             The clinical research sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CommissioningParty             The commissioning party            
 * @property {OpenIZModel.EntityRelationship} relationship.Contact             The contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Cousin             The cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.CoverageSponsor             The coverage sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CoveredParty             The covered party            
 * @property {OpenIZModel.EntityRelationship} relationship.Daughter             The daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.DaughterInlaw             The daughter inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.DedicatedServiceDeliveryLocation             The dedicated service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Dependent             The dependent            
 * @property {OpenIZModel.EntityRelationship} relationship.DistributedMaterial             The distributed material            
 * @property {OpenIZModel.EntityRelationship} relationship.DomesticPartner             The domestic partner            
 * @property {OpenIZModel.EntityRelationship} relationship.EmergencyContact             The emergency contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Employee             The employee            
 * @property {OpenIZModel.EntityRelationship} relationship.ExposedEntity             The exposed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.FamilyMember             The family member            
 * @property {OpenIZModel.EntityRelationship} relationship.Father             The father            
 * @property {OpenIZModel.EntityRelationship} relationship.Fatherinlaw             The fatherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterChild             The foster child            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterDaughter             The foster daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterSon             The foster son            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandchild             The grandchild            
 * @property {OpenIZModel.EntityRelationship} relationship.Granddaughter             The granddaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandfather             The grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandmother             The grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandparent             The grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandson             The grandson            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandfather             The great grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandmother             The great grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandparent             The great grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Guarantor             The guarantor            
 * @property {OpenIZModel.EntityRelationship} relationship.GUARD             The guard            
 * @property {OpenIZModel.EntityRelationship} relationship.Guardian             The guardian            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfbrother             The halfbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsibling             The halfsibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsister             The halfsister            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthcareProvider             The healthcare provider            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthChart             The health chart            
 * @property {OpenIZModel.EntityRelationship} relationship.HeldEntity             The held entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Husband             The husband            
 * @property {OpenIZModel.EntityRelationship} relationship.IdentifiedEntity             The identified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.IncidentalServiceDeliveryLocation             The incidental service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Individual             The individual            
 * @property {OpenIZModel.EntityRelationship} relationship.InvestigationSubject             The investigation subject            
 * @property {OpenIZModel.EntityRelationship} relationship.InvoicePayor             The invoice payor            
 * @property {OpenIZModel.EntityRelationship} relationship.Isolate             The isolate            
 * @property {OpenIZModel.EntityRelationship} relationship.LicensedEntity             The licensed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.MaintainedEntity             The maintained entity            
 * @property {OpenIZModel.EntityRelationship} relationship.ManufacturedProduct             The manufactured product            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalAunt             The maternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalCousin             The maternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandfather             The maternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandmother             The maternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandparent             The maternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandfather             The maternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandmother             The maternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandparent             The maternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalUncle             The maternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.MilitaryPerson             The military person            
 * @property {OpenIZModel.EntityRelationship} relationship.Mother             The mother            
 * @property {OpenIZModel.EntityRelationship} relationship.Motherinlaw             The motherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.NamedInsured             The named insured            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalBrother             The natural brother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalChild             The natural child            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalDaughter             The natural daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFather             The natural father            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFatherOfFetus             The natural father of fetus            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalMother             The natural mother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalParent             The natural parent            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSibling             The natural sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSister             The natural sister            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSon             The natural son            
 * @property {OpenIZModel.EntityRelationship} relationship.Nephew             The nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NextOfKin             The next of kin            
 * @property {OpenIZModel.EntityRelationship} relationship.Niece             The niece            
 * @property {OpenIZModel.EntityRelationship} relationship.NieceNephew             The niece nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NotaryPublic             The notary public            
 * @property {OpenIZModel.EntityRelationship} relationship.OwnedEntity             The owned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Parent             The parent            
 * @property {OpenIZModel.EntityRelationship} relationship.ParentInlaw             The parent inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Part             The part            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalAunt             The paternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalCousin             The paternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandfather             The paternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandmother             The paternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandparent             The paternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandfather             The paternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandmother             The paternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandparent             The paternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalUncle             The paternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Patient             The patient            
 * @property {OpenIZModel.EntityRelationship} relationship.Payee             The payee            
 * @property {OpenIZModel.EntityRelationship} relationship.PersonalRelationship             The personal relationship            
 * @property {OpenIZModel.EntityRelationship} relationship.PlaceOfDeath             The place of death            
 * @property {OpenIZModel.EntityRelationship} relationship.PolicyHolder             The policy holder            
 * @property {OpenIZModel.EntityRelationship} relationship.ProgramEligible             The program eligible            
 * @property {OpenIZModel.EntityRelationship} relationship.QualifiedEntity             The qualified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.RegulatedProduct             The regulated product            
 * @property {OpenIZModel.EntityRelationship} relationship.ResearchSubject             The research subject            
 * @property {OpenIZModel.EntityRelationship} relationship.RetailedMaterial             The retailed material            
 * @property {OpenIZModel.EntityRelationship} relationship.Roomate             The roomate            
 * @property {OpenIZModel.EntityRelationship} relationship.ServiceDeliveryLocation             The service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Sibling             The sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.SiblingInlaw             The sibling inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.SignificantOther             The significant other            
 * @property {OpenIZModel.EntityRelationship} relationship.SigningAuthorityOrOfficer             The signing authority or officer            
 * @property {OpenIZModel.EntityRelationship} relationship.Sister             The sister            
 * @property {OpenIZModel.EntityRelationship} relationship.Sisterinlaw             The sisterinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Son             The son            
 * @property {OpenIZModel.EntityRelationship} relationship.SonInlaw             The son inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Specimen             The specimen            
 * @property {OpenIZModel.EntityRelationship} relationship.Spouse             The spouse            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepbrother             The stepbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepChild             The step child            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepdaughter             The stepdaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepfather             The stepfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepmother             The stepmother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepParent             The step parent            
 * @property {OpenIZModel.EntityRelationship} relationship.StepSibling             The step sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepsister             The stepsister            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepson             The stepson            
 * @property {OpenIZModel.EntityRelationship} relationship.Student             The student            
 * @property {OpenIZModel.EntityRelationship} relationship.Subscriber             The subscriber            
 * @property {OpenIZModel.EntityRelationship} relationship.TerritoryOfAuthority             The territory of authority            
 * @property {OpenIZModel.EntityRelationship} relationship.TherapeuticAgent             The therapeutic agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Uncle             The uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Underwriter             The underwriter            
 * @property {OpenIZModel.EntityRelationship} relationship.UsedEntity             The used entity            
 * @property {OpenIZModel.EntityRelationship} relationship.WarrantedProduct             The warranted product            
 * @property {OpenIZModel.EntityRelationship} relationship.Wife             The wife            
 * @property {OpenIZModel.EntityRelationship} relationship.Replaces             The replaces            
 * @property {OpenIZModel.EntityRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {object} tag            Gets a list of all tags associated with the entity            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {object} telecom            Gets a list of all telecommunications addresses associated with the entity            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.AnsweringService             answering service            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.EmergencyContact             Emergency contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.MobileContact             Mobile phone contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Pager             pager            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Public             public (800 number example) contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.TemporaryAddress             temporary contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.WorkPlace             For use in the workplace            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.$other Unclassified
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ManufacturedMaterial} copyData Copy constructor (if present)
 */
ManufacturedMaterial : function(copyData) { 
	this.$type = 'ManufacturedMaterial';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.typeConcept = copyData.typeConcept;
	this.typeConceptModel = copyData.typeConceptModel;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.telecom = copyData.telecom;
	this.tag = copyData.tag;
	this.statusConcept = copyData.statusConcept;
	this.statusConceptModel = copyData.statusConceptModel;
	this.relationship = copyData.relationship;
	this.participation = copyData.participation;
	this.note = copyData.note;
	this.name = copyData.name;
	this.identifier = copyData.identifier;
	this.extension = copyData.extension;
	this.determinerConcept = copyData.determinerConcept;
	this.determinerConceptModel = copyData.determinerConceptModel;
	this.creationAct = copyData.creationAct;
	this.creationActModel = copyData.creationActModel;
	this.classConcept = copyData.classConcept;
	this.classConceptModel = copyData.classConceptModel;
	this.address = copyData.address;
	this.quantityConcept = copyData.quantityConcept;
	this.quantityConceptModel = copyData.quantityConceptModel;
	this.quantity = copyData.quantity;
	this.isAdministrative = copyData.isAdministrative;
	this.formConcept = copyData.formConcept;
	this.formConceptModel = copyData.formConceptModel;
	this.expiryDate = copyData.expiryDate;
	this.lotNumber = copyData.lotNumber;
	}
},  // ManufacturedMaterial 
// OpenIZ.Core.Model.Entities.Material, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Entity
 * @summary             Represents a material            
 * @property {date} expiryDate            Gets or sets the expiry date of the material            
 * @property {OpenIZModel.Concept} formConceptModel [Delay loaded from formConcept],             Gets or sets the concept which dictates the form of the material (solid, liquid, capsule, injection, etc.)            
 * @property {uuid} formConcept            Gets or sets the form concept's key            
 * @property {bool} isAdministrative            True if the material is simply administrative            
 * @property {number} quantity            The base quantity of the object in the units. This differs from quantity on the relationship            which is a /per ...            
 * @property {OpenIZModel.Concept} quantityConceptModel [Delay loaded from quantityConcept],             Gets or sets the concept which dictates the unit of measure for a single instance of this entity            
 * @property {uuid} quantityConcept            Gets or sets the quantity concept ref            
 * @property {object} address            Gets a list of all addresses associated with the entity            
 * @property {OpenIZModel.EntityAddress} address.Alphabetic             Represents an alphabetic address used for matching             
 * @property {OpenIZModel.EntityAddress} address.BadAddress             Represents a bad address, i.e. an address which is old or invalid.            
 * @property {OpenIZModel.EntityAddress} address.Direct             Represents a workplace address that reaches the person directly without intermediaries.            
 * @property {OpenIZModel.EntityAddress} address.HomeAddress             The home address            
 * @property {OpenIZModel.EntityAddress} address.Ideographic             Represents an address expressed in an ideographic manner (example: Kaji)            
 * @property {OpenIZModel.EntityAddress} address.Phonetic             Represents an address expressed as a phonetic spelling of an ideographic address            
 * @property {OpenIZModel.EntityAddress} address.PhysicalVisit             The address is a physical place where visits should occur            
 * @property {OpenIZModel.EntityAddress} address.PostalAddress             The address is a postal address used for the delivery of mail and materials            
 * @property {OpenIZModel.EntityAddress} address.PrimaryHome             Represents a primary address to reach a contact after business hours.            
 * @property {OpenIZModel.EntityAddress} address.Public             Represents an address that is a standard address that may be subject to a switchboard or operator prior to reaching the intended entity.            
 * @property {OpenIZModel.EntityAddress} address.Soundex             Represents an address used for soundex matching purposes.            
 * @property {OpenIZModel.EntityAddress} address.Syllabic             Represents a syllabic address.            
 * @property {OpenIZModel.EntityAddress} address.TemporaryAddress             Represents a temporary address that may be good for visiting or mailing.            
 * @property {OpenIZModel.EntityAddress} address.VacationHome             Represents a vacation home to reach a person while on vacation.            
 * @property {OpenIZModel.EntityAddress} address.WorkPlace             Represents an office address, should be used for business communications            
 * @property {OpenIZModel.EntityAddress} address.$other Unclassified
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.EntityClassKeys} for values)
 * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
 * @property {uuid} creationAct            Creation act reference            
 * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
 * @property {uuid} determinerConcept            Determiner concept            (see: {@link OpenIZModel.DeterminerKeys} for values)
 * @property {object} extension            Gets a list of all extensions associated with the entity            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {object} identifier            Gets the identifiers associated with this entity            
 * @property {OpenIZModel.EntityIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} name            Gets a list of all names associated with the entity            
 * @property {OpenIZModel.EntityName} name.Alphabetic             The name used is an alphabetic representation of the name (ex: romaji in Japanese)            
 * @property {OpenIZModel.EntityName} name.Anonymous             The name is an anonymous name for the object (not the real name but a name used for care delivery)            
 * @property {OpenIZModel.EntityName} name.Artist             The name represents an artist name or stage name            
 * @property {OpenIZModel.EntityName} name.Assigned             The name represents an assigned name (given or bestowed by an authority)            
 * @property {OpenIZModel.EntityName} name.Ideographic             THe name represents an ideographic representation of the name            
 * @property {OpenIZModel.EntityName} name.Indigenous             The name is an indigenous name or tribal name for the patient            
 * @property {OpenIZModel.EntityName} name.Legal             The name represents the current legal name of an object (such as a corporate name)            
 * @property {OpenIZModel.EntityName} name.License             The name represents a name as displayed on a license or known to a license authority            
 * @property {OpenIZModel.EntityName} name.MaidenName             THe name is a maiden name (name of a patient before marriage)            
 * @property {OpenIZModel.EntityName} name.OfficialRecord             The name as it appears on an official record            
 * @property {OpenIZModel.EntityName} name.Phonetic             The name represents a phonetic representation of a name such as a SOUNDEX code            
 * @property {OpenIZModel.EntityName} name.Pseudonym             The name is a pseudonym for the object or an synonym name            
 * @property {OpenIZModel.EntityName} name.Religious             The name is to be used for religious purposes (such as baptismal name)            
 * @property {OpenIZModel.EntityName} name.Search             The name is to be used in the performing of matches only            
 * @property {OpenIZModel.EntityName} name.Soundex             The name represents the computed soundex code of a name            
 * @property {OpenIZModel.EntityName} name.Syllabic             The name represents a syllabic name.            
 * @property {OpenIZModel.EntityName} name.$other Unclassified
 * @property {string} note            Gets a list of all notes associated with the entity            
 * @property {object} participation            Gets the acts in which this entity participates            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {object} relationship            Gets a list of all associated entities for this entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Access             The access            
 * @property {OpenIZModel.EntityRelationship} relationship.ActiveMoiety             The active moiety            
 * @property {OpenIZModel.EntityRelationship} relationship.AdministerableMaterial             The administerable material            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedChild             The adopted child            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedDaughter             The adopted daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedSon             The adopted son            
 * @property {OpenIZModel.EntityRelationship} relationship.Affiliate             The affiliate            
 * @property {OpenIZModel.EntityRelationship} relationship.Agent             The agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Aliquot             The aliquot            
 * @property {OpenIZModel.EntityRelationship} relationship.Assigned             The assigned            
 * @property {OpenIZModel.EntityRelationship} relationship.AssignedEntity             The assigned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Aunt             The aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.Birthplace             The birthplace            
 * @property {OpenIZModel.EntityRelationship} relationship.Brother             The brother            
 * @property {OpenIZModel.EntityRelationship} relationship.Brotherinlaw             The brotherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Caregiver             The caregiver            
 * @property {OpenIZModel.EntityRelationship} relationship.CaseSubject             The case subject            
 * @property {OpenIZModel.EntityRelationship} relationship.Child             The child            
 * @property {OpenIZModel.EntityRelationship} relationship.ChildInlaw             The child inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Citizen             The citizen            
 * @property {OpenIZModel.EntityRelationship} relationship.Claimant             The claimant            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchInvestigator             The clinical research investigator            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchSponsor             The clinical research sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CommissioningParty             The commissioning party            
 * @property {OpenIZModel.EntityRelationship} relationship.Contact             The contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Cousin             The cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.CoverageSponsor             The coverage sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CoveredParty             The covered party            
 * @property {OpenIZModel.EntityRelationship} relationship.Daughter             The daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.DaughterInlaw             The daughter inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.DedicatedServiceDeliveryLocation             The dedicated service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Dependent             The dependent            
 * @property {OpenIZModel.EntityRelationship} relationship.DistributedMaterial             The distributed material            
 * @property {OpenIZModel.EntityRelationship} relationship.DomesticPartner             The domestic partner            
 * @property {OpenIZModel.EntityRelationship} relationship.EmergencyContact             The emergency contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Employee             The employee            
 * @property {OpenIZModel.EntityRelationship} relationship.ExposedEntity             The exposed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.FamilyMember             The family member            
 * @property {OpenIZModel.EntityRelationship} relationship.Father             The father            
 * @property {OpenIZModel.EntityRelationship} relationship.Fatherinlaw             The fatherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterChild             The foster child            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterDaughter             The foster daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterSon             The foster son            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandchild             The grandchild            
 * @property {OpenIZModel.EntityRelationship} relationship.Granddaughter             The granddaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandfather             The grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandmother             The grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandparent             The grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandson             The grandson            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandfather             The great grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandmother             The great grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandparent             The great grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Guarantor             The guarantor            
 * @property {OpenIZModel.EntityRelationship} relationship.GUARD             The guard            
 * @property {OpenIZModel.EntityRelationship} relationship.Guardian             The guardian            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfbrother             The halfbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsibling             The halfsibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsister             The halfsister            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthcareProvider             The healthcare provider            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthChart             The health chart            
 * @property {OpenIZModel.EntityRelationship} relationship.HeldEntity             The held entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Husband             The husband            
 * @property {OpenIZModel.EntityRelationship} relationship.IdentifiedEntity             The identified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.IncidentalServiceDeliveryLocation             The incidental service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Individual             The individual            
 * @property {OpenIZModel.EntityRelationship} relationship.InvestigationSubject             The investigation subject            
 * @property {OpenIZModel.EntityRelationship} relationship.InvoicePayor             The invoice payor            
 * @property {OpenIZModel.EntityRelationship} relationship.Isolate             The isolate            
 * @property {OpenIZModel.EntityRelationship} relationship.LicensedEntity             The licensed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.MaintainedEntity             The maintained entity            
 * @property {OpenIZModel.EntityRelationship} relationship.ManufacturedProduct             The manufactured product            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalAunt             The maternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalCousin             The maternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandfather             The maternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandmother             The maternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandparent             The maternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandfather             The maternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandmother             The maternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandparent             The maternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalUncle             The maternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.MilitaryPerson             The military person            
 * @property {OpenIZModel.EntityRelationship} relationship.Mother             The mother            
 * @property {OpenIZModel.EntityRelationship} relationship.Motherinlaw             The motherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.NamedInsured             The named insured            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalBrother             The natural brother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalChild             The natural child            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalDaughter             The natural daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFather             The natural father            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFatherOfFetus             The natural father of fetus            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalMother             The natural mother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalParent             The natural parent            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSibling             The natural sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSister             The natural sister            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSon             The natural son            
 * @property {OpenIZModel.EntityRelationship} relationship.Nephew             The nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NextOfKin             The next of kin            
 * @property {OpenIZModel.EntityRelationship} relationship.Niece             The niece            
 * @property {OpenIZModel.EntityRelationship} relationship.NieceNephew             The niece nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NotaryPublic             The notary public            
 * @property {OpenIZModel.EntityRelationship} relationship.OwnedEntity             The owned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Parent             The parent            
 * @property {OpenIZModel.EntityRelationship} relationship.ParentInlaw             The parent inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Part             The part            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalAunt             The paternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalCousin             The paternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandfather             The paternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandmother             The paternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandparent             The paternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandfather             The paternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandmother             The paternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandparent             The paternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalUncle             The paternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Patient             The patient            
 * @property {OpenIZModel.EntityRelationship} relationship.Payee             The payee            
 * @property {OpenIZModel.EntityRelationship} relationship.PersonalRelationship             The personal relationship            
 * @property {OpenIZModel.EntityRelationship} relationship.PlaceOfDeath             The place of death            
 * @property {OpenIZModel.EntityRelationship} relationship.PolicyHolder             The policy holder            
 * @property {OpenIZModel.EntityRelationship} relationship.ProgramEligible             The program eligible            
 * @property {OpenIZModel.EntityRelationship} relationship.QualifiedEntity             The qualified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.RegulatedProduct             The regulated product            
 * @property {OpenIZModel.EntityRelationship} relationship.ResearchSubject             The research subject            
 * @property {OpenIZModel.EntityRelationship} relationship.RetailedMaterial             The retailed material            
 * @property {OpenIZModel.EntityRelationship} relationship.Roomate             The roomate            
 * @property {OpenIZModel.EntityRelationship} relationship.ServiceDeliveryLocation             The service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Sibling             The sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.SiblingInlaw             The sibling inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.SignificantOther             The significant other            
 * @property {OpenIZModel.EntityRelationship} relationship.SigningAuthorityOrOfficer             The signing authority or officer            
 * @property {OpenIZModel.EntityRelationship} relationship.Sister             The sister            
 * @property {OpenIZModel.EntityRelationship} relationship.Sisterinlaw             The sisterinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Son             The son            
 * @property {OpenIZModel.EntityRelationship} relationship.SonInlaw             The son inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Specimen             The specimen            
 * @property {OpenIZModel.EntityRelationship} relationship.Spouse             The spouse            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepbrother             The stepbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepChild             The step child            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepdaughter             The stepdaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepfather             The stepfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepmother             The stepmother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepParent             The step parent            
 * @property {OpenIZModel.EntityRelationship} relationship.StepSibling             The step sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepsister             The stepsister            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepson             The stepson            
 * @property {OpenIZModel.EntityRelationship} relationship.Student             The student            
 * @property {OpenIZModel.EntityRelationship} relationship.Subscriber             The subscriber            
 * @property {OpenIZModel.EntityRelationship} relationship.TerritoryOfAuthority             The territory of authority            
 * @property {OpenIZModel.EntityRelationship} relationship.TherapeuticAgent             The therapeutic agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Uncle             The uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Underwriter             The underwriter            
 * @property {OpenIZModel.EntityRelationship} relationship.UsedEntity             The used entity            
 * @property {OpenIZModel.EntityRelationship} relationship.WarrantedProduct             The warranted product            
 * @property {OpenIZModel.EntityRelationship} relationship.Wife             The wife            
 * @property {OpenIZModel.EntityRelationship} relationship.Replaces             The replaces            
 * @property {OpenIZModel.EntityRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {object} tag            Gets a list of all tags associated with the entity            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {object} telecom            Gets a list of all telecommunications addresses associated with the entity            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.AnsweringService             answering service            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.EmergencyContact             Emergency contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.MobileContact             Mobile phone contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Pager             pager            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Public             public (800 number example) contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.TemporaryAddress             temporary contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.WorkPlace             For use in the workplace            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.$other Unclassified
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Material} copyData Copy constructor (if present)
 */
Material : function(copyData) { 
	this.$type = 'Material';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.typeConcept = copyData.typeConcept;
	this.typeConceptModel = copyData.typeConceptModel;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.telecom = copyData.telecom;
	this.tag = copyData.tag;
	this.statusConcept = copyData.statusConcept;
	this.statusConceptModel = copyData.statusConceptModel;
	this.relationship = copyData.relationship;
	this.participation = copyData.participation;
	this.note = copyData.note;
	this.name = copyData.name;
	this.identifier = copyData.identifier;
	this.extension = copyData.extension;
	this.determinerConcept = copyData.determinerConcept;
	this.determinerConceptModel = copyData.determinerConceptModel;
	this.creationAct = copyData.creationAct;
	this.creationActModel = copyData.creationActModel;
	this.classConcept = copyData.classConcept;
	this.classConceptModel = copyData.classConceptModel;
	this.address = copyData.address;
	this.quantityConcept = copyData.quantityConcept;
	this.quantityConceptModel = copyData.quantityConceptModel;
	this.quantity = copyData.quantity;
	this.isAdministrative = copyData.isAdministrative;
	this.formConcept = copyData.formConcept;
	this.formConceptModel = copyData.formConceptModel;
	this.expiryDate = copyData.expiryDate;
	}
},  // Material 
// OpenIZ.Core.Model.Entities.Organization, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Entity
 * @summary             Organization entity            
 * @property {OpenIZModel.Concept} industryConceptModel [Delay loaded from industryConcept],             Gets or sets the industry concept key            
 * @property {uuid} industryConcept
 * @property {object} address            Gets a list of all addresses associated with the entity            
 * @property {OpenIZModel.EntityAddress} address.Alphabetic             Represents an alphabetic address used for matching             
 * @property {OpenIZModel.EntityAddress} address.BadAddress             Represents a bad address, i.e. an address which is old or invalid.            
 * @property {OpenIZModel.EntityAddress} address.Direct             Represents a workplace address that reaches the person directly without intermediaries.            
 * @property {OpenIZModel.EntityAddress} address.HomeAddress             The home address            
 * @property {OpenIZModel.EntityAddress} address.Ideographic             Represents an address expressed in an ideographic manner (example: Kaji)            
 * @property {OpenIZModel.EntityAddress} address.Phonetic             Represents an address expressed as a phonetic spelling of an ideographic address            
 * @property {OpenIZModel.EntityAddress} address.PhysicalVisit             The address is a physical place where visits should occur            
 * @property {OpenIZModel.EntityAddress} address.PostalAddress             The address is a postal address used for the delivery of mail and materials            
 * @property {OpenIZModel.EntityAddress} address.PrimaryHome             Represents a primary address to reach a contact after business hours.            
 * @property {OpenIZModel.EntityAddress} address.Public             Represents an address that is a standard address that may be subject to a switchboard or operator prior to reaching the intended entity.            
 * @property {OpenIZModel.EntityAddress} address.Soundex             Represents an address used for soundex matching purposes.            
 * @property {OpenIZModel.EntityAddress} address.Syllabic             Represents a syllabic address.            
 * @property {OpenIZModel.EntityAddress} address.TemporaryAddress             Represents a temporary address that may be good for visiting or mailing.            
 * @property {OpenIZModel.EntityAddress} address.VacationHome             Represents a vacation home to reach a person while on vacation.            
 * @property {OpenIZModel.EntityAddress} address.WorkPlace             Represents an office address, should be used for business communications            
 * @property {OpenIZModel.EntityAddress} address.$other Unclassified
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.EntityClassKeys} for values)
 * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
 * @property {uuid} creationAct            Creation act reference            
 * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
 * @property {uuid} determinerConcept            Determiner concept            (see: {@link OpenIZModel.DeterminerKeys} for values)
 * @property {object} extension            Gets a list of all extensions associated with the entity            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {object} identifier            Gets the identifiers associated with this entity            
 * @property {OpenIZModel.EntityIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} name            Gets a list of all names associated with the entity            
 * @property {OpenIZModel.EntityName} name.Alphabetic             The name used is an alphabetic representation of the name (ex: romaji in Japanese)            
 * @property {OpenIZModel.EntityName} name.Anonymous             The name is an anonymous name for the object (not the real name but a name used for care delivery)            
 * @property {OpenIZModel.EntityName} name.Artist             The name represents an artist name or stage name            
 * @property {OpenIZModel.EntityName} name.Assigned             The name represents an assigned name (given or bestowed by an authority)            
 * @property {OpenIZModel.EntityName} name.Ideographic             THe name represents an ideographic representation of the name            
 * @property {OpenIZModel.EntityName} name.Indigenous             The name is an indigenous name or tribal name for the patient            
 * @property {OpenIZModel.EntityName} name.Legal             The name represents the current legal name of an object (such as a corporate name)            
 * @property {OpenIZModel.EntityName} name.License             The name represents a name as displayed on a license or known to a license authority            
 * @property {OpenIZModel.EntityName} name.MaidenName             THe name is a maiden name (name of a patient before marriage)            
 * @property {OpenIZModel.EntityName} name.OfficialRecord             The name as it appears on an official record            
 * @property {OpenIZModel.EntityName} name.Phonetic             The name represents a phonetic representation of a name such as a SOUNDEX code            
 * @property {OpenIZModel.EntityName} name.Pseudonym             The name is a pseudonym for the object or an synonym name            
 * @property {OpenIZModel.EntityName} name.Religious             The name is to be used for religious purposes (such as baptismal name)            
 * @property {OpenIZModel.EntityName} name.Search             The name is to be used in the performing of matches only            
 * @property {OpenIZModel.EntityName} name.Soundex             The name represents the computed soundex code of a name            
 * @property {OpenIZModel.EntityName} name.Syllabic             The name represents a syllabic name.            
 * @property {OpenIZModel.EntityName} name.$other Unclassified
 * @property {string} note            Gets a list of all notes associated with the entity            
 * @property {object} participation            Gets the acts in which this entity participates            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {object} relationship            Gets a list of all associated entities for this entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Access             The access            
 * @property {OpenIZModel.EntityRelationship} relationship.ActiveMoiety             The active moiety            
 * @property {OpenIZModel.EntityRelationship} relationship.AdministerableMaterial             The administerable material            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedChild             The adopted child            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedDaughter             The adopted daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedSon             The adopted son            
 * @property {OpenIZModel.EntityRelationship} relationship.Affiliate             The affiliate            
 * @property {OpenIZModel.EntityRelationship} relationship.Agent             The agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Aliquot             The aliquot            
 * @property {OpenIZModel.EntityRelationship} relationship.Assigned             The assigned            
 * @property {OpenIZModel.EntityRelationship} relationship.AssignedEntity             The assigned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Aunt             The aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.Birthplace             The birthplace            
 * @property {OpenIZModel.EntityRelationship} relationship.Brother             The brother            
 * @property {OpenIZModel.EntityRelationship} relationship.Brotherinlaw             The brotherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Caregiver             The caregiver            
 * @property {OpenIZModel.EntityRelationship} relationship.CaseSubject             The case subject            
 * @property {OpenIZModel.EntityRelationship} relationship.Child             The child            
 * @property {OpenIZModel.EntityRelationship} relationship.ChildInlaw             The child inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Citizen             The citizen            
 * @property {OpenIZModel.EntityRelationship} relationship.Claimant             The claimant            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchInvestigator             The clinical research investigator            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchSponsor             The clinical research sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CommissioningParty             The commissioning party            
 * @property {OpenIZModel.EntityRelationship} relationship.Contact             The contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Cousin             The cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.CoverageSponsor             The coverage sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CoveredParty             The covered party            
 * @property {OpenIZModel.EntityRelationship} relationship.Daughter             The daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.DaughterInlaw             The daughter inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.DedicatedServiceDeliveryLocation             The dedicated service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Dependent             The dependent            
 * @property {OpenIZModel.EntityRelationship} relationship.DistributedMaterial             The distributed material            
 * @property {OpenIZModel.EntityRelationship} relationship.DomesticPartner             The domestic partner            
 * @property {OpenIZModel.EntityRelationship} relationship.EmergencyContact             The emergency contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Employee             The employee            
 * @property {OpenIZModel.EntityRelationship} relationship.ExposedEntity             The exposed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.FamilyMember             The family member            
 * @property {OpenIZModel.EntityRelationship} relationship.Father             The father            
 * @property {OpenIZModel.EntityRelationship} relationship.Fatherinlaw             The fatherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterChild             The foster child            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterDaughter             The foster daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterSon             The foster son            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandchild             The grandchild            
 * @property {OpenIZModel.EntityRelationship} relationship.Granddaughter             The granddaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandfather             The grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandmother             The grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandparent             The grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandson             The grandson            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandfather             The great grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandmother             The great grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandparent             The great grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Guarantor             The guarantor            
 * @property {OpenIZModel.EntityRelationship} relationship.GUARD             The guard            
 * @property {OpenIZModel.EntityRelationship} relationship.Guardian             The guardian            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfbrother             The halfbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsibling             The halfsibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsister             The halfsister            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthcareProvider             The healthcare provider            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthChart             The health chart            
 * @property {OpenIZModel.EntityRelationship} relationship.HeldEntity             The held entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Husband             The husband            
 * @property {OpenIZModel.EntityRelationship} relationship.IdentifiedEntity             The identified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.IncidentalServiceDeliveryLocation             The incidental service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Individual             The individual            
 * @property {OpenIZModel.EntityRelationship} relationship.InvestigationSubject             The investigation subject            
 * @property {OpenIZModel.EntityRelationship} relationship.InvoicePayor             The invoice payor            
 * @property {OpenIZModel.EntityRelationship} relationship.Isolate             The isolate            
 * @property {OpenIZModel.EntityRelationship} relationship.LicensedEntity             The licensed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.MaintainedEntity             The maintained entity            
 * @property {OpenIZModel.EntityRelationship} relationship.ManufacturedProduct             The manufactured product            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalAunt             The maternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalCousin             The maternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandfather             The maternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandmother             The maternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandparent             The maternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandfather             The maternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandmother             The maternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandparent             The maternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalUncle             The maternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.MilitaryPerson             The military person            
 * @property {OpenIZModel.EntityRelationship} relationship.Mother             The mother            
 * @property {OpenIZModel.EntityRelationship} relationship.Motherinlaw             The motherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.NamedInsured             The named insured            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalBrother             The natural brother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalChild             The natural child            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalDaughter             The natural daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFather             The natural father            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFatherOfFetus             The natural father of fetus            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalMother             The natural mother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalParent             The natural parent            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSibling             The natural sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSister             The natural sister            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSon             The natural son            
 * @property {OpenIZModel.EntityRelationship} relationship.Nephew             The nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NextOfKin             The next of kin            
 * @property {OpenIZModel.EntityRelationship} relationship.Niece             The niece            
 * @property {OpenIZModel.EntityRelationship} relationship.NieceNephew             The niece nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NotaryPublic             The notary public            
 * @property {OpenIZModel.EntityRelationship} relationship.OwnedEntity             The owned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Parent             The parent            
 * @property {OpenIZModel.EntityRelationship} relationship.ParentInlaw             The parent inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Part             The part            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalAunt             The paternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalCousin             The paternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandfather             The paternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandmother             The paternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandparent             The paternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandfather             The paternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandmother             The paternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandparent             The paternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalUncle             The paternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Patient             The patient            
 * @property {OpenIZModel.EntityRelationship} relationship.Payee             The payee            
 * @property {OpenIZModel.EntityRelationship} relationship.PersonalRelationship             The personal relationship            
 * @property {OpenIZModel.EntityRelationship} relationship.PlaceOfDeath             The place of death            
 * @property {OpenIZModel.EntityRelationship} relationship.PolicyHolder             The policy holder            
 * @property {OpenIZModel.EntityRelationship} relationship.ProgramEligible             The program eligible            
 * @property {OpenIZModel.EntityRelationship} relationship.QualifiedEntity             The qualified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.RegulatedProduct             The regulated product            
 * @property {OpenIZModel.EntityRelationship} relationship.ResearchSubject             The research subject            
 * @property {OpenIZModel.EntityRelationship} relationship.RetailedMaterial             The retailed material            
 * @property {OpenIZModel.EntityRelationship} relationship.Roomate             The roomate            
 * @property {OpenIZModel.EntityRelationship} relationship.ServiceDeliveryLocation             The service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Sibling             The sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.SiblingInlaw             The sibling inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.SignificantOther             The significant other            
 * @property {OpenIZModel.EntityRelationship} relationship.SigningAuthorityOrOfficer             The signing authority or officer            
 * @property {OpenIZModel.EntityRelationship} relationship.Sister             The sister            
 * @property {OpenIZModel.EntityRelationship} relationship.Sisterinlaw             The sisterinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Son             The son            
 * @property {OpenIZModel.EntityRelationship} relationship.SonInlaw             The son inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Specimen             The specimen            
 * @property {OpenIZModel.EntityRelationship} relationship.Spouse             The spouse            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepbrother             The stepbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepChild             The step child            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepdaughter             The stepdaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepfather             The stepfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepmother             The stepmother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepParent             The step parent            
 * @property {OpenIZModel.EntityRelationship} relationship.StepSibling             The step sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepsister             The stepsister            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepson             The stepson            
 * @property {OpenIZModel.EntityRelationship} relationship.Student             The student            
 * @property {OpenIZModel.EntityRelationship} relationship.Subscriber             The subscriber            
 * @property {OpenIZModel.EntityRelationship} relationship.TerritoryOfAuthority             The territory of authority            
 * @property {OpenIZModel.EntityRelationship} relationship.TherapeuticAgent             The therapeutic agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Uncle             The uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Underwriter             The underwriter            
 * @property {OpenIZModel.EntityRelationship} relationship.UsedEntity             The used entity            
 * @property {OpenIZModel.EntityRelationship} relationship.WarrantedProduct             The warranted product            
 * @property {OpenIZModel.EntityRelationship} relationship.Wife             The wife            
 * @property {OpenIZModel.EntityRelationship} relationship.Replaces             The replaces            
 * @property {OpenIZModel.EntityRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {object} tag            Gets a list of all tags associated with the entity            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {object} telecom            Gets a list of all telecommunications addresses associated with the entity            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.AnsweringService             answering service            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.EmergencyContact             Emergency contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.MobileContact             Mobile phone contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Pager             pager            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Public             public (800 number example) contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.TemporaryAddress             temporary contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.WorkPlace             For use in the workplace            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.$other Unclassified
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Organization} copyData Copy constructor (if present)
 */
Organization : function(copyData) { 
	this.$type = 'Organization';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.typeConcept = copyData.typeConcept;
	this.typeConceptModel = copyData.typeConceptModel;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.telecom = copyData.telecom;
	this.tag = copyData.tag;
	this.statusConcept = copyData.statusConcept;
	this.statusConceptModel = copyData.statusConceptModel;
	this.relationship = copyData.relationship;
	this.participation = copyData.participation;
	this.note = copyData.note;
	this.name = copyData.name;
	this.identifier = copyData.identifier;
	this.extension = copyData.extension;
	this.determinerConcept = copyData.determinerConcept;
	this.determinerConceptModel = copyData.determinerConceptModel;
	this.creationAct = copyData.creationAct;
	this.creationActModel = copyData.creationActModel;
	this.classConcept = copyData.classConcept;
	this.classConceptModel = copyData.classConceptModel;
	this.address = copyData.address;
	this.industryConcept = copyData.industryConcept;
	this.industryConceptModel = copyData.industryConceptModel;
	}
},  // Organization 
// OpenIZ.Core.Model.Entities.Person, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Entity
 * @summary             Represents an entity which is a person            
 * @property {OpenIZModel.DatePrecision} dateOfBirthPrecision            Gets or sets the precision ofthe date of birth            (see: {@link OpenIZModel.DatePrecision} for values)
 * @property {string} dateOfBirth            Gets the date of birth as XML            
 * @property {OpenIZModel.PersonLanguageCommunication} language            Gets the person's languages of communication            
 * @property {object} address            Gets a list of all addresses associated with the entity            
 * @property {OpenIZModel.EntityAddress} address.Alphabetic             Represents an alphabetic address used for matching             
 * @property {OpenIZModel.EntityAddress} address.BadAddress             Represents a bad address, i.e. an address which is old or invalid.            
 * @property {OpenIZModel.EntityAddress} address.Direct             Represents a workplace address that reaches the person directly without intermediaries.            
 * @property {OpenIZModel.EntityAddress} address.HomeAddress             The home address            
 * @property {OpenIZModel.EntityAddress} address.Ideographic             Represents an address expressed in an ideographic manner (example: Kaji)            
 * @property {OpenIZModel.EntityAddress} address.Phonetic             Represents an address expressed as a phonetic spelling of an ideographic address            
 * @property {OpenIZModel.EntityAddress} address.PhysicalVisit             The address is a physical place where visits should occur            
 * @property {OpenIZModel.EntityAddress} address.PostalAddress             The address is a postal address used for the delivery of mail and materials            
 * @property {OpenIZModel.EntityAddress} address.PrimaryHome             Represents a primary address to reach a contact after business hours.            
 * @property {OpenIZModel.EntityAddress} address.Public             Represents an address that is a standard address that may be subject to a switchboard or operator prior to reaching the intended entity.            
 * @property {OpenIZModel.EntityAddress} address.Soundex             Represents an address used for soundex matching purposes.            
 * @property {OpenIZModel.EntityAddress} address.Syllabic             Represents a syllabic address.            
 * @property {OpenIZModel.EntityAddress} address.TemporaryAddress             Represents a temporary address that may be good for visiting or mailing.            
 * @property {OpenIZModel.EntityAddress} address.VacationHome             Represents a vacation home to reach a person while on vacation.            
 * @property {OpenIZModel.EntityAddress} address.WorkPlace             Represents an office address, should be used for business communications            
 * @property {OpenIZModel.EntityAddress} address.$other Unclassified
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.EntityClassKeys} for values)
 * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
 * @property {uuid} creationAct            Creation act reference            
 * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
 * @property {uuid} determinerConcept            Determiner concept            (see: {@link OpenIZModel.DeterminerKeys} for values)
 * @property {object} extension            Gets a list of all extensions associated with the entity            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {object} identifier            Gets the identifiers associated with this entity            
 * @property {OpenIZModel.EntityIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} name            Gets a list of all names associated with the entity            
 * @property {OpenIZModel.EntityName} name.Alphabetic             The name used is an alphabetic representation of the name (ex: romaji in Japanese)            
 * @property {OpenIZModel.EntityName} name.Anonymous             The name is an anonymous name for the object (not the real name but a name used for care delivery)            
 * @property {OpenIZModel.EntityName} name.Artist             The name represents an artist name or stage name            
 * @property {OpenIZModel.EntityName} name.Assigned             The name represents an assigned name (given or bestowed by an authority)            
 * @property {OpenIZModel.EntityName} name.Ideographic             THe name represents an ideographic representation of the name            
 * @property {OpenIZModel.EntityName} name.Indigenous             The name is an indigenous name or tribal name for the patient            
 * @property {OpenIZModel.EntityName} name.Legal             The name represents the current legal name of an object (such as a corporate name)            
 * @property {OpenIZModel.EntityName} name.License             The name represents a name as displayed on a license or known to a license authority            
 * @property {OpenIZModel.EntityName} name.MaidenName             THe name is a maiden name (name of a patient before marriage)            
 * @property {OpenIZModel.EntityName} name.OfficialRecord             The name as it appears on an official record            
 * @property {OpenIZModel.EntityName} name.Phonetic             The name represents a phonetic representation of a name such as a SOUNDEX code            
 * @property {OpenIZModel.EntityName} name.Pseudonym             The name is a pseudonym for the object or an synonym name            
 * @property {OpenIZModel.EntityName} name.Religious             The name is to be used for religious purposes (such as baptismal name)            
 * @property {OpenIZModel.EntityName} name.Search             The name is to be used in the performing of matches only            
 * @property {OpenIZModel.EntityName} name.Soundex             The name represents the computed soundex code of a name            
 * @property {OpenIZModel.EntityName} name.Syllabic             The name represents a syllabic name.            
 * @property {OpenIZModel.EntityName} name.$other Unclassified
 * @property {string} note            Gets a list of all notes associated with the entity            
 * @property {object} participation            Gets the acts in which this entity participates            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {object} relationship            Gets a list of all associated entities for this entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Access             The access            
 * @property {OpenIZModel.EntityRelationship} relationship.ActiveMoiety             The active moiety            
 * @property {OpenIZModel.EntityRelationship} relationship.AdministerableMaterial             The administerable material            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedChild             The adopted child            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedDaughter             The adopted daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedSon             The adopted son            
 * @property {OpenIZModel.EntityRelationship} relationship.Affiliate             The affiliate            
 * @property {OpenIZModel.EntityRelationship} relationship.Agent             The agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Aliquot             The aliquot            
 * @property {OpenIZModel.EntityRelationship} relationship.Assigned             The assigned            
 * @property {OpenIZModel.EntityRelationship} relationship.AssignedEntity             The assigned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Aunt             The aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.Birthplace             The birthplace            
 * @property {OpenIZModel.EntityRelationship} relationship.Brother             The brother            
 * @property {OpenIZModel.EntityRelationship} relationship.Brotherinlaw             The brotherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Caregiver             The caregiver            
 * @property {OpenIZModel.EntityRelationship} relationship.CaseSubject             The case subject            
 * @property {OpenIZModel.EntityRelationship} relationship.Child             The child            
 * @property {OpenIZModel.EntityRelationship} relationship.ChildInlaw             The child inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Citizen             The citizen            
 * @property {OpenIZModel.EntityRelationship} relationship.Claimant             The claimant            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchInvestigator             The clinical research investigator            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchSponsor             The clinical research sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CommissioningParty             The commissioning party            
 * @property {OpenIZModel.EntityRelationship} relationship.Contact             The contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Cousin             The cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.CoverageSponsor             The coverage sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CoveredParty             The covered party            
 * @property {OpenIZModel.EntityRelationship} relationship.Daughter             The daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.DaughterInlaw             The daughter inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.DedicatedServiceDeliveryLocation             The dedicated service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Dependent             The dependent            
 * @property {OpenIZModel.EntityRelationship} relationship.DistributedMaterial             The distributed material            
 * @property {OpenIZModel.EntityRelationship} relationship.DomesticPartner             The domestic partner            
 * @property {OpenIZModel.EntityRelationship} relationship.EmergencyContact             The emergency contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Employee             The employee            
 * @property {OpenIZModel.EntityRelationship} relationship.ExposedEntity             The exposed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.FamilyMember             The family member            
 * @property {OpenIZModel.EntityRelationship} relationship.Father             The father            
 * @property {OpenIZModel.EntityRelationship} relationship.Fatherinlaw             The fatherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterChild             The foster child            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterDaughter             The foster daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterSon             The foster son            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandchild             The grandchild            
 * @property {OpenIZModel.EntityRelationship} relationship.Granddaughter             The granddaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandfather             The grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandmother             The grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandparent             The grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandson             The grandson            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandfather             The great grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandmother             The great grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandparent             The great grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Guarantor             The guarantor            
 * @property {OpenIZModel.EntityRelationship} relationship.GUARD             The guard            
 * @property {OpenIZModel.EntityRelationship} relationship.Guardian             The guardian            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfbrother             The halfbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsibling             The halfsibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsister             The halfsister            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthcareProvider             The healthcare provider            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthChart             The health chart            
 * @property {OpenIZModel.EntityRelationship} relationship.HeldEntity             The held entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Husband             The husband            
 * @property {OpenIZModel.EntityRelationship} relationship.IdentifiedEntity             The identified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.IncidentalServiceDeliveryLocation             The incidental service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Individual             The individual            
 * @property {OpenIZModel.EntityRelationship} relationship.InvestigationSubject             The investigation subject            
 * @property {OpenIZModel.EntityRelationship} relationship.InvoicePayor             The invoice payor            
 * @property {OpenIZModel.EntityRelationship} relationship.Isolate             The isolate            
 * @property {OpenIZModel.EntityRelationship} relationship.LicensedEntity             The licensed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.MaintainedEntity             The maintained entity            
 * @property {OpenIZModel.EntityRelationship} relationship.ManufacturedProduct             The manufactured product            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalAunt             The maternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalCousin             The maternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandfather             The maternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandmother             The maternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandparent             The maternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandfather             The maternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandmother             The maternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandparent             The maternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalUncle             The maternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.MilitaryPerson             The military person            
 * @property {OpenIZModel.EntityRelationship} relationship.Mother             The mother            
 * @property {OpenIZModel.EntityRelationship} relationship.Motherinlaw             The motherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.NamedInsured             The named insured            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalBrother             The natural brother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalChild             The natural child            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalDaughter             The natural daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFather             The natural father            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFatherOfFetus             The natural father of fetus            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalMother             The natural mother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalParent             The natural parent            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSibling             The natural sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSister             The natural sister            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSon             The natural son            
 * @property {OpenIZModel.EntityRelationship} relationship.Nephew             The nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NextOfKin             The next of kin            
 * @property {OpenIZModel.EntityRelationship} relationship.Niece             The niece            
 * @property {OpenIZModel.EntityRelationship} relationship.NieceNephew             The niece nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NotaryPublic             The notary public            
 * @property {OpenIZModel.EntityRelationship} relationship.OwnedEntity             The owned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Parent             The parent            
 * @property {OpenIZModel.EntityRelationship} relationship.ParentInlaw             The parent inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Part             The part            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalAunt             The paternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalCousin             The paternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandfather             The paternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandmother             The paternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandparent             The paternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandfather             The paternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandmother             The paternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandparent             The paternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalUncle             The paternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Patient             The patient            
 * @property {OpenIZModel.EntityRelationship} relationship.Payee             The payee            
 * @property {OpenIZModel.EntityRelationship} relationship.PersonalRelationship             The personal relationship            
 * @property {OpenIZModel.EntityRelationship} relationship.PlaceOfDeath             The place of death            
 * @property {OpenIZModel.EntityRelationship} relationship.PolicyHolder             The policy holder            
 * @property {OpenIZModel.EntityRelationship} relationship.ProgramEligible             The program eligible            
 * @property {OpenIZModel.EntityRelationship} relationship.QualifiedEntity             The qualified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.RegulatedProduct             The regulated product            
 * @property {OpenIZModel.EntityRelationship} relationship.ResearchSubject             The research subject            
 * @property {OpenIZModel.EntityRelationship} relationship.RetailedMaterial             The retailed material            
 * @property {OpenIZModel.EntityRelationship} relationship.Roomate             The roomate            
 * @property {OpenIZModel.EntityRelationship} relationship.ServiceDeliveryLocation             The service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Sibling             The sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.SiblingInlaw             The sibling inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.SignificantOther             The significant other            
 * @property {OpenIZModel.EntityRelationship} relationship.SigningAuthorityOrOfficer             The signing authority or officer            
 * @property {OpenIZModel.EntityRelationship} relationship.Sister             The sister            
 * @property {OpenIZModel.EntityRelationship} relationship.Sisterinlaw             The sisterinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Son             The son            
 * @property {OpenIZModel.EntityRelationship} relationship.SonInlaw             The son inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Specimen             The specimen            
 * @property {OpenIZModel.EntityRelationship} relationship.Spouse             The spouse            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepbrother             The stepbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepChild             The step child            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepdaughter             The stepdaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepfather             The stepfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepmother             The stepmother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepParent             The step parent            
 * @property {OpenIZModel.EntityRelationship} relationship.StepSibling             The step sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepsister             The stepsister            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepson             The stepson            
 * @property {OpenIZModel.EntityRelationship} relationship.Student             The student            
 * @property {OpenIZModel.EntityRelationship} relationship.Subscriber             The subscriber            
 * @property {OpenIZModel.EntityRelationship} relationship.TerritoryOfAuthority             The territory of authority            
 * @property {OpenIZModel.EntityRelationship} relationship.TherapeuticAgent             The therapeutic agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Uncle             The uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Underwriter             The underwriter            
 * @property {OpenIZModel.EntityRelationship} relationship.UsedEntity             The used entity            
 * @property {OpenIZModel.EntityRelationship} relationship.WarrantedProduct             The warranted product            
 * @property {OpenIZModel.EntityRelationship} relationship.Wife             The wife            
 * @property {OpenIZModel.EntityRelationship} relationship.Replaces             The replaces            
 * @property {OpenIZModel.EntityRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {object} tag            Gets a list of all tags associated with the entity            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {object} telecom            Gets a list of all telecommunications addresses associated with the entity            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.AnsweringService             answering service            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.EmergencyContact             Emergency contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.MobileContact             Mobile phone contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Pager             pager            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Public             public (800 number example) contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.TemporaryAddress             temporary contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.WorkPlace             For use in the workplace            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.$other Unclassified
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Person} copyData Copy constructor (if present)
 */
Person : function(copyData) { 
	this.$type = 'Person';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.typeConcept = copyData.typeConcept;
	this.typeConceptModel = copyData.typeConceptModel;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.telecom = copyData.telecom;
	this.tag = copyData.tag;
	this.statusConcept = copyData.statusConcept;
	this.statusConceptModel = copyData.statusConceptModel;
	this.relationship = copyData.relationship;
	this.participation = copyData.participation;
	this.note = copyData.note;
	this.name = copyData.name;
	this.identifier = copyData.identifier;
	this.extension = copyData.extension;
	this.determinerConcept = copyData.determinerConcept;
	this.determinerConceptModel = copyData.determinerConceptModel;
	this.creationAct = copyData.creationAct;
	this.creationActModel = copyData.creationActModel;
	this.classConcept = copyData.classConcept;
	this.classConceptModel = copyData.classConceptModel;
	this.address = copyData.address;
	this.language = copyData.language;
	this.dateOfBirth = copyData.dateOfBirth;
	this.dateOfBirthPrecision = copyData.dateOfBirthPrecision;
	}
},  // Person 
// OpenIZ.Core.Model.Entities.PersonLanguageCommunication, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.VersionedAssociation
 * @summary             Represents a single preferred communication method for the entity            
 * @property {bool} isPreferred            Gets or set the user's preference indicator            
 * @property {string} languageCode            Gets or sets the language code            
 * @property {number} effectiveVersionSequence
 * @property {number} obsoleteVersionSequence
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.PersonLanguageCommunication} copyData Copy constructor (if present)
 */
PersonLanguageCommunication : function(copyData) { 
	this.$type = 'PersonLanguageCommunication';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.languageCode = copyData.languageCode;
	this.isPreferred = copyData.isPreferred;
	}
},  // PersonLanguageCommunication 
// OpenIZ.Core.Model.Entities.Place, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Entity
 * @summary             An entity which is a place where healthcare services are delivered            
 * @property {uuid} classConcept            Gets or sets the class concept key            (see: {@link OpenIZModel.EntityClassKeys} for values)
 * @property {bool} isMobile            True if location is mobile            
 * @property {Double} lat            Gets or sets the latitude            
 * @property {Double} lng            Gets or sets the longitude            
 * @property {OpenIZModel.PlaceService} service            Gets the services            
 * @property {object} address            Gets a list of all addresses associated with the entity            
 * @property {OpenIZModel.EntityAddress} address.Alphabetic             Represents an alphabetic address used for matching             
 * @property {OpenIZModel.EntityAddress} address.BadAddress             Represents a bad address, i.e. an address which is old or invalid.            
 * @property {OpenIZModel.EntityAddress} address.Direct             Represents a workplace address that reaches the person directly without intermediaries.            
 * @property {OpenIZModel.EntityAddress} address.HomeAddress             The home address            
 * @property {OpenIZModel.EntityAddress} address.Ideographic             Represents an address expressed in an ideographic manner (example: Kaji)            
 * @property {OpenIZModel.EntityAddress} address.Phonetic             Represents an address expressed as a phonetic spelling of an ideographic address            
 * @property {OpenIZModel.EntityAddress} address.PhysicalVisit             The address is a physical place where visits should occur            
 * @property {OpenIZModel.EntityAddress} address.PostalAddress             The address is a postal address used for the delivery of mail and materials            
 * @property {OpenIZModel.EntityAddress} address.PrimaryHome             Represents a primary address to reach a contact after business hours.            
 * @property {OpenIZModel.EntityAddress} address.Public             Represents an address that is a standard address that may be subject to a switchboard or operator prior to reaching the intended entity.            
 * @property {OpenIZModel.EntityAddress} address.Soundex             Represents an address used for soundex matching purposes.            
 * @property {OpenIZModel.EntityAddress} address.Syllabic             Represents a syllabic address.            
 * @property {OpenIZModel.EntityAddress} address.TemporaryAddress             Represents a temporary address that may be good for visiting or mailing.            
 * @property {OpenIZModel.EntityAddress} address.VacationHome             Represents a vacation home to reach a person while on vacation.            
 * @property {OpenIZModel.EntityAddress} address.WorkPlace             Represents an office address, should be used for business communications            
 * @property {OpenIZModel.EntityAddress} address.$other Unclassified
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {OpenIZModel.Act} creationActModel [Delay loaded from creationAct],             Creation act reference            
 * @property {uuid} creationAct            Creation act reference            
 * @property {OpenIZModel.Concept} determinerConceptModel [Delay loaded from determinerConcept],             Determiner concept            
 * @property {uuid} determinerConcept            Determiner concept            (see: {@link OpenIZModel.DeterminerKeys} for values)
 * @property {object} extension            Gets a list of all extensions associated with the entity            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {object} identifier            Gets the identifiers associated with this entity            
 * @property {OpenIZModel.EntityIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} name            Gets a list of all names associated with the entity            
 * @property {OpenIZModel.EntityName} name.Alphabetic             The name used is an alphabetic representation of the name (ex: romaji in Japanese)            
 * @property {OpenIZModel.EntityName} name.Anonymous             The name is an anonymous name for the object (not the real name but a name used for care delivery)            
 * @property {OpenIZModel.EntityName} name.Artist             The name represents an artist name or stage name            
 * @property {OpenIZModel.EntityName} name.Assigned             The name represents an assigned name (given or bestowed by an authority)            
 * @property {OpenIZModel.EntityName} name.Ideographic             THe name represents an ideographic representation of the name            
 * @property {OpenIZModel.EntityName} name.Indigenous             The name is an indigenous name or tribal name for the patient            
 * @property {OpenIZModel.EntityName} name.Legal             The name represents the current legal name of an object (such as a corporate name)            
 * @property {OpenIZModel.EntityName} name.License             The name represents a name as displayed on a license or known to a license authority            
 * @property {OpenIZModel.EntityName} name.MaidenName             THe name is a maiden name (name of a patient before marriage)            
 * @property {OpenIZModel.EntityName} name.OfficialRecord             The name as it appears on an official record            
 * @property {OpenIZModel.EntityName} name.Phonetic             The name represents a phonetic representation of a name such as a SOUNDEX code            
 * @property {OpenIZModel.EntityName} name.Pseudonym             The name is a pseudonym for the object or an synonym name            
 * @property {OpenIZModel.EntityName} name.Religious             The name is to be used for religious purposes (such as baptismal name)            
 * @property {OpenIZModel.EntityName} name.Search             The name is to be used in the performing of matches only            
 * @property {OpenIZModel.EntityName} name.Soundex             The name represents the computed soundex code of a name            
 * @property {OpenIZModel.EntityName} name.Syllabic             The name represents a syllabic name.            
 * @property {OpenIZModel.EntityName} name.$other Unclassified
 * @property {string} note            Gets a list of all notes associated with the entity            
 * @property {object} participation            Gets the acts in which this entity participates            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {object} relationship            Gets a list of all associated entities for this entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Access             The access            
 * @property {OpenIZModel.EntityRelationship} relationship.ActiveMoiety             The active moiety            
 * @property {OpenIZModel.EntityRelationship} relationship.AdministerableMaterial             The administerable material            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedChild             The adopted child            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedDaughter             The adopted daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.AdoptedSon             The adopted son            
 * @property {OpenIZModel.EntityRelationship} relationship.Affiliate             The affiliate            
 * @property {OpenIZModel.EntityRelationship} relationship.Agent             The agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Aliquot             The aliquot            
 * @property {OpenIZModel.EntityRelationship} relationship.Assigned             The assigned            
 * @property {OpenIZModel.EntityRelationship} relationship.AssignedEntity             The assigned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Aunt             The aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.Birthplace             The birthplace            
 * @property {OpenIZModel.EntityRelationship} relationship.Brother             The brother            
 * @property {OpenIZModel.EntityRelationship} relationship.Brotherinlaw             The brotherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Caregiver             The caregiver            
 * @property {OpenIZModel.EntityRelationship} relationship.CaseSubject             The case subject            
 * @property {OpenIZModel.EntityRelationship} relationship.Child             The child            
 * @property {OpenIZModel.EntityRelationship} relationship.ChildInlaw             The child inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Citizen             The citizen            
 * @property {OpenIZModel.EntityRelationship} relationship.Claimant             The claimant            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchInvestigator             The clinical research investigator            
 * @property {OpenIZModel.EntityRelationship} relationship.ClinicalResearchSponsor             The clinical research sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CommissioningParty             The commissioning party            
 * @property {OpenIZModel.EntityRelationship} relationship.Contact             The contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Cousin             The cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.CoverageSponsor             The coverage sponsor            
 * @property {OpenIZModel.EntityRelationship} relationship.CoveredParty             The covered party            
 * @property {OpenIZModel.EntityRelationship} relationship.Daughter             The daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.DaughterInlaw             The daughter inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.DedicatedServiceDeliveryLocation             The dedicated service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Dependent             The dependent            
 * @property {OpenIZModel.EntityRelationship} relationship.DistributedMaterial             The distributed material            
 * @property {OpenIZModel.EntityRelationship} relationship.DomesticPartner             The domestic partner            
 * @property {OpenIZModel.EntityRelationship} relationship.EmergencyContact             The emergency contact            
 * @property {OpenIZModel.EntityRelationship} relationship.Employee             The employee            
 * @property {OpenIZModel.EntityRelationship} relationship.ExposedEntity             The exposed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.FamilyMember             The family member            
 * @property {OpenIZModel.EntityRelationship} relationship.Father             The father            
 * @property {OpenIZModel.EntityRelationship} relationship.Fatherinlaw             The fatherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterChild             The foster child            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterDaughter             The foster daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.FosterSon             The foster son            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandchild             The grandchild            
 * @property {OpenIZModel.EntityRelationship} relationship.Granddaughter             The granddaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandfather             The grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandmother             The grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandparent             The grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Grandson             The grandson            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandfather             The great grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandmother             The great grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.GreatGrandparent             The great grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.Guarantor             The guarantor            
 * @property {OpenIZModel.EntityRelationship} relationship.GUARD             The guard            
 * @property {OpenIZModel.EntityRelationship} relationship.Guardian             The guardian            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfbrother             The halfbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsibling             The halfsibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Halfsister             The halfsister            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthcareProvider             The healthcare provider            
 * @property {OpenIZModel.EntityRelationship} relationship.HealthChart             The health chart            
 * @property {OpenIZModel.EntityRelationship} relationship.HeldEntity             The held entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Husband             The husband            
 * @property {OpenIZModel.EntityRelationship} relationship.IdentifiedEntity             The identified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.IncidentalServiceDeliveryLocation             The incidental service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Individual             The individual            
 * @property {OpenIZModel.EntityRelationship} relationship.InvestigationSubject             The investigation subject            
 * @property {OpenIZModel.EntityRelationship} relationship.InvoicePayor             The invoice payor            
 * @property {OpenIZModel.EntityRelationship} relationship.Isolate             The isolate            
 * @property {OpenIZModel.EntityRelationship} relationship.LicensedEntity             The licensed entity            
 * @property {OpenIZModel.EntityRelationship} relationship.MaintainedEntity             The maintained entity            
 * @property {OpenIZModel.EntityRelationship} relationship.ManufacturedProduct             The manufactured product            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalAunt             The maternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalCousin             The maternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandfather             The maternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandmother             The maternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGrandparent             The maternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandfather             The maternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandmother             The maternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalGreatgrandparent             The maternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.MaternalUncle             The maternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.MilitaryPerson             The military person            
 * @property {OpenIZModel.EntityRelationship} relationship.Mother             The mother            
 * @property {OpenIZModel.EntityRelationship} relationship.Motherinlaw             The motherinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.NamedInsured             The named insured            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalBrother             The natural brother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalChild             The natural child            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalDaughter             The natural daughter            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFather             The natural father            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalFatherOfFetus             The natural father of fetus            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalMother             The natural mother            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalParent             The natural parent            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSibling             The natural sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSister             The natural sister            
 * @property {OpenIZModel.EntityRelationship} relationship.NaturalSon             The natural son            
 * @property {OpenIZModel.EntityRelationship} relationship.Nephew             The nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NextOfKin             The next of kin            
 * @property {OpenIZModel.EntityRelationship} relationship.Niece             The niece            
 * @property {OpenIZModel.EntityRelationship} relationship.NieceNephew             The niece nephew            
 * @property {OpenIZModel.EntityRelationship} relationship.NotaryPublic             The notary public            
 * @property {OpenIZModel.EntityRelationship} relationship.OwnedEntity             The owned entity            
 * @property {OpenIZModel.EntityRelationship} relationship.Parent             The parent            
 * @property {OpenIZModel.EntityRelationship} relationship.ParentInlaw             The parent inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Part             The part            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalAunt             The paternal aunt            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalCousin             The paternal cousin            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandfather             The paternal grandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandmother             The paternal grandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGrandparent             The paternal grandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandfather             The paternal greatgrandfather            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandmother             The paternal greatgrandmother            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalGreatgrandparent             The paternal greatgrandparent            
 * @property {OpenIZModel.EntityRelationship} relationship.PaternalUncle             The paternal uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Patient             The patient            
 * @property {OpenIZModel.EntityRelationship} relationship.Payee             The payee            
 * @property {OpenIZModel.EntityRelationship} relationship.PersonalRelationship             The personal relationship            
 * @property {OpenIZModel.EntityRelationship} relationship.PlaceOfDeath             The place of death            
 * @property {OpenIZModel.EntityRelationship} relationship.PolicyHolder             The policy holder            
 * @property {OpenIZModel.EntityRelationship} relationship.ProgramEligible             The program eligible            
 * @property {OpenIZModel.EntityRelationship} relationship.QualifiedEntity             The qualified entity            
 * @property {OpenIZModel.EntityRelationship} relationship.RegulatedProduct             The regulated product            
 * @property {OpenIZModel.EntityRelationship} relationship.ResearchSubject             The research subject            
 * @property {OpenIZModel.EntityRelationship} relationship.RetailedMaterial             The retailed material            
 * @property {OpenIZModel.EntityRelationship} relationship.Roomate             The roomate            
 * @property {OpenIZModel.EntityRelationship} relationship.ServiceDeliveryLocation             The service delivery location            
 * @property {OpenIZModel.EntityRelationship} relationship.Sibling             The sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.SiblingInlaw             The sibling inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.SignificantOther             The significant other            
 * @property {OpenIZModel.EntityRelationship} relationship.SigningAuthorityOrOfficer             The signing authority or officer            
 * @property {OpenIZModel.EntityRelationship} relationship.Sister             The sister            
 * @property {OpenIZModel.EntityRelationship} relationship.Sisterinlaw             The sisterinlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Son             The son            
 * @property {OpenIZModel.EntityRelationship} relationship.SonInlaw             The son inlaw            
 * @property {OpenIZModel.EntityRelationship} relationship.Specimen             The specimen            
 * @property {OpenIZModel.EntityRelationship} relationship.Spouse             The spouse            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepbrother             The stepbrother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepChild             The step child            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepdaughter             The stepdaughter            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepfather             The stepfather            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepmother             The stepmother            
 * @property {OpenIZModel.EntityRelationship} relationship.StepParent             The step parent            
 * @property {OpenIZModel.EntityRelationship} relationship.StepSibling             The step sibling            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepsister             The stepsister            
 * @property {OpenIZModel.EntityRelationship} relationship.Stepson             The stepson            
 * @property {OpenIZModel.EntityRelationship} relationship.Student             The student            
 * @property {OpenIZModel.EntityRelationship} relationship.Subscriber             The subscriber            
 * @property {OpenIZModel.EntityRelationship} relationship.TerritoryOfAuthority             The territory of authority            
 * @property {OpenIZModel.EntityRelationship} relationship.TherapeuticAgent             The therapeutic agent            
 * @property {OpenIZModel.EntityRelationship} relationship.Uncle             The uncle            
 * @property {OpenIZModel.EntityRelationship} relationship.Underwriter             The underwriter            
 * @property {OpenIZModel.EntityRelationship} relationship.UsedEntity             The used entity            
 * @property {OpenIZModel.EntityRelationship} relationship.WarrantedProduct             The warranted product            
 * @property {OpenIZModel.EntityRelationship} relationship.Wife             The wife            
 * @property {OpenIZModel.EntityRelationship} relationship.Replaces             The replaces            
 * @property {OpenIZModel.EntityRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {object} tag            Gets a list of all tags associated with the entity            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {object} telecom            Gets a list of all telecommunications addresses associated with the entity            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.AnsweringService             answering service            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.EmergencyContact             Emergency contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.MobileContact             Mobile phone contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Pager             pager            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.Public             public (800 number example) contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.TemporaryAddress             temporary contact            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.WorkPlace             For use in the workplace            
 * @property {OpenIZModel.EntityTelecomAddress} telecom.$other Unclassified
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Place} copyData Copy constructor (if present)
 */
Place : function(copyData) { 
	this.$type = 'Place';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.typeConcept = copyData.typeConcept;
	this.typeConceptModel = copyData.typeConceptModel;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.telecom = copyData.telecom;
	this.tag = copyData.tag;
	this.statusConcept = copyData.statusConcept;
	this.statusConceptModel = copyData.statusConceptModel;
	this.relationship = copyData.relationship;
	this.participation = copyData.participation;
	this.note = copyData.note;
	this.name = copyData.name;
	this.identifier = copyData.identifier;
	this.extension = copyData.extension;
	this.determinerConcept = copyData.determinerConcept;
	this.determinerConceptModel = copyData.determinerConceptModel;
	this.creationAct = copyData.creationAct;
	this.creationActModel = copyData.creationActModel;
	this.classConceptModel = copyData.classConceptModel;
	this.address = copyData.address;
	this.service = copyData.service;
	this.lng = copyData.lng;
	this.lat = copyData.lat;
	this.isMobile = copyData.isMobile;
	this.classConcept = copyData.classConcept;
	}
},  // Place 
// OpenIZ.Core.Model.Entities.PlaceService, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.VersionedAssociation
 * @summary             Represents a service for a place            
 * @property {OpenIZModel.Concept} serviceConceptModel [Delay loaded from serviceConcept],             Gets or sets the service concept            
 * @property {uuid} serviceConcept            Gets or sets the service concept key            
 * @property {Object} serviceSchedule            The schedule that the service is offered            
 * @property {number} effectiveVersionSequence
 * @property {number} obsoleteVersionSequence
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.PlaceService} copyData Copy constructor (if present)
 */
PlaceService : function(copyData) { 
	this.$type = 'PlaceService';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.serviceSchedule = copyData.serviceSchedule;
	this.serviceConcept = copyData.serviceConcept;
	this.serviceConceptModel = copyData.serviceConceptModel;
	}
},  // PlaceService 
// OpenIZ.Core.Model.DataTypes.AssigningAuthority, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @property {string} validation            Gets or sets the validation regex            
 * @property {bool} isUnique            True if the assigning authority values should be unique            
 * @property {object} scopeModel [Delay loaded from scope],             Gets concept sets to which this concept is a member            
 * @property {OpenIZModel.Concept} scope.classifier  where classifier is from {@link OpenIZModel.Concept} mnemonic
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.AssigningAuthority} copyData Copy constructor (if present)
 */
AssigningAuthority : function(copyData) { 
	this.$type = 'AssigningAuthority';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.scopeModel = copyData.scopeModel;
	this.isUnique = copyData.isUnique;
	this.validation = copyData.validation;
	this.assigningDevice = copyData.assigningDevice;
	this.scope = copyData.scope;
	this.url = copyData.url;
	this.oid = copyData.oid;
	this.description = copyData.description;
	this.domainName = copyData.domainName;
	this.name = copyData.name;
	}
},  // AssigningAuthority 
// OpenIZ.Core.Model.DataTypes.CodeSystem, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.CodeSystem} copyData Copy constructor (if present)
 */
CodeSystem : function(copyData) { 
	this.$type = 'CodeSystem';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.updatedBy = copyData.updatedBy;
	this.modifiedOn = copyData.modifiedOn;
	this.updatedTime = copyData.updatedTime;
	this.description = copyData.description;
	this.version = copyData.version;
	this.url = copyData.url;
	this.obsoletionReason = copyData.obsoletionReason;
	this.authority = copyData.authority;
	this.oid = copyData.oid;
	this.name = copyData.name;
	}
},  // CodeSystem 
// OpenIZ.Core.Model.DataTypes.Concept, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.VersionedEntityData
 * @summary             A class representing a generic concept used in the OpenIZ datamodel            
 * @property {bool} isReadonly            Gets or sets an indicator which dictates whether the concept is a system concept            
 * @property {string} mnemonic            Gets or sets the unchanging mnemonic for the concept            
 * @property {uuid} statusConcept            Gets or sets the status concept key            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Gets or sets the status of the concept            
 * @property {object} relationship            Gets a list of concept relationships            
 * @property {OpenIZModel.ConceptRelationship} relationship.InverseOf             The source concept has the inverse meaning of the target concept            
 * @property {OpenIZModel.ConceptRelationship} relationship.MemberOf             The source concept is a member of the target concept            
 * @property {OpenIZModel.ConceptRelationship} relationship.NegationOf             The source concept is a negation of the target concept            
 * @property {OpenIZModel.ConceptRelationship} relationship.SameAs             The source concept has the same meaning as the target concept            
 * @property {OpenIZModel.ConceptRelationship} relationship.$other Unclassified
 * @property {uuid} conceptClass            Gets or sets the class identifier            (see: {@link OpenIZModel.ConceptClassKeys} for values)
 * @property {OpenIZModel.ConceptClass} conceptClassModel [Delay loaded from conceptClass],             Gets or sets the classification of the concept            
 * @property {object} referenceTerm            Gets a list of concept reference terms            
 * @property {OpenIZModel.ConceptReferenceTerm} referenceTerm.classifier  where classifier is from {@link OpenIZModel.ConceptReferenceTerm} term
 * @property {object} name            Gets the concept names            
 * @property {string} name.classifier  where classifier is from {@link OpenIZModel.ConceptName} language
 * @property {uuid} conceptSet            Concept sets as identifiers for XML purposes only            
 * @property {object} conceptSetModel [Delay loaded from conceptSet],             Gets concept sets to which this concept is a member            
 * @property {OpenIZModel.ConceptSet} conceptSet.classifier  where classifier is from {@link OpenIZModel.ConceptSet} mnemonic
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Concept} copyData Copy constructor (if present)
 */
Concept : function(copyData) { 
	this.$type = 'Concept';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.conceptSetModel = copyData.conceptSetModel;
	this.conceptSet = copyData.conceptSet;
	this.name = copyData.name;
	this.referenceTerm = copyData.referenceTerm;
	this.conceptClassModel = copyData.conceptClassModel;
	this.conceptClass = copyData.conceptClass;
	this.relationship = copyData.relationship;
	this.statusConceptModel = copyData.statusConceptModel;
	this.statusConcept = copyData.statusConcept;
	this.mnemonic = copyData.mnemonic;
	this.isReadonly = copyData.isReadonly;
	}
},  // Concept 
// OpenIZ.Core.Model.DataTypes.ConceptClass, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ConceptClass} copyData Copy constructor (if present)
 */
ConceptClass : function(copyData) { 
	this.$type = 'ConceptClass';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.updatedBy = copyData.updatedBy;
	this.modifiedOn = copyData.modifiedOn;
	this.updatedTime = copyData.updatedTime;
	this.mnemonic = copyData.mnemonic;
	this.name = copyData.name;
	}
},  // ConceptClass 
// OpenIZ.Core.Model.DataTypes.ConceptName, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.VersionedAssociation
 * @summary             Represents a name (human name) that a concept may have            
 * @property {string} language            Gets or sets the language code of the object            
 * @property {string} value            Gets or sets the name of the reference term            
 * @property {string} phoneticCode            Gets or sets the phonetic code of the reference term            
 * @property {uuid} phoneticAlgorithm            Gets or sets the identifier of the phonetic code            (see: {@link OpenIZModel.PhoneticAlgorithmKeys} for values)
 * @property {OpenIZModel.PhoneticAlgorithm} phoneticAlgorithmModel [Delay loaded from phoneticAlgorithm],             Gets or sets the phonetic algorithm            
 * @property {number} effectiveVersionSequence
 * @property {number} obsoleteVersionSequence
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.Concept} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ConceptName} copyData Copy constructor (if present)
 */
ConceptName : function(copyData) { 
	this.$type = 'ConceptName';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.phoneticAlgorithmModel = copyData.phoneticAlgorithmModel;
	this.phoneticAlgorithm = copyData.phoneticAlgorithm;
	this.phoneticCode = copyData.phoneticCode;
	this.value = copyData.value;
	this.language = copyData.language;
	}
},  // ConceptName 
// OpenIZ.Core.Model.DataTypes.ConceptReferenceTerm, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @param {OpenIZModel.ConceptReferenceTerm} copyData Copy constructor (if present)
 */
ConceptReferenceTerm : function(copyData) { 
	this.$type = 'ConceptReferenceTerm';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.relationshipTypeModel = copyData.relationshipTypeModel;
	this.relationshipType = copyData.relationshipType;
	this.termModel = copyData.termModel;
	this.term = copyData.term;
	}
},  // ConceptReferenceTerm 
// OpenIZ.Core.Model.DataTypes.ConceptRelationship, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.VersionedAssociation
 * @summary             Represents a relationship between two concepts            
 * @property {uuid} targetConcept            Gets or sets the target concept identifier            
 * @property {OpenIZModel.Concept} targetConceptModel [Delay loaded from targetConcept],             Gets or sets the target concept            
 * @property {uuid} relationshipType            Relationship type            (see: {@link OpenIZModel.ConceptRelationshipTypeKeys} for values)
 * @property {OpenIZModel.ConceptRelationshipType} relationshipTypeModel [Delay loaded from relationshipType],             Gets or sets the relationship type            
 * @property {number} effectiveVersionSequence
 * @property {number} obsoleteVersionSequence
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.Concept} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ConceptRelationship} copyData Copy constructor (if present)
 */
ConceptRelationship : function(copyData) { 
	this.$type = 'ConceptRelationship';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.relationshipTypeModel = copyData.relationshipTypeModel;
	this.relationshipType = copyData.relationshipType;
	this.targetConceptModel = copyData.targetConceptModel;
	this.targetConcept = copyData.targetConcept;
	}
},  // ConceptRelationship 
// OpenIZ.Core.Model.DataTypes.ConceptRelationshipType, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ConceptRelationshipType} copyData Copy constructor (if present)
 */
ConceptRelationshipType : function(copyData) { 
	this.$type = 'ConceptRelationshipType';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.updatedBy = copyData.updatedBy;
	this.modifiedOn = copyData.modifiedOn;
	this.updatedTime = copyData.updatedTime;
	this.mnemonic = copyData.mnemonic;
	this.name = copyData.name;
	}
},  // ConceptRelationshipType 
// OpenIZ.Core.Model.DataTypes.ConceptSet, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.NonVersionedEntityData
 * @summary             Represents set of concepts            
 * @property {string} name            Gets or sets the name of the concept set            
 * @property {string} mnemonic            Gets or sets the mnemonic for the concept set (used for convenient lookup)            
 * @property {string} oid            Gets or sets the oid of the concept set            
 * @property {string} url            Gets or sets the url of the concept set            
 * @property {uuid} concept            Concepts as identifiers for XML purposes only            
 * @property {string} updatedTime            Gets or sets the creation time in XML format            
 * @property {date} modifiedOn            Gets the time this item was modified            
 * @property {uuid} updatedBy            Gets or sets the created by identifier            
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ConceptSet} copyData Copy constructor (if present)
 */
ConceptSet : function(copyData) { 
	this.$type = 'ConceptSet';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.updatedBy = copyData.updatedBy;
	this.modifiedOn = copyData.modifiedOn;
	this.updatedTime = copyData.updatedTime;
	this.concept = copyData.concept;
	this.url = copyData.url;
	this.oid = copyData.oid;
	this.mnemonic = copyData.mnemonic;
	this.name = copyData.name;
	}
},  // ConceptSet 
// OpenIZ.Core.Model.DataTypes.Extension`1, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @abstract
 * @extends OpenIZModel.VersionedAssociation
 * @summary             Represents a base entity extension            
 * @property {bytea} value            Gets or sets the value of the extension            
 * @property {string} valueModel [Delay loaded from value],             Value as string of bytes            
 * @property {uuid} extensionType            Gets or sets the extension type key            
 * @property {OpenIZModel.ExtensionType} extensionTypeModel [Delay loaded from extensionType],             Gets or sets the extension type            
 * @property {number} effectiveVersionSequence
 * @property {number} obsoleteVersionSequence
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.VersionedEntityData} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Extension} copyData Copy constructor (if present)
 */
Extension : function(copyData) { 
	this.$type = 'Extension';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.extensionTypeModel = copyData.extensionTypeModel;
	this.extensionType = copyData.extensionType;
	this.valueModel = copyData.valueModel;
	this.value = copyData.value;
	}
},  // Extension 
// OpenIZ.Core.Model.DataTypes.EntityExtension, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Extension
 * @summary             Extension bound to entity            
 * @property {bytea} value
 * @property {string} valueModel [Delay loaded from value], 
 * @property {uuid} extensionType
 * @property {OpenIZModel.ExtensionType} extensionTypeModel [Delay loaded from extensionType], 
 * @property {number} effectiveVersionSequence
 * @property {number} obsoleteVersionSequence
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.Entity} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.EntityExtension} copyData Copy constructor (if present)
 */
EntityExtension : function(copyData) { 
	this.$type = 'EntityExtension';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.extensionTypeModel = copyData.extensionTypeModel;
	this.extensionType = copyData.extensionType;
	this.valueModel = copyData.valueModel;
	this.value = copyData.value;
	}
},  // EntityExtension 
// OpenIZ.Core.Model.DataTypes.ActExtension, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Extension
 * @summary             Act extension            
 * @property {bytea} value
 * @property {string} valueModel [Delay loaded from value], 
 * @property {uuid} extensionType
 * @property {OpenIZModel.ExtensionType} extensionTypeModel [Delay loaded from extensionType], 
 * @property {number} effectiveVersionSequence
 * @property {number} obsoleteVersionSequence
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.Act} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ActExtension} copyData Copy constructor (if present)
 */
ActExtension : function(copyData) { 
	this.$type = 'ActExtension';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.extensionTypeModel = copyData.extensionTypeModel;
	this.extensionType = copyData.extensionType;
	this.valueModel = copyData.valueModel;
	this.value = copyData.value;
	}
},  // ActExtension 
// OpenIZ.Core.Model.DataTypes.ExtensionType, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ExtensionType} copyData Copy constructor (if present)
 */
ExtensionType : function(copyData) { 
	this.$type = 'ExtensionType';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.updatedBy = copyData.updatedBy;
	this.modifiedOn = copyData.modifiedOn;
	this.updatedTime = copyData.updatedTime;
	this.name = copyData.name;
	this.handlerClass = copyData.handlerClass;
	}
},  // ExtensionType 
// OpenIZ.Core.Model.DataTypes.EntityIdentifier, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @param {OpenIZModel.EntityIdentifier} copyData Copy constructor (if present)
 */
EntityIdentifier : function(copyData) { 
	this.$type = 'EntityIdentifier';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.authority = copyData.authority;
	this.type = copyData.type;
	this.value = copyData.value;
	}
},  // EntityIdentifier 
// OpenIZ.Core.Model.DataTypes.ActIdentifier, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @param {OpenIZModel.ActIdentifier} copyData Copy constructor (if present)
 */
ActIdentifier : function(copyData) { 
	this.$type = 'ActIdentifier';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.authority = copyData.authority;
	this.type = copyData.type;
	this.value = copyData.value;
	}
},  // ActIdentifier 
// OpenIZ.Core.Model.DataTypes.IdentifierBase`1, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @abstract
 * @extends OpenIZModel.VersionedAssociation
 * @summary             Represents an external assigned identifier            
 * @property {string} value            Gets or sets the value of the identifier            
 * @property {OpenIZModel.IdentifierType} type            Gets or sets the identifier type            
 * @property {OpenIZModel.AssigningAuthority} authority            Gets or sets a minimal assigning authority from XML data            
 * @property {number} effectiveVersionSequence
 * @property {number} obsoleteVersionSequence
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.VersionedEntityData} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.IdentifierBase} copyData Copy constructor (if present)
 */
IdentifierBase : function(copyData) { 
	this.$type = 'IdentifierBase';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.authority = copyData.authority;
	this.type = copyData.type;
	this.value = copyData.value;
	}
},  // IdentifierBase 
// OpenIZ.Core.Model.DataTypes.IdentifierType, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.IdentifierType} copyData Copy constructor (if present)
 */
IdentifierType : function(copyData) { 
	this.$type = 'IdentifierType';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.scopeConceptModel = copyData.scopeConceptModel;
	this.typeConceptModel = copyData.typeConceptModel;
	this.typeConcept = copyData.typeConcept;
	this.scopeConcept = copyData.scopeConcept;
	}
},  // IdentifierType 
// OpenIZ.Core.Model.DataTypes.Note`1, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @param {OpenIZModel.Note} copyData Copy constructor (if present)
 */
Note : function(copyData) { 
	this.$type = 'Note';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.authorModel = copyData.authorModel;
	this.author = copyData.author;
	this.text = copyData.text;
	}
},  // Note 
// OpenIZ.Core.Model.DataTypes.EntityNote, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @param {OpenIZModel.EntityNote} copyData Copy constructor (if present)
 */
EntityNote : function(copyData) { 
	this.$type = 'EntityNote';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.authorModel = copyData.authorModel;
	this.author = copyData.author;
	this.text = copyData.text;
	}
},  // EntityNote 
// OpenIZ.Core.Model.DataTypes.ActNote, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @param {OpenIZModel.ActNote} copyData Copy constructor (if present)
 */
ActNote : function(copyData) { 
	this.$type = 'ActNote';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.authorModel = copyData.authorModel;
	this.author = copyData.author;
	this.text = copyData.text;
	}
},  // ActNote 
// OpenIZ.Core.Model.DataTypes.PhoneticAlgorithm, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.PhoneticAlgorithm} copyData Copy constructor (if present)
 */
PhoneticAlgorithm : function(copyData) { 
	this.$type = 'PhoneticAlgorithm';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.updatedBy = copyData.updatedBy;
	this.modifiedOn = copyData.modifiedOn;
	this.updatedTime = copyData.updatedTime;
	this.handler = copyData.handler;
	this.name = copyData.name;
	}
},  // PhoneticAlgorithm 
// OpenIZ.Core.Model.DataTypes.ReferenceTerm, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.NonVersionedEntityData
 * @summary             Represents a basic reference term            
 * @property {string} mnemonic            Gets or sets the mnemonic for the reference term            
 * @property {OpenIZModel.CodeSystem} codeSystemModel [Delay loaded from codeSystem],             Gets or sets the code system             
 * @property {uuid} codeSystem            Gets or sets the code system identifier            (see: {@link OpenIZModel.CodeSystemKeys} for values)
 * @property {object} name            Gets display names associated with the reference term            
 * @property {string} name.classifier  where classifier is from {@link OpenIZModel.ReferenceTermName} language
 * @property {string} updatedTime            Gets or sets the creation time in XML format            
 * @property {date} modifiedOn            Gets the time this item was modified            
 * @property {uuid} updatedBy            Gets or sets the created by identifier            
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ReferenceTerm} copyData Copy constructor (if present)
 */
ReferenceTerm : function(copyData) { 
	this.$type = 'ReferenceTerm';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.updatedBy = copyData.updatedBy;
	this.modifiedOn = copyData.modifiedOn;
	this.updatedTime = copyData.updatedTime;
	this.name = copyData.name;
	this.codeSystem = copyData.codeSystem;
	this.codeSystemModel = copyData.codeSystemModel;
	this.mnemonic = copyData.mnemonic;
	}
},  // ReferenceTerm 
// OpenIZ.Core.Model.DataTypes.ReferenceTermName, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.BaseEntityData
 * @summary             Display name of a code system or reference term            
 * @property {string} language            Gets or sets the language code of the object            
 * @property {string} value            Gets or sets the name of the reference term            
 * @property {string} phoneticCode            Gets or sets the phonetic code of the reference term            
 * @property {uuid} phoneticAlgorithm            Gets or sets the identifier of the phonetic code            (see: {@link OpenIZModel.PhoneticAlgorithmKeys} for values)
 * @property {OpenIZModel.PhoneticAlgorithm} phoneticAlgorithmModel [Delay loaded from phoneticAlgorithm],             Gets or sets the phonetic algorithm            
 * @property {uuid} source            Gets the source entity key            
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ReferenceTermName} copyData Copy constructor (if present)
 */
ReferenceTermName : function(copyData) { 
	this.$type = 'ReferenceTermName';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.source = copyData.source;
	this.phoneticAlgorithmModel = copyData.phoneticAlgorithmModel;
	this.phoneticAlgorithm = copyData.phoneticAlgorithm;
	this.phoneticCode = copyData.phoneticCode;
	this.value = copyData.value;
	this.language = copyData.language;
	}
},  // ReferenceTermName 
// OpenIZ.Core.Model.DataTypes.Tag`1, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Tag} copyData Copy constructor (if present)
 */
Tag : function(copyData) { 
	this.$type = 'Tag';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.value = copyData.value;
	this.key = copyData.key;
	}
},  // Tag 
// OpenIZ.Core.Model.DataTypes.EntityTag, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.EntityTag} copyData Copy constructor (if present)
 */
EntityTag : function(copyData) { 
	this.$type = 'EntityTag';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.value = copyData.value;
	this.key = copyData.key;
	}
},  // EntityTag 
// OpenIZ.Core.Model.DataTypes.ActTag, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ActTag} copyData Copy constructor (if present)
 */
ActTag : function(copyData) { 
	this.$type = 'ActTag';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.value = copyData.value;
	this.key = copyData.key;
	}
},  // ActTag 
// OpenIZ.Core.Model.DataTypes.TemplateDefinition, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.TemplateDefinition} copyData Copy constructor (if present)
 */
TemplateDefinition : function(copyData) { 
	this.$type = 'TemplateDefinition';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.updatedBy = copyData.updatedBy;
	this.modifiedOn = copyData.modifiedOn;
	this.updatedTime = copyData.updatedTime;
	this.description = copyData.description;
	this.oid = copyData.oid;
	this.name = copyData.name;
	this.mnemonic = copyData.mnemonic;
	}
},  // TemplateDefinition 
// OpenIZ.Core.Model.Collection.Bundle, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @param {OpenIZModel.Bundle} copyData Copy constructor (if present)
 */
Bundle : function(copyData) { 
	this.$type = 'Bundle';
	if(copyData) {
	this.id = copyData.id;
	this.totalResults = copyData.totalResults;
	this.count = copyData.count;
	this.offset = copyData.offset;
	this.entry = copyData.entry;
	this.item = copyData.item;
	this.modifiedOn = copyData.modifiedOn;
	}
},  // Bundle 
// OpenIZ.Core.Model.Acts.Act, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.VersionedEntityData
 * @summary             Represents the base class for an act            
 * @description 
 *             An Act, in the context of the reference information model (RIM) represents something that is done to a patient. More precisely, an Act
 *             is anything that occurs involving entities in which the entity's state is changed or is documented.
 *             
 *             Examples of Acts Include:
 *              - The administration of a substance to a patient - The observing of a value for the patient - An encounter or visit that occurs where the patient receives one or more services - Any other action such as supply request, or problem recordation
 *             The property which classifies what specific type of action an act represents is its , which dictates
 *             what type an act is. Class concept keys can be found in here .
 *             
 *             This structure is used to represent events, proposals, and requests. That is to say, the Act structure can represent the request to 
 *             do an act, the intent to perform an act, or the actual act being performed itself. This classification of mode happens based on the 
 *              mood concept. Mood concept keys can be found on the  structure.
 *             
 * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {string} actTime            Gets or sets the creation time in XML format            
 * @property {string} startTime            Gets or sets the creation time in XML format            
 * @property {string} stopTime            Gets or sets the creation time in XML format            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.ActClassKeys} for values)
 * @property {uuid} moodConcept            Mood concept            (see: {@link OpenIZModel.ActMoodKeys} for values)
 * @property {uuid} reasonConcept            Reason concept            (see: {@link OpenIZModel.ActReasonKeys} for values)
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {object} identifier            Gets the identifiers associated with this act            
 * @property {OpenIZModel.ActIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} relationship            Gets a list of all associated acts for this act            
 * @property {OpenIZModel.ActRelationship} relationship.Appends             Indicates that the source act appends information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Arrival             Links the transortation act from another act            
 * @property {OpenIZModel.ActRelationship} relationship.Departure             Links a transporation act from another act indicating departure of the subject            
 * @property {OpenIZModel.ActRelationship} relationship.Documents             The source act documents the target act            
 * @property {OpenIZModel.ActRelationship} relationship.EpisodeLink             Links two instances of the same act over time (example: chronic conditions)            
 * @property {OpenIZModel.ActRelationship} relationship.Evaluates             Used to link a goal to an observation            
 * @property {OpenIZModel.ActRelationship} relationship.Fulfills             Indicates that the source act fulfills the target act            
 * @property {OpenIZModel.ActRelationship} relationship.HasAuthorization             Indicates that the target act authorizes the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasComponent             Indicates that the target act is a component of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasControlVariable             Relationship from an act to one or more control variables (for example: device settings, or environment)            
 * @property {OpenIZModel.ActRelationship} relationship.HasManifestation             The assertion that a new observation may be a manifestation of another            
 * @property {OpenIZModel.ActRelationship} relationship.HasPrecondition             Indicates that the target act is a pre-condition of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasReason             Indicates a reasoning as to why the source act is occurring            
 * @property {OpenIZModel.ActRelationship} relationship.HasReferenceValues             Indicates that the source act contains reference values from the target            
 * @property {OpenIZModel.ActRelationship} relationship.HasSubject             Indicates the subject of a particular act (example: clinical act is a subject of a control act)            
 * @property {OpenIZModel.ActRelationship} relationship.HasSupport             Indicates an existing act is suggesting evidence for a new observation.            
 * @property {OpenIZModel.ActRelationship} relationship.IsCauseOf             Indicates that the source act is the cause of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsDerivedFrom             Indicates the source act is derived from information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsExcerptOf             Indicates that the source act is an excerpt of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.RefersTo             Indicates that the source act refers to the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Replaces             The source act replaces the target act            
 * @property {OpenIZModel.ActRelationship} relationship.StartsAfterStartOf             Indicates that the source act starts after the start of another act            
 * @property {OpenIZModel.ActRelationship} relationship.Transforms             Indicates that the source act transforms the target act            
 * @property {OpenIZModel.ActRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
 * @property {object} extension            Gets a list of all extensions associated with the act            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {string} note            Gets a list of all notes associated with the act            
 * @property {object} tag            Gets a list of all tags associated with the act            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {OpenIZModel.ActProtocol} protocol            Identifies protocols attached to the act            
 * @property {object} participation            Participations            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Act} copyData Copy constructor (if present)
 */
Act : function(copyData) { 
	this.$type = 'Act';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.participation = copyData.participation;
	this.protocol = copyData.protocol;
	this.tag = copyData.tag;
	this.note = copyData.note;
	this.extension = copyData.extension;
	this.policy = copyData.policy;
	this.relationship = copyData.relationship;
	this.identifier = copyData.identifier;
	this.typeConceptModel = copyData.typeConceptModel;
	this.statusConceptModel = copyData.statusConceptModel;
	this.reasonConceptModel = copyData.reasonConceptModel;
	this.moodConceptModel = copyData.moodConceptModel;
	this.classConceptModel = copyData.classConceptModel;
	this.typeConcept = copyData.typeConcept;
	this.statusConcept = copyData.statusConcept;
	this.reasonConcept = copyData.reasonConcept;
	this.moodConcept = copyData.moodConcept;
	this.classConcept = copyData.classConcept;
	this.stopTime = copyData.stopTime;
	this.startTime = copyData.startTime;
	this.actTime = copyData.actTime;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.isNegated = copyData.isNegated;
	}
},  // Act 
// OpenIZ.Core.Model.Acts.ActParticipation, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.VersionedAssociation
 * @summary             Associates an entity which participates in an act            
 * @description 
 *             An act participation instance is used to link an  entity instance to an  act instance. It is said that the
 *             player  participates in the act  in a particular role .
 *             
 *             Act participations can also be quantified. For example, if 100 doses of a particlar material  were consumed
 *             as part of an act, then the quantity would be 100.
 *             
 * @property {uuid} player            Gets or sets the target entity reference            
 * @property {uuid} participationRole            Gets or sets the participation role key            (see: {@link OpenIZModel.ActParticipationKey} for values)
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
 * @param {OpenIZModel.ActParticipation} copyData Copy constructor (if present)
 */
ActParticipation : function(copyData) { 
	this.$type = 'ActParticipation';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.quantity = copyData.quantity;
	this.actModel = copyData.actModel;
	this.act = copyData.act;
	this.participationRoleModel = copyData.participationRoleModel;
	this.playerModel = copyData.playerModel;
	this.participationRole = copyData.participationRole;
	this.player = copyData.player;
	}
},  // ActParticipation 
// OpenIZ.Core.Model.Acts.ActProtocol, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Association
 * @summary             Represents information related to the clinical protocol to which an act is a member of            
 * @description 
 *             The  class is used to link an act instance  with the clinical 
 *             protocol  to which the act belongs.
 *             
 * @property {uuid} protocol            Gets or sets the protocol  to which this act belongs            
 * @property {OpenIZModel.Protocol} protocolModel [Delay loaded from protocol],             Gets or sets the protocol data related to the protocol            
 * @property {number} sequence            Represents the sequence of the act in the protocol            
 * @property {bytea} state            Represents any state data related to the act / protocol link            
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.Act} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ActProtocol} copyData Copy constructor (if present)
 */
ActProtocol : function(copyData) { 
	this.$type = 'ActProtocol';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.state = copyData.state;
	this.sequence = copyData.sequence;
	this.protocolModel = copyData.protocolModel;
	this.protocol = copyData.protocol;
	}
},  // ActProtocol 
// OpenIZ.Core.Model.Acts.ActRelationship, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.VersionedAssociation
 * @summary             Used to link two or more acts together            
 * @description 
 *             An act relationship is used to link a source act with a target act  in a particular type of 
 *             relationship . This structure is often used to link together sub-components of an 
 *             encounter with the encounter, but can also be used to link together chronic care episodes.
 *             
 * @property {uuid} target            The target of the association            
 * @property {OpenIZModel.Act} targetModel [Delay loaded from target],             Target act reference            
 * @property {uuid} relationshipType            Association type key            (see: {@link OpenIZModel.ActRelationshipTypeKeys} for values)
 * @property {OpenIZModel.Concept} relationshipTypeModel [Delay loaded from relationshipType],             Gets or sets the association type            
 * @property {number} effectiveVersionSequence
 * @property {number} obsoleteVersionSequence
 * @property {date} modifiedOn
 * @property {uuid} source
 * @property {OpenIZModel.Act} sourceModel [Delay loaded from source], 
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ActRelationship} copyData Copy constructor (if present)
 */
ActRelationship : function(copyData) { 
	this.$type = 'ActRelationship';
	if(copyData) {
	this.id = copyData.id;
	this.sourceModel = copyData.sourceModel;
	this.source = copyData.source;
	this.modifiedOn = copyData.modifiedOn;
	this.obsoleteVersionSequence = copyData.obsoleteVersionSequence;
	this.effectiveVersionSequence = copyData.effectiveVersionSequence;
	this.relationshipTypeModel = copyData.relationshipTypeModel;
	this.relationshipType = copyData.relationshipType;
	this.targetModel = copyData.targetModel;
	this.target = copyData.target;
	}
},  // ActRelationship 
// OpenIZ.Core.Model.Acts.CarePlan, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.BaseEntityData
 * @summary             Represents a care plan            
 * @description 
 *             The care plan object is used to represent a collection of clinical protocols which the care planning
 *             engine proposes should be done as part of the patient's course of care.
 *             
 * @property {OpenIZModel.Patient} target            Target of the careplan            
 * @property {object} act            Action to take            
 * @property {OpenIZModel.Act} act.AccountManagement             The act represents generic account management such as adjudications, financial adjustments, stock counting, etc.            
 * @property {OpenIZModel.Act} act.Act             The act represents a generic act which has no special classification            
 * @property {OpenIZModel.Act} act.Battery             The act represents a simple battery of procedures/administrations/tests/etc.            
 * @property {OpenIZModel.Act} act.CareProvision             The act represents some provision of care such as the seeking out services.            
 * @property {OpenIZModel.Act} act.Condition             The act represents a problem or condition which the patient is suffering from.            
 * @property {OpenIZModel.Act} act.ControlAct             The control act event key is used to describe an infrastructural act which has no clinical meaning but can be used to wrap technical details.            
 * @property {OpenIZModel.Act} act.Encounter             The act represents an encounter such as the patient presenting for care and receiving services during a visit.            
 * @property {OpenIZModel.Act} act.Inform             The act represents an attempt to provide additional clinical information.            
 * @property {OpenIZModel.Act} act.Observation             The act represents an observation that is made about a patient such as a vital sign, an allergy, cause of death, etc..            
 * @property {OpenIZModel.Act} act.Procedure             The act represents a procedure (something done to a patient).            
 * @property {OpenIZModel.Act} act.Registration             The act represents a registration event such as the registration of a patient.            
 * @property {OpenIZModel.Act} act.SubstanceAdministration             The act represents that a substance (medication, or otherwise) was, should, or will be administered to the patient.            
 * @property {OpenIZModel.Act} act.Supply             The act represents a supply of some material or financial instrument between entities.            
 * @property {OpenIZModel.Act} act.Transport             The physical transporting of materials or people from one place to another.            
 * @property {OpenIZModel.Act} act.$other Unclassified
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.CarePlan} copyData Copy constructor (if present)
 */
CarePlan : function(copyData) { 
	this.$type = 'CarePlan';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.act = copyData.act;
	this.target = copyData.target;
	}
},  // CarePlan 
// OpenIZ.Core.Model.Acts.ControlAct, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Act
 * @summary             Represents an act which indicates why data was created/changed            
 * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {string} actTime            Gets or sets the creation time in XML format            
 * @property {string} startTime            Gets or sets the creation time in XML format            
 * @property {string} stopTime            Gets or sets the creation time in XML format            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.ActClassKeys} for values)
 * @property {uuid} moodConcept            Mood concept            (see: {@link OpenIZModel.ActMoodKeys} for values)
 * @property {uuid} reasonConcept            Reason concept            (see: {@link OpenIZModel.ActReasonKeys} for values)
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {object} identifier            Gets the identifiers associated with this act            
 * @property {OpenIZModel.ActIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} relationship            Gets a list of all associated acts for this act            
 * @property {OpenIZModel.ActRelationship} relationship.Appends             Indicates that the source act appends information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Arrival             Links the transortation act from another act            
 * @property {OpenIZModel.ActRelationship} relationship.Departure             Links a transporation act from another act indicating departure of the subject            
 * @property {OpenIZModel.ActRelationship} relationship.Documents             The source act documents the target act            
 * @property {OpenIZModel.ActRelationship} relationship.EpisodeLink             Links two instances of the same act over time (example: chronic conditions)            
 * @property {OpenIZModel.ActRelationship} relationship.Evaluates             Used to link a goal to an observation            
 * @property {OpenIZModel.ActRelationship} relationship.Fulfills             Indicates that the source act fulfills the target act            
 * @property {OpenIZModel.ActRelationship} relationship.HasAuthorization             Indicates that the target act authorizes the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasComponent             Indicates that the target act is a component of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasControlVariable             Relationship from an act to one or more control variables (for example: device settings, or environment)            
 * @property {OpenIZModel.ActRelationship} relationship.HasManifestation             The assertion that a new observation may be a manifestation of another            
 * @property {OpenIZModel.ActRelationship} relationship.HasPrecondition             Indicates that the target act is a pre-condition of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasReason             Indicates a reasoning as to why the source act is occurring            
 * @property {OpenIZModel.ActRelationship} relationship.HasReferenceValues             Indicates that the source act contains reference values from the target            
 * @property {OpenIZModel.ActRelationship} relationship.HasSubject             Indicates the subject of a particular act (example: clinical act is a subject of a control act)            
 * @property {OpenIZModel.ActRelationship} relationship.HasSupport             Indicates an existing act is suggesting evidence for a new observation.            
 * @property {OpenIZModel.ActRelationship} relationship.IsCauseOf             Indicates that the source act is the cause of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsDerivedFrom             Indicates the source act is derived from information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsExcerptOf             Indicates that the source act is an excerpt of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.RefersTo             Indicates that the source act refers to the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Replaces             The source act replaces the target act            
 * @property {OpenIZModel.ActRelationship} relationship.StartsAfterStartOf             Indicates that the source act starts after the start of another act            
 * @property {OpenIZModel.ActRelationship} relationship.Transforms             Indicates that the source act transforms the target act            
 * @property {OpenIZModel.ActRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
 * @property {object} extension            Gets a list of all extensions associated with the act            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {string} note            Gets a list of all notes associated with the act            
 * @property {object} tag            Gets a list of all tags associated with the act            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {OpenIZModel.ActProtocol} protocol            Identifies protocols attached to the act            
 * @property {object} participation            Participations            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.ControlAct} copyData Copy constructor (if present)
 */
ControlAct : function(copyData) { 
	this.$type = 'ControlAct';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.participation = copyData.participation;
	this.protocol = copyData.protocol;
	this.tag = copyData.tag;
	this.note = copyData.note;
	this.extension = copyData.extension;
	this.policy = copyData.policy;
	this.relationship = copyData.relationship;
	this.identifier = copyData.identifier;
	this.typeConceptModel = copyData.typeConceptModel;
	this.statusConceptModel = copyData.statusConceptModel;
	this.reasonConceptModel = copyData.reasonConceptModel;
	this.moodConceptModel = copyData.moodConceptModel;
	this.classConceptModel = copyData.classConceptModel;
	this.typeConcept = copyData.typeConcept;
	this.statusConcept = copyData.statusConcept;
	this.reasonConcept = copyData.reasonConcept;
	this.moodConcept = copyData.moodConcept;
	this.classConcept = copyData.classConcept;
	this.stopTime = copyData.stopTime;
	this.startTime = copyData.startTime;
	this.actTime = copyData.actTime;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.isNegated = copyData.isNegated;
	}
},  // ControlAct 
// OpenIZ.Core.Model.Acts.Observation, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Act
 * @summary             Represents a act () which is an observation            
 * @description The observation class itself is an abstract class which is generically used to represent something that is observed about a patient.It is not recommended to use this class directly, rather one of its sub classes based on the type of observation being made such as:Coded observation  for observations whose values are codified (example: blood type, presentation, etc.), Quantity observations  for observations whose values are quantified values (example: weight, height, etc.), Text observations  for observations whose values are textual in nature.
 * @property {uuid} interpretationConcept            Gets or sets the interpretation concept            
 * @property {string} valueType            Value type            
 * @property {OpenIZModel.Concept} interpretationConceptModel [Delay loaded from interpretationConcept],             Gets or sets the concept which indicates the interpretation of the observtion            
 * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {string} actTime            Gets or sets the creation time in XML format            
 * @property {string} startTime            Gets or sets the creation time in XML format            
 * @property {string} stopTime            Gets or sets the creation time in XML format            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.ActClassKeys} for values)
 * @property {uuid} moodConcept            Mood concept            (see: {@link OpenIZModel.ActMoodKeys} for values)
 * @property {uuid} reasonConcept            Reason concept            (see: {@link OpenIZModel.ActReasonKeys} for values)
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {object} identifier            Gets the identifiers associated with this act            
 * @property {OpenIZModel.ActIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} relationship            Gets a list of all associated acts for this act            
 * @property {OpenIZModel.ActRelationship} relationship.Appends             Indicates that the source act appends information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Arrival             Links the transortation act from another act            
 * @property {OpenIZModel.ActRelationship} relationship.Departure             Links a transporation act from another act indicating departure of the subject            
 * @property {OpenIZModel.ActRelationship} relationship.Documents             The source act documents the target act            
 * @property {OpenIZModel.ActRelationship} relationship.EpisodeLink             Links two instances of the same act over time (example: chronic conditions)            
 * @property {OpenIZModel.ActRelationship} relationship.Evaluates             Used to link a goal to an observation            
 * @property {OpenIZModel.ActRelationship} relationship.Fulfills             Indicates that the source act fulfills the target act            
 * @property {OpenIZModel.ActRelationship} relationship.HasAuthorization             Indicates that the target act authorizes the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasComponent             Indicates that the target act is a component of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasControlVariable             Relationship from an act to one or more control variables (for example: device settings, or environment)            
 * @property {OpenIZModel.ActRelationship} relationship.HasManifestation             The assertion that a new observation may be a manifestation of another            
 * @property {OpenIZModel.ActRelationship} relationship.HasPrecondition             Indicates that the target act is a pre-condition of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasReason             Indicates a reasoning as to why the source act is occurring            
 * @property {OpenIZModel.ActRelationship} relationship.HasReferenceValues             Indicates that the source act contains reference values from the target            
 * @property {OpenIZModel.ActRelationship} relationship.HasSubject             Indicates the subject of a particular act (example: clinical act is a subject of a control act)            
 * @property {OpenIZModel.ActRelationship} relationship.HasSupport             Indicates an existing act is suggesting evidence for a new observation.            
 * @property {OpenIZModel.ActRelationship} relationship.IsCauseOf             Indicates that the source act is the cause of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsDerivedFrom             Indicates the source act is derived from information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsExcerptOf             Indicates that the source act is an excerpt of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.RefersTo             Indicates that the source act refers to the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Replaces             The source act replaces the target act            
 * @property {OpenIZModel.ActRelationship} relationship.StartsAfterStartOf             Indicates that the source act starts after the start of another act            
 * @property {OpenIZModel.ActRelationship} relationship.Transforms             Indicates that the source act transforms the target act            
 * @property {OpenIZModel.ActRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
 * @property {object} extension            Gets a list of all extensions associated with the act            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {string} note            Gets a list of all notes associated with the act            
 * @property {object} tag            Gets a list of all tags associated with the act            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {OpenIZModel.ActProtocol} protocol            Identifies protocols attached to the act            
 * @property {object} participation            Participations            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Observation} copyData Copy constructor (if present)
 */
Observation : function(copyData) { 
	this.$type = 'Observation';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.participation = copyData.participation;
	this.protocol = copyData.protocol;
	this.tag = copyData.tag;
	this.note = copyData.note;
	this.extension = copyData.extension;
	this.policy = copyData.policy;
	this.relationship = copyData.relationship;
	this.identifier = copyData.identifier;
	this.typeConceptModel = copyData.typeConceptModel;
	this.statusConceptModel = copyData.statusConceptModel;
	this.reasonConceptModel = copyData.reasonConceptModel;
	this.moodConceptModel = copyData.moodConceptModel;
	this.classConceptModel = copyData.classConceptModel;
	this.typeConcept = copyData.typeConcept;
	this.statusConcept = copyData.statusConcept;
	this.reasonConcept = copyData.reasonConcept;
	this.moodConcept = copyData.moodConcept;
	this.classConcept = copyData.classConcept;
	this.stopTime = copyData.stopTime;
	this.startTime = copyData.startTime;
	this.actTime = copyData.actTime;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.isNegated = copyData.isNegated;
	this.interpretationConceptModel = copyData.interpretationConceptModel;
	this.valueType = copyData.valueType;
	this.interpretationConcept = copyData.interpretationConcept;
	}
},  // Observation 
// OpenIZ.Core.Model.Acts.QuantityObservation, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Observation
 * @summary             Represents an observation that contains a quantity            
 * @property {number} value            Gets or sets the observed quantity            
 * @property {string} valueType            Value type            
 * @property {uuid} unitOfMeasure            Gets or sets the key of the uom concept            
 * @property {OpenIZModel.Concept} unitOfMeasureModel [Delay loaded from unitOfMeasure],             Gets or sets the unit of measure            
 * @property {uuid} interpretationConcept            Gets or sets the interpretation concept            
 * @property {OpenIZModel.Concept} interpretationConceptModel [Delay loaded from interpretationConcept],             Gets or sets the concept which indicates the interpretation of the observtion            
 * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {string} actTime            Gets or sets the creation time in XML format            
 * @property {string} startTime            Gets or sets the creation time in XML format            
 * @property {string} stopTime            Gets or sets the creation time in XML format            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.ActClassKeys} for values)
 * @property {uuid} moodConcept            Mood concept            (see: {@link OpenIZModel.ActMoodKeys} for values)
 * @property {uuid} reasonConcept            Reason concept            (see: {@link OpenIZModel.ActReasonKeys} for values)
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {object} identifier            Gets the identifiers associated with this act            
 * @property {OpenIZModel.ActIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} relationship            Gets a list of all associated acts for this act            
 * @property {OpenIZModel.ActRelationship} relationship.Appends             Indicates that the source act appends information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Arrival             Links the transortation act from another act            
 * @property {OpenIZModel.ActRelationship} relationship.Departure             Links a transporation act from another act indicating departure of the subject            
 * @property {OpenIZModel.ActRelationship} relationship.Documents             The source act documents the target act            
 * @property {OpenIZModel.ActRelationship} relationship.EpisodeLink             Links two instances of the same act over time (example: chronic conditions)            
 * @property {OpenIZModel.ActRelationship} relationship.Evaluates             Used to link a goal to an observation            
 * @property {OpenIZModel.ActRelationship} relationship.Fulfills             Indicates that the source act fulfills the target act            
 * @property {OpenIZModel.ActRelationship} relationship.HasAuthorization             Indicates that the target act authorizes the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasComponent             Indicates that the target act is a component of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasControlVariable             Relationship from an act to one or more control variables (for example: device settings, or environment)            
 * @property {OpenIZModel.ActRelationship} relationship.HasManifestation             The assertion that a new observation may be a manifestation of another            
 * @property {OpenIZModel.ActRelationship} relationship.HasPrecondition             Indicates that the target act is a pre-condition of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasReason             Indicates a reasoning as to why the source act is occurring            
 * @property {OpenIZModel.ActRelationship} relationship.HasReferenceValues             Indicates that the source act contains reference values from the target            
 * @property {OpenIZModel.ActRelationship} relationship.HasSubject             Indicates the subject of a particular act (example: clinical act is a subject of a control act)            
 * @property {OpenIZModel.ActRelationship} relationship.HasSupport             Indicates an existing act is suggesting evidence for a new observation.            
 * @property {OpenIZModel.ActRelationship} relationship.IsCauseOf             Indicates that the source act is the cause of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsDerivedFrom             Indicates the source act is derived from information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsExcerptOf             Indicates that the source act is an excerpt of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.RefersTo             Indicates that the source act refers to the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Replaces             The source act replaces the target act            
 * @property {OpenIZModel.ActRelationship} relationship.StartsAfterStartOf             Indicates that the source act starts after the start of another act            
 * @property {OpenIZModel.ActRelationship} relationship.Transforms             Indicates that the source act transforms the target act            
 * @property {OpenIZModel.ActRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
 * @property {object} extension            Gets a list of all extensions associated with the act            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {string} note            Gets a list of all notes associated with the act            
 * @property {object} tag            Gets a list of all tags associated with the act            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {OpenIZModel.ActProtocol} protocol            Identifies protocols attached to the act            
 * @property {object} participation            Participations            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.QuantityObservation} copyData Copy constructor (if present)
 */
QuantityObservation : function(copyData) { 
	this.$type = 'QuantityObservation';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.participation = copyData.participation;
	this.protocol = copyData.protocol;
	this.tag = copyData.tag;
	this.note = copyData.note;
	this.extension = copyData.extension;
	this.policy = copyData.policy;
	this.relationship = copyData.relationship;
	this.identifier = copyData.identifier;
	this.typeConceptModel = copyData.typeConceptModel;
	this.statusConceptModel = copyData.statusConceptModel;
	this.reasonConceptModel = copyData.reasonConceptModel;
	this.moodConceptModel = copyData.moodConceptModel;
	this.classConceptModel = copyData.classConceptModel;
	this.typeConcept = copyData.typeConcept;
	this.statusConcept = copyData.statusConcept;
	this.reasonConcept = copyData.reasonConcept;
	this.moodConcept = copyData.moodConcept;
	this.classConcept = copyData.classConcept;
	this.stopTime = copyData.stopTime;
	this.startTime = copyData.startTime;
	this.actTime = copyData.actTime;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.isNegated = copyData.isNegated;
	this.interpretationConceptModel = copyData.interpretationConceptModel;
	this.interpretationConcept = copyData.interpretationConcept;
	this.unitOfMeasureModel = copyData.unitOfMeasureModel;
	this.unitOfMeasure = copyData.unitOfMeasure;
	this.valueType = copyData.valueType;
	this.value = copyData.value;
	}
},  // QuantityObservation 
// OpenIZ.Core.Model.Acts.TextObservation, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Observation
 * @summary             Represents an observation with a text value            
 * @property {string} valueType            Value type            
 * @property {string} value            Gets or sets the textual value            
 * @property {uuid} interpretationConcept            Gets or sets the interpretation concept            
 * @property {OpenIZModel.Concept} interpretationConceptModel [Delay loaded from interpretationConcept],             Gets or sets the concept which indicates the interpretation of the observtion            
 * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {string} actTime            Gets or sets the creation time in XML format            
 * @property {string} startTime            Gets or sets the creation time in XML format            
 * @property {string} stopTime            Gets or sets the creation time in XML format            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.ActClassKeys} for values)
 * @property {uuid} moodConcept            Mood concept            (see: {@link OpenIZModel.ActMoodKeys} for values)
 * @property {uuid} reasonConcept            Reason concept            (see: {@link OpenIZModel.ActReasonKeys} for values)
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {object} identifier            Gets the identifiers associated with this act            
 * @property {OpenIZModel.ActIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} relationship            Gets a list of all associated acts for this act            
 * @property {OpenIZModel.ActRelationship} relationship.Appends             Indicates that the source act appends information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Arrival             Links the transortation act from another act            
 * @property {OpenIZModel.ActRelationship} relationship.Departure             Links a transporation act from another act indicating departure of the subject            
 * @property {OpenIZModel.ActRelationship} relationship.Documents             The source act documents the target act            
 * @property {OpenIZModel.ActRelationship} relationship.EpisodeLink             Links two instances of the same act over time (example: chronic conditions)            
 * @property {OpenIZModel.ActRelationship} relationship.Evaluates             Used to link a goal to an observation            
 * @property {OpenIZModel.ActRelationship} relationship.Fulfills             Indicates that the source act fulfills the target act            
 * @property {OpenIZModel.ActRelationship} relationship.HasAuthorization             Indicates that the target act authorizes the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasComponent             Indicates that the target act is a component of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasControlVariable             Relationship from an act to one or more control variables (for example: device settings, or environment)            
 * @property {OpenIZModel.ActRelationship} relationship.HasManifestation             The assertion that a new observation may be a manifestation of another            
 * @property {OpenIZModel.ActRelationship} relationship.HasPrecondition             Indicates that the target act is a pre-condition of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasReason             Indicates a reasoning as to why the source act is occurring            
 * @property {OpenIZModel.ActRelationship} relationship.HasReferenceValues             Indicates that the source act contains reference values from the target            
 * @property {OpenIZModel.ActRelationship} relationship.HasSubject             Indicates the subject of a particular act (example: clinical act is a subject of a control act)            
 * @property {OpenIZModel.ActRelationship} relationship.HasSupport             Indicates an existing act is suggesting evidence for a new observation.            
 * @property {OpenIZModel.ActRelationship} relationship.IsCauseOf             Indicates that the source act is the cause of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsDerivedFrom             Indicates the source act is derived from information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsExcerptOf             Indicates that the source act is an excerpt of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.RefersTo             Indicates that the source act refers to the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Replaces             The source act replaces the target act            
 * @property {OpenIZModel.ActRelationship} relationship.StartsAfterStartOf             Indicates that the source act starts after the start of another act            
 * @property {OpenIZModel.ActRelationship} relationship.Transforms             Indicates that the source act transforms the target act            
 * @property {OpenIZModel.ActRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
 * @property {object} extension            Gets a list of all extensions associated with the act            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {string} note            Gets a list of all notes associated with the act            
 * @property {object} tag            Gets a list of all tags associated with the act            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {OpenIZModel.ActProtocol} protocol            Identifies protocols attached to the act            
 * @property {object} participation            Participations            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.TextObservation} copyData Copy constructor (if present)
 */
TextObservation : function(copyData) { 
	this.$type = 'TextObservation';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.participation = copyData.participation;
	this.protocol = copyData.protocol;
	this.tag = copyData.tag;
	this.note = copyData.note;
	this.extension = copyData.extension;
	this.policy = copyData.policy;
	this.relationship = copyData.relationship;
	this.identifier = copyData.identifier;
	this.typeConceptModel = copyData.typeConceptModel;
	this.statusConceptModel = copyData.statusConceptModel;
	this.reasonConceptModel = copyData.reasonConceptModel;
	this.moodConceptModel = copyData.moodConceptModel;
	this.classConceptModel = copyData.classConceptModel;
	this.typeConcept = copyData.typeConcept;
	this.statusConcept = copyData.statusConcept;
	this.reasonConcept = copyData.reasonConcept;
	this.moodConcept = copyData.moodConcept;
	this.classConcept = copyData.classConcept;
	this.stopTime = copyData.stopTime;
	this.startTime = copyData.startTime;
	this.actTime = copyData.actTime;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.isNegated = copyData.isNegated;
	this.interpretationConceptModel = copyData.interpretationConceptModel;
	this.interpretationConcept = copyData.interpretationConcept;
	this.value = copyData.value;
	this.valueType = copyData.valueType;
	}
},  // TextObservation 
// OpenIZ.Core.Model.Acts.CodedObservation, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Observation
 * @summary             Represents an observation with a concept value            
 * @property {string} valueType            Value type            
 * @property {uuid} value            Gets or sets the key of the uom concept            
 * @property {OpenIZModel.Concept} valueModel [Delay loaded from value],             Gets or sets the coded value of the observation            
 * @property {uuid} interpretationConcept            Gets or sets the interpretation concept            
 * @property {OpenIZModel.Concept} interpretationConceptModel [Delay loaded from interpretationConcept],             Gets or sets the concept which indicates the interpretation of the observtion            
 * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {string} actTime            Gets or sets the creation time in XML format            
 * @property {string} startTime            Gets or sets the creation time in XML format            
 * @property {string} stopTime            Gets or sets the creation time in XML format            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.ActClassKeys} for values)
 * @property {uuid} moodConcept            Mood concept            (see: {@link OpenIZModel.ActMoodKeys} for values)
 * @property {uuid} reasonConcept            Reason concept            (see: {@link OpenIZModel.ActReasonKeys} for values)
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {object} identifier            Gets the identifiers associated with this act            
 * @property {OpenIZModel.ActIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} relationship            Gets a list of all associated acts for this act            
 * @property {OpenIZModel.ActRelationship} relationship.Appends             Indicates that the source act appends information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Arrival             Links the transortation act from another act            
 * @property {OpenIZModel.ActRelationship} relationship.Departure             Links a transporation act from another act indicating departure of the subject            
 * @property {OpenIZModel.ActRelationship} relationship.Documents             The source act documents the target act            
 * @property {OpenIZModel.ActRelationship} relationship.EpisodeLink             Links two instances of the same act over time (example: chronic conditions)            
 * @property {OpenIZModel.ActRelationship} relationship.Evaluates             Used to link a goal to an observation            
 * @property {OpenIZModel.ActRelationship} relationship.Fulfills             Indicates that the source act fulfills the target act            
 * @property {OpenIZModel.ActRelationship} relationship.HasAuthorization             Indicates that the target act authorizes the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasComponent             Indicates that the target act is a component of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasControlVariable             Relationship from an act to one or more control variables (for example: device settings, or environment)            
 * @property {OpenIZModel.ActRelationship} relationship.HasManifestation             The assertion that a new observation may be a manifestation of another            
 * @property {OpenIZModel.ActRelationship} relationship.HasPrecondition             Indicates that the target act is a pre-condition of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasReason             Indicates a reasoning as to why the source act is occurring            
 * @property {OpenIZModel.ActRelationship} relationship.HasReferenceValues             Indicates that the source act contains reference values from the target            
 * @property {OpenIZModel.ActRelationship} relationship.HasSubject             Indicates the subject of a particular act (example: clinical act is a subject of a control act)            
 * @property {OpenIZModel.ActRelationship} relationship.HasSupport             Indicates an existing act is suggesting evidence for a new observation.            
 * @property {OpenIZModel.ActRelationship} relationship.IsCauseOf             Indicates that the source act is the cause of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsDerivedFrom             Indicates the source act is derived from information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsExcerptOf             Indicates that the source act is an excerpt of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.RefersTo             Indicates that the source act refers to the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Replaces             The source act replaces the target act            
 * @property {OpenIZModel.ActRelationship} relationship.StartsAfterStartOf             Indicates that the source act starts after the start of another act            
 * @property {OpenIZModel.ActRelationship} relationship.Transforms             Indicates that the source act transforms the target act            
 * @property {OpenIZModel.ActRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
 * @property {object} extension            Gets a list of all extensions associated with the act            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {string} note            Gets a list of all notes associated with the act            
 * @property {object} tag            Gets a list of all tags associated with the act            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {OpenIZModel.ActProtocol} protocol            Identifies protocols attached to the act            
 * @property {object} participation            Participations            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.CodedObservation} copyData Copy constructor (if present)
 */
CodedObservation : function(copyData) { 
	this.$type = 'CodedObservation';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.participation = copyData.participation;
	this.protocol = copyData.protocol;
	this.tag = copyData.tag;
	this.note = copyData.note;
	this.extension = copyData.extension;
	this.policy = copyData.policy;
	this.relationship = copyData.relationship;
	this.identifier = copyData.identifier;
	this.typeConceptModel = copyData.typeConceptModel;
	this.statusConceptModel = copyData.statusConceptModel;
	this.reasonConceptModel = copyData.reasonConceptModel;
	this.moodConceptModel = copyData.moodConceptModel;
	this.classConceptModel = copyData.classConceptModel;
	this.typeConcept = copyData.typeConcept;
	this.statusConcept = copyData.statusConcept;
	this.reasonConcept = copyData.reasonConcept;
	this.moodConcept = copyData.moodConcept;
	this.classConcept = copyData.classConcept;
	this.stopTime = copyData.stopTime;
	this.startTime = copyData.startTime;
	this.actTime = copyData.actTime;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.isNegated = copyData.isNegated;
	this.interpretationConceptModel = copyData.interpretationConceptModel;
	this.interpretationConcept = copyData.interpretationConcept;
	this.valueModel = copyData.valueModel;
	this.value = copyData.value;
	this.valueType = copyData.valueType;
	}
},  // CodedObservation 
// OpenIZ.Core.Model.Acts.PatientEncounter, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.Act
 * @summary             Represents an encounter a patient has with the health system            
 * @property {uuid} dischargeDisposition            Gets or sets the key of discharge disposition            
 * @property {OpenIZModel.Concept} dischargeDispositionModel [Delay loaded from dischargeDisposition],             Gets or sets the discharge disposition (how the patient left the encounter            
 * @property {bool} isNegated            Gets or sets an indicator which identifies whether the object is negated            
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {string} actTime            Gets or sets the creation time in XML format            
 * @property {string} startTime            Gets or sets the creation time in XML format            
 * @property {string} stopTime            Gets or sets the creation time in XML format            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.ActClassKeys} for values)
 * @property {uuid} moodConcept            Mood concept            (see: {@link OpenIZModel.ActMoodKeys} for values)
 * @property {uuid} reasonConcept            Reason concept            (see: {@link OpenIZModel.ActReasonKeys} for values)
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {object} identifier            Gets the identifiers associated with this act            
 * @property {OpenIZModel.ActIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} relationship            Gets a list of all associated acts for this act            
 * @property {OpenIZModel.ActRelationship} relationship.Appends             Indicates that the source act appends information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Arrival             Links the transortation act from another act            
 * @property {OpenIZModel.ActRelationship} relationship.Departure             Links a transporation act from another act indicating departure of the subject            
 * @property {OpenIZModel.ActRelationship} relationship.Documents             The source act documents the target act            
 * @property {OpenIZModel.ActRelationship} relationship.EpisodeLink             Links two instances of the same act over time (example: chronic conditions)            
 * @property {OpenIZModel.ActRelationship} relationship.Evaluates             Used to link a goal to an observation            
 * @property {OpenIZModel.ActRelationship} relationship.Fulfills             Indicates that the source act fulfills the target act            
 * @property {OpenIZModel.ActRelationship} relationship.HasAuthorization             Indicates that the target act authorizes the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasComponent             Indicates that the target act is a component of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasControlVariable             Relationship from an act to one or more control variables (for example: device settings, or environment)            
 * @property {OpenIZModel.ActRelationship} relationship.HasManifestation             The assertion that a new observation may be a manifestation of another            
 * @property {OpenIZModel.ActRelationship} relationship.HasPrecondition             Indicates that the target act is a pre-condition of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasReason             Indicates a reasoning as to why the source act is occurring            
 * @property {OpenIZModel.ActRelationship} relationship.HasReferenceValues             Indicates that the source act contains reference values from the target            
 * @property {OpenIZModel.ActRelationship} relationship.HasSubject             Indicates the subject of a particular act (example: clinical act is a subject of a control act)            
 * @property {OpenIZModel.ActRelationship} relationship.HasSupport             Indicates an existing act is suggesting evidence for a new observation.            
 * @property {OpenIZModel.ActRelationship} relationship.IsCauseOf             Indicates that the source act is the cause of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsDerivedFrom             Indicates the source act is derived from information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsExcerptOf             Indicates that the source act is an excerpt of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.RefersTo             Indicates that the source act refers to the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Replaces             The source act replaces the target act            
 * @property {OpenIZModel.ActRelationship} relationship.StartsAfterStartOf             Indicates that the source act starts after the start of another act            
 * @property {OpenIZModel.ActRelationship} relationship.Transforms             Indicates that the source act transforms the target act            
 * @property {OpenIZModel.ActRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
 * @property {object} extension            Gets a list of all extensions associated with the act            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {string} note            Gets a list of all notes associated with the act            
 * @property {object} tag            Gets a list of all tags associated with the act            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {OpenIZModel.ActProtocol} protocol            Identifies protocols attached to the act            
 * @property {object} participation            Participations            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.PatientEncounter} copyData Copy constructor (if present)
 */
PatientEncounter : function(copyData) { 
	this.$type = 'PatientEncounter';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.participation = copyData.participation;
	this.protocol = copyData.protocol;
	this.tag = copyData.tag;
	this.note = copyData.note;
	this.extension = copyData.extension;
	this.policy = copyData.policy;
	this.relationship = copyData.relationship;
	this.identifier = copyData.identifier;
	this.typeConceptModel = copyData.typeConceptModel;
	this.statusConceptModel = copyData.statusConceptModel;
	this.reasonConceptModel = copyData.reasonConceptModel;
	this.moodConceptModel = copyData.moodConceptModel;
	this.classConceptModel = copyData.classConceptModel;
	this.typeConcept = copyData.typeConcept;
	this.statusConcept = copyData.statusConcept;
	this.reasonConcept = copyData.reasonConcept;
	this.moodConcept = copyData.moodConcept;
	this.classConcept = copyData.classConcept;
	this.stopTime = copyData.stopTime;
	this.startTime = copyData.startTime;
	this.actTime = copyData.actTime;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.isNegated = copyData.isNegated;
	this.dischargeDispositionModel = copyData.dischargeDispositionModel;
	this.dischargeDisposition = copyData.dischargeDisposition;
	}
},  // PatientEncounter 
// OpenIZ.Core.Model.Acts.Protocol, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @class
 * @memberof OpenIZModel
 * @public
 * @extends OpenIZModel.BaseEntityData
 * @summary             Represents the model of a protocol            
 * @property {string} name            Gets or sets the name of the protocol            
 * @property {string} handlerClass            Gets or sets the handler class AQN            
 * @property {string} oid            Gets or sets the OID            
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.Protocol} copyData Copy constructor (if present)
 */
Protocol : function(copyData) { 
	this.$type = 'Protocol';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.oid = copyData.oid;
	this.handlerClass = copyData.handlerClass;
	this.name = copyData.name;
	}
},  // Protocol 
// OpenIZ.Core.Model.Acts.SubstanceAdministration, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
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
 * @property {uuid} template            Gets the template key            
 * @property {OpenIZModel.TemplateDefinition} templateModel [Delay loaded from template],             Gets or sets the template definition            
 * @property {string} actTime            Gets or sets the creation time in XML format            
 * @property {string} startTime            Gets or sets the creation time in XML format            
 * @property {string} stopTime            Gets or sets the creation time in XML format            
 * @property {uuid} classConcept            Class concept            (see: {@link OpenIZModel.ActClassKeys} for values)
 * @property {uuid} moodConcept            Mood concept            (see: {@link OpenIZModel.ActMoodKeys} for values)
 * @property {uuid} reasonConcept            Reason concept            (see: {@link OpenIZModel.ActReasonKeys} for values)
 * @property {uuid} statusConcept            Status concept id            (see: {@link OpenIZModel.StatusKeys} for values)
 * @property {uuid} typeConcept            Type concept identifier            
 * @property {OpenIZModel.Concept} classConceptModel [Delay loaded from classConcept],             Class concept datal load property            
 * @property {OpenIZModel.Concept} moodConceptModel [Delay loaded from moodConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} reasonConceptModel [Delay loaded from reasonConcept],             Mood concept data load property            
 * @property {OpenIZModel.Concept} statusConceptModel [Delay loaded from statusConcept],             Status concept id            
 * @property {OpenIZModel.Concept} typeConceptModel [Delay loaded from typeConcept],             Type concept identifier            
 * @property {object} identifier            Gets the identifiers associated with this act            
 * @property {OpenIZModel.ActIdentifier} identifier.classifier  where classifier is from {@link OpenIZModel.IdentifierBase} authority
 * @property {object} relationship            Gets a list of all associated acts for this act            
 * @property {OpenIZModel.ActRelationship} relationship.Appends             Indicates that the source act appends information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Arrival             Links the transortation act from another act            
 * @property {OpenIZModel.ActRelationship} relationship.Departure             Links a transporation act from another act indicating departure of the subject            
 * @property {OpenIZModel.ActRelationship} relationship.Documents             The source act documents the target act            
 * @property {OpenIZModel.ActRelationship} relationship.EpisodeLink             Links two instances of the same act over time (example: chronic conditions)            
 * @property {OpenIZModel.ActRelationship} relationship.Evaluates             Used to link a goal to an observation            
 * @property {OpenIZModel.ActRelationship} relationship.Fulfills             Indicates that the source act fulfills the target act            
 * @property {OpenIZModel.ActRelationship} relationship.HasAuthorization             Indicates that the target act authorizes the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasComponent             Indicates that the target act is a component of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasControlVariable             Relationship from an act to one or more control variables (for example: device settings, or environment)            
 * @property {OpenIZModel.ActRelationship} relationship.HasManifestation             The assertion that a new observation may be a manifestation of another            
 * @property {OpenIZModel.ActRelationship} relationship.HasPrecondition             Indicates that the target act is a pre-condition of the source act            
 * @property {OpenIZModel.ActRelationship} relationship.HasReason             Indicates a reasoning as to why the source act is occurring            
 * @property {OpenIZModel.ActRelationship} relationship.HasReferenceValues             Indicates that the source act contains reference values from the target            
 * @property {OpenIZModel.ActRelationship} relationship.HasSubject             Indicates the subject of a particular act (example: clinical act is a subject of a control act)            
 * @property {OpenIZModel.ActRelationship} relationship.HasSupport             Indicates an existing act is suggesting evidence for a new observation.            
 * @property {OpenIZModel.ActRelationship} relationship.IsCauseOf             Indicates that the source act is the cause of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsDerivedFrom             Indicates the source act is derived from information contained in the target act            
 * @property {OpenIZModel.ActRelationship} relationship.IsExcerptOf             Indicates that the source act is an excerpt of the target act            
 * @property {OpenIZModel.ActRelationship} relationship.RefersTo             Indicates that the source act refers to the target act            
 * @property {OpenIZModel.ActRelationship} relationship.Replaces             The source act replaces the target act            
 * @property {OpenIZModel.ActRelationship} relationship.StartsAfterStartOf             Indicates that the source act starts after the start of another act            
 * @property {OpenIZModel.ActRelationship} relationship.Transforms             Indicates that the source act transforms the target act            
 * @property {OpenIZModel.ActRelationship} relationship.$other Unclassified
 * @property {OpenIZModel.SecurityPolicyInstance} policy            Gets or sets the policy instances            
 * @property {object} extension            Gets a list of all extensions associated with the act            
 * @property {string} extension.classifier  where classifier is from {@link OpenIZModel.Extension} extensionType
 * @property {string} note            Gets a list of all notes associated with the act            
 * @property {object} tag            Gets a list of all tags associated with the act            
 * @property {string} tag.classifier  where classifier is from {@link OpenIZModel.Tag} key
 * @property {OpenIZModel.ActProtocol} protocol            Identifies protocols attached to the act            
 * @property {object} participation            Participations            
 * @property {OpenIZModel.ActParticipation} participation.Admitter             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
 * @property {OpenIZModel.ActParticipation} participation.Attender             The player entity represents the attending physician for the patient            
 * @property {OpenIZModel.ActParticipation} participation.Authenticator             The player entity represents an entity which authenticates the provision of care            
 * @property {OpenIZModel.ActParticipation} participation.Authororiginator             The player entity is responsible for the creation of data described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Baby             The player is a resultant person in that it was the baby             
 * @property {OpenIZModel.ActParticipation} participation.Beneficiary             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
 * @property {OpenIZModel.ActParticipation} participation.CallbackContact             The player entity represents something that should be contacted upon completion of the act            
 * @property {OpenIZModel.ActParticipation} participation.CausativeAgent             The player entity is an agent which caused the act to occur            
 * @property {OpenIZModel.ActParticipation} participation.Consultant             The player entity is acting as a consult to the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Consumable             The player entity was or is to be consumed during the process of carrying out the act.            
 * @property {OpenIZModel.ActParticipation} participation.CoverageTarget             The player entity represents the target coverage entity of the act            
 * @property {OpenIZModel.ActParticipation} participation.Custodian             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
 * @property {OpenIZModel.ActParticipation} participation.DataEnterer             The player entity represents the person or device which phisically entered the data at the terminal            
 * @property {OpenIZModel.ActParticipation} participation.Destination             The player etity represents the ultimate destination of the goods/materials/services described in the act            
 * @property {OpenIZModel.ActParticipation} participation.Device             The player entity represents the device on which the act or data from the act was acquired or recorded            
 * @property {OpenIZModel.ActParticipation} participation.DirectTarget             The player entity represents the directed target of care provided in the act            
 * @property {OpenIZModel.ActParticipation} participation.Discharger             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
 * @property {OpenIZModel.ActParticipation} participation.Distributor             The player entity is the source distribution point for the financial or material instruments contained in the message            
 * @property {OpenIZModel.ActParticipation} participation.Donor             The player entity represents the donor of tissue or materials used in the act            
 * @property {OpenIZModel.ActParticipation} participation.EntryLocation             The location where the act was entered.            
 * @property {OpenIZModel.ActParticipation} participation.Escort             The player entity was responsible for escorting the patient during the course of the act            
 * @property {OpenIZModel.ActParticipation} participation.Exposure             The player entity something to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureAgent             The player entity represents the agent (material) to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureSource             The player entity describes the source of the material to which the patient was exposed            
 * @property {OpenIZModel.ActParticipation} participation.ExposureTarget             The player entity describes the target to which the agent was exposed            
 * @property {OpenIZModel.ActParticipation} participation.GuarantorParty             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.Holder             The player is responsible for holding the act            
 * @property {OpenIZModel.ActParticipation} participation.IndirectTarget             The entity not directly present in the act but which will be the focust of th act.            
 * @property {OpenIZModel.ActParticipation} participation.Informant             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
 * @property {OpenIZModel.ActParticipation} participation.InformationRecipient             The player entity represents something that should be cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.LegalAuthenticator             The player entity is responsible for legally authenticating the content of the act            
 * @property {OpenIZModel.ActParticipation} participation.Location             The location where the service was performed.            
 * @property {OpenIZModel.ActParticipation} participation.NonreuseableDevice             The player represents a consumable that can no longer be used            
 * @property {OpenIZModel.ActParticipation} participation.Origin             The player represents the origin of the act            
 * @property {OpenIZModel.ActParticipation} participation.Participation             The player entity participates in the act in no particular classification            
 * @property {OpenIZModel.ActParticipation} participation.Performer             The player entity is responsible for performing the clinical steps documented in the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryInformationRecipient             The player entity represents a high priority contact which should be informed or cc'ed on the act            
 * @property {OpenIZModel.ActParticipation} participation.PrimaryPerformer             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
 * @property {OpenIZModel.ActParticipation} participation.Product             The player represents a product that is not necessarily consumed but informs the act            
 * @property {OpenIZModel.ActParticipation} participation.Receiver             The player represents the entity which is the intended receiver of the act            
 * @property {OpenIZModel.ActParticipation} participation.RecordTarget             The player represents the entity to which the act is recorded against            
 * @property {OpenIZModel.ActParticipation} participation.ReferredBy             The player represents the entity which referred the act or caused the act to be undertaken            
 * @property {OpenIZModel.ActParticipation} participation.ReferredTo             The player entity represents the entity which was referred to            
 * @property {OpenIZModel.ActParticipation} participation.Referrer             The player entity represents the person who was originally the referrer.            
 * @property {OpenIZModel.ActParticipation} participation.Remote             The player entity represents a remote portion of the act            
 * @property {OpenIZModel.ActParticipation} participation.ResponsibleParty             The player entity is ultimately responsible for the carrying out of the act            
 * @property {OpenIZModel.ActParticipation} participation.ReusableDevice             The player entity represents a device which can be reused in future acts            
 * @property {OpenIZModel.ActParticipation} participation.SecondaryPerformer             The secondary performing person (support clinician).            
 * @property {OpenIZModel.ActParticipation} participation.Specimen             The player entity represents a specimen collected for the purpose of testing and diagnosis            
 * @property {OpenIZModel.ActParticipation} participation.Subject             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
 * @property {OpenIZModel.ActParticipation} participation.Tracker             The player entity is responsible for tracking the progress of the act            
 * @property {OpenIZModel.ActParticipation} participation.Transcriber             The person who transcribed data from the original act.            
 * @property {OpenIZModel.ActParticipation} participation.UgentNotificationContact             The player entity represents a contact entity in case of an emergency occurs during the act.            
 * @property {OpenIZModel.ActParticipation} participation.Verifier             The player entity was responsible for verifying the accuracy of the data in the act            
 * @property {OpenIZModel.ActParticipation} participation.Via             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
 * @property {OpenIZModel.ActParticipation} participation.Witness             The player entity represents a legal witness to the act occurring.            
 * @property {OpenIZModel.ActParticipation} participation.$other Unclassified
 * @property {uuid} previousVersion
 * @property {uuid} version
 * @property {number} sequence
 * @property {date} creationTimeModel [Delay loaded from creationTime],             Gets or sets the time at which the data was created            
 * @property {string} creationTime            Gets or sets the creation time in XML format            
 * @property {date} obsoletionTimeModel [Delay loaded from obsoletionTime],             Gets or sets the time when the data is or will become invalid            
 * @property {string} obsoletionTime            Gets or sets the creation time in XML format            
 * @property {OpenIZModel.SecurityUser} createdByModel [Delay loaded from createdBy],             Gets or sets the user that created this base data            
 * @property {date} modifiedOn            Gets the time that the object was last modified (from base data, default to CreationTime)            
 * @property {OpenIZModel.SecurityUser} obsoletedByModel [Delay loaded from obsoletedBy],             Gets or sets the user that obsoleted this base data            
 * @property {uuid} createdBy            Gets or sets the identifier of the user which created the data            
 * @property {uuid} obsoletedBy            Gets or sets the identifier of the user which obsoleted the data            
 * @property {uuid} id            The internal primary key value of the entity            
 * @property {string} $type            Gets the type            
 * @param {OpenIZModel.SubstanceAdministration} copyData Copy constructor (if present)
 */
SubstanceAdministration : function(copyData) { 
	this.$type = 'SubstanceAdministration';
	if(copyData) {
	this.id = copyData.id;
	this.obsoletedBy = copyData.obsoletedBy;
	this.createdBy = copyData.createdBy;
	this.obsoletedByModel = copyData.obsoletedByModel;
	this.modifiedOn = copyData.modifiedOn;
	this.createdByModel = copyData.createdByModel;
	this.obsoletionTime = copyData.obsoletionTime;
	this.obsoletionTimeModel = copyData.obsoletionTimeModel;
	this.creationTime = copyData.creationTime;
	this.creationTimeModel = copyData.creationTimeModel;
	this.sequence = copyData.sequence;
	this.version = copyData.version;
	this.previousVersion = copyData.previousVersion;
	this.participation = copyData.participation;
	this.protocol = copyData.protocol;
	this.tag = copyData.tag;
	this.note = copyData.note;
	this.extension = copyData.extension;
	this.policy = copyData.policy;
	this.relationship = copyData.relationship;
	this.identifier = copyData.identifier;
	this.typeConceptModel = copyData.typeConceptModel;
	this.statusConceptModel = copyData.statusConceptModel;
	this.reasonConceptModel = copyData.reasonConceptModel;
	this.moodConceptModel = copyData.moodConceptModel;
	this.classConceptModel = copyData.classConceptModel;
	this.typeConcept = copyData.typeConcept;
	this.statusConcept = copyData.statusConcept;
	this.reasonConcept = copyData.reasonConcept;
	this.moodConcept = copyData.moodConcept;
	this.classConcept = copyData.classConcept;
	this.stopTime = copyData.stopTime;
	this.startTime = copyData.startTime;
	this.actTime = copyData.actTime;
	this.templateModel = copyData.templateModel;
	this.template = copyData.template;
	this.isNegated = copyData.isNegated;
	this.siteModel = copyData.siteModel;
	this.site = copyData.site;
	this.doseSequence = copyData.doseSequence;
	this.doseQuantity = copyData.doseQuantity;
	this.doseUnitModel = copyData.doseUnitModel;
	this.routeModel = copyData.routeModel;
	this.doseUnit = copyData.doseUnit;
	this.route = copyData.route;
	}
},  // SubstanceAdministration 
// OpenIZ.Core.Model.Constants.UserClassKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Represents user classification keys            
 */
UserClassKeys : { 
	/** 
	 *             Represents a user which is an application            
	 */
	ApplicationUser : 'e9cd4dad-2759-4022-ab07-92fcfb236a98',
	/** 
	 *             Represents a user which is a human            
	 */
	HumanUser : '33932b42-6f4b-4659-8849-6aca54139d8e',
	/** 
	 *             Represents a user which is a system user            
	 */
	SystemUser : '9f71bb34-9691-440f-8249-9c831ea16d58',
},  // UserClassKeys 
// OpenIZ.Core.Model.DataTypes.DatePrecision, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Represents a date precision object            
 */
DatePrecision : { 
	/** 
	 *             Represents full date precision.            
	 */
	Full : 'Full',
	/** 
	 *             Represents year date precision.            
	 */
	Year : 'Year',
	/** 
	 *             Represents month date precision.            
	 */
	Month : 'Month',
	/** 
	 *             Represents day date precision.            
	 */
	Day : 'Day',
	/** 
	 *             Represents hour date precision.            
	 */
	Hour : 'Hour',
	/** 
	 *             Represents minute date precision.            
	 */
	Minute : 'Minute',
	/** 
	 *             Represents second date precision.            
	 */
	Second : 'Second',
},  // DatePrecision 
// OpenIZ.Core.Model.Constants.AddressUseKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Address use keys            
 */
AddressUseKeys : { 
	/** 
	 *             Represents an alphabetic address used for matching             
	 */
	Alphabetic : '71d1c07c-6ee6-4240-8a95-19f96583512e',
	/** 
	 *             Represents a bad address, i.e. an address which is old or invalid.            
	 */
	BadAddress : 'f3132fc0-aadd-40b7-b875-961c40695389',
	/** 
	 *             Represents a workplace address that reaches the person directly without intermediaries.            
	 */
	Direct : 'd0db6edb-6cdc-4671-8bc2-00f1c808e188',
	/** 
	 *             The home address            
	 */
	HomeAddress : '493c3e9d-4f65-4e4d-9582-c9008f4f2eb4',
	/** 
	 *             Represents an address expressed in an ideographic manner (example: Kaji)            
	 */
	Ideographic : '09000479-4672-44f8-bb4a-72fb25f7356a',
	/** 
	 *             Represents an address expressed as a phonetic spelling of an ideographic address            
	 */
	Phonetic : '2b085d38-3308-4664-9f89-48d8ef4daba7',
	/** 
	 *             The address is a physical place where visits should occur            
	 */
	PhysicalVisit : '5724a9b6-24b6-43b7-8075-7a0d61fcb814',
	/** 
	 *             The address is a postal address used for the delivery of mail and materials            
	 */
	PostalAddress : '7246e98d-20c6-4ae6-85ad-4aa09649feb7',
	/** 
	 *             Represents a primary address to reach a contact after business hours.            
	 */
	PrimaryHome : 'c4faafd8-fc90-4330-8b4b-e4e64c86b87b',
	/** 
	 *             Represents an address that is a standard address that may be subject to a switchboard or operator prior to reaching the intended entity.            
	 */
	Public : 'ec35ea7c-55d2-4619-a56b-f7a986412f7f',
	/** 
	 *             Represents an address used for soundex matching purposes.            
	 */
	Soundex : 'e5794e3b-3025-436f-9417-5886feead55a',
	/** 
	 *             Represents a syllabic address.            
	 */
	Syllabic : 'b4ca3bf0-a7fc-44f3-87d5-e126beda93ff',
	/** 
	 *             Represents a temporary address that may be good for visiting or mailing.            
	 */
	TemporaryAddress : 'cef6ea31-a097-4f59-8723-a38c727c6597',
	/** 
	 *             Represents a vacation home to reach a person while on vacation.            
	 */
	VacationHome : '5d69534c-4597-4d11-bb98-56a9918f5238',
	/** 
	 *             Represents an office address, should be used for business communications            
	 */
	WorkPlace : 'eaa6f08e-bb8e-4457-9dc0-3a1555fadf5c',
},  // AddressUseKeys 
// OpenIZ.Core.Model.Constants.EntityClassKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Entity class concept keys            
 */
EntityClassKeys : { 
	/** 
	 *             Animal            
	 */
	Animal : '61fcbf42-b5e0-4fb5-9392-108a5c6dbec7',
	/** 
	 *             Chemical Substance            
	 */
	ChemicalSubstance : '2e9fa332-9391-48c6-9fc8-920a750b25d3',
	/** 
	 *             City or town            
	 */
	CityOrTown : '79dd4f75-68e8-4722-a7f5-8bc2e08f5cd6',
	/** 
	 *             Container            
	 */
	Container : 'b76ff324-b174-40b7-a6ac-d1fdf8e23967',
	/** 
	 *             Country or nation            
	 */
	Country : '48b2ffb3-07db-47ba-ad73-fc8fb8502471',
	/** 
	 *             County or parish            
	 */
	CountyOrParish : 'd9489d56-ddac-4596-b5c6-8f41d73d8dc5',
	/** 
	 *             Device            
	 */
	Device : '1373ff04-a6ef-420a-b1d0-4a07465fe8e8',
	/** 
	 *             Entity            
	 */
	Entity : 'e29fcfad-ec1d-4c60-a055-039a494248ae',
	/** 
	 *             Food            
	 */
	Food : 'e5a09cc2-5ae5-40c2-8e32-687dba06715d',
	/** 
	 *             Living Subject            
	 */
	LivingSubject : '8ba5e5c9-693b-49d4-973c-d7010f3a23ee',
	/** 
	 *             Manufactured material            
	 */
	ManufacturedMaterial : 'fafec286-89d5-420b-9085-054aca9d1eef',
	/** 
	 *             Material            
	 */
	Material : 'd39073be-0f8f-440e-b8c8-7034cc138a95',
	/** 
	 *             Non living subject            
	 */
	NonLivingSubject : '9025e5c9-693b-49d4-973c-d7010f3a23ee',
	/** 
	 *             Organization            
	 */
	Organization : '7c08bd55-4d42-49cd-92f8-6388d6c4183f',
	/** 
	 *             Patient            
	 */
	Patient : 'bacd9c6f-3fa9-481e-9636-37457962804d',
	/** 
	 *             Person            
	 */
	Person : '9de2a846-ddf2-4ebc-902e-84508c5089ea',
	/** 
	 *             Place            
	 */
	Place : '21ab7873-8ef3-4d78-9c19-4582b3c40631',
	/** 
	 *             Service delivery location            
	 */
	Provider : '6b04fed8-c164-469c-910b-f824c2bda4f0',
	/** 
	 *             Service delivery location            
	 */
	ServiceDeliveryLocation : 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c',
	/** 
	 *             State            
	 */
	State : '8cf4b0b0-84e5-4122-85fe-6afa8240c218',
},  // EntityClassKeys 
// OpenIZ.Core.Model.Constants.DeterminerKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Determiner codes classify an entity into one of three categories.             
 */
DeterminerKeys : { 
	/** 
	 *             Indicates the entity is not a particular instance of a thing, rather a type of thing            
	 */
	Described : 'ad28a7ac-a66b-42c4-91b4-de40a2b11980',
	/** 
	 *             Indicates the entity is a type of thing that has been qualified further            
	 */
	DescribedQualified : '604cf1b7-8891-49fb-b95f-3e4e875691bc',
	/** 
	 *             Indicates the entity is a specific instance of a thing            
	 */
	Specific : 'f29f08de-78a7-4a5e-aeaf-7b545ba19a09',
},  // DeterminerKeys 
// OpenIZ.Core.Model.Constants.NameUseKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Name use keys            
 */
NameUseKeys : { 
	/** 
	 *             The name used is an alphabetic representation of the name (ex: romaji in Japanese)            
	 */
	Alphabetic : '71d1c07c-6ee6-4240-8a95-19f96583512e',
	/** 
	 *             The name is an anonymous name for the object (not the real name but a name used for care delivery)            
	 */
	Anonymous : '95e6843a-26ff-4046-b6f4-eb440d4b85f7',
	/** 
	 *             The name represents an artist name or stage name            
	 */
	Artist : '4a7bf199-f33b-42f9-8b99-32433ea67bd7',
	/** 
	 *             The name represents an assigned name (given or bestowed by an authority)            
	 */
	Assigned : 'a87a6d21-2ca6-4aea-88f3-6135cceb58d1',
	/** 
	 *             THe name represents an ideographic representation of the name            
	 */
	Ideographic : '09000479-4672-44f8-bb4a-72fb25f7356a',
	/** 
	 *             The name is an indigenous name or tribal name for the patient            
	 */
	Indigenous : 'a3fb2a05-5ebe-47ae-afd0-4c1b22336090',
	/** 
	 *             The name represents the current legal name of an object (such as a corporate name)            
	 */
	Legal : 'effe122d-8d30-491d-805d-addcb4466c35',
	/** 
	 *             The name represents a name as displayed on a license or known to a license authority            
	 */
	License : '48075d19-7b29-4ca5-9c73-0cbd31248446',
	/** 
	 *             THe name is a maiden name (name of a patient before marriage)            
	 */
	MaidenName : '0674c1c8-963a-4658-aff9-8cdcd308fa68',
	/** 
	 *             The name as it appears on an official record            
	 */
	OfficialRecord : '1ec9583a-b019-4baa-b856-b99caf368656',
	/** 
	 *             The name represents a phonetic representation of a name such as a SOUNDEX code            
	 */
	Phonetic : '2b085d38-3308-4664-9f89-48d8ef4daba7',
	/** 
	 *             The name is a pseudonym for the object or an synonym name            
	 */
	Pseudonym : 'c31564ef-ca8d-4528-85a8-88245fcef344',
	/** 
	 *             The name is to be used for religious purposes (such as baptismal name)            
	 */
	Religious : '15207687-5290-4672-a7df-2880a23dcbb5',
	/** 
	 *             The name is to be used in the performing of matches only            
	 */
	Search : '87964bff-e442-481d-9749-69b2a84a1fbe',
	/** 
	 *             The name represents the computed soundex code of a name            
	 */
	Soundex : 'e5794e3b-3025-436f-9417-5886feead55a',
	/** 
	 *             The name represents a syllabic name.            
	 */
	Syllabic : 'b4ca3bf0-a7fc-44f3-87d5-e126beda93ff',
},  // NameUseKeys 
// OpenIZ.Core.Model.Constants.ActParticipationKey, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Represents the participation concepts which an entity can participate in an act            
 */
ActParticipationKey : { 
	/** 
	 *             The player entity was the person who was responsible for admitting the patient into a facility or care scenario.            
	 */
	Admitter : 'a0174216-6439-4351-9483-a241a48029b7',
	/** 
	 *             The player entity represents the attending physician for the patient            
	 */
	Attender : '6cbf29ad-ac51-48c9-885a-cfe3026ecf6e',
	/** 
	 *             The player entity represents an entity which authenticates the provision of care            
	 */
	Authenticator : '1b2dbf82-a503-4cf4-9ecb-a8e111b4674e',
	/** 
	 *             The player entity is responsible for the creation of data described in the act            
	 */
	Authororiginator : 'f0cb3faf-435d-4704-9217-b884f757bc14',
	/** 
	 *             The player is a resultant person in that it was the baby             
	 */
	Baby : '479896b0-35d5-4842-8109-5fdbee14e8a4',
	/** 
	 *             The player is a beneficiary of the act such a receiver of a financial instrument, or other good            
	 */
	Beneficiary : '28c744df-d889-4a44-bc1a-2e9e9d64af13',
	/** 
	 *             The player entity represents something that should be contacted upon completion of the act            
	 */
	CallbackContact : '9c4c40ae-2c15-4581-a496-be1abfe4eb66',
	/** 
	 *             The player entity is an agent which caused the act to occur            
	 */
	CausativeAgent : '7f81b83e-0d78-4685-8ba4-224eb315ce54',
	/** 
	 *             The player entity is acting as a consult to the carrying out of the act            
	 */
	Consultant : '0a364ad7-f961-4d8a-93f0-1fd4176548b3',
	/** 
	 *             The player entity was or is to be consumed during the process of carrying out the act.            
	 */
	Consumable : 'a5cac7f7-e3b7-4dd8-872c-db0e7fcc2d84',
	/** 
	 *             The player entity represents the target coverage entity of the act            
	 */
	CoverageTarget : '4b5471d4-e3fe-45f7-85a2-ae2b4f224757',
	/** 
	 *             The player entity is the data custodian of the act (is responsible for storing and securing the act)            
	 */
	Custodian : '649d6d69-139c-4006-ae45-aff4649d6079',
	/** 
	 *             The player entity represents the person or device which phisically entered the data at the terminal            
	 */
	DataEnterer : 'c50d66d2-e5da-4a34-b2b7-4cd4fe4ef2c4',
	/** 
	 *             The player etity represents the ultimate destination of the goods/materials/services described in the act            
	 */
	Destination : '727b3624-ea62-46bb-a68b-b9e49e302eca',
	/** 
	 *             The player entity represents the device on which the act or data from the act was acquired or recorded            
	 */
	Device : '1373ff04-a6ef-420a-b1d0-4a07465fe8e8',
	/** 
	 *             The player entity represents the directed target of care provided in the act            
	 */
	DirectTarget : 'd9f63423-ba9b-48d9-ba38-c404b784b670',
	/** 
	 *             The player entity represents the person who is responsible for the discharging of the patient from an encounter            
	 */
	Discharger : 'a2594e6e-e8fe-4c68-82a5-d3a46dbec87d',
	/** 
	 *             The player entity is the source distribution point for the financial or material instruments contained in the message            
	 */
	Distributor : '693f08fa-625a-40d2-b928-6856099c0349',
	/** 
	 *             The player entity represents the donor of tissue or materials used in the act            
	 */
	Donor : 'be1235ee-710a-4732-88fd-6e895de7c56d',
	/** 
	 *             The location where the act was entered.            
	 */
	EntryLocation : 'ac05185b-5a80-47a8-b924-060deb6d0eb2',
	/** 
	 *             The player entity was responsible for escorting the patient during the course of the act            
	 */
	Escort : '727a61ed-2f35-4e09-8bb6-6d09e2ba8fec',
	/** 
	 *             The player entity something to which the patient was exposed            
	 */
	Exposure : '5a6a6766-8e1d-4d36-ae50-9b7d82d8a182',
	/** 
	 *             The player entity represents the agent (material) to which the patient was exposed            
	 */
	ExposureAgent : 'ea60a5a9-e971-4f0d-bb5d-dc7a0c74a2c9',
	/** 
	 *             The player entity describes the source of the material to which the patient was exposed            
	 */
	ExposureSource : 'cbb6297b-743c-453c-8476-ba4c10a1c965',
	/** 
	 *             The player entity describes the target to which the agent was exposed            
	 */
	ExposureTarget : 'ec401b5c-4c33-4229-9c72-428fc5db37ff',
	/** 
	 *             The player represents a party which is used as a financial guarantor for payment in the carrying out of the act            
	 */
	GuarantorParty : '28fb791e-179e-461a-b16c-cac13a04bd0a',
	/** 
	 *             The player is responsible for holding the act            
	 */
	Holder : '2452b691-f122-4121-b9df-76d990b43f35',
	/** 
	 *             The entity not directly present in the act but which will be the focust of th act.            
	 */
	IndirectTarget : '3a9f0c2f-e322-4639-a8e7-0df67cac761b',
	/** 
	 *             The player was a person or device which informed data presented in the act. (Example: A mother telling a nurse that their child had a reaction)            
	 */
	Informant : '39604248-7812-4b60-bc54-8cc1fffb1de6',
	/** 
	 *             The player entity represents something that should be cc'ed on the act            
	 */
	InformationRecipient : '9790b291-b8a3-4c85-a240-c2c38885ad5d',
	/** 
	 *             The player entity is responsible for legally authenticating the content of the act            
	 */
	LegalAuthenticator : '0716a333-cd46-439d-bfd6-bf788f3885fa',
	/** 
	 *             The location where the service was performed.            
	 */
	Location : '61848557-d78d-40e5-954f-0b9c97307a04',
	/** 
	 *             The player represents a consumable that can no longer be used            
	 */
	NonreuseableDevice : '6792db6c-fd5c-4ab8-96f5-ace5665bdcb9',
	/** 
	 *             The player represents the origin of the act            
	 */
	Origin : '5d175f21-1963-4589-a400-b5ef5f64842c',
	/** 
	 *             The player entity participates in the act in no particular classification            
	 */
	Participation : 'c704a23d-86ef-4e11-9050-f8aa10919ff2',
	/** 
	 *             The player entity is responsible for performing the clinical steps documented in the act            
	 */
	Performer : 'fa5e70a4-a46e-4665-8a20-94d4d7b86fc8',
	/** 
	 *             The player entity represents a high priority contact which should be informed or cc'ed on the act            
	 */
	PrimaryInformationRecipient : '02bb7934-76b5-4cc5-bd42-58570f15eb4d',
	/** 
	 *             The player entity was the primary performer of the act. This is used in procedures where more than one performer is present            
	 */
	PrimaryPerformer : '79f6136c-1465-45e8-917e-e7832bc8e3b2',
	/** 
	 *             The player represents a product that is not necessarily consumed but informs the act            
	 */
	Product : '99e77288-cb09-4050-a8cf-385513f32f0a',
	/** 
	 *             The player represents the entity which is the intended receiver of the act            
	 */
	Receiver : '53c694b8-27d8-43dd-95a4-bb318431d17c',
	/** 
	 *             The player represents the entity to which the act is recorded against            
	 */
	RecordTarget : '3f92dbee-a65e-434f-98ce-841feeb02e3f',
	/** 
	 *             The player represents the entity which referred the act or caused the act to be undertaken            
	 */
	ReferredBy : '6da3a6ca-2ab0-4d32-9588-e094f277f06d',
	/** 
	 *             The player entity represents the entity which was referred to            
	 */
	ReferredTo : '353f9255-765e-4336-8007-1d61ab09aad6',
	/** 
	 *             The player entity represents the person who was originally the referrer.            
	 */
	Referrer : '5e8e0f8b-bc23-4847-82ab-49b8dd79981e',
	/** 
	 *             The player entity represents a remote portion of the act            
	 */
	Remote : '3c1225de-194e-49ce-a41a-0f9376b04c11',
	/** 
	 *             The player entity is ultimately responsible for the carrying out of the act            
	 */
	ResponsibleParty : '64474c12-b978-4bb6-a584-46dadec2d952',
	/** 
	 *             The player entity represents a device which can be reused in future acts            
	 */
	ReusableDevice : '76990d3d-3f27-4b39-836b-ba87eeba3328',
	/** 
	 *             The secondary performing person (support clinician).            
	 */
	SecondaryPerformer : '4ff91e06-2e39-44e3-9fbe-0d828fe318fe',
	/** 
	 *             The player entity represents a specimen collected for the purpose of testing and diagnosis            
	 */
	Specimen : 'bce17b21-05b2-4f02-bf7a-c6d3561aa948',
	/** 
	 *             The player entity is the subject of an act, but not necessarily the record target (meaning the act is about a particular entity but not to be attached to their record)            
	 */
	Subject : '03067700-ce37-405f-8ed3-e4965ba2f601',
	/** 
	 *             The player entity is responsible for tracking the progress of the act            
	 */
	Tracker : 'c3be013a-20c5-4c20-840c-d9dbb15d040e',
	/** 
	 *             The person who transcribed data from the original act.            
	 */
	Transcriber : 'de3f7527-e3c9-45ef-8574-00ca4495f767',
	/** 
	 *             The player entity represents a contact entity in case of an emergency occurs during the act.            
	 */
	UgentNotificationContact : '01b87999-85a7-4f5c-9b7e-892f1195cfe3',
	/** 
	 *             The player entity was responsible for verifying the accuracy of the data in the act            
	 */
	Verifier : 'f9dc5787-dd4d-42c6-a082-ac7d11956fda',
	/** 
	 *             The player entity represents an entity where the act occurred "via" this entity (i.e. in transport)            
	 */
	Via : '5b0fac74-5ac6-44e6-99a4-6813c0e2f4a9',
	/** 
	 *             The player entity represents a legal witness to the act occurring.            
	 */
	Witness : '0b82357f-5ae0-4543-ab8e-a33e9b315bab',
},  // ActParticipationKey 
// OpenIZ.Core.Model.Constants.EntityRelationshipTypeKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Base entity relationship type keys            
 */
EntityRelationshipTypeKeys : { 
	/** 
	 *             The access            
	 */
	Access : 'ddc1b705-c768-4c7a-8f69-76ad4b167b40',
	/** 
	 *             The active moiety            
	 */
	ActiveMoiety : '212b1b6b-b074-4a75-862d-e4e194252044',
	/** 
	 *             The administerable material            
	 */
	AdministerableMaterial : 'b52c7e95-88b8-4c4c-836a-934277afdb92',
	/** 
	 *             The adopted child            
	 */
	AdoptedChild : '8fa25b69-c9c2-4c40-84c1-0ea9641a12ec',
	/** 
	 *             The adopted daughter            
	 */
	AdoptedDaughter : '2b4b2ed8-f90c-4193-870a-f48bc39657c1',
	/** 
	 *             The adopted son            
	 */
	AdoptedSon : 'ce50ba92-cd21-43c4-8582-34e7fbb3170f',
	/** 
	 *             The affiliate            
	 */
	Affiliate : '8de7b5e7-c941-42bd-b735-52d750efc5e6',
	/** 
	 *             The agent            
	 */
	Agent : '867fd445-d490-4619-804e-75c04b8a0e57',
	/** 
	 *             The aliquot            
	 */
	Aliquot : 'cff670e4-965e-4288-b966-47a44479d2ad',
	/** 
	 *             The assigned            
	 */
	Assigned : 'a87a6d21-2ca6-4aea-88f3-6135cceb58d1',
	/** 
	 *             The assigned entity            
	 */
	AssignedEntity : '77b7a04b-c065-4faf-8ec0-2cdad4ae372b',
	/** 
	 *             The aunt            
	 */
	Aunt : '0ff2ab03-6e0a-40d1-8947-04c4937b4cc4',
	/** 
	 *             The birthplace            
	 */
	Birthplace : 'f3ef7e48-d8b7-4030-b431-aff7e0e1cb76',
	/** 
	 *             The brother            
	 */
	Brother : '24380d53-ea22-4820-9f06-8671f774f133',
	/** 
	 *             The brotherinlaw            
	 */
	Brotherinlaw : '0a4c87e2-16c3-4361-be3c-dd765ee4bc7d',
	/** 
	 *             The caregiver            
	 */
	Caregiver : '31b0dfcb-d7ba-452a-98b9-45ebccd30732',
	/** 
	 *             The case subject            
	 */
	CaseSubject : 'd7ad48c0-889d-41e2-99e9-be5e6c5327b2',
	/** 
	 *             The child            
	 */
	Child : '739457d0-835a-4a9c-811c-42b5e92ed1ca',
	/** 
	 *             The child inlaw            
	 */
	ChildInlaw : '8bf23192-de75-48eb-abee-81a9a15332f8',
	/** 
	 *             The citizen            
	 */
	Citizen : '35b13152-e43c-4bcb-8649-a9e83bee33a2',
	/** 
	 *             The claimant            
	 */
	Claimant : '9d256279-f1ac-46b3-a974-dd13e2ad4f72',
	/** 
	 *             The clinical research investigator            
	 */
	ClinicalResearchInvestigator : '43ad7bc0-2ed8-4b27-97e5-b3db00a07d17',
	/** 
	 *             The clinical research sponsor            
	 */
	ClinicalResearchSponsor : '66c96ae6-c5c4-4d66-9bd0-a00c56e831da',
	/** 
	 *             The commissioning party            
	 */
	CommissioningParty : '33bd1401-dfdb-40e7-a914-0a695ad5186e',
	/** 
	 *             The contact            
	 */
	Contact : 'b1d2148d-bb35-4337-8fe6-021f5a3ac8a3',
	/** 
	 *             The cousin            
	 */
	Cousin : '1c0f931c-9c49-4a52-8fbf-5217c52ea778',
	/** 
	 *             The coverage sponsor            
	 */
	CoverageSponsor : '8ff9d9a5-a206-4566-82cd-67b770d7ce8a',
	/** 
	 *             The covered party            
	 */
	CoveredParty : 'd4844672-c0d7-434c-8377-6dd0655b0532',
	/** 
	 *             The daughter            
	 */
	Daughter : '8165b43f-8103-4ed3-bac6-4fc0df8c1a84',
	/** 
	 *             The daughter inlaw            
	 */
	DaughterInlaw : '76fdf0e7-cfe0-47b4-9630-c645f254cdfd',
	/** 
	 *             The dedicated service delivery location            
	 */
	DedicatedServiceDeliveryLocation : '455f1772-f580-47e8-86bd-b5ce25d351f9',
	/** 
	 *             The dependent            
	 */
	Dependent : 'f28ed78f-85ab-47a1-ba08-b5051e62d6c3',
	/** 
	 *             The distributed material            
	 */
	DistributedMaterial : 'f5547ada-1eb9-40bb-b163-081567d869e7',
	/** 
	 *             The domestic partner            
	 */
	DomesticPartner : '3db182e2-653b-4bfd-a300-32f23345d1c0',
	/** 
	 *             The emergency contact            
	 */
	EmergencyContact : '25985f42-476a-4455-a977-4e97a554d710',
	/** 
	 *             The employee            
	 */
	Employee : 'b43c9513-1c1c-4ed0-92db-55a904c122e6',
	/** 
	 *             The exposed entity            
	 */
	ExposedEntity : 'ab39087c-17d3-421a-a1e3-2de4e0ab9faf',
	/** 
	 *             The family member            
	 */
	FamilyMember : '38d66ec7-0cc8-4609-9675-b6ff91ede605',
	/** 
	 *             The father            
	 */
	Father : '40d18ecc-8ff8-4e03-8e58-97a980f04060',
	/** 
	 *             The fatherinlaw            
	 */
	Fatherinlaw : 'b401dd81-931c-4aad-8fd8-22a6ac2ea3dc',
	/** 
	 *             The foster child            
	 */
	FosterChild : 'abfe2637-d338-4090-b3a5-3ec19a47be6a',
	/** 
	 *             The foster daughter            
	 */
	FosterDaughter : 'e81d6773-97e3-4b2d-b6a3-a4624ba5c6a9',
	/** 
	 *             The foster son            
	 */
	FosterSon : 'decd6250-7e8b-4b77-895d-31953cf1387a',
	/** 
	 *             The grandchild            
	 */
	Grandchild : 'c33adda2-a4ed-4092-8d9c-b8e3fbd5d90b',
	/** 
	 *             The granddaughter            
	 */
	Granddaughter : '3cb1993f-3703-453f-87be-21b606db7631',
	/** 
	 *             The grandfather            
	 */
	Grandfather : '48c59444-fec0-43b8-aa2c-7aedb70733ad',
	/** 
	 *             The grandmother            
	 */
	Grandmother : 'b630ba2c-8a00-46d8-bf64-870d381d8917',
	/** 
	 *             The grandparent            
	 */
	Grandparent : 'fa646df9-7d64-4d1f-ae9a-6261fd5fd6ae',
	/** 
	 *             The grandson            
	 */
	Grandson : 'f7a64463-bc75-44d4-a8ca-c9fbc2c87175',
	/** 
	 *             The great grandfather            
	 */
	GreatGrandfather : 'bfe24b5d-9c32-4df3-ad7b-eaa19e7d4afb',
	/** 
	 *             The great grandmother            
	 */
	GreatGrandmother : '02fbc345-1a25-4f78-aeea-a12584a1eec3',
	/** 
	 *             The great grandparent            
	 */
	GreatGrandparent : '528feb11-ae81-426a-be1f-ce74c83009eb',
	/** 
	 *             The guarantor            
	 */
	Guarantor : 'f5b10c57-3ae1-41ea-8649-1cf8d9848ae1',
	/** 
	 *             The guard            
	 */
	GUARD : '845120de-e6f7-4cec-94aa-e6e943c91367',
	/** 
	 *             The guardian            
	 */
	Guardian : '3b8e2334-4ccc-4f24-8aae-37341ea03d3e',
	/** 
	 *             The halfbrother            
	 */
	Halfbrother : '25cae2f2-d1ec-4efe-a92f-d479785f7d8a',
	/** 
	 *             The halfsibling            
	 */
	Halfsibling : '8452ecb9-d762-4c4a-96b2-81d130cb729b',
	/** 
	 *             The halfsister            
	 */
	Halfsister : 'ce42c680-a783-4cde-bcd1-e261d6fd68a0',
	/** 
	 *             The healthcare provider            
	 */
	HealthcareProvider : '6b04fed8-c164-469c-910b-f824c2bda4f0',
	/** 
	 *             The health chart            
	 */
	HealthChart : '5b0f8c93-57c9-4dff-b59a-9564739ef445',
	/** 
	 *             The held entity            
	 */
	HeldEntity : '9c02a621-8565-46b4-94ff-a2bd210989b1',
	/** 
	 *             The husband            
	 */
	Husband : '62aca44c-b57c-44fd-9703-fcdff97c04b6',
	/** 
	 *             The identified entity            
	 */
	IdentifiedEntity : 'c5c8b935-294f-4c90-9d81-cbf914bf5808',
	/** 
	 *             The incidental service delivery location            
	 */
	IncidentalServiceDeliveryLocation : '41baf7aa-5ffd-4421-831f-42d4ab3de38a',
	/** 
	 *             The individual            
	 */
	Individual : '47049b0f-f189-4e19-9aa8-7c38adb2491a',
	/** 
	 *             The investigation subject            
	 */
	InvestigationSubject : '0c522bd1-dfa2-43cb-a98e-f6ff137968ae',
	/** 
	 *             The invoice payor            
	 */
	InvoicePayor : '07c922d2-12c9-415a-95d4-9b3fed4959d6',
	/** 
	 *             The isolate            
	 */
	Isolate : '020c28a0-7c52-42f4-a046-db9e329d5a42',
	/** 
	 *             The licensed entity            
	 */
	LicensedEntity : 'b9fe057e-7f57-42eb-89d7-67c69646c0c4',
	/** 
	 *             The maintained entity            
	 */
	MaintainedEntity : '77b6d8cd-05a0-4b1f-9e14-b895203bf40c',
	/** 
	 *             The manufactured product            
	 */
	ManufacturedProduct : '6780df3b-afbd-44a3-8627-cbb3dc2f02f6',
	/** 
	 *             The maternal aunt            
	 */
	MaternalAunt : '96ea355d-0c68-481f-8b6f-1b00a101ab8f',
	/** 
	 *             The maternal cousin            
	 */
	MaternalCousin : 'd874cde5-7d76-4f1d-97e6-db7e82bac958',
	/** 
	 *             The maternal grandfather            
	 */
	MaternalGrandfather : '360f6a77-fdb5-4fb6-b223-3cd1047fd08e',
	/** 
	 *             The maternal grandmother            
	 */
	MaternalGrandmother : 'ea13832b-2e38-4bb6-b55d-ae749ccaba95',
	/** 
	 *             The maternal grandparent            
	 */
	MaternalGrandparent : '66e0dbd1-9065-4af8-808d-89edd302f264',
	/** 
	 *             The maternal greatgrandfather            
	 */
	MaternalGreatgrandfather : 'abe6d0d1-4e37-4b7c-9acc-eedb2c36f9cd',
	/** 
	 *             The maternal greatgrandmother            
	 */
	MaternalGreatgrandmother : 'fe4f72e6-84f8-4276-ae64-2ef1f2ff406f',
	/** 
	 *             The maternal greatgrandparent            
	 */
	MaternalGreatgrandparent : '59bc87d3-1618-4f14-81d2-71072c1f37e9',
	/** 
	 *             The maternal uncle            
	 */
	MaternalUncle : '4e299c46-f06f-4efc-b3c0-b7b659a120f2',
	/** 
	 *             The military person            
	 */
	MilitaryPerson : '1bcfb08d-c6fa-41dd-98bf-06336a33a3b7',
	/** 
	 *             The mother            
	 */
	Mother : '29ff64e5-b564-411a-92c7-6818c02a9e48',
	/** 
	 *             The motherinlaw            
	 */
	Motherinlaw : 'f941988a-1c55-4408-ab57-e9ed35b2a24d',
	/** 
	 *             The named insured            
	 */
	NamedInsured : '3d907f37-085c-4c26-b59b-62e40621dafd',
	/** 
	 *             The natural brother            
	 */
	NaturalBrother : 'daf11eb1-fcc2-4521-a1c0-daebaf0a923a',
	/** 
	 *             The natural child            
	 */
	NaturalChild : '80097e75-a232-4a9f-878f-7e60ec70f921',
	/** 
	 *             The natural daughter            
	 */
	NaturalDaughter : '6a181a3c-7241-4325-b011-630d3ca6dc4a',
	/** 
	 *             The natural father            
	 */
	NaturalFather : '233d890b-04ef-4365-99ad-26cb4e1f75f3',
	/** 
	 *             The natural father of fetus            
	 */
	NaturalFatherOfFetus : '8e88debc-d175-46f3-9b48-106f9c151cd2',
	/** 
	 *             The natural mother            
	 */
	NaturalMother : '059d689a-2392-4ffb-b6ae-682c9ded8da2',
	/** 
	 *             The natural parent            
	 */
	NaturalParent : 'e6851b39-a771-4a5e-8aa8-9ba140b3dca3',
	/** 
	 *             The natural sibling            
	 */
	NaturalSibling : '0b89fb65-ca8e-4a4d-9d25-0bae3f4d7a59',
	/** 
	 *             The natural sister            
	 */
	NaturalSister : '8ea21d7d-6ee9-449b-a1dc-c4aa0ff7f5b9',
	/** 
	 *             The natural son            
	 */
	NaturalSon : '9f17d4cf-a67f-4ac6-8c50-718af6e264ee',
	/** 
	 *             The nephew            
	 */
	Nephew : '5c5af1d2-0e6d-458f-9574-3ad61c393a90',
	/** 
	 *             The next of kin            
	 */
	NextOfKin : '1ee4e74f-542d-4544-96f6-266a6247f274',
	/** 
	 *             The niece            
	 */
	Niece : '0a50962a-60b4-44d8-a7f6-1eb2aa5967cc',
	/** 
	 *             The niece nephew            
	 */
	NieceNephew : 'a907e4d8-d823-478f-9c5a-6facae6b4b5b',
	/** 
	 *             The notary public            
	 */
	NotaryPublic : 'f1ef6c46-05eb-4482-baeb-eaf0a8e5ffef',
	/** 
	 *             The owned entity            
	 */
	OwnedEntity : '117da15c-0864-4f00-a987-9b9854cba44e',
	/** 
	 *             The parent            
	 */
	Parent : 'bfcbb345-86db-43ba-b47e-e7411276ac7c',
	/** 
	 *             The parent inlaw            
	 */
	ParentInlaw : '5e2b0afe-724e-41cd-9be2-9030646f2529',
	/** 
	 *             The part            
	 */
	Part : 'b2feb552-8eaf-45fe-a397-f789d6f4728a',
	/** 
	 *             The paternal aunt            
	 */
	PaternalAunt : '6a1e9e8b-d0c3-44f0-9906-a6458685e269',
	/** 
	 *             The paternal cousin            
	 */
	PaternalCousin : '60affe56-126d-43ee-9fde-5f117e41c7a8',
	/** 
	 *             The paternal grandfather            
	 */
	PaternalGrandfather : '2fd5c939-c508-4250-8efb-13b772e56b7f',
	/** 
	 *             The paternal grandmother            
	 */
	PaternalGrandmother : 'bfdb07db-9721-4ec3-94e1-4bd9f0d6985c',
	/** 
	 *             The paternal grandparent            
	 */
	PaternalGrandparent : 'a3d362a4-4931-4bef-af18-ac59dd092981',
	/** 
	 *             The paternal greatgrandfather            
	 */
	PaternalGreatgrandfather : '0aeec758-c20f-43e4-9789-8c44629f5941',
	/** 
	 *             The paternal greatgrandmother            
	 */
	PaternalGreatgrandmother : '0fcba203-1238-4001-beb7-19a667506ade',
	/** 
	 *             The paternal greatgrandparent            
	 */
	PaternalGreatgrandparent : '08a98950-3391-4a66-a1c8-421c6fd82911',
	/** 
	 *             The paternal uncle            
	 */
	PaternalUncle : '853c85de-4817-4328-a121-6a3bdafbf82e',
	/** 
	 *             The patient            
	 */
	Patient : 'bacd9c6f-3fa9-481e-9636-37457962804d',
	/** 
	 *             The payee            
	 */
	Payee : '734551e1-2960-4a68-93a2-b277db072a43',
	/** 
	 *             The personal relationship            
	 */
	PersonalRelationship : 'abfd3fe8-9526-48fb-b366-35baca9bd170',
	/** 
	 *             The place of death            
	 */
	PlaceOfDeath : '9bbe0cfe-faab-4dc9-a28f-c001e3e95e6e',
	/** 
	 *             The policy holder            
	 */
	PolicyHolder : 'cec017ef-4e49-41af-8596-abad1a91c9d0',
	/** 
	 *             The program eligible            
	 */
	ProgramEligible : 'cbe2a00c-e1d5-44e9-aae3-d7d03e3c2efa',
	/** 
	 *             The qualified entity            
	 */
	QualifiedEntity : '6521dd09-334b-4fbf-9c89-1ad5a804326c',
	/** 
	 *             The regulated product            
	 */
	RegulatedProduct : '20e98d17-e24d-4c64-b09e-521a177ccd05',
	/** 
	 *             The research subject            
	 */
	ResearchSubject : 'ef597ffe-d965-4398-b55a-650530ebb997',
	/** 
	 *             The retailed material            
	 */
	RetailedMaterial : '703df8f4-b124-44c5-9506-1ab74ddfd91d',
	/** 
	 *             The roomate            
	 */
	Roomate : 'bbfac1ed-5464-4100-93c3-8685b052a2cf',
	/** 
	 *             The service delivery location            
	 */
	ServiceDeliveryLocation : 'ff34dfa7-c6d3-4f8b-bc9f-14bcdc13ba6c',
	/** 
	 *             The sibling            
	 */
	Sibling : '685eb506-6b97-41c1-b201-b6b932a3f3aa',
	/** 
	 *             The sibling inlaw            
	 */
	SiblingInlaw : 'fd892cf8-db4f-4e4e-a13b-4eb3bdde5be5',
	/** 
	 *             The significant other            
	 */
	SignificantOther : '2eab5298-bc83-492c-9004-ed3499246afe',
	/** 
	 *             The signing authority or officer            
	 */
	SigningAuthorityOrOfficer : '757f98df-14e0-446a-bd50-bb0affb34f09',
	/** 
	 *             The sister            
	 */
	Sister : 'cd1e8904-31dc-4374-902d-c91f1de23c46',
	/** 
	 *             The sisterinlaw            
	 */
	Sisterinlaw : 'dcae9718-ab81-4737-b071-36cf1175804d',
	/** 
	 *             The son            
	 */
	Son : 'f115c204-8485-4cf3-8815-3c6738465e30',
	/** 
	 *             The son inlaw            
	 */
	SonInlaw : '34f7bc11-2288-471a-af38-553ae6b8410c',
	/** 
	 *             The specimen            
	 */
	Specimen : 'bce17b21-05b2-4f02-bf7a-c6d3561aa948',
	/** 
	 *             The spouse            
	 */
	Spouse : '89bdc57b-d85c-4e85-94e8-c17049540a0d',
	/** 
	 *             The stepbrother            
	 */
	Stepbrother : '5951097b-1a13-4bce-bbf2-9abf52f98dc8',
	/** 
	 *             The step child            
	 */
	StepChild : '4cdef917-4fb0-4cdf-b44d-b73486c41845',
	/** 
	 *             The stepdaughter            
	 */
	Stepdaughter : 'f71e193a-0562-46e9-99dd-437d23663ec3',
	/** 
	 *             The stepfather            
	 */
	Stepfather : 'bb437e4d-7472-48c1-a6e7-576545a782fa',
	/** 
	 *             The stepmother            
	 */
	Stepmother : '5a0539cc-093b-448e-aec6-0d529ed0087f',
	/** 
	 *             The step parent            
	 */
	StepParent : 'f172eee7-7f4b-4022-81d0-76393a1200ae',
	/** 
	 *             The step sibling            
	 */
	StepSibling : '7e6bc25d-5dea-4645-af3d-aa854b7b6f2f',
	/** 
	 *             The stepsister            
	 */
	Stepsister : 'cb73d085-026c-4bc7-a1de-356bfd636246',
	/** 
	 *             The stepson            
	 */
	Stepson : 'cfa978f4-140c-430d-82f8-1e6f2d74f48d',
	/** 
	 *             The student            
	 */
	Student : '0c157566-d1e9-4976-8542-473caa9ba2a4',
	/** 
	 *             The subscriber            
	 */
	Subscriber : 'f31a2a5b-ce13-47e1-a0fb-d704f31547db',
	/** 
	 *             The territory of authority            
	 */
	TerritoryOfAuthority : 'c6b92576-1d62-4896-8799-6f931f8ab607',
	/** 
	 *             The therapeutic agent            
	 */
	TherapeuticAgent : 'd6657fdb-4ef3-4131-af79-14e01a21faca',
	/** 
	 *             The uncle            
	 */
	Uncle : 'cdd99260-107c-4a4e-acaf-d7c9c7e90fdd',
	/** 
	 *             The underwriter            
	 */
	Underwriter : 'a8fcd83f-808b-494b-8a1c-ec2c6dbc3dfa',
	/** 
	 *             The used entity            
	 */
	UsedEntity : '08fff7d9-bac7-417b-b026-c9bee52f4a37',
	/** 
	 *             The warranted product            
	 */
	WarrantedProduct : '639b4b8f-afd3-4963-9e79-ef0d3928796a',
	/** 
	 *             The wife            
	 */
	Wife : 'a3ff423e-81d5-4571-8edf-03c295189a23',
	/** 
	 *             The replaces            
	 */
	Replaces : 'd1578637-e1cb-415e-b319-4011da033813',
},  // EntityRelationshipTypeKeys 
// OpenIZ.Core.Model.Constants.StatusKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Status concepts represent the current status of entities, acts, and concepts.            
 */
StatusKeys : { 
	/** 
	 *             When an entity or act is active, it means the information or entity is currently correct and ongoing            
	 */
	Active : 'c8064cbd-fa06-4530-b430-1a52f1530c27',
	/** 
	 *             Indicates that an act has been completed and now represents an act in the past            
	 */
	Completed : 'afc33800-8225-4061-b168-bacc09cdbae3',
	/** 
	 *             Indicates that the data is new, and may require additional verification or actions            
	 */
	New : 'c34fcbf1-e0fe-4989-90fd-0dc49e1b9685',
	/** 
	 *             Indicates that the entity or act never existed, and was entered in error            
	 */
	Nullified : 'cd4aa3c4-02d5-4cc9-9088-ef8f31e321c5',
	/** 
	 *             Indicates that the act was cancelled before being completed            
	 */
	Cancelled : '3efd3b6e-02d5-4cc9-9088-ef8f31e321c5',
	/** 
	 *             Indicates that the entity or act did exist at one point, however it no longer exists            
	 */
	Obsolete : 'bdef5f90-5497-4f26-956c-8f818cce2bd2',
},  // StatusKeys 
// OpenIZ.Core.Model.Constants.TelecomAddressUseKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Telecommunications address use keys            
 */
TelecomAddressUseKeys : { 
	/** 
	 *             answering service            
	 */
	AnsweringService : '1ecd7b17-b5ff-4cae-9c3b-c1258132d137',
	/** 
	 *             Emergency contact            
	 */
	EmergencyContact : '25985f42-476a-4455-a977-4e97a554d710',
	/** 
	 *             Mobile phone contact            
	 */
	MobileContact : 'e161f90e-5939-430e-861a-f8e885cc353d',
	/** 
	 *             pager            
	 */
	Pager : '788000b4-e37a-4055-a2aa-c650089ce3b1',
	/** 
	 *             public (800 number example) contact            
	 */
	Public : 'ec35ea7c-55d2-4619-a56b-f7a986412f7f',
	/** 
	 *             temporary contact            
	 */
	TemporaryAddress : 'cef6ea31-a097-4f59-8723-a38c727c6597',
	/** 
	 *             For use in the workplace            
	 */
	WorkPlace : 'eaa6f08e-bb8e-4457-9dc0-3a1555fadf5c',
},  // TelecomAddressUseKeys 
// OpenIZ.Core.Model.Patch.PatchOperationType, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Represents a patch operation type            
 */
PatchOperationType : { 
	/** 
	 *             Patch operation adds the specified value to the array            
	 */
	Add : 'Add',
	/** 
	 *             Patch operation removes the specified value from the array            
	 */
	Remove : 'Remove',
	/** 
	 *             Patch operation replaces the specified item at the path             
	 */
	Replace : 'Replace',
	/** 
	 *             Patch should test value before proceeding            
	 */
	Test : 'Test',
},  // PatchOperationType 
// OpenIZ.Core.Model.Constants.AddressComponentKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Represents address component types            
 */
AddressComponentKeys : { 
	/** 
	 *             An additional locator (example: Beside the red barn).            
	 */
	AdditionalLocator : 'd2312b8e-bdfb-4012-9397-f14336f8d206',
	/** 
	 *             An address line as would appear on an address (example: 123 Main Street West)            
	 */
	AddressLine : '4f342d28-8850-4daf-8bca-0b44a255f7ed',
	/** 
	 *             Identifies a particular building on a street (example: A23 Building)            
	 */
	BuildingNumber : 'f3c86e99-8afc-4947-9dd8-86412a34b1c7',
	/** 
	 *             Identifies a numeric identifier for a building (example: 123)            
	 */
	BuildingNumberNumeric : '3258b4d6-e4dc-43e6-9f29-fd8423a2ae12',
	/** 
	 *             Identifies a suffix to the building number (example: 123 *SECTOR 7*)            
	 */
	BuildingNumberSuffix : 'b2dbf05c-584d-46db-8cbf-026a6ea30d81',
	/** 
	 *             Identifies the person where deliveries should be care-of (example: c/o Bob Smith)            
	 */
	CareOf : '8c89a89e-08c5-4374-87f9-adb3c9261df6',
	/** 
	 *             The census tract which is used for political counting of the census            
	 */
	CensusTract : '4b3a347c-28fa-4560-a1a9-3795c9db3d3b',
	/** 
	 *             The town or city (example: Toronto)            
	 */
	City : '05b85461-578b-4988-bca6-e3e94be9db76',
	/** 
	 *             The country in which the address resides (example: Canada)            
	 */
	Country : '48b2ffb3-07db-47ba-ad73-fc8fb8502471',
	/** 
	 *             The county or sub-division of a sub-national unit (example: Clark County)            
	 */
	County : 'd9489d56-ddac-4596-b5c6-8f41d73d8dc5',
	/** 
	 *             Represents a meaningless delimiter such as dash, or newline            
	 */
	Delimiter : '4c6b9519-a493-44a9-80e6-32d85109b04b',
	/** 
	 *             Represents an address line to be used for delivery rather than physical location (example: Loading Dock #4)            
	 */
	DeliveryAddressLine : 'f6139b21-3a36-4a3f-b498-0c661f06df59',
	/** 
	 *             Represents the area where the delivery should take place            
	 */
	DeliveryInstallationArea : 'ec9d5ab8-3be1-448f-9346-6a08253f9dea',
	/** 
	 *             The delivery installation qualifier.            
	 */
	DeliveryInstallationQualifier : '78fb6eed-6549-4f22-ab3e-f3696da050bc',
	/** 
	 *             The delivery installation type.            
	 */
	DeliveryInstallationType : '684fb800-145c-47c5-98c5-e7aa53802b69',
	/** 
	 *             The delivery mode.            
	 */
	DeliveryMode : '12608636-910d-4bac-b849-7f999de20332',
	/** 
	 *             The delivery mode identifier.            
	 */
	DeliveryModeIdentifier : '08bd6027-47eb-43de-8454-59b7a5d00a3e',
	/** 
	 *             Represents a directory such as north, south, east, or west            
	 */
	Direction : '1f678716-ab8f-4856-9f76-d82fe3165c22',
	/** 
	 *             A codified adminsitrative unit used to locate the address (zip code or postal code)            
	 */
	PostalCode : '78a47122-f9bf-450f-a93f-90a103c5f1e8',
	/** 
	 *             Represents a PO box where delivery of mail should take place            
	 */
	PostBox : '2047f216-f41e-4cfb-a024-05d4d3de52f5',
	/** 
	 *             Represents a precinct or sub-division of a city such as a burrogh            
	 */
	Precinct : 'acafe0f2-e209-43bb-8633-3665fd7c90ba',
	/** 
	 *             Represents a state or province, or a sub-division of a national boundary            
	 */
	State : '8cf4b0b0-84e5-4122-85fe-6afa8240c218',
	/** 
	 *             Represents a physical street delivery line (example: 123 Main Street West)            
	 */
	StreetAddressLine : 'f69dcfa8-df18-403b-9217-c59680bad99e',
	/** 
	 *             Represents the name portion of a street address (example: Main St.)            
	 */
	StreetName : '0432d671-abc3-4249-872c-afd5274c2298',
	/** 
	 *             The street name base portion of a street address (Example: Main)            
	 */
	StreetNameBase : '37c7dbc8-4ac6-464a-af65-d65fcba60238',
	/** 
	 *             The street type (example: Street, Road, Hwy)            
	 */
	StreetType : '121953f6-0465-41de-8f7a-b0e08204c771',
	/** 
	 *             Identifies the type of unit (example: Suite, Apartment, Unit)            
	 */
	UnitDesignator : 'b18e71cb-203c-4640-83f0-cc86debbbbc0',
	/** 
	 *             The identifier of the unit (example: 820)            
	 */
	UnitIdentifier : '908c09df-81fe-45ac-9233-0881a278a401',
},  // AddressComponentKeys 
// OpenIZ.Core.Model.Constants.NameComponentKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Name component type keys            
 */
NameComponentKeys : { 
	/** 
	 *             The name component represents a delimeter in a name such as hyphen or space            
	 */
	Delimiter : '4c6b9519-a493-44a9-80e6-32d85109b04b',
	/** 
	 *             The name component represents the surname            
	 */
	Family : '29b98455-ed61-49f8-a161-2d73363e1df0',
	/** 
	 *             The name component represents the given name            
	 */
	Given : '2f64bde2-a696-4b0a-9690-b21ebd7e5092',
	/** 
	 *             The name component represents the prefix such as Von or Van            
	 */
	Prefix : 'a787187b-6be4-401e-8836-97fc000c5d16',
	/** 
	 *             The name component represents a suffix such as III or Esq.            
	 */
	Suffix : '064523df-bb03-4932-9323-cdf0cc9590ba',
	/** 
	 *             The name component represents a formal title like Mr, Dr, Capt.            
	 */
	Title : '4386d92a-d81b-4033-b968-01e57e20d5e0',
},  // NameComponentKeys 
// OpenIZ.Core.Model.Constants.PhoneticAlgorithmKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Phonetic algorithm keys for built in phonetic algorithms in OpenIZ            
 */
PhoneticAlgorithmKeys : { 
	/** 
	 *             Represents the metaphone phonetic algorithm            
	 */
	Metaphone : 'd79a4dc6-66a6-4602-8fcb-7dc09a895793',
	/** 
	 *             Represents the null phonetic algorithm            
	 */
	None : '402cd339-d0e4-46ce-8fc2-12a4b0e17226',
	/** 
	 *             Represents the soundex algorithm            
	 */
	Soundex : '3352a79a-d2e0-4e0c-9b48-6fd2a202c681',
},  // PhoneticAlgorithmKeys 
// OpenIZ.Core.Model.Constants.ConceptRelationshipTypeKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Reference type identifiers            
 */
ConceptRelationshipTypeKeys : { 
	/** 
	 *             The source concept has the inverse meaning of the target concept            
	 */
	InverseOf : 'ad27293d-433c-4b75-88d2-b5360cd95450',
	/** 
	 *             The source concept is a member of the target concept            
	 */
	MemberOf : 'a159d45b-3c34-4e1b-9b75-9193a7528ced',
	/** 
	 *             The source concept is a negation of the target concept            
	 */
	NegationOf : 'ae8b4f2f-009f-4e0d-b35e-5a89555c5947',
	/** 
	 *             The source concept has the same meaning as the target concept            
	 */
	SameAs : '2c4dafc2-566a-41ae-9ebc-3097d7d22f4a',
},  // ConceptRelationshipTypeKeys 
// OpenIZ.Core.Model.Constants.ConceptClassKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Concept classification identifiers for built-in concept classes            
 */
ConceptClassKeys : { 
	/** 
	 *             Classification codes            
	 */
	ClassCode : '17fd5254-8c25-4abb-b246-083fbe9afa15',
	/** 
	 *             Diagnosis codes            
	 */
	Diagnosis : '92cdea39-b9a3-4a5b-bc88-a6646c74240d',
	/** 
	 *             Clinical findings            
	 */
	Finding : 'e445e207-60a3-401a-9b81-a8ac2479f4a6',
	/** 
	 *             Form codes (shape, texture, etc.)            
	 */
	Form : '17ee5254-8c25-4abb-b246-083fbe9afa15',
	/** 
	 *             Material classifications            
	 */
	Material : 'dc9cbc32-b8ea-4144-bef1-dc618e28f4d7',
	/** 
	 *             Mood classifications            
	 */
	Mood : 'bba99722-23ce-469a-8fa5-10deba853d35',
	/** 
	 *             Other classifications            
	 */
	Other : '0d6b3439-c9be-4480-af39-eeb457c052d0',
	/** 
	 *             Problems or condition codes            
	 */
	Problem : '4bd7f8e6-e4b8-4dbc-93a7-cf14fbaf9700',
	/** 
	 *             Relationship class identifier            
	 */
	Relationship : 'f51dfdcd-039b-4e1f-90be-3cf56aef8da4',
	/** 
	 *             Routes of adminstration class identifier            
	 */
	Route : 'a8a900d3-a07e-4e02-b45f-580d09baf047',
	/** 
	 *             Status codes             
	 */
	Status : '54b93182-fc19-47a2-82c6-089fd70a4f45',
	/** 
	 *             Stock classification codes            
	 */
	Stock : 'ffd8304a-43ec-4ebc-95fc-fb4a4f2338f0',
	/** 
	 *             Unit of measure classification            
	 */
	UnitOfMeasure : '1ef69347-ef03-4ff7-b3c5-6334448845e6',
},  // ConceptClassKeys 
// OpenIZ.Core.Model.Constants.CodeSystemKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Code system identifiers            
 */
CodeSystemKeys : { 
	/** 
	 *             Commonn Vaccination Codes (CDC)            
	 */
	CVX : 'eba4f94a-2cad-4bb3-aca7-f4e54eaac4bd',
	/** 
	 *             International Classification of Diseases Version 10            
	 */
	ICD10 : 'f7a5cbd8-5425-415e-8308-d14b94f56917',
	/** 
	 *             International Classification of Diseases Version 10 - Clinical Management             
	 */
	ICD10CM : 'ed9742e5-fa5b-4644-9fb5-2f935ed08b1e',
	/** 
	 *             International Classification of Diseases Version 9            
	 */
	ICD9 : '51ea1e1b-edc0-455a-a72b-9076860e284d',
	/** 
	 *             ISO-639-1 (Language Codes)            
	 */
	ISO6391 : 'eb04fe20-bbbc-4c70-9eef-045bc4f70982',
	/** 
	 *             ISO639-2 (3 letter Language Codes)            
	 */
	ISO6392 : '089044ea-dd41-4258-a497-e6247dd364f6',
	/** 
	 *             Logical Observations Identifiers Names and Codes (maintained by Regenstrief Institute)            
	 */
	LOINC : '08c59397-706b-456a-aeb1-9e7d5a2adc94',
	/** 
	 *             Systematized Nomenclature of Medicine-Clinical Terms (maintained by IHTSDO)            
	 */
	SNOMEDCT : 'b3030751-d4db-420b-b765-e837607820cd',
	/** 
	 *             Universal Codes for the Unit Of Measure            
	 */
	UCUM : '4853a702-fff3-4efb-8dd7-54aacca53664',
	/** 
	 *             The postal address use code system key.            
	 */
	PostalAddressUse : '0c4d091e-8701-4953-b16d-b8ca8e85de46',
	/** 
	 *             The entity name use code system key.            
	 */
	EntityNameUse : '77816823-9392-4ca7-83dd-6e7d4b4164e7',
	/** 
	 *             The administrative gender code system key.            
	 */
	AdministrativeGender : '7a3a7139-b93e-4a99-bd54-749e30fe712a',
},  // CodeSystemKeys 
// OpenIZ.Core.Model.Constants.ActClassKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Represents a series of class keys for use on acts.            
 */
ActClassKeys : { 
	/** 
	 *             The act represents generic account management such as adjudications, financial adjustments, stock counting, etc.            
	 */
	AccountManagement : 'ca44a469-81d7-4484-9189-ca1d55afecbc',
	/** 
	 *             The act represents a generic act which has no special classification            
	 */
	Act : 'd874424e-c692-4fd8-b94e-642e1cbf83e9',
	/** 
	 *             The act represents a simple battery of procedures/administrations/tests/etc.            
	 */
	Battery : '676de278-64aa-44f2-9b69-60d61fc1f5f5',
	/** 
	 *             The act represents some provision of care such as the seeking out services.            
	 */
	CareProvision : '1071d24e-6fe9-480f-8a20-b1825ae4d707',
	/** 
	 *             The act represents a problem or condition which the patient is suffering from.            
	 */
	Condition : '1987c53c-7ab8-4461-9ebc-0d428744a8c0',
	/** 
	 *             The control act event key is used to describe an infrastructural act which has no clinical meaning but can be used to wrap technical details.            
	 */
	ControlAct : 'b35488ce-b7cd-4dd4-b4de-5f83dc55af9f',
	/** 
	 *             The act represents an encounter such as the patient presenting for care and receiving services during a visit.            
	 */
	Encounter : '54b52119-1709-4098-8911-5df6d6c84140',
	/** 
	 *             The act represents an attempt to provide additional clinical information.            
	 */
	Inform : '192f1768-d39e-409d-87be-5afd0ee0d1fe',
	/** 
	 *             The act represents an observation that is made about a patient such as a vital sign, an allergy, cause of death, etc..            
	 */
	Observation : '28d022c6-8a8b-47c4-9e6a-2bc67308739e',
	/** 
	 *             The act represents a procedure (something done to a patient).            
	 */
	Procedure : '8cc5ef0d-3911-4d99-937f-6cfdc2a27d55',
	/** 
	 *             The act represents a registration event such as the registration of a patient.            
	 */
	Registration : '6be8d358-f591-4a3a-9a57-1889b0147c7e',
	/** 
	 *             The act represents that a substance (medication, or otherwise) was, should, or will be administered to the patient.            
	 */
	SubstanceAdministration : '932a3c7e-ad77-450a-8a1f-030fc2855450',
	/** 
	 *             The act represents a supply of some material or financial instrument between entities.            
	 */
	Supply : 'a064984f-9847-4480-8bea-dddf64b3c77c',
	/** 
	 *             The physical transporting of materials or people from one place to another.            
	 */
	Transport : '61677f76-dc05-466d-91de-47efc8e7a3e6',
},  // ActClassKeys 
// OpenIZ.Core.Model.Constants.ActMoodKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Act Mood keys            
 */
ActMoodKeys : { 
	/** 
	 *             The ACT represents an appointment that was made to do something            
	 */
	Appointment : 'c46eee70-5612-473f-8d24-595ea15c9c39',
	/** 
	 *             The ACT represents a special type of request to create an appointment            
	 */
	AppointmentRequest : '0395f357-6821-4562-8192-49ac3c94f548',
	/** 
	 *             The ACT represents a definition of a type of act            
	 */
	Definition : '3b14a426-6337-4f2a-b83b-e6be7dbcd5a5',
	/** 
	 *             The ACT represents something that has occurred            
	 */
	Eventoccurrence : 'ec74541f-87c4-4327-a4b9-97f325501747',
	/** 
	 *             The ACT represents some sort of GOAL            
	 */
	Goal : '13925967-e748-4dd6-b562-1e1da3ddfb06',
	/** 
	 *             The ACT represents an intent made by a human to do something            
	 */
	Intent : '099bcc5e-8e2f-4d50-b509-9f9d5bbeb58e',
	/** 
	 *             The ACT represents a promise to do something            
	 */
	Promise : 'b389dedf-be61-456b-aa70-786e1a5a69e0',
	/** 
	 *             The ACT represents a proposal that a human should do something            
	 */
	Propose : 'acf7baf2-221f-4bc2-8116-ceb5165be079',
	/** 
	 *             The ACT represents a request to do something            
	 */
	Request : 'e658ca72-3b6a-4099-ab6e-7cf6861a5b61',
},  // ActMoodKeys 
// OpenIZ.Core.Model.Constants.ActReasonKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Represents act reasons (reasons for an act)            
 */
ActReasonKeys : { 
	/** 
	 *             The patient started too late for the therapy            
	 */
	StartedTooLate : 'b75bf533-9804-4450-83c7-23f0332f87b8',
	/** 
	 *             The patient is allergic or intolerant to the consumable            
	 */
	AllergyOrIntolerance : '4ff3617b-bb91-4f3f-b4d2-2425f477037f',
	/** 
	 *             The vaccine or drug was expired            
	 */
	Expired : '4b518938-b1ea-44e3-b837-31617fa188a4',
	/** 
	 *             The vaccine was considered unsafe            
	 */
	VaccineSafety : 'c6718df8-c8c0-49fd-a73d-52f6981ccbf7',
	/** 
	 *             The vaccine was not performed per the professional judgement of the provider            
	 */
	ProfessionalJudgement : '9d947e6d-8406-42f3-bb8a-634fb3c81a08',
	/** 
	 *             The patient had a religious objection            
	 */
	ReligiousObjecton : '0d40c2b6-7ceb-4492-ab2a-6e7c730eaf22',
	/** 
	 *             The patient refused the treatment            
	 */
	PatientRefused : '42351a36-f60f-4687-b334-7a41b091bae1',
	/** 
	 *             There was insufficient stock to perform the action            
	 */
	OutOfStock : 'c7469fad-f190-40a2-a28d-f97d1863e8cf',
	/** 
	 *             The items are broken and can no longer be used to deliver care            
	 */
	Broken : 'dcff308d-cca5-4eb3-ad92-770917d88e56',
	/** 
	 *             There was a cold-storage failure which resulted in the material being unusable.            
	 */
	ColdStorageFailure : '06922eac-0cae-49af-a33c-fc7096349e4a',
},  // ActReasonKeys 
// OpenIZ.Core.Model.Constants.ActRelationshipTypeKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             Act relationship types            
 */
ActRelationshipTypeKeys : { 
	/** 
	 *             Indicates that the source act appends information contained in the target act            
	 */
	Appends : 'dc3df205-18ef-4854-ac00-68c295c9c744',
	/** 
	 *             Links the transortation act from another act            
	 */
	Arrival : '26fe590c-3684-4574-9359-057fdd06ba61',
	/** 
	 *             Links a transporation act from another act indicating departure of the subject            
	 */
	Departure : '28c81cdc-ca56-4c92-b691-094e89630642',
	/** 
	 *             The source act documents the target act            
	 */
	Documents : '0f4ba634-5107-4eab-9658-25be293cd831',
	/** 
	 *             Links two instances of the same act over time (example: chronic conditions)            
	 */
	EpisodeLink : 'ebf9ac10-b5c9-407a-91a4-360bfb7e0fb9',
	/** 
	 *             Used to link a goal to an observation            
	 */
	Evaluates : '8dbeac94-cccb-4412-a990-09bab26dd048',
	/** 
	 *             Indicates that the source act fulfills the target act            
	 */
	Fulfills : '646542bc-72e4-488b-bbf4-865d452e62ec',
	/** 
	 *             Indicates that the target act authorizes the source act            
	 */
	HasAuthorization : '29894070-a76b-47ef-8c16-d84e0acd9ea6',
	/** 
	 *             Indicates that the target act is a component of the source act            
	 */
	HasComponent : '78b9540f-438b-4b6f-8d83-aaf4979dbc64',
	/** 
	 *             Relationship from an act to one or more control variables (for example: device settings, or environment)            
	 */
	HasControlVariable : '85f68168-2a43-4532-bc79-191fa0b47c8b',
	/** 
	 *             The assertion that a new observation may be a manifestation of another            
	 */
	HasManifestation : '22918d17-d3dc-4135-a003-4c1c52e57e75',
	/** 
	 *             Indicates that the target act is a pre-condition of the source act            
	 */
	HasPrecondition : '5a280fc0-8c26-4191-b204-b1b1e4e19462',
	/** 
	 *             Indicates a reasoning as to why the source act is occurring            
	 */
	HasReason : '55da61a2-7b86-47f3-9b0b-ba47dc99c950',
	/** 
	 *             Indicates that the source act contains reference values from the target            
	 */
	HasReferenceValues : '99488a1d-6d97-4013-8c91-ded6ad3b8e89',
	/** 
	 *             Indicates the subject of a particular act (example: clinical act is a subject of a control act)            
	 */
	HasSubject : '9871c3bc-b57a-479d-a031-7b56cb06fa84',
	/** 
	 *             Indicates an existing act is suggesting evidence for a new observation.            
	 */
	HasSupport : '3209e3f1-2258-4b63-8182-2c888da66cf0',
	/** 
	 *             Indicates that the source act is the cause of the target act            
	 */
	IsCauseOf : '57d81685-e399-4abd-8744-96454188a9fa',
	/** 
	 *             Indicates the source act is derived from information contained in the target act            
	 */
	IsDerivedFrom : '81b6a0f8-b86a-495f-9d5d-8a4073fdd882',
	/** 
	 *             Indicates that the source act is an excerpt of the target act            
	 */
	IsExcerptOf : 'ffc6e905-161d-4c0b-8cde-a04e9e9d0cd5',
	/** 
	 *             Indicates that the source act refers to the target act            
	 */
	RefersTo : '8fce259a-b859-4ae3-8160-0221f6ab1650',
	/** 
	 *             The source act replaces the target act            
	 */
	Replaces : 'd1578637-e1cb-415e-b319-4011da033813',
	/** 
	 *             Indicates that the source act starts after the start of another act            
	 */
	StartsAfterStartOf : 'c66d7ca9-c6c2-46b1-9276-ad76baf04b07',
	/** 
	 *             Indicates that the source act transforms the target act            
	 */
	Transforms : 'db2ae02a-ff12-4c1b-9c5b-ecdd41af8583',
},  // ActRelationshipTypeKeys 
// OpenIZ.Core.Model.Constants.NullReasonKeys, OpenIZ.Core.Model, Version=0.9.7.27948, Culture=neutral, PublicKeyToken=null
/**
 * @enum {uuid}
 * @memberof OpenIZModel
 * @public
 * @readonly
 * @summary             In OpenIZ, any concept can be replaced with a null reason. A null reason indicates why a particular field is not present rather than being null            
 */
NullReasonKeys : { 
	/** 
	 *             The reason that the requested value was not provided is that it is not available            
	 */
	Unavailable : '31e01921-82dc-4622-b3db-21429ea9e406',
	/** 
	 *             The reason that the requested value was not provided is that it is not applicable (for example last menstrual period of a male)            
	 */
	NotApplicable : 'fea2cfb1-f231-413d-b113-372779092e56',
	/** 
	 *             The reason that the value is not provided is that it can be derived from other information            
	 */
	Derived : '8ef137b3-e717-492b-8d8f-3817c99aed88',
	/** 
	 *             The value was not provided because it does not fall within the acceptable values             
	 */
	Other : '6052712a-340e-4480-ad6b-409ba320db4f',
	/** 
	 *             The value was asked for but the target did not know that answer            
	 */
	AskedUnknown : '21b0ffc8-ca4e-408d-a104-41fc924d3a39',
	/** 
	 *             The value was entered but it is invalid according to business rules            
	 */
	Invalid : 'd3f92eb1-fece-4dea-bed2-515af2b0fb38',
	/** 
	 *             There is a value present, but the quantity of the value is so small that it cannot be registered            
	 */
	Trace : '085069d8-0ca8-4771-986b-5eb3466580ff',
	/** 
	 *             The value is not prvovided because it is negative infinity            
	 */
	NegativeInfinity : 'fed3fe1b-b2c7-480b-b0af-5fd2e0200ce5',
	/** 
	 *             The exact value is not known, but there is sufficient quantity to perform an act            
	 */
	SufficientQuantity : 'c139841a-7d5a-40ba-9ac7-7628a7cdf443',
	/** 
	 *             The value is available however it cannot be encoded in the desired format            
	 */
	UnEncoded : '7da45c51-eb8e-4c75-a40b-7db66cb3f3cb',
	/** 
	 *             The value is unavailable because it was not asked for            
	 */
	NotAsked : '09919a72-808c-44c4-8b44-86fd3725f100',
	/** 
	 *             The value may have been asked for and was not known or is unknown (this differes from AskedUnknown)            
	 */
	Unknown : '70fe34ce-caff-4f46-b6e6-9cd6d8f289d6',
	/** 
	 *             The value is not provided because it is positive infinity            
	 */
	PositiveInfinity : 'e6d6fee2-fa53-4027-8eb8-9dd0f35d053d',
	/** 
	 *             The value is not provided because there is no available information            
	 */
	NoInformation : '61d8f65c-747e-4a99-982f-a42ac5437473',
	/** 
	 *             The value is available however it has been masked due to privacy concerns            
	 */
	Masked : '9b16bf12-073e-4ea4-b6c5-e1b93e8fd490',
},  // NullReasonKeys 
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
 * Date: 2016-11-30
 */

// Empty guid
EmptyGuid : "00000000-0000-0000-0000-000000000000",

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
Exception : function (type, message, detail, cause) {
    _self = this;

    this.type = type;
    this.message = message;
    this.details = detail;
    this.caused_by = cause;

}} // OpenIZModel