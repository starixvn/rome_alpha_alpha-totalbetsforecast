/// <reference path="match-forecast-renderer.js" />

var forecast = {
    rootUrl: document.getElementById('RootURL').value,
    maxNumberOfRenderMatch: 5,
    countDownSeconds: 60,
    loadMatchesInterval: 5 * 60 * 1000,
    isStartCountdown: false,
    matchForecasts: [],
    urls: {
        getMatches: 'Forecast/GetMatches',
        getMatchForecast: 'Forecast/MatchForecast',
        getJsonMatchForecast: 'Forecast/GetMatchForecast'
    },

    domElements: {
        btnSubmit: null,
        ddlDeadBall: null,
        ddlLeague: null,
        ddlMatch: null,
        btnRefresh: null,
        mainContent: null,
        browserWarning: null,
        closeWarning: null,
        btnMatchSelect: null
    },

    init: function () {
        forecast.cachedDOM();
        forecast.registerEvents();

        $(document).ajaxStop($.unblockUI);
        forecast.loadMatches();

        setInterval(forecast.loadMatches, forecast.loadMatchesInterval);
    },

    getAbsoluteUrl: function (relativeUrl) {
        return forecast.rootUrl + relativeUrl;
    },

    cachedDOM: function () {
        forecast.domElements.btnSubmit = $('#btnSubmit');
        forecast.domElements.btnRefresh = $('#btnRefresh');
        forecast.domElements.ddlMatch = $('#ddlMatch');
        forecast.domElements.ddlLeague = $('#LeagueId');
        forecast.domElements.ddlDeadBall = $('#ddlDeadBall');
        forecast.domElements.mainContent = $('#mainContent');
        forecast.domElements.browserWarning = $('#browser-warning');
        forecast.domElements.closeWarning = $('#close-warning');
    },

    registerEvents: function () {

        forecast.checkIEBrowserAndShowWarning();

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

        forecast.domElements.btnSubmit
            .off('click.forecast')
            .on('click.forecast', function () {

                var matchIds = forecast.domElements.ddlMatch.val();

                if (matchIds == null) {
                    forecast.domElements.btnMatchSelect.attr('title', forecast.domElements.ddlMatch.data('select-at-least')).tooltip('show');
                    forecast.stopCountdown();
                    return;
                }

                //try { $.blockUI(); } catch (e) { }
                var matchIdsLength = matchIds.length;

                for (var i = 0; i < matchIdsLength; i++) {
                    var matcheForecastQuery =
                                   {
                                       matchId: matchIds[i],
                                       isLive: forecast.domElements.ddlDeadBall.val() === 'true',
                                       version: Math.random()
                                   };

                    //$.get(forecast.getAbsoluteUrl(
                    //    forecast.urls.getJsonMatchForecast
                    //    + '?'
                    //    + $.param(matcheForecastQuery)),
                    //    function (html) {
                    //        if (html) {
                   
                    //        }
                    //    });
                    $.ajax({
                        url: forecast.getAbsoluteUrl(forecast.urls.getJsonMatchForecast),
                        type: "GET",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        data: matcheForecastQuery,
                    }).done(function (matchForecast) {
                        if (matchForecast) {

                            var isChanged = matchForecastRenderer.trackChanged(matchForecast);

                            if (isChanged) {
                                
                            }
                            else {
                                var html = matchForecastRenderer.renderMatchForecast(matchForecast);
                                var newMatchForecast = $(html);
                                var isNoInfoBody = newMatchForecast.attr('id') === 'no-info-body';

                                if (isNoInfoBody) {
                                    forecast.addNoInfoRow(html);
                                } else {
                                    forecast.addLeagueRow();
                                    forecast.addMatchForecast(newMatchForecast, html);
                                }

                                forecast.domElements.mainContent.parent().show();

                                //if (!forecast.isStartCountdown) {
                                //    forecast.startCountdown();
                                //}
                            }
                        }
                    });
                }
            });
    },

    addMatchForecast: function (newMatchForecast, rawHtml) {
        var currentMatchForecast = forecast.domElements.mainContent.find('tbody[id=' + newMatchForecast.attr('id') + ']');
        if (currentMatchForecast.length > 0) {
            currentMatchForecast.html(newMatchForecast.html());
        }
        else {
            forecast.domElements.mainContent.append(rawHtml);
        }

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
            var noInfoHtml = $(html);
            var noInfoMatch = forecast.domElements.mainContent.find('tbody[id=' + noInfoHtml.attr('data-match-id') + ']')
            if (noInfoMatch.length > 0) {
                noInfoMatch.remove();
                forecast.addNoInfoRow();
            }
        }
    },

    startCountdown: function () {
        forecast.domElements.btnRefresh.countdown({
            date: +(new Date) + forecast.countDownSeconds * 1000,
            render: function (data) {
                if (data.sec != 0) {
                    forecast.domElements.btnRefresh.text(data.sec);
                }
            },
            onEnd: function () {
                forecast.refreshDone();
            }
        }).off('click.forecast')
                        .on("click.forecast", function () {
                            forecast.refreshDone();
                        });
        forecast.domElements.btnRefresh.data('countdown').update(+(new Date) + forecast.countDownSeconds * 1000).start();

        forecast.isStartCountdown = true;
    },

    refreshDone: function () {
        if (forecast.isStartCountdown
            && forecast.domElements.ddlMatch.find('option:selected').length > 0) {
            forecast.domElements.btnSubmit.trigger('click.forecast');
            forecast.domElements.btnRefresh.data('countdown').update(+(new Date) + forecast.countDownSeconds * 1000).start();
        }
        else {
            forecast.stopCountdown();
        }
    },

    stopCountdown: function () {
        forecast.isStartCountdown = false;
        var countDown = forecast.domElements.btnRefresh.data('countdown');
        if (countDown) {
            countDown.stop();
            forecast.domElements.btnRefresh.text(forecast.countDownSeconds);
            forecast.domElements.mainContent.parent().hide().find('tbody').remove();
        }
    },

    loadMatches: function () {
        var matchesQuery =
            {
                leagueId: forecast.domElements.ddlLeague.val(),
                isLive: forecast.domElements.ddlDeadBall.val() === 'true',
                version: Math.random()
            };

        $.ajax({
            url: forecast.getAbsoluteUrl(forecast.urls.getMatches),
            type: "GET",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            data: matchesQuery,
            beforeSend: function () {
                try { $.blockUI(); } catch (e) { }
            }
        }).done(function (viewModel) {
            if (viewModel) {
                forecast.renderMatchList(viewModel);
            }
        });
    },
    renderMatchList: function (viewModel) {
        var matchIds = forecast.domElements.ddlMatch.val() || [];

        var options = '';
        var processedMatches = _.map(viewModel.Matches, function (match) {
            return {
                label: match.Name,
                value: match.Id,
                selected: _.contains(matchIds, match.Id.toString())
            };
        });

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