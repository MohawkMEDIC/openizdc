/// <reference path="~/js/openiz.js"/>

/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>
layoutApp.controller('LayoutController', ['$scope', '$interval', '$rootScope', function ($scope, $interval, $rootScope) {
    // Add menu items
    OpenIZ.App.getMenusAsync({
        continueWith: function (menus) {
            $scope.menuItems = menus;
            $scope.$applyAsync();
        }
    });
    
    
    // Perform a logout of the session
    $scope.logout = $scope.logout || function ()
    {
        if (confirm(OpenIZ.Localization.getString('locale.layout.navbar.logout.confirm')))
        {
            OpenIZ.Authentication.abandonSession({
                continueWith: function(data)
                {
                    console.log(data);
                    window.location.href = "/tz.timr.applet/views/security/login.html";
                },
                onException: function(ex)
                {
                    console.log(ex);
                }
            });
        }
    };

    // Set locale
    $scope.setLocale = $scope.setLocale || function (locale) {
        OpenIZ.Localization.setLocale(locale);
        window.location.reload();
    };

    $scope.checkMessages = function () {
        OpenIZ.App.getAlertsAsync({
            query: {
                flags: "!2",
                _count: 5
            },
            continueWith: function (d) {
                if ($scope.messages == null || d.length != $scope.messages.length) {
                    $scope.messages = d;
                    $scope.$apply();
                }

                setTimeout($scope.checkMessages, 30000);
            }
        });
    };
    setTimeout($scope.checkMessages, 30000);
    $scope.checkMessages();
}]);
