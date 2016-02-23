var forecast = {
    rootUrl: document.getElementById('RootURL').value,
    maxNumberOfRenderMatch: 5,
    countDownSeconds: 60,
    isStartCountdown: false,

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

        $(document).ajaxStop($.unblockUI);
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

                if (matchIds == null) {
                    forecast.isStartCountdown = false;
                    return;
                }

                try { $.blockUI(); } catch (e) { }
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
                                var newMatchForecast = $(html);
                                var isNoInfoBody = newMatchForecast.attr('id') === 'no-info-body';

                                if (isNoInfoBody) {
                                    forecast.addNoInfoRow(html);
                                } else {
                                    forecast.addLeagueRow();
                                    forecast.addMatchForecast(newMatchForecast, html);
                                }

                                forecast.domElements.mainContent.parent().show();
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

        if (!forecast.isStartCountdown) {
            forecast.startRefrestCountdown();
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
        }
    },

    startRefrestCountdown: function () {
        forecast.domElements.btnRefresh.countdown({
            date: +(new Date) + forecast.countDownSeconds * 1000,
            render: function (data) {
                if (data.sec != 0) {
                    forecast.domElements.btnRefresh.text(data.sec);
                }
            },
            onEnd: function () {
                if (!forecast.isStartCountdown) {
                    return;
                }

                forecast.refreshDone();
            }
        }).off('click.forecast')
                    .on("click.forecast", function () {
                        forecast.refreshDone();
                    });
        forecast.isStartCountdown = true;
    },

    refreshDone: function () {
        forecast.domElements.btnSubmit.trigger('click.forecast');
        forecast.domElements.btnRefresh.data('countdown').update(+(new Date) + forecast.countDownSeconds * 1000).start();
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
            beforeSend: function () {
                try { $.blockUI(); } catch (e) { }
            }
        }).done(function (viewModel) {
            if (viewModel) {
                var options = '';
                var processedMatches = _.map(viewModel.Matches, function (match) {
                    return {
                        label: match.Name,
                        value: match.Id
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
            }
        }).fail(function (jqXHR, textStatus, errorThrown) {
        });
    }
};

$(function () {
    forecast.init();
});