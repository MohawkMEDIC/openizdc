/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 * 
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: justi
 * Date: 2017-3-31
 */

/// <refernece path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>

layoutApp.controller('AppointmentSchedulerController', ['$scope', '$rootScope', '$stateParams', function ($scope, $rootScope, $stateParams) {
    $scope._isCalendarInitialized = false;
    $scope.isLoading = true;
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
                    right: null
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
        $scope.getAppointment();
    });


    // Gather the care plan
    $scope.getAppointment = function () {

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
                        if (!proposals.item) // There are no proposals, at the end of the planning process
                        {
                            OpenIZ.CarePlan.getActTemplateAsync({
                                templateId: "act.patientencounter.appointment",
                                continueWith: function (appointment) {
                                    $scope.appointment = appointment;
                                    $scope.$apply();
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
                            if (Array.isArray(proposalsToday.item))
                                proposalsToday.item.push(proposals.item);
                            else
                                proposalsToday = proposals;

                            // Grab the first appointment
                            $scope.appointment = $scope.appointments[0];
                            if ($scope.appointment.actTime < $rootScope.page.loadTime) {
                                $scope.appointment.actTime = $rootScope.page.loadTime;
                            }
                            if ($scope.appointment.startTime < $rootScope.page.loadTime) {
                                $scope.appointment.startTime = $rootScope.page.loadTime;
                            }
                            if (Array.isArray($scope.appointment.relationship.HasComponent))
                                $.each($scope.appointment.relationship.HasComponent, function (i, e) {
                                    e._enabled = true;
                                });
                            else {
                                $scope.appointment.relationship.HasComponent = [
                                    $scope.appointment.relationship.HasComponent
                                ];
                                $scope.appointment.relationship.HasComponent[0]._enabled = true;
                            }

                            $scope.$apply(); // must call apply so UI doesn't look like it is hanging
                        }

                        // Updates the scheduling assistant view
                        $scope.$watch('appointment.actTime', function (newvalue, oldvalue) {
                            if (newvalue != null && $scope._isCalendarInitialized &&
                                newvalue != oldvalue)
                                $("#schedulingAssistantCalendar").fullCalendar('select', newvalue);
                        });

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

    };

    var refs = {};

    $scope.renderAppointments = function (start, end, timezone, callback) {
        if ($scope.appointment == null) return;

        var appointments = [{
            id: $scope.appointment.id,
            title: OpenIZ.Localization.getString("locale.encounters.appointment.recommended"),
            backgroundColor: "rgba(75, 192, 192, 0.6)",
            textColor: "#fff",
            start: OpenIZ.Util.toDateInputString($scope.appointment.startTime),
            end: OpenIZ.Util.toDateInputString($scope.appointment.stopTime),
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

        // Function using closures to query the warehouse data
        var queryWarehouse = function () {
            OpenIZWarehouse.Adhoc.query({
                martId: "oizcp",
                queryId: "bymonth",
                parameters: {
                    "act_date": [">" + OpenIZ.Util.toDateInputString($scope.appointment.startTime), "<" + OpenIZ.Util.toDateInputString($scope.appointment.stopTime)],
                    "location_id": $rootScope.session.entity.relationship.DedicatedServiceDeliveryLocation.target
                },
                /** @param {OpenIZModel.Material} state */
                continueWith: function (data) {
                    var aptDates = {};
                    for (var apt in data) {
                        if (data[apt].product_id) {
                            var dateStr = OpenIZ.Util.toDateInputString(data[apt].act_date);
                            var dateApt = aptDates[dateStr];
                            if (dateApt)
                                dateApt.title += "\r\n" + data[apt].acts + " " + OpenIZ.Util.renderName(refs[data[apt].product_id]);
                            else {
                                dateApt = {
                                    title: OpenIZ.Localization.getString("locale.encounters.appointment.planned") + "\r\n" + data[apt].acts + " " + OpenIZ.Util.renderName(refs[data[apt].product_id]),
                                    start: OpenIZ.Util.toDateInputString(data[apt].act_date),
                                    end: OpenIZ.Util.toDateInputString(data[apt].act_date)
                                };
                                appointments.push(dateApt);
                                aptDates[dateStr] = dateApt;
                            }
                        }
                    }

                    callback(appointments);
                }
            });
        };

        // Use the datawarehouse data
        if (Object.keys(refs).length == 0)
            OpenIZ.Material.findMaterialAsync(
                {
                    query: { "typeConcept.mnemonic": "~VaccineType", "obsoletionTime": "null", "relationship[ManufacturedProduct].target.relationship[OwnedEntity].source": $rootScope.session.entity.relationship.DedicatedServiceDeliveryLocation.target, "relationship[ManufacturedProduct].target@Material.expiryDate": ">" + OpenIZ.Util.toDateInputString(new Date()) },
                    continueWith: function (materials) {
                        for (var i in materials.item)
                            refs[materials.item[i].id] = OpenIZ.Util.renderName(materials.item[i].name.Assigned);

                        queryWarehouse();
                    }
                });
        else
            queryWarehouse();

    };

    /**
     * Schedule an appointment
     */
    $scope.scheduleAppointment = function (form) {
        if (form.$invalid) return;

        OpenIZ.App.showWait();
        try {
            var bundle = new OpenIZModel.Bundle();

            // schedule the appointment
            var encounter = new OpenIZModel.PatientEncounter($scope.appointment);
            encounter.moodConcept = OpenIZModel.ActMoodKeys.Appointment;
            encounter.startTime = encounter.stopTime = null;
            encounter.participation.Location = new OpenIZModel.ActParticipation({
                player: $rootScope.session.entity.relationship.DedicatedServiceDeliveryLocation.target
            });
            encounter.participation.Authororiginator = new OpenIZModel.ActParticipation({
                player: $rootScope.session.entity.id
            });
            if (encounter.note && encounter.note.text) {
                encounter.note.author = encounter.participation.Authororiginator.player;
            };
            bundle.item = [encounter];

            // Iterate through the has-components and turn them to INT or remove them if not selected
            if (!Array.isArray(encounter.relationship.HasComponent))
                encounter.relationship.HasComponent = [encounter.relationship.HasComponent];

            for (var i in encounter.relationship.HasComponent) {
                /** @type {OpenIZModel.ActParticipation} */
                var actRelationship = encounter.relationship.HasComponent[i];
                if (!actRelationship._enabled) {
                    encounter.relationship.HasComponent.splice(i, 1);
                }
                else {
                    actRelationship.targetModel.moodConcept = OpenIZModel.ActMoodKeys.Intent;
                    actRelationship.targetModel.actTime = encounter.actTime;
                    //actRelationship.targetModel.startTime = actRelationship.targetModel.stopTime = null;
                    actRelationship.targetModel.participation.RecordTarget = encounter.participation.RecordTarget;
                    actRelationship.targetModel.participation.Location = encounter.participation.Location;
                    actRelationship.targetModel.participation.Authororiginator = encounter.participation.Authororiginator;
                    if (actRelationship.targetModel.participation && actRelationship.targetModel.participation.Product)
                        delete (actRelationship.targetModel.participation.Product.playerModel);

                    delete (actRelationship.targetModel.moodConceptModel);
                    // Now push the act relationship into the submission bundle
                    bundle.item.push(actRelationship.targetModel);
                    delete (actRelationship.targetModel);
                }
            }

            // Is there an encounters collection above us? if so we should probably push this up
            if ($scope.$parent.encounters)
                $scope.$parent.encounters.push(encounter);

            // Now submit the bundle
            OpenIZ.Bundle.insertAsync({
                data: bundle,
                continueWith: function (data) {
                    $scope.getAppointments();
                    $scope.$parent.encounters = null;
                    $scope.$parent.refreshEncounters();
                    $("#appointmentScheduler").modal("hide");

                },
                onException: function (ex) {
                    if (ex.message)
                        alert(ex.message);
                    else
                        console.error(ex);
                },
                finally: function () {
                    OpenIZ.App.hideWait();
                }
            });
        }
        catch (e) {
            console.error(e);
        }
        finally {
            OpenIZ.App.hideWait();
        }

    }

    //$scope.getAppointment();
}]);