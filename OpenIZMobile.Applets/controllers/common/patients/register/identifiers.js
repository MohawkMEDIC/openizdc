/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>

angular.element(document).ready(function () {
    $('.oiz-patient-identifiers').each(function (i, e) {
        // Get the current scope that we're in
        var scope = angular.element(e).scope();


        // Init data
        scope.patient = scope.patient || new OpenIZModel.Patient({});
        scope.patient.identifier = scope.patient.identifier || { $other : {authority: null, value: null } };
        
            
        // Rebind the domain scope
        scope.rebindDomain = scope.rebindDomain || function (authority, identifier) {
            scope.patient.identifier[identifier.authority] = identifier;
            delete scope.patient.identifier["NEW"];
        };

        // Scan the specified barcode
        scope.scanBarcode = scope.scanBarcode || function (authority, identifier) {
            identifier.value = OpenIZ.App.scanBarcode();
        };

        // Add identifier
        scope.addIdentifier = scope.addIdentifier || function () {
            if (scope.patient.identifier["NEW"] != null &&
                scope.patient.identifier["NEW"].authority != "NEW") {
                scope.patient.identifier[scope.patient.identifier["NEW"].authority] = scope.patient.identifier["NEW"];
                scope.patient.identifier["NEW"] = { authority: "NEW", value: null };
            }
            else if (scope.patient.identifier["NEW"] == null)
                scope.patient.identifier["NEW"] = { authority: "NEW", value: null };
        };

        // Remove identifier
        scope.removeIdentifier = scope.removeIdentifier || function (id) {
            delete scope.patient.identifier[id];
            if (Object.keys(scope.patient.identifier) == 0)
                scope.addIdentifier();
        };
    })
});
