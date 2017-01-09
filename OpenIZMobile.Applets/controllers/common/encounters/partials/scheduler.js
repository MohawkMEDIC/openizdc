/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('AppointmentSchedulerController', ['$scope', '$stateParams', '$rootScope', function ($scope, $stateParams, $rootScope) {
    $scope._isCalendarInitialized = false;

    // Scheduling assistant shown
    $('a[data-target="#schedulingAssistant"]').on('shown.bs.tab', function () {
        if (!$scope._isCalendarInitialized) {
            $scope._isCalendarInitialized = true;
            $("#schedulingAssistantCalendar").fullCalendar({
                defaultView: 'month',
                defaultDate: $scope.appointment != null ? $scope.appointment.actTime : OpenIZ.Util.toDateInputString($rootScope.page.loadTime),
                displayEventTime: false,
                editable: true,
                weekends: false,
                navLinks: false,
                height: "parent",
                businessHours: {
                    dow: [1, 2, 3, 4, 5],
                    start: '09:00',
                    end: '16:00'
                },
                header: {
                    left: "prev, today, next",
                    center: "title",
                    right:null
                },
                eventLimit: false, // allow "more" link when too many events
                eventSources: [
                    {
                        events: $scope.renderAppointments
                    }
                ]
            });
        }
    });

    // Modal shown
    $("#appointmentScheduler").on('show.bs.modal', function () {
        
        
        // Gather the care plan
        OpenIZ.CarePlan.getCarePlanAsync({
            query: "_patientId=" + $stateParams.patientId + "&_appointments=true",
            minDate: new Date().tomorrow(),
            maxDate: new Date().addDays(90),
            /** @param {OpenIZModel.Bundle} proposals */
            continueWith: function (proposals) {
                if (!proposals.item) // There are no proposals, at the end of the planning process
                {
                    OpenIZ.CarePlan.getActTemplateAsync({
                        templateId: "act.patientencounter.appointment",
                        continueWith: function (appointment) {
                            $scope.appointment = appointment;
                        },
                        onException: function (ex) {
                            if (ex.message)
                                alert(ex.message);
                            else
                                console.error(ex);
                        }
                    });
                }
                else {
                    // Grab the first appointment
                    $scope.appointments = proposals.item;
                    $scope.appointments.sort(
                              function (a, b) {
                                  return a.actTime > b.actTime ? 1 : -1;
                              }
                            );
                    $scope.appointment = $scope.appointments[0];
                    $.each($scope.appointment.relationship.HasComponent, function (i, e) {
                        e._enabled = true;
                    });
                }

                // Updates the scheduling assistant view
                $scope.$watch('appointment.actTime', function (newvalue, oldvalue) {
                    if (newvalue != null && $scope._isCalendarInitialized &&
                        newvalue != oldvalue)
                        $("#schedulingAssistantCalendar").fullCalendar('select', newvalue);
                });

                $scope.$apply();
            },
            onException: function (ex) {
                if (ex.message)
                    alert(ex.message);
                else
                    console.error(ex);
            }
        });

    });

    $scope.renderAppointments = function (start, end, timezone, callback) {
        if ($scope.appointment == null) return;

        // First we're going to push onto the calendar the safe time for the appointment
        var items = [{
            id: $scope.appointment.id,
            title: OpenIZ.Localization.getString("locale.encounters.appointment.recommended"),
            backgroundColor: "rgba(75, 192, 192, 0.4)",
            textColor: "#fff",
            start: OpenIZ.Util.toDateInputString($scope.appointment.startTime),
            end: OpenIZ.Util.toDateInputString( $scope.appointment.stopTime),
            rendering: 'background'
        },
        {
            id: $scope.appointment.id,
            title: OpenIZ.Localization.getString("locale.encounters.appointment.proposed"),
            backgroundColor: "rgba(75, 192, 192, 1.0)",
            textColor: "#fff",
            start: OpenIZ.Util.toDateInputString($scope.appointment.actTime),
            end: OpenIZ.Util.toDateInputString($scope.appointment.actTime)
        }];


        // Now gather other load times
        var date = $scope.appointment.startTime;
        while(date < $scope.appointment.stopTime)
        {
            var title = "";
            // HACK: This should be an aggregate query when local RISI services are available
            for(var i in $scope.appointment.relationship.HasComponent)
            {
                var comp = $scope.appointment.relationship.HasComponent[i].targetModel;
                if(comp.$type == 'SubstanceAdministration') {
                    title += "0 " + OpenIZ.Util.renderName(comp.participation.Product.playerModel.name.Assigned) + "\r\n";
                }
            }
            items.push({
                id: $scope.appointment.id,
                title: title,
                backgroundColor: "rgba(54, 162, 235, 0.4)",
                textColor: "#fff",
                start: OpenIZ.Util.toDateInputString(date),
                end: OpenIZ.Util.toDateInputString(date)
            });
            date = date.addDays(1);
        }

        callback(items);

    };

   
}]);