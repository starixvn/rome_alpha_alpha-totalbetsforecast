
var bullfightingForecast = {
    rootUrl: document.getElementById('RootURL').value,
    countDownSeconds: 30,
    countDownDeadBallSeconds: 60,
    countDownLiveBallSeconds: 30,
    loadMatchesInterval: 5 * 60 * 1000,
    isStartCountdown: false,
    loadingMatches: [],
    currentRequest: null,

    urls: {
        getMatches: 'Bullfighting/GetMatches'
    },

    cookies: {
        status: 'forecast_match_status',
        league: 'forecast_league',
        match: 'forecast_match'
    },
    cookieExpireTime: 1,

    domElements: {
        btnSubmit: null,
        mainContent: null,
        refreshValues: null,
        ddlRefresh: null
    },

    init: function () {
        $(document).ajaxStop($.unblockUI);
        bullfightingForecast.cachedDOM();
        bullfightingForecast.registerEvents();

        forecastRenderer.init();

        setInterval(bullfightingForecast.loadLeagues, bullfightingForecast.loadMatchesInterval);
    },

    getAbsoluteUrl: function (relativeUrl) {
        return bullfightingForecast.rootUrl + relativeUrl;
    },

    cachedDOM: function () {
        bullfightingForecast.domElements.btnSubmit = $('#btnSubmit');
        bullfightingForecast.domElements.mainContent = $('#mainContent');
        bullfightingForecast.domElements.refreshValues = $('#refresh-value a');
        bullfightingForecast.domElements.ddlRefresh = $('#ddlRefresh');
    },

    registerEvents: function () {

        bullfightingForecast.domElements.btnSubmit
            .off('click.forecast')
            .on('click.forecast', bullfightingForecast.loadMatchForecasts);

        bullfightingForecast.domElements.refreshValues
            .off('click.forecast')
            .on('click.forecast', function () {
                bullfightingForecast.countDownSeconds = parseInt($(this).data('refresh-value'));
                bullfightingForecast.domElements.ddlRefresh.text(bullfightingForecast.countDownSeconds);
                if (bullfightingForecast.isStartCountdown) {
                    bullfightingForecast.stopCountdown();
                    bullfightingForecast.startCountdown();
                }
            });
    },

    loadMatchForecasts: function () {
        if (bullfightingForecast.currentRequest) {
            bullfightingForecast.currentRequest.abort();
        }

        bullfightingForecast.stopCountdown();
        bullfightingForecast.domElements.blockIconSmall.show();

        bullfightingForecast.currentRequest = $.ajax({
            url: bullfightingForecast.getAbsoluteUrl(bullfightingForecast.urls.getMatchForecasts),
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
        }).done(function (matchForecasts) {
            if (matchForecasts) {

                bullfightingForecast.removeLoadingIcon();

                if (matchForecasts.length === 0) {
                    var noInfoHtml = forecastRenderer.renderNoInfo('no-info-body');
                    bullfightingForecast.addNoInfoRow(noInfoHtml);
                }
                else {
                    for (var i = 0; i < matchForecasts.length; i++) {
                        var matchForecast = matchForecasts[i];
                        if (matchForecast) {
                            var html = forecastRenderer.renderMatchForecast(matchForecast),
                                newMatchForecast = $(html);

                            bullfightingForecast.addMatchForecast(newMatchForecast, html);
                        }
                    }

                    forecastRenderer.updateMatchesCached(matchForecasts);
                }

                bullfightingForecast.domElements.mainContent.parent().show();
            }

        }).fail(function (jqXHR, textStatus, errorThrown) {
            bullfightingForecast.removeLoadingIcon();
        });
    },

    removeLoadingIcon: function () {
        bullfightingForecast.domElements.ddlRefresh.removeClass('block-icon');
        bullfightingForecast.domElements.blockIconSmall.hide();
        if (!bullfightingForecast.isStartCountdown) {
            bullfightingForecast.startCountdown();
        }
    },

    addMatchForecast: function (newMatchForecast, rawHtml) {
        var currentMatchForecast = bullfightingForecast.domElements.mainContent.find('tbody[id=' + newMatchForecast.attr('id') + ']');

        currentMatchForecast.length > 0
                ? currentMatchForecast.html(newMatchForecast.html())
                : bullfightingForecast.domElements.mainContent.append(rawHtml)

        bullfightingForecast.domElements.mainContent.find('tbody[id=no-info-body]').remove();
    },

    addNoInfoRow: function (html) {
        var noInfo = bullfightingForecast.domElements.mainContent.find('tbody');

        if (noInfo.length === 0) {
            bullfightingForecast.domElements.mainContent.append(html);
            bullfightingForecast.domElements.mainContent.find('tbody').attr('data-match-id', '');
        }
        else {
            var noInfoHtml = $(html),
                noInfoMatch = bullfightingForecast.domElements.mainContent.find('tbody[id=' + noInfoHtml.attr('data-match-id') + ']');

            if (noInfoMatch.length > 0) {
                noInfoMatch.remove();
                bullfightingForecast.addNoInfoRow();
            }
        }
    },

    startCountdown: function () {
        bullfightingForecast.domElements.ddlRefresh.countdown({
            date: +(new Date) + bullfightingForecast.countDownSeconds * 1000,
            render: function (data) {
                if (data.sec != 0) {
                    bullfightingForecast.domElements.ddlRefresh.text(data.sec);
                }
            },
            onEnd: function () {
                bullfightingForecast.refreshDone();
            }
        });

        bullfightingForecast.domElements.ddlRefresh.data('countdown').update(+(new Date) + bullfightingForecast.countDownSeconds * 1000).start();

        bullfightingForecast.isStartCountdown = true;
    },

    refreshDone: function () {
        if (bullfightingForecast.isStartCountdown
            && bullfightingForecast.domElements.ddlMatch.find('option:selected').length > 0) {
            bullfightingForecast.domElements.btnSubmit.trigger('click.forecast');
            bullfightingForecast.domElements.ddlRefresh.data('countdown').update(+(new Date) + bullfightingForecast.countDownSeconds * 1000).start();
        }
        else {
            bullfightingForecast.stopCountdown();
            bullfightingForecast.domElements.mainContent.parent().hide().find('tbody').remove();
        }
    },

    stopCountdown: function () {
        bullfightingForecast.isStartCountdown = false;
        var countDown = bullfightingForecast.domElements.ddlRefresh.data('countdown');
        if (countDown) {
            countDown.stop();
            bullfightingForecast.domElements.ddlRefresh.text(bullfightingForecast.countDownSeconds);
        }
    }
};

$(function () {
    bullfightingForecast.init();
});