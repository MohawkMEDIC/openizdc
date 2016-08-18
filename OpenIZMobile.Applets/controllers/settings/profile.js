/// <reference path="../../js/openiz-model.js"/>
/// <reference path="../../js/openiz.js"/>
layoutApp.controller('UserProfileController', ['$scope', function ($scope) {
    $scope.saveProfile = function (userEntity) {

        console.log(userEntity);

        OpenIZ.UserEntity.updateAsync({
            data: userEntity,
            continueWith: function (e) {
                OpenIZ.App.toast("Profile updated successfully");
            },
            onException: function (ex) {
                console.log(ex);
                OpenIZ.App.hideWait();

                OpenIZ.App.toast("Unable to update profile");
            }
        });
    };
}]);