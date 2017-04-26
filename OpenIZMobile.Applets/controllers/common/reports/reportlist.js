
/// <reference path="~/js/openiz.js"/>

// Report list controller
layoutApp.controller('ReportListController', ['$scope', '$compile', function ($scope, $compile) {

    $scope.isLoading = true;
    $scope.reports = [];
    $scope.executeReport = executeReport;
    $scope.currentReport = {};
    $scope.selectItem = selectItem;
    $scope.currentFilter = {};
    $scope.reportBody = '';
    $scope.$watch('reportBody', function (nv, ov) {
        $("#reportBody").html(nv);
        $compile($("#reportBody").contents())($scope);

    });
    // Get reports asynchronously
    OpenIZ.Risi.getReportsAsync({
        continueWith: function (data) {
            $scope.reports = data;
        },
        onException: function (ex) {
            if (ex.message)
                alert(OpenIZ.Localization.getString(ex.message));
            else
                console.error(ex);
        },
        finally: function () {
            $scope.isLoading = false;
            $scope.$apply();
        }
    });

    // Select item from list
    function selectItem(rpt) {

        $scope.isLoading = true;
        OpenIZ.Risi.getReportsAsync({
            name: rpt.name,
            continueWith: function (data) {
                $scope.currentFilter = {};
                $scope.currentReport = data[0];
                for (var i in data[0].parameter) {
                    $scope.currentFilter[data[0].parameter[i].name] = null;
                    if (data[0].parameter[i].valueSet) {
                        data[0].parameter[i].valueSet.values = [{ 0: null, 1: OpenIZ.Localization.getString('locale.dialog.wait.text') }];
                        OpenIZ.Risi.getParameterValuesAsync({
                            name: data[0].parameter[i].name,
                            report: rpt.name,
                            query: { locale: OpenIZ.Localization.getLocale() },
                            continueWith: function (d, state) {
                                data[0].parameter[state].valueSet.values = d;
                                $scope.$apply();
                            },
                            state: i
                        });
                    }
                }
            },
            onException: function (ex) {
                if (ex.message)
                    alert(OpenIZ.Localization.getString(ex.message));
                else
                    console.error(ex);
            },
            finally: function () {
                $scope.isLoading = false;
                $scope.$apply();
            }
        });

    }

    // Execute report
    function executeReport() {
        $scope.isLoading = true;
        OpenIZ.Risi.executeReportAsync({
            name: $scope.currentReport.info.name,
            view: $scope.currentReport.view[0].name,
            query: $scope.currentFilter,
            continueWith: function (data) {
                if (data == null)
                    $scope.reportBody = "<h2>" + OpenIZ.Localization.getString("locale.reports.nodata") + "</h2>";
                else {
                    $scope.reportBody = data;
                }
            },
            onException: function (ex) {
                alert(OpenIZ.Localization.getString("locale.reports.error.locked"));
            },
            finally: function () {
                $scope.isLoading = false;
                $scope.$apply();
            }
        })
    }
}]);