/// <reference path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/lib/jquery.min.js"/>
layoutApp.controller('LayoutController', ['$scope', function ($scope) {

    // Add menu items
    $scope.menuItems = OpenIZ.App.getMenus();
    
    // Perform a logout of the session
    $scope.logout = $scope.logout || function () {
        if (confirm(OpenIZ.Localization.getString('locale.layout.navbar.logout.confirm'))) {
            OpenIZ.Authentication.abandonSession();
            window.location.reload();
        }
    };

    // Set locale
    $scope.setLocale = $scope.setLocale || function (locale) {
        OpenIZ.Localization.setLocale(locale);
        window.location.reload();
    };
}]);