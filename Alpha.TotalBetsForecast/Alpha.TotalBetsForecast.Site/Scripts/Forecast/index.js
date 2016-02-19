var forecast = {
    rootUrl: document.getElementById('RootURL').value,
    urls: {
        getMatches: 'Forecast/GetMatches',
        getMatchForecast: 'Forecast/GetMatchForecast'
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

        forecast.domElements.btnSubmit
            .off('click.forecast')
            .on('click.forecast', function () {
                var matcheForecastQuery =
                   {
                       matchId: forecast.domElements.ddlMatch.val(),
                       isLive: forecast.domElements.ddlDeadBall.val() === '1'
                   };

                $.ajax({
                    url: forecast.getAbsoluteUrl(forecast.urls.getMatchForecast),
                    type: "GET",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    data: ajaxOption.data,
                    beforeSend: function () {
                    }
                }).done(function (data) {
                }).fail(function (jqXHR, textStatus, errorThrown) {
                }).always(function () {
                });
            });
    },
    loadMatches: function () {
        var matchId = parseInt(forecast.domElements.ddlMatch.val());
        var matchesQuery =
            {
                leagueId: forecast.domElements.ddlLeague.val(),
                isLive: forecast.domElements.ddlDeadBall.val() === '1'
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

                forecast.domElements.ddlMatch.select2({
                    data: processedMatches
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