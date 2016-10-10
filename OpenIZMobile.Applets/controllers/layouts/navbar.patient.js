/// <reference path="~/js/openiz.js"/>

/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('PatientLayoutController', ['$scope', function ($scope, $rootScope)
{
    // Get the patient 
    OpenIZ.Patient.findAsync({
        query: { _id: OpenIZ.urlParams['patientId'], _count: 1, _all: true },
        continueWith: function (data)
        {
            $scope.patient = data;

            // Set multiple birth text
            switch (data.multipleBirthOrder)
            {
                case 1:
                    $scope.patient.multipleBirthOrderText = OpenIZ.Localization.getString('locale.patient.demographics.multipleBirth.first');
                    break;
                case 2:
                    $scope.patient.multipleBirthOrderText = OpenIZ.Localization.getString('locale.patient.demographics.multipleBirth.second');
                    break;
                case 3:
                    $scope.patient.multipleBirthOrderText = OpenIZ.Localization.getString('locale.patient.demographics.multipleBirth.third');
                    break;
                case 0:
                    $scope.patient.multipleBirthOrderText = OpenIZ.Localization.getString('locale.patient.demographics.multipleBirth.unknown');
                    break;
            };

            $scope.$apply();
        },
        onException: function (ex)
        {
            OpenIZ.App.hideWait();
            if (typeof (ex) == "string")
            {
                alert(ex);
            }
            else if (ex.message != undefined)
            {
                alert("" + ex.message + " - " + ex.details);
            }
            else
            {
                alert(ex);
            }
        }
    });
}]);