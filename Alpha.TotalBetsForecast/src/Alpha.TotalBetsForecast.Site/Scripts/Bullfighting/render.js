if (typeof String.prototype.trim !== 'function') {
    String.prototype.trim = function () {
        return this.replace(/^\s+|\s+$/g, '');
    };
}

var BFRender =
{
    disableHighlight: true,

    matches: [],

    resources:
    {
        mathcOfferLive: 'mathcOfferLive',
        noDataText: 'noDataText',
        total: 'totalText'
    },

    init: function () {
        BFRender.mainContent = $('#mainContent');

        BFRender.resources.noDataText = BFRender.mainContent.data('no-data-text');
        BFRender.disableHighlight = BFRender.mainContent.data('disable-highlight') === 'True';
    },

    formatNumber: function (number, formatString) {
        return numeral(number).format(formatString || '0,0.00');
    },

    noInfoTemplate: _.template('<tbody id="no-info-body" data-match-id="match-<%= matchId %>"> \
                    <tr class="no-info text-center"> \
                        <td colspan="6"><%= noDataText %></td> \
                    </tr> \
                </tbody>'),
    renderNoInfo: function (matchId) {
        return BFRender.noInfoTemplate({
            matchId: matchId,
            noDataText: BFRender.resources.noDataText
        });
    },

    winLossColumnNameTemplate: _.template('<td class="win-loss-name text-center ex-col head <%= highLightScore %>" colspan="<%= colSpan %>">\
                                            <span class="winloss-name-<%= betTypeId %>-<%= winLossIndex %>"><%= winLossName %></span>\
                                        </td>'),
    renderWinLossColumnName: function (betTypeForecast, matchForecast) {
        var html = [];
        html.push('<td class="ex-col winloss-name-empty">&nbsp;</td> \
                    <td class="ex-col winloss-name-empty">&nbsp;</td> \
                    <td class="ex-col winloss-name-empty">&nbsp;</td>');

        for (var i = 0; i < betTypeForecast.WinLossColumnNames.length; i++) {
            var winlossName = betTypeForecast.WinLossColumnNames[i], 
                colSpan = betTypeForecast.IsOneXTwo
                ? (i === 0 || i === 2 ? 2 : 1)
                : 1,
                currentScoreClass = '';

            if (betTypeForecast.MainWinLossIndex === i) {
                currentScoreClass = 'high-light-score';
            }

            html.push(BFRender.winLossColumnNameTemplate({
                betTypeId: betTypeForecast.BetTypeId,
                winLossIndex: i,
                winLossName: winlossName,
                colSpan: colSpan,
                highLightScore: currentScoreClass
            }));
        }

        return html.join('');
    },

    renderWinLossNumber: function (winLoss, index) {
        return winLoss > 0
                ? _.template('<span class="win-loss-value positive winloss-value-<%= index %>"><%= winLoss %></span>')({ winLoss: BFRender.formatNumber(winLoss), index: index })
                : _.template('<span class="win-loss-value winloss-value-<%= index %>"><%= winLoss %></span>')({ winLoss: BFRender.formatNumber(winLoss), index: index });
    },

    betTypeNameTemplate: _.template('<td class="ex-col">\
                                            <span class="pull-left"><%= betTypeName %></span>\
                                        </td>'),
    betCountTemplate: _.template('<td class="ex-col text-right"><span class="bet-count-<%= betTypeId %>"><%= betCount %></span></td>'),
    turnoverTemplate: _.template('<td class="ex-col text-right"><span class="turnover-<%= betTypeId %>"><%= turnover %></span></td>'),
    winLossValueTemplate: _.template('<td class="ex-col <%= textAlign %> <%= winLossClass %>" colspan="<%= colSpan %>" >\
                                        <%= winLossValue %> \
                                    </td>'),
    renderBetTypeForecast: function (betTypeForecast, matchForecast) {
        var html = [],
            betTypeId = betTypeForecast.BetTypeId;

        html.push('<tr class="last-row '+ (betTypeForecast.IsOneXTwo ? 'one-x-two-row' : '') +'">');

        html.push(BFRender.betTypeNameTemplate({ betTypeName: betTypeForecast.BetTypeName }));

        html.push(BFRender.betCountTemplate(
           {
               betTypeId: betTypeId,
               betCount: BFRender.formatNumber(betTypeForecast.BetCount, '0,0')
           }));

        html.push(BFRender.turnoverTemplate(
            {
                betTypeId: betTypeId,
                turnover: BFRender.formatNumber(betTypeForecast.Turnover)
            }));

        for (var i = 0; i < betTypeForecast.WinLossValues.length; i++) {
            var winLoss = betTypeForecast.WinLossValues[i],
                colSpan = (betTypeForecast.IsOneXTwo
                ? (i === 0 || i === 2 ? 2 : 1)
                : 1),
                textAlign = betTypeForecast.IsOneXTwo && i !== 1 ? 'text-center' : 'text-right',
                highLightWinLossClass = '';

            if (betTypeForecast.MainWinLossIndex === i) {
                highLightWinLossClass = 'high-light-win-loss'
            }

            html.push(BFRender.winLossValueTemplate({
                textAlign: textAlign,
                colSpan: colSpan,
                winLossValue: BFRender.renderWinLossNumber(winLoss, betTypeId + '-' + i),
                winLossClass: highLightWinLossClass
            }));
        }

        html.push('</tr>');

        return html.join('');
    },

    liveMatchInfoTemplate: _.template('<td rowspan="<%= matchRowSpan %>" class="col-time text-center"><span class="match-score">\
                            <span class="home-score"><%= homeScore %></span>-<span class="away-score"><%= awayScore %></span></span><br />\
                            <span class="offer-live"><%= liveText %></span></td>'),
    deadballMatchInfoTemplate: _.template('<td rowspan="<%= matchRowSpan %>" class="col-time text-center show-time">\
                                                <%= showTime %> \
                                            </td>'),
    matchNameTemplate: _.template('<td rowspan="<%= matchRowSpan %>" class="col-event"> \
            <span class="home-name"><%= homeName %></span> <br /> \
            <span class="away-name"><%= awayName %></span> \
        </td>'),
    renderMatchInfo: function (matchForecast) {
        var html = [];

        if (matchForecast.IsLive) {
            html.push(BFRender.liveMatchInfoTemplate({
                matchRowSpan: matchForecast.MatchRowSpan,
                homeScore: matchForecast.LiveHomeScore,
                awayScore: matchForecast.LiveAwayScore,
                liveText: matchForecast.HasLive ? BFRender.resources.mathcOfferLive : ''
            }));
        }
        else {
            html.push(BFRender.deadballMatchInfoTemplate({
                matchRowSpan: matchForecast.MatchRowSpan,
                showTime: matchForecast.ShowTime
            }));
        }

        html.push(BFRender.matchNameTemplate({
            matchRowSpan: matchForecast.MatchRowSpan,
            homeName: matchForecast.HomeName,
            awayName: matchForecast.AwayName
        }));

        return html.join('');
    },

    firstBetTypeTemplate: _.template('<tbody id="match-<%= matchId %>" data-match-id="<%= matchId %>" class="match-data <%= matchLive %>">\
                    <tr"> \
                        <%= matchInfoHtml %>\
                        <%= firstWinLossColumnNameHtml %>\
                    </tr><%= firstBetTypeForecastHtml %>'),
    renderFirstBetTypeRow: function (matchForecast) {
        var firstBetType = matchForecast.BetTypeForecasts[0],
            firstBetTypeTemplateData = {
                matchRowSpan: matchForecast.MatchRowSpan,
                matchId: matchForecast.Id,
                matchLive: matchForecast.IsLive ? 'match-live' : '',
                matchInfoHtml: BFRender.renderMatchInfo(matchForecast),
                firstWinLossColumnNameHtml: BFRender.renderWinLossColumnName(firstBetType, matchForecast),
                firstBetTypeForecastHtml: BFRender.renderBetTypeForecast(firstBetType, matchForecast),
            };

        return BFRender.firstBetTypeTemplate(firstBetTypeTemplateData);
    },

    totalRowTemplate: _.template('<tr class="last-row total-row">\
                                    <td class="ex-col total-text">\
                                        <%= totalText %>\
                                    </td>\
                                    <td class="ex-col text-right"><span class="total-bet-count-<%= matchId %>"><%= betCount %></span></td>\
                                    <td class="ex-col text-right"><span class="total-turn-over-<%= matchId %>"><%= turnOver %></span></td>\
                                    <td class="ex-col text-center" colspan="5"><span class="win-loss-value total-win-loss-<%= matchId %> <%= positiveClass %>"><%= winloss %></span></td>\
                                </tr>'),
    renderTotalRow: function (matchForecast) {
        return BFRender.totalRowTemplate({
            totalText: BFRender.resources.total,
            betCount: BFRender.formatNumber(matchForecast.TotalBetCount, '0,0.'),
            turnOver: BFRender.formatNumber(matchForecast.TotalTurnover),
            winloss: matchForecast.IsLive ? BFRender.formatNumber(matchForecast.TotalForecastWinLoss) : '',
            matchId: matchForecast.Id,
            positiveClass: matchForecast.TotalForecastWinLoss > 0 ? 'positive' : ''
        });
    },

    firstWinLossColumnNameTemplate: _.template('<tr class="<%= oneXTwoClass %>"><%= winLossColumnNameHtml %></tr>'),
    renderMatchForecast: function (matchForecast) {
        if (matchForecast.BetTypeForecasts.length === 0) {
            return BFRender.renderNoInfo(matchForecast.Id);
        }

        var html = [];

        html.push(BFRender.renderFirstBetTypeRow(matchForecast));

        for (var i = 1; i < matchForecast.BetTypeForecasts.length; i++) {
            var currentBetTypeForecast = matchForecast.BetTypeForecasts[i];

            html.push(BFRender.firstWinLossColumnNameTemplate({
                winLossColumnNameHtml: BFRender.renderWinLossColumnName(currentBetTypeForecast, matchForecast),
                oneXTwoClass: currentBetTypeForecast.IsOneXTwo ? 'one-x-two-row' : ''
            }));

            html.push(BFRender.renderBetTypeForecast(currentBetTypeForecast, matchForecast));
        }

        html.push(BFRender.renderTotalRow(matchForecast));

        html.push('</tbody>');

        return html.join('');
    },

    highlightChanged: function (element, isIncrease) {
        if (!BFRender.disableHighlight) {
            element.pulse({
                backgroundColor: '#fdd64c',
                color: '#000'
            }, {
                returnDelay: 1000,
                interval: 1000,
                pulses: 3
            });
        }
    },

    checkChangedAndHighlight: function (element, value, formatString) {
        if (_.isNumber(value)) {
            value = BFRender.formatNumber(value, formatString);
        }

        var oldValue = parseFloat(element.text());

        if (value 
            && element.text().trim() !== value.trim()) {

            if (element.hasClass('win-loss-value')) {
                if (parseFloat(value) > 0) {
                    element.addClass('positive');
                }
                else {
                    element.removeClass('positive');
                }
            }

            element.html(value);

            BFRender.highlightChanged(
                element.next(),
                oldValue < parseFloat(value.trim()));
        }
    },

    updateMatchesCached: function (matchForecasts) {
        var matchLength = BFRender.matches.length,
            removeMatchIds = [];

        for (var i = 0; i < matchLength; i++) {
            var currentMatch = BFRender.matches[i];
            var cachedMatchForecast = _.find(matchForecasts, function (match) { return match.Id === currentMatch.Id; });

            if (!cachedMatchForecast) {
                removeMatchIds.push(currentMatch.Id);
            }
        }

        BFRender.matches = matchForecasts;
        _.each(removeMatchIds, function (matchId)
        {
            BFRender.mainContent.find('#match-' + matchId).remove();
        });
    },

    trackMatchInfoChanged: function (matchForecastElement, matchForecast) {
        var matchScore = matchForecastElement.find('.match-score'),
            homeScore = matchScore.find('.home-score'),
            awayScore = matchScore.find('.away-score');

        if (parseInt(homeScore.text()) !== matchForecast.LiveHomeScore
            || parseInt(awayScore.text()) !== matchForecast.LiveAwayScore) {

            homeScore.text(matchForecast.LiveHomeScore);
            awayScore.text(matchForecast.LiveAwayScore);
        };

        if (!matchForecast.IsLive) {
            BFRender.checkChangedAndHighlight(
                   matchForecastElement.find('.show-time'),
                   matchForecast.ShowTime);
        }
    },

    trackChangedAndUpdatedValues: function (matchForecast) {
        var matchForecastElement = $('#match-' + matchForecast.Id);

        if (matchForecastElement.length > 0) {

            var cachedMatchForecast = _.find(BFRender.matches, function (match) { return match.Id === matchForecast.Id; });

            if (cachedMatchForecast.MatchRowSpan !== matchForecast.MatchRowSpan) {
                return false;
            }

            BFRender.trackMatchInfoChanged(matchForecastElement, matchForecast);
            matchForecastElement.find('.one-x-two-row .high-light-score, .one-x-two-row .high-light-win-loss')
                        .removeClass('high-light-score')
                        .removeClass('high-light-win-loss');

            for (var betTypeIndex = 0; betTypeIndex < matchForecast.BetTypeForecasts.length; betTypeIndex++) {
                var currentBetType = matchForecast.BetTypeForecasts[betTypeIndex];

                BFRender.checkChangedAndHighlight(
                    matchForecastElement.find('.bet-count-' + currentBetType.BetTypeId),
                    currentBetType.BetCount, '0,0');
                BFRender.checkChangedAndHighlight(
                    matchForecastElement.find('.turnover-' + currentBetType.BetTypeId),
                    currentBetType.Turnover);

                for (var winLossIndex = 0; winLossIndex < currentBetType.WinLossValues.length; winLossIndex++) {
                    BFRender.checkChangedAndHighlight(
                                    matchForecastElement.find('.winloss-name-' + currentBetType.BetTypeId + '-' + winLossIndex),
                                    currentBetType.WinLossColumnNames[winLossIndex]);

                    BFRender.checkChangedAndHighlight(
                        matchForecastElement.find('.winloss-value-' + currentBetType.BetTypeId + '-' + winLossIndex),
                        currentBetType.WinLossValues[winLossIndex]);

                    if (currentBetType.MainWinLossIndex === winLossIndex) {

                        matchForecastElement.find('.winloss-name-' + currentBetType.BetTypeId + '-' + winLossIndex).parent().addClass('high-light-score');
                        matchForecastElement.find('.winloss-value-' + currentBetType.BetTypeId + '-' + winLossIndex).parent().addClass('high-light-win-loss');
                    }
                }
            }

            BFRender.trackTotalRowChange(matchForecastElement, matchForecast);

            return true;
        }

        return false;
    },

    trackTotalRowChange: function (matchForecastElement, matchForecast) {
        BFRender.checkChangedAndHighlight(
               matchForecastElement.find('.total-bet-count-' + matchForecast.Id),
               matchForecast.TotalBetCount, '0,0');
        BFRender.checkChangedAndHighlight(
            matchForecastElement.find('.total-turn-over-' + matchForecast.Id),
            matchForecast.TotalTurnover);

        if (matchForecast.IsLive) {
            BFRender.checkChangedAndHighlight(
                matchForecastElement.find('.total-win-loss-' + matchForecast.Id),
                matchForecast.TotalForecastWinLoss);
        }
    }
};