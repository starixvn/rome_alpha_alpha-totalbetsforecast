alpha.matchForecast.directive('betTypeForecast', [
        function () {
            'use strict';
            return {
                restrict: 'A',
                replace: true,
                scope:
                    {
                        betType: '='
                    },
                controller: 'matchForecastController',
                templateUrl: alpha.matchForecast.buildTemplateUrlWithVersion('Scripts/Forecast/templates/bet-type-forecast.html'),
                link: function ($scope) {
                    var demo = $scope.betType;

                }
            };
        }
]);