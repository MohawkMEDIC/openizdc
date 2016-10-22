/// <reference path="~/js/openiz.js"/>

/// <reference path="~/js/openiz-model.js"/>
/// <reference path="~/lib/jquery.min.js"/>
/// <reference path="~/lib/angular.min.js"/>

layoutApp.controller('AboutApplicationController', ['$scope', function ($scope) {

    $scope.data = {};
    // Get information asynchronously
    OpenIZ.App.getInfoAsync({
        continueWith: function (data) {
            $scope.about = data;
            $scope.$apply();
            OpenIZ.App.getLogInfoAsync({
                continueWith: function (data) {
                    $scope.about.logInfo = data;
                    $scope.$apply();
                    OpenIZ.App.getLogInfoAsync({
                        query: { _id: $scope.about.logInfo[0].id },
                        continueWith: function (data) {
                            $scope.log = data[0];
                            $scope.logLength = 1024;
                            $scope.log.text = data[0].text.substring(data[0].text.length - $scope.logLength, data[0].text.length);
                            $scope.$apply();
                        }
                    });
                }
            });
        }
    });

    $scope.log = { size: 0 };
    $scope.logLength = 1024;
    // Load log file contents
    $scope.loadLogContents = function () {
        OpenIZ.App.getLogInfoAsync({
            query : { _id : $scope.logid },
            continueWith: function (data) {
                $scope.log=data[0];
                $scope.log.text = data[0].text.substring(data[0].text.length - $scope.logLength, data[0].text.length);
                $scope.$apply();
            }
        });
    };
}]);