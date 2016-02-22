var forecast = {
    rootUrl: document.getElementById('RootURL').value,
    maxNumberOfRenderMatch: 5,
    urls: {
        getMatches: 'Forecast/GetMatches',
        getMatchForecast: 'Forecast/MatchForecast'
    },
    domElements: {
        btnSubmit: null,
        ddlDeadBall: null,
        ddlLeague: null,
        ddlMatch: null,
        btnRefresh: null,
        mainContent: null
    },
    init: function () {
        forecast.cachedDOM();
        forecast.registerEvents();

        forecast.loadMatches();
    },
    getAbsoluteUrl: function(relativeUrl) {
        return forecast.rootUrl + relativeUrl;
    },
    cachedDOM: function () {
        forecast.domElements.btnSubmit = $('#btnSubmit');
        forecast.domElements.btnRefresh = $('#btnRefresh');
        forecast.domElements.ddlMatch = $('#ddlMatch');
        forecast.domElements.ddlLeague = $('#LeagueId');
        forecast.domElements.ddlDeadBall = $('#ddlDeadBall');
        forecast.domElements.mainContent = $('#mainContent');
    },
    registerEvents: function () {

        forecast.domElements.ddlLeague.select2();

        forecast.domElements.ddlLeague.on("change", function (e) {
            forecast.loadMatches();
        });

        forecast.domElements.btnSubmit
            .off('click.forecast')
            .on('click.forecast', function () {
                var matchIds = forecast.domElements.ddlMatch.val();
                var matchIdsLength = matchIds.length;
                for (var i = 0; i < matchIdsLength; i++) {
                    var matcheForecastQuery =
                                   {
                                       matchId: matchIds[i],
                                       isLive: forecast.domElements.ddlDeadBall.val() === 'true'
                                   };

                    $.get(forecast.getAbsoluteUrl(forecast.urls.getMatchForecast
                        + '?'
                        + $.param(matcheForecastQuery)),
                        function (html) {
                            if (html) {
                                var league = forecast.domElements.mainContent.find('tbody[id=league-name]');
                                if (league.length === 0) {
                                    forecast.domElements.mainContent.append('<tbody id="league-name"><tr class="cl-league"><td colspan="9">'
                                        + forecast.domElements.ddlLeague.find('option:selected').text()
                                        +'</td></tr>');
                                }

                                var newMatchForecast = $(html);

                                var currentMatchForecast = forecast.domElements.mainContent.find('tbody[id=' + newMatchForecast.attr('id') + ']');

                                if (currentMatchForecast.length > 0) {
                                    currentMatchForecast.html(newMatchForecast.html());
                                }
                                else {
                                    forecast.domElements.mainContent.append(html);
                                    forecast.domElements.mainContent.parent().show();
                                }
                            }
                        });
                }

            });
    },
    loadMatches: function () {
        var matchId = parseInt(forecast.domElements.ddlMatch.val());
        var matchesQuery =
            {
                leagueId: forecast.domElements.ddlLeague.val(),
                isLive: forecast.domElements.ddlDeadBall.val() === 'true'
            };

        $.ajax({
            url: forecast.getAbsoluteUrl(forecast.urls.getMatches),
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: matchesQuery,
        }).done(function (viewModel) {
            if (viewModel) {
                var options = '';
                var processedMatches = _.map(viewModel.Matches, function (match) {
                    return {
                        id: match.Id,
                        text: match.Name
                    };
                });

                forecast.domElements.ddlMatch.val(null);
                forecast.domElements.ddlMatch.select2({
                    data: processedMatches,
                    maximumSelectionLength: forecast.maxNumberOfRenderMatch,
                    placeholder: forecast.domElements.ddlMatch.data('init-lable')
                });
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
        }).always(function () {
        });
    }
};

$(function () {
    forecast.init();
});