var alpha = alpha || {};

alpha.matchForecast = angular.module('alpha.matchForecast', []);

alpha.matchForecast.rootUrl = document.getElementById('RootURL') ? document.getElementById('RootURL').value : '/';
alpha.matchForecast.templateVersion = '?v=' + Math.random();
alpha.matchForecast.buildTemplateUrlWithVersion = function (templateUrl) {
    'use strict';
    return alpha.matchForecast.rootUrl + templateUrl + alpha.matchForecast.templateVersion;
};

alpha.matchForecast.config(['$httpProvider', function ($httpProvider) {
    'use strict';
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
}]);

alpha.matchForecast.controller('matchForecastController',
    [
        '$scope',
        function ($scope) {
            $scope.matchList = [];

        }
    ]);