/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/jquery.min.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/select2.min.js"/>

$(document).ready(function () {

    $('.select2-tag').select2({
        tags: true,
        tokenSeparators: [' ', ',', ';'],
        //templateResult : function(data) { return null; },
        language: {
            noResults: null //function (term) { return 'No'; }
        }
    });
    //$('.select2-tag').each(function (e, d) {
    //    $(d).on('select2:open', function (s) {
    //        s.preventDefault();
    //        return true;
    //    });
    //});

});