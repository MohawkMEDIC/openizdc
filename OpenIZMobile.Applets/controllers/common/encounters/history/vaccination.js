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
        console.log(scope.patient);
        // Iterate through vaccinations and organize them by antigen
        // TODO: Change this to be an AJAX call
        scope.display._vaccineAdministrations = {};

        scope.$watch(function (s) { return s.patient.participation }, function (newValue, oldValue) {
            scope.display._vaccineAdministrations = {};
            if(newValue != null && newValue.RecordTarget !== undefined)
            {
                if (newValue.RecordTarget instanceof Array)
                {
                    // Sort based on dose
                    newValue.RecordTarget.sort(function (a, b)
                    {
                        if (a.actModel.actTime > b.actModel.actTime)
                            return 1;
                        else if (a.actModel.actTime < b.actModel.actTime)
                            return -1;
                        else
                        {
                            return 0;
                        }
                    });

                    // Record target

                    for (var ptcpt in newValue.RecordTarget)
                    {

                        var acts = [];

                        // Patient encounters are the grouping of objects
                        var participation = newValue.RecordTarget[ptcpt];
                        if (participation.actModel.$type != 'PatientEncounter')
                            acts.push(participation);
                        else
                            acts = participation.actModel.relationship;

                        for (var act in acts)
                        {

                            var model = acts[act].actModel || acts[act].targetModel;
                            // Ignore anything except substance admins
                            console.log(model);
                            if (model.$type != 'SubstanceAdministration' || model.typeConceptModel == undefined || (model.typeConceptModel.mnemonic != 'InitialImmunization' && model.typeConceptModel.mnemonic != 'Immunization' && model.typeConceptModel.mnemonic != 'BoosterImmunization'))
                                continue;
                            if (model.participation.Product === undefined || model.participation.Product === null) {
                                continue;
                            }
                            var antigenId = model.participation.Product.playerModel.name.Assigned.component.$other.value;
                            if (scope.display._vaccineAdministrations[antigenId] == null)
                            {
                                scope.display._vaccineAdministrations[antigenId] = [];
                                for (var i = 0; i < model.doseSequence; i++)
                                    scope.display._vaccineAdministrations[antigenId].push(null);
                                //model._enabled = true;
                            }
                            scope.display._vaccineAdministrations[antigenId].push(model);
                            //                scope.$apply();
                        }
                    }
                }
            }
        });
    });
});
