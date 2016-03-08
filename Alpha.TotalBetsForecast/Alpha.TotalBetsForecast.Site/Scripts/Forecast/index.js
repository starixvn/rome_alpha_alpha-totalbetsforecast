/// <reference path="match-forecast-renderer.js" />

var forecast = {
    rootUrl: document.getElementById('RootURL').value,
    maxNumberOfRenderMatch: 5,
    countDownSeconds: 30,
    loadMatchesInterval: 5 * 60 * 1000,
    isStartCountdown: false,
    loadingMatches: [],
    urls: {
        getMatches: 'Forecast/GetMatches',
        getMatchForecast: 'Forecast/GetMatchForecast',
        getLeagues: 'Forecast/GetLeagues'
    },

    domElements: {
        btnSubmit: null,
        ddlDeadBall: null,
        ddlLeague: null,
        ddlMatch: null,
        mainContent: null,
        browserWarning: null,
        closeWarning: null,
        btnMatchSelect: null,
        txtStatus: null,
        blockIconSmall: null,
        refreshValues: null,
        ddlRefresh: null
    },

    init: function () {
        $(document).ajaxStop($.unblockUI);

        forecast.cachedDOM();
        forecast.registerEvents();

        forecastRenderer.init();

        setInterval(forecast.loadMatches, forecast.loadMatchesInterval);
    },

    getAbsoluteUrl: function (relativeUrl) {
        return forecast.rootUrl + relativeUrl;
    },

    cachedDOM: function () {
        forecast.domElements.btnSubmit = $('#btnSubmit');
        forecast.domElements.ddlMatch = $('#ddlMatch');
        forecast.domElements.ddlLeague = $('#ddlLeague');
        forecast.domElements.ddlDeadBall = $('#ddlDeadBall');
        forecast.domElements.mainContent = $('#mainContent');
        forecast.domElements.browserWarning = $('#browser-warning');
        forecast.domElements.closeWarning = $('#close-warning');
        forecast.domElements.txtStatus = $('#text-status');
        forecast.domElements.blockIconSmall = $('#blockIconSmall');
        forecast.domElements.refreshValues = $('#refresh-value a');
        forecast.domElements.ddlRefresh = $('#ddlRefresh');
    },

    registerEvents: function () {

        forecast.checkIEBrowserAndShowWarning();

        forecast.domElements.ddlDeadBall
            .off('change.forecast')
            .on('change.forecast', function () {
                forecast.domElements.mainContent.parent().hide().find('tbody').remove();
                forecast.loadLeagues();
                forecast.domElements.txtStatus.text(
                    forecast.domElements.ddlDeadBall.val() === 'true'
                    ? forecast.domElements.txtStatus.data('offer-live')
                    : forecast.domElements.txtStatus.data('dead-ball'))
            }).trigger('change');

        forecast.domElements.btnSubmit
            .off('click.forecast')
            .on('click.forecast', forecast.loadMatchForecasts);

        forecast.domElements.refreshValues
            .off('click.forecast')
            .on('click.forecast', function () {
                forecast.countDownSeconds = parseInt($(this).data('refresh-value'));
                forecast.stopCountdown();
                forecast.startCountdown();
            });
    },

    loadMatchForecasts: function () {
        forecast.stopCountdown();
        forecast.domElements.blockIconSmall.show();
        var matchIds = forecast.domElements.ddlMatch.val();

        if (matchIds == null) {
            forecast.domElements.btnMatchSelect.attr('title', forecast.domElements.ddlMatch.data('select-at-least')).tooltip('show');
            forecast.domElements.mainContent.parent().hide().find('tbody').remove();
            return;
        }

        forecast.removeMatches(matchIds);

        var matchIdsLength = matchIds.length;
        forecast.loadingMatches = [];
        forecast.domElements.ddlRefresh.addClass('block-icon');

        for (var i = 0; i < matchIdsLength; i++) {
            var currentMatchId = parseInt(matchIds[i]);
            forecast.loadingMatches.push(currentMatchId);
            forecast.loadSingleMatchForecast(currentMatchId);
        }
    },

    loadSingleMatchForecast: function (matchId) {
        $.ajax({
            url: forecast.getAbsoluteUrl(forecast.urls.getMatchForecast),
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                matchId: matchId,
                isLive: forecast.domElements.ddlDeadBall.val() === 'true',
                version: Math.random()
            },
        }).done(function (matchForecast) {
            if (matchForecast) {

                var isUpdated = forecastRenderer.trackChangedAndUpdatedValues(matchForecast);

                if (!isUpdated) {
                    var html = forecastRenderer.renderMatchForecast(matchForecast),
                        newMatchForecast = $(html),
                        isNoInfoBody = newMatchForecast.attr('id') === 'no-info-body';

                    if (isNoInfoBody) {
                        forecast.addNoInfoRow(html);
                    } else {
                        forecast.addLeagueRow();
                        forecast.addMatchForecast(newMatchForecast, html);
                    }

                    forecast.domElements.mainContent.parent().show();

                    forecastRenderer.updateMatchesCached(matchForecast);
                }

                forecast.removeLoadingMatches(matchForecast.MatchId);
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
            forecast.removeLoadingMatches(matchId);
        });
    },

    removeLoadingMatches: function (matchId) {
        for (var i = forecast.loadingMatches.length; i >= 0; i--) {
            if (forecast.loadingMatches[i] === matchId) {
                forecast.loadingMatches.splice(i, 1);
            }
        }

        if (forecast.loadingMatches.length == 0) {
            forecast.domElements.ddlRefresh.removeClass('block-icon');
            forecast.domElements.blockIconSmall.hide();
            if (!forecast.isStartCountdown) {
                forecast.startCountdown();
            }
        }
    },

    removeMatches: function (matchIds) {
        var matchElements = forecast.domElements.mainContent.find('tbody.match-data');

        for (var matchIndex = 0; matchIndex < matchElements.length; matchIndex++) {
            var currentMatch = $(matchElements[matchIndex]);
            var currentMatchId = currentMatch.data('match-id'),
                renderMatchId = _.find(matchIds, function (matchId) { return parseInt(matchId) === parseInt(currentMatchId); });

            if (typeof (renderMatchId) === 'undefined') {
                currentMatch.remove();
            }
        }
    },

    addMatchForecast: function (newMatchForecast, rawHtml) {
        var currentMatchForecast = forecast.domElements.mainContent.find('tbody[id=' + newMatchForecast.attr('id') + ']');

        currentMatchForecast.length > 0
                ? currentMatchForecast.html(newMatchForecast.html())
                : forecast.domElements.mainContent.append(rawHtml)

        forecast.domElements.mainContent.find('tbody[id=no-info-body]').remove();
    },

    addLeagueRow: function () {
        var league = forecast.domElements.mainContent.find('tbody[id=league-name]');
        if (league.length === 0) {
            forecast.domElements.mainContent.append('<tbody id="league-name"><tr class="cl-league"><td colspan="9">'
                + forecast.domElements.ddlLeague.find('option:selected').text()
                + '</td></tr>');
        }
    },

    addNoInfoRow: function (html) {
        var noInfo = forecast.domElements.mainContent.find('tbody');

        if (noInfo.length === 0) {
            forecast.domElements.mainContent.append(html);
            forecast.domElements.mainContent.find('tbody').attr('data-match-id', '');
        }
        else {
            var noInfoHtml = $(html),
                noInfoMatch = forecast.domElements.mainContent.find('tbody[id=' + noInfoHtml.attr('data-match-id') + ']');

            if (noInfoMatch.length > 0) {
                noInfoMatch.remove();
                forecast.addNoInfoRow();
            }
        }
    },

    startCountdown: function () {
        forecast.domElements.ddlRefresh.countdown({
            date: +(new Date) + forecast.countDownSeconds * 1000,
            render: function (data) {
                if (data.sec != 0) {
                    forecast.domElements.ddlRefresh.text(data.sec);
                }
            },
            onEnd: function () {
                forecast.refreshDone();
            }
        });

        forecast.domElements.ddlRefresh.data('countdown').update(+(new Date) + forecast.countDownSeconds * 1000).start();

        forecast.isStartCountdown = true;
    },

    refreshDone: function () {
        if (forecast.isStartCountdown
            && forecast.domElements.ddlMatch.find('option:selected').length > 0) {
            forecast.domElements.btnSubmit.trigger('click.forecast');
            forecast.domElements.ddlRefresh.data('countdown').update(+(new Date) + forecast.countDownSeconds * 1000).start();
        }
        else {
            forecast.stopCountdown();
            forecast.domElements.mainContent.parent().hide().find('tbody').remove();
        }
    },

    stopCountdown: function () {
        forecast.isStartCountdown = false;
        var countDown = forecast.domElements.ddlRefresh.data('countdown');
        if (countDown) {
            countDown.stop();
            forecast.domElements.ddlRefresh.text(forecast.countDownSeconds);
        }
    },

    loadLeagues: function () {
        $.ajax({
            url: forecast.getAbsoluteUrl(forecast.urls.getLeagues),
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                isLive: forecast.domElements.ddlDeadBall.val() === 'true',
                version: Math.random()
            },
            beforeSend: function () {
                try { $.blockUI(); } catch (e) { }
            }
        }).done(function (leagues) {
            if (leagues) {
                forecast.renderLeagues(leagues);
                forecast.loadMatches();
            }
        });
    },

    renderLeagues: function(leagues){
        var leagueId = forecast.domElements.ddlLeague.val() || leagues[0].Id,
            options = '',
            processedLeagues = _.map(leagues, function (league) {
                return {
                    label: league.Name,
                    value: league.Id,
                    selected: league.Id === parseInt(leagueId)
                };
            });

        forecast.domElements.ddlLeague.multiselect({
            maxHeight: 300,
            enableFiltering: true,
            numberDisplayed: 1,
            enableCaseInsensitiveFiltering: true,
            onChange: function (option, checked) {
                forecast.loadMatches();
                forecast.domElements.mainContent.parent().hide().find('tbody').remove();
                forecast.stopCountdown();
            }
        });

        forecast.domElements.ddlLeague.multiselect('dataprovider', processedLeagues);
    },

    loadMatches: function () {
        $.ajax({
            url: forecast.getAbsoluteUrl(forecast.urls.getMatches),
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: {
                leagueId: forecast.domElements.ddlLeague.val(),
                isLive: forecast.domElements.ddlDeadBall.val() === 'true',
                version: Math.random()
            },
            beforeSend: function () {
                try { $.blockUI(); } catch (e) { }
            }
        }).done(function (matches) {
            if (matches) {
                forecast.renderMatches(matches);
            }
        });
    },

    renderMatches: function (matches) {
        var matchIds = forecast.domElements.ddlMatch.val() || [matches[0].Id.toString()],
            options = '',
            anyMatchSelected = false,
            processedMatches = _.map(matches, function (match) {
                var selected = _.contains(matchIds, match.Id.toString());

                if (selected) {
                    anyMatchSelected = true;
                }

                return {
                    label: match.Name,
                    value: match.Id,
                    selected: selected
                };
            });

        if (!anyMatchSelected) {
            processedMatches[0].selected = true;
        }

        forecast.domElements.ddlMatch.multiselect({
            maxHeight: 300,
            enableFiltering: true,
            numberDisplayed: 1,
            enableCaseInsensitiveFiltering: true,
            nonSelectedText: forecast.domElements.ddlMatch.data('init-lable'),
            nSelectedText: ' ' + forecast.domElements.ddlMatch.data('selected-event-text'),
            onChange: function (option, checked) {
                forecast.domElements.btnMatchSelect.tooltip('destroy');
                // Get selected options.
                var selectedOptions = forecast.domElements.ddlMatch.find('option:selected');

                if (selectedOptions.length >= forecast.maxNumberOfRenderMatch) {
                    // Disable all other checkboxes.
                    var nonSelectedOptions = forecast.domElements.ddlMatch.find('option').filter(function () {
                        return !$(this).is(':selected');
                    });

                    var dropdown = forecast.domElements.ddlMatch.siblings('.multiselect-container');
                    nonSelectedOptions.each(function () {
                        var input = $('input[value="' + $(this).val() + '"]');
                        input.prop('disabled', true);
                        input.parent('li').addClass('disabled');
                    });
                }
                else {
                    // Enable all checkboxes.
                    var dropdown = forecast.domElements.ddlMatch.siblings('.multiselect-container');
                    forecast.domElements.ddlMatch.find('option').each(function () {
                        var input = $('input[value="' + $(this).val() + '"]');
                        input.prop('disabled', false);
                        input.parent('li').addClass('disabled');
                    });
                }
            }
        });

        forecast.domElements.ddlMatch.multiselect('dataprovider', processedMatches);
        forecast.domElements.btnMatchSelect = forecast.domElements.ddlMatch.next().find('button.multiselect');
    },

    checkIEBrowserAndShowWarning: function () {
        var myNav = navigator.userAgent.toLowerCase();
        var ieVersion = (myNav.indexOf('msie') != -1) ? parseInt(myNav.split('msie')[1]) : 10;

        if (ieVersion <= 9) {
            forecast.domElements.browserWarning.show();
            forecast.domElements.closeWarning
                .off('click.forecast')
                .on('click.forecast', function () {
                    forecast.domElements.browserWarning.hide();
                });
        }
    }
};

$(function () {
    forecast.init();
});