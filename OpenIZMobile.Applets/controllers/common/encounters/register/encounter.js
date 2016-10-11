/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

angular.element(document).ready(function () {
    $('.oiz-encounter-entry').each(function (i, e) {

        // Get the current scope that we're in
        var scope = angular.element(e).scope();


        /** 
        * Add sub-encounter sub-encounter
        */
        scope.addSubEncounter = scope.addSubEncounter || function (bind, templateName) {
            OpenIZ.CarePlan.getActTemplateAsync({
                templateId: templateName,
                continueWith: function (d) {
                    bind.push({
                        _created: true,
                        _enabled: true,
                        targetModel: d
                    });
                    scope.$apply();
                }
            });

        };

        /** 
         * Delete sub-encounter
         */
        scope.delSubEncounter = function (bind, index) {
            bind.splice(index, 1);
        };

        // Encounter 
        scope.validateAct = scope.validateAct || function (act) {

            var validation = [];

            // Act not done!?
            if (act.negationInd && act.reasonConcept == null) // not done - There must be a reason why ... 
                validation.push(OpenIZ.Localization.getString('locale.encounter.validation.reasonRequired'));
            else {
               
            }

            return validation;

        };
    });
});
