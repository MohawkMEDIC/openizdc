/// <reference path="~/js/openiz.js"/>

/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('PatientLayoutController', ['$scope', function ($scope, $rootScope) {


    // Get the patient 
    OpenIZ.Patient.findAsync({
        query: { _id: OpenIZ.urlParams['patientId'], _count: 1, _all: true },
        continueWith: function (data) {
            $scope.patient = data;

            // Set multiple birth text
            switch (data.multipleBirthOrder) {
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
            console.log($scope.patient);
            getActs(data);
            $scope.$apply();
        },
        onException: function (ex) {
            OpenIZ.App.hideWait();
            if (typeof (ex) == "string")
                alert(ex);
            else if (ex.message != undefined)
                alert("" + ex.message + " - " + ex.details);
            else
                alert(ex);
        }
    });

    function getActs(data) {
        var today = new Date();
        today.setYear(today.getFullYear()+1);
        for (var i = 0; i < $scope.patient.participation.RecordTarget.length; i++) {
            if ($scope.patient.participation.RecordTarget[i].actModel.startTime < today && $scope.patient.participation.RecordTarget[i].actModel.typeConceptModel.name.en!='BODY WEIGHT (MEASURED)') {
                getAct(i);
            }
        }
    }
    function getAct(index) {
        OpenIZ.Ims.get({
            resource: "Act",
            query: "id=" + $scope.patient.participation.RecordTarget[index].act,
            continueWith: function (acts) {
                //$scope.patient.participation.RecordTarget[i].vaccineName = acts.item[0].participation.Product.playerModel.name.Assigned.component.$other.value;
                if(acts!=null){
                   $scope.patient.participation.RecordTarget[index].vaccineName = acts.item[0].participation.Product.playerModel.name.Assigned.component.$other.value;
                }
            },
            onException: function (ex) {
                console.log(ex);
            }
        });
    }

}]);