/// <reference path="../../js/openiz-model.js"/>
/// <reference path="../../js/openiz.js"/>
var layoutApp = angular.module('layout', []);

// Configure the safe ng-urls
layoutApp.config(['$compileProvider', function ($compileProvider) {
    $compileProvider.aHrefSanitizationWhitelist(/^\s*(app|tel):/);
    $compileProvider.imgSrcSanitizationWhitelist(/^\s*(app|tel):/);
}]);