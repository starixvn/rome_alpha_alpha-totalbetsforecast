alpha.matchForecast = angular.module('alpha.matchForecast', ['ngSanitize']);

alpha.matchForecast.config(['$httpProvider', function ($httpProvider) {
    'use strict';
    $httpProvider.defaults.headers.common["X-Requested-With"] = 'XMLHttpRequest';
}]);

alpha.matchForecast.controller('matchForecastController',
    [
        '$scope',
        function ($scope) {
            $scope.mathList = [];

        }
    ]);