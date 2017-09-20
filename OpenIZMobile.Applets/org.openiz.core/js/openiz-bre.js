/// <reference path="openiz.js"/>
/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
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
 * Date: 2017-3-31
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