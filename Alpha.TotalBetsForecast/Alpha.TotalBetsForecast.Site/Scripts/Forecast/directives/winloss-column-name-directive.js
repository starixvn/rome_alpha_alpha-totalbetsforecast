alpha.matchForecast.directive('winlossColumnName', [
        function () {
            'use strict';
            return {
                restrict: 'E',
                replace: true,
                scope:
                    {
                        winLossColumnNames: '='
                    },
                controller: 'matchForecastController',
                templateUrl: alpha.matchForecast.buildTemplateUrlWithVersion('Scripts/Forecast/templates/winloss-column-name.html'),
                link: function ($scope) {

                }
            };
        }
]);