/// <reference path="../../js/openiz-model.js"/>
/// <reference path="../../js/openiz.js"/>
layoutApp.controller('UserProfileController', ['$scope', function ($scope) {
    $scope.saveProfile = function (userEntity) {

        OpenIZ.App.showWait();
        // Fix the type of telecom
        var telKey = Object.keys(userEntity.telecom)[0];
        userEntity.telecom.$other = userEntity.telecom[telKey];
        delete userEntity.telecom[telKey];
        
        // When we update the facility we clear the model properties
        userEntity.relationship.DedicatedServiceDeliveryLocation.targetModel = null;

        // Update async
        OpenIZ.UserEntity.updateAsync({
            data: userEntity,
            continueWith: function (data) {
                OpenIZ.App.toast("Profile updated successfully");

                if (data.language !== undefined)
                {
                    OpenIZ.Localization.setLocale(data.language[0].languageCode);
                }

                window.location.reload();
            },
            onException: function (ex) {
                console.log(ex);
                OpenIZ.App.hideWait();

                OpenIZ.App.toast("Unable to update profile");
            }
        });
    };

    // HACK: Copy first telecom to $other for view

}]);