/// <reference path="../../js/openiz-model.js"/>
/// <reference path="../../js/openiz.js"/>

layoutApp.controller('SettingsController', ['$scope', function ($scope) {

    $scope.config = {
        security: OpenIZ.Configuration.getSection("SecurityConfigurationSection"),
        realmName: OpenIZ.Configuration.getRealm(),
        data: OpenIZ.Configuration.getSection("DataConfigurationSection"),
        applet: OpenIZ.Configuration.getSection("AppletConfigurationSection"),
        application: OpenIZ.Configuration.getSection("ApplicationConfigurationSection"),
        log: OpenIZ.Configuration.getSection("DiagnosticsConfigurationSection")
    };

    $scope.config.data.mode = OpenIZ.App.getService("SynchronizationManagerService") == null ?
        OpenIZ.App.getService("LocalPersistenceService") == null ? "online" : "offline" : "sync";
    $scope.config.data.sync = {
        event: [],
        enablePoll: OpenIZ.App.getService("ImsiPollingService") != null,
        pollInterval: OpenIZ.Configuration.getApplicationSetting("imsi.poll.interval")
    };
    $scope.config.log.mode = $scope.config.log.trace[0].filter || "Warning";
    $scope.config.security.hasher = OpenIZ.App.getService("IPasswordHashingService") || "SHA256PasswordHasher";

    $scope.config.security.offline = {
        enable: false
    };
    $scope.master = {};

    // leave realm
    $scope.leaveRealm = function (realm) {
        if (confirm(OpenIZ.Localization.getString("locale.settings.confirm.leaveRealm")))
            OpenIZ.Configuration.leaveRealm(realm);
    };

    // join realm
    $scope.joinRealm = function (realm) {
        if (OpenIZ.Configuration.joinRealm(realm.domain, realm.deviceName))
            alert(OpenIZ.Localization.getString("locale.settings.status.joinRealm"));
    };

    // Save config
    $scope.save = function (config) {
        if (OpenIZ.Configuration.getRealm() == null)
            alert(OpenIZ.Localization.getString("locale.settings.error.noRealm"));
        else if (OpenIZ.Configuration.save(config))
            OpenIZ.App.close();
    };

    $scope.reset = function () {
        $scope.user = angular.copy($scope.master);
    };

    $scope.reset();
}]);
