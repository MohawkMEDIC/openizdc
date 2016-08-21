/// <reference path="../../js/openiz-model.js"/>
/// <reference path="../../js/openiz.js"/>
layoutApp.controller('UserProfileController', ['$scope', function ($scope) {
    $scope.saveProfile = function (userEntity) {

        if (userEntity.name.OfficialRecord !== undefined)
        {
            if (userEntity.name.OfficialRecord.component.Family !== undefined)
            {
                for (var i = 0; i < userEntity.name.OfficialRecord.component.Family.length; i++)
                {
                    userEntity.name[0].component.push(
                    {
                        $type: "EntityNameComponent",
                        type: OpenIZModel.NameComponentKeys.Family,
                        value: userEntity.name.OfficialRecord.component.Family[i]
                    });
                }
            }


            if (userEntity.name.OfficialRecord.component.Given !== undefined)
            {
                for (var i = 0; i < userEntity.name.OfficialRecord.component.Given.length; i++)
                {
                    userEntity.name[0].component.push(
                    {
                        $type: "EntityNameComponent",
                        type: OpenIZModel.NameComponentKeys.Given,
                        value: userEntity.name.OfficialRecord.component.Given[i]
                    });
                }
            }
        }

        if (userEntity.telecom.$other !== undefined)
        {
            if (userEntity.telecom.$other.use !== undefined)
            {
                userEntity.telecom[0].use = userEntity.telecom.$other.use;
            }

            if (userEntity.telecom.$other.value !== undefined)
            {
                userEntity.telecom[0].value = userEntity.telecom.$other.value;
            }
        }

        if (userEntity.language.languageCode !== undefined)
        {
            userEntity.language.push(
            {
                $type: "PersonLanguageCommunication",
                isPreferred: true,
                languageCode: userEntity.language.languageCode
            });
        }



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