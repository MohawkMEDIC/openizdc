// FROM:
// https://gist.githubusercontent.com/govorov/3ee75f54170735153349b0a430581195/raw/6e73e7a27f7432b47c2e2062fbff807c808daa74/select2_extended_ajax_adapter.js
// No License Attached to this 
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