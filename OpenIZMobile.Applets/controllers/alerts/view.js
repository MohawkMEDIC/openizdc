/// <reference path="~/js/openiz.js"/>

/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/jquery.min.js"/>
/// <reference path="~/lib/angular.min.js"/>
/// <reference path="~/controllers/layouts/navbar.js"/>

layoutApp.controller('ViewAlertController', ['$scope', function ($scope) {
    
    OpenIZ.App.getAlertsAsync({
        query: {
            key: OpenIZ.urlParams["alertId"],
            _count: 1
        },
        onExpcetion: function(ex) {
            OpenIZ.App.hideWait();
            if (typeof (ex) == "string")
                alert(ex);
            else if (ex.message != undefined)
                alert("" + ex.message + " - " + ex.details);
            else
                alert(ex);
        },
        continueWith: function (data) {
            $scope.alert = data[0];
            var foo = "";
            
            $scope.alert.body = $scope.alert.body.replace("\n", "<br/>");
            $scope.$apply();
        }
    });

}]);