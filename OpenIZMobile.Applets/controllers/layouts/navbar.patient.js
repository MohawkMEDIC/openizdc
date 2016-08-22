﻿/// <reference path="~/js/openiz.js"/>

/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('PatientLayoutController', ['$scope', function ($scope) {


    // Get the patient 
    OpenIZ.Patient.findAsync({
        query: { _id: OpenIZ.urlParams['patientId'], _count: 1, _all: true },
        continueWith: function (data) {
            $scope.patient = data;

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

   
}]);