/// <reference path="../../js/openiz-model.js"/>
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
 * Date: 2016-8-17
 */

/// <reference path="~/lib/angular.min.js"/>
/// <reference path="../../js/openiz.js"/>
layoutApp.controller('UserProfileController', ['$scope', '$rootScope', '$window', function ($scope, $rootScope, $window) {
    $scope.editObject = {};

    $rootScope.$watch('session', function (n, o) {
        if ($rootScope.session != null) {
            //$scope.editObject = $rootScope.session;
            $scope.editObject = $rootScope.session;
            if (!$scope.editObject.entity.language)
                $scope.editObject.entity.language = [{ languageCode: 'en' }];
            
        }
    });

    $scope.saveProfile = function (userProfileForm, userEntity) {

        if (!userProfileForm.$valid) {
            return;
        }

        OpenIZ.App.showWait('#saveUserProfileButton');

        // Fix the type of telecom
        var telKey = Object.keys(userEntity.telecom)[0];
        userEntity.telecom.$other = userEntity.telecom[telKey];
        delete userEntity.telecom[telKey];
        
        // When we update the facility we clear the model properties
        if (userEntity.relationship &&
            userEntity.relationship.DedicatedServiceDeliveryLocation)
            delete (userEntity.relationship.DedicatedServiceDeliveryLocation.targetModel);
        userEntity.securityUser = $rootScope.session.user.id;
        userEntity.statusConcept = 'C8064CBD-FA06-4530-B430-1A52F1530C27';
        userEntity.language
        // Update async
        OpenIZ.UserEntity.updateAsync({
            data: userEntity,
            continueWith: function (data) {
                OpenIZ.App.toast("Profile updated successfully");

                if (data.language !== undefined)
                {
                    OpenIZ.Localization.setLocale(data.language[0].languageCode);
                    window.location.reload(true);
                }
            },
            onException: function (ex) {
                console.log(ex);
                OpenIZ.App.toast("Unable to update profile");
            },
            finally: function () {
                OpenIZ.App.hideWait('#saveUserProfileButton');
            }
        });
    };

    // HACK: Copy first telecom to $other for view

}]);