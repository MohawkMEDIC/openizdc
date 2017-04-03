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
$.fn.select2.amd.define('select2/data/extended-ajax', ['./ajax', '../utils', 'jquery'], function (AjaxAdapter, Utils, $) {

    function ExtendedAjaxAdapter($element, options) {
        //we need explicitly process minimumInputLength value 
        //to decide should we use AjaxAdapter or return defaultResults,
        //so it is impossible to use MinimumLength decorator here
        this.minimumInputLength = options.get('minimumInputLength');
        this.defaultResults = options.get('defaultResults');

        ExtendedAjaxAdapter.__super__.constructor.call(this, $element, options);
    }

    Utils.Extend(ExtendedAjaxAdapter, AjaxAdapter);

    //override original query function to support default results
    var originQuery = AjaxAdapter.prototype.query;

    ExtendedAjaxAdapter.prototype.query = function (params, callback) {

        var defaultResults = (typeof this.defaultResults == 'function') ? this.defaultResults.call(this) : this.defaultResults;
        if (this._request === undefined && defaultResults && defaultResults.length && (!params.term || params.term.length < this.minimumInputLength)) {
            var processedResults = this.processResults(defaultResults, params.term);
            callback(processedResults);
        }
        else {
            originQuery.call(this, params, callback);
        }
    };

    return ExtendedAjaxAdapter;
});