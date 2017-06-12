// Audit controller
/// <reference path="~/js/openiz.js"/>
/// <reference path="~/js/openiz-audit.js"/>
/// <reference path="~/js/openiz-model.js"/>

layoutApp.controller('AuditController', ['$scope', function ($scope) {

    $scope.audit = { selected: null };
    $scope.audit.query = {
        "type.code": "!SecurityAuditCode-AuditLogUsed",
        _offset: 0,
        _count: 10
    };
    $scope.getAudit = getAudit;
    $scope.searchAudits = searchAudits;
    $scope.trunc = function (i) { return Math.trunc(i); };
    $scope.resetQueryTimestamp = resetQueryTimestamp;
    $scope.resetQueryTimestamp();

    // Get audits
    function searchAudits() {

        //if ($scope.audit.query.timestamp && $scope.audit.query.timestamp)
        //    $scope.audit.query.timestamp = "~" + OpenIZ.Util.toDateInputString($scope.audit.query.timestamp);

        $scope.isLoading = true;
        OpenIZ.Audit.getAuditAsync({
            query: $scope.audit.query,
            continueWith: function (data) {
                $scope.audit.log = data;
            },
            finally: function () {
                $scope.isLoading = false;
                $scope.$apply();

            }
        });
    }

    $scope.$watch('audit.query', function (ov, nv) {
        searchAudits();
    }, true);

    function getAudit(id) {
        if (id) {
            $scope.isLoading = true;
            OpenIZ.Audit.getAuditAsync({
                query: { _id: id},
                continueWith: function (data) {
                    $scope.audit.current = data.CollectionItem[0].Audit;
                   
                },
                finally: function () {
                    $scope.isLoading = false;
                    $scope.$apply();    
                }
            });
        }
    };

    function resetQueryTimestamp() {
        $scope.audit.queryTimestamp = moment().toDate();
    }
}]);
