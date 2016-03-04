alpha.matchForecast.directive('winlossColumnName', [
        function () {
            'use strict';
            return {
                restrict: 'E',
                replace: true,
                controller: 'matchForecastController',
                templateUrl: builtUrlTemplate('.htm'),
                link: function ($scope) {

                }
            };
        }
]);