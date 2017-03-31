/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('UpcomingAppointmentController', ['$scope', '$stateParams', '$rootScope', function ($scope, $stateParams, $rootScope) {
    $scope._isCalendarInitialized = false;

    // Gather the care plan
    OpenIZ.CarePlan.getCarePlanAsync({
        query: "_patientId=" + $stateParams.patientId + "&_appointments=true&_viewModel=full&stopTime=>" + OpenIZ.Util.toDateInputString(new Date()),
        onDate: new Date(),
        /** @param {OpenIZModel.Bundle} proposals */
        continueWith: function (proposalsToday) {
            OpenIZ.CarePlan.getCarePlanAsync({
                query: "_patientId=" + $stateParams.patientId + "&_appointments=true&_viewModel=full",
                minDate: new Date().tomorrow(),
                maxDate: new Date().addDays(90),
                continueWith: function (proposals) {
                    
                    if (Array.isArray(proposalsToday.item))
                        proposalsToday.item.push(proposals.item);
                    else
                        proposalsToday = proposals;

                    // Grab the first appointment
                    $scope.appointments = proposalsToday.item;
                    $scope.appointments.sort(
                              function (a, b) {
                                  return a.actTime > b.actTime ? 1 : -1;
                              }
                            );

                    for (var i in $scope.appointments)
                        if ($scope.appointments[i].startTime < $rootScope.page.loadTime) {
                            $scope.appointments[i].startTime = $rootScope.page.loadTime
                        }
                        if (!Array.isArray($scope.appointments[i]) && !Array.isArray($scope.appointments[i].relationship.HasComponent))
                            $scope.appointments[i].relationship.HasComponent = [$scope.appointments[i].relationship.HasComponent];

                },
                onException: function (ex) {
                    if (ex.message)
                        alert(ex.message);
                    else
                        console.error(ex);
                }
            });
        },
        onException: function (ex) {
            if (ex.message)
                alert(ex.message);
            else
                console.error(ex);
        }
    });
    
}]);