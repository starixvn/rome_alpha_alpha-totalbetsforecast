alpha.matchForecast.directive('betChoiceForecast', [
        function () {
            'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'matchForecastController',
                templateUrl: builtUrlTemplate('Scripts/Forecast/templates/bet-choice-forecast.html'),
                link: function ($scope) {

                }
            };
        }
]);