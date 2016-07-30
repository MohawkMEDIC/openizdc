/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>


angular.element(document).ready(function () {
    $('.oiz-vaccination-history').each(function (i, e) {
        // Get the current scope that we're in
        var scope = angular.element(e).scope();


        // Init data
        scope.patient = scope.patient || new OpenIZModel.Patient({});
        scope.patient.participation = scope.patient.participation || {};
        scope.display = scope.display || {};

        // Iterate through vaccinations and organize them by antigen
        // TODO: Change this to be an AJAX call
        scope.display._vaccineAdministrations = {};

        scope.$watch(function (s) { return s.patient.participation }, function (newValue, oldValue) {
            scope.display._vaccineAdministrations = {};
            if(newValue != null)
                for (var ptcpt in newValue.RecordTarget) {
                    var model = newValue.RecordTarget[ptcpt].actModel;

                    // Ignore anything except substance admins
                    if (model.$type != 'SubstanceAdministration' || (model.typeConceptModel.mnemonic != 'InitialImmunization' && model.typeConceptModel.mnemonic != 'Immunization' && model.typeConceptModel.mnemonic != 'BoosterImmunization'))
                        continue;

                    var antigenId = model.participation.Product[0].playerModel.name.OfficialRecord[0].component.$other[0];
                    if (scope.display._vaccineAdministrations[antigenId] == null) {
                        scope.display._vaccineAdministrations[antigenId] = [];
                        for (var i = 0; i < model.doseSequence; i++)
                            scope.display._vaccineAdministrations[antigenId].push(null);
                        //model._enabled = true;
                    }
                    scope.display._vaccineAdministrations[antigenId].push(model);
    //                scope.$apply();
                }
        });
    });
});
