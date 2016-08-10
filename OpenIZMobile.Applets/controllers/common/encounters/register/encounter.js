/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

angular.element(document).ready(function () {
    $('.oiz-encounter-entry').each(function (i, e) {

        // Get the current scope that we're in
        var scope = angular.element(e).scope();

        // Encounter 
    });

    OpenIZ.Place.bindSelect($('#encounterFacility'), {
        classConcept: 'FF34DFA7-C6D3-4F8B-BC9F-14BCDC13BA6C',
        statusConcept: 'C8064CBD-FA06-4530-B430-1A52F1530C27'
    });
    OpenIZ.Provider.bindSelect($('#encounterVerifier'), {
        statusConcept: 'C8064CBD-FA06-4530-B430-1A52F1530C27'
    });
    OpenIZ.Provider.bindSelect($('#encounterPerformer'), {
        statusConcept: 'C8064CBD-FA06-4530-B430-1A52F1530C27'
    });
});
