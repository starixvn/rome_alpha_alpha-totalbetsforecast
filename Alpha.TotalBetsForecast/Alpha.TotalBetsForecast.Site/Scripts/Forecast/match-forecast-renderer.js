if (typeof String.prototype.trim !== 'function') {
    String.prototype.trim = function () {
        return this.replace(/^\s+|\s+$/g, '');
    };
}

var forecastRenderer =
{
    disableHighlight: true,

    matches: [],

    resources:
    {
        mathcOfferLive: 'mathcOfferLive',
        noDataText: 'noDataText'
    },

    init: function () {
        var mainContent = $('#mainContent');

        forecastRenderer.resources.noDataText = mainContent.data('no-data-text');
        forecastRenderer.resources.mathcOfferLive = mainContent.data('offer-live');
        forecastRenderer.disableHighlight = mainContent.data('disable-highlight') === 'True';
    },

    formatNumber: function (number, formatString) {
        return numeral(number).format(formatString || '0,0.00');
    },

    noInfoTemplate: _.template('<tbody id="no-info-body" data-match-id="match-<%= matchId %>"> \
                    <tr class="no-info text-center"> \
                        <td colspan="9"><%= noDataText %></td> \
                    </tr> \
                </tbody>'),
    renderNoInfo: function (matchId) {
        return forecastRenderer.noInfoTemplate({
            matchId: matchId,
            noDataText: forecastRenderer.resources.noDataText
        });
    },

    winLossColumnNameTemplate: _.template('<td class="win-loss-name text-center ex-col head" colspan="<%= colSpan %>">\
                                            <span class="winloss-name-<%= betTypeId %>-<%= winLossIndex %>"><%= winLossName %></span>\
                                        </td>'),
    renderWinLossColumnName: function (betTypeForecast) {
        var html = [];
        html.push('<td class="ex-col">&nbsp;</td> \
                    <td class="ex-col">&nbsp;</td> \
                    <td class="ex-col">&nbsp;</td>');

        for (var i = 0; i < betTypeForecast.WinLossColumnNames.length; i++) {
            var winlossName = betTypeForecast.WinLossColumnNames[i];
            var colSpan = betTypeForecast.IsOneXTwoBetType
            ? (i === 0 || i === 2 ? 2 : 1)
            : 1;

            html.push(forecastRenderer.winLossColumnNameTemplate({
                betTypeId: betTypeForecast.BetTypeId,
                winLossIndex: i,
                winLossName: winlossName,
                colSpan: colSpan
            }));
        }

        return html.join('');
    },

    renderWinLossNumber: function (winLoss, index) {
        return winLoss > 0
                ? _.template('<span class="positive winloss-value-<%= index %>"><%= winLoss %></span>')({ winLoss: forecastRenderer.formatNumber(winLoss), index: index })
                : _.template('<span class="winloss-value-<%= index %>"><%= winLoss %></span>')({ winLoss: forecastRenderer.formatNumber(winLoss), index: index });
    },

    betTypeNameTemplate: _.template('<td class="ex-col">\
                                            <span class="pull-left"><%= betTypeName %></span>\
                                        </td>'),
    betCountTemplate: _.template('<td class="ex-col text-right"><span class="bet-count-<%= choiceIdentify %>"><%= betCount %></span></td>'),
    turnoverTemplate: _.template('<td class="ex-col text-right"><span class="turnover-<%= choiceIdentify %>"><%= turnover %></span></td>'),
    winLossValueTemplate: _.template('<td class="ex-col <%= textAlign %>" colspan="<%= colSpan %>" >\
                                        <%= winLossValue %> \
                                    </td>'),
    renderBetTypeForecast: function (betTypeForecast) {
        var html = [],
            betTypeId = betTypeForecast.BetTypeId;

        html.push('<tr>');

        html.push(forecastRenderer.betTypeNameTemplate({ betTypeName: betTypeForecast.BetTypeName }));

        html.push(forecastRenderer.betCountTemplate(
           {
               choiceIdentify: betTypeId,
               betCount: forecastRenderer.formatNumber(betTypeForecast.BetCount, '0,0')
           }));

        html.push(forecastRenderer.turnoverTemplate(
            {
                choiceIdentify: betTypeId,
                turnover: forecastRenderer.formatNumber(betTypeForecast.Turnover)
            }));

        for (var i = 0; i < betTypeForecast.WinLossValues.length; i++) {
            var winLoss = betTypeForecast.WinLossValues[i],
                colSpan = betTypeForecast.IsOneXTwoBetType
                ? (i === 0 || i === 2 ? 2 : 1)
                : 1,
                textAlign = betTypeForecast.IsOneXTwoBetType && i !== 1 ? 'text-center' : 'text-right';

            html.push(forecastRenderer.winLossValueTemplate({
                textAlign: textAlign,
                colSpan: colSpan,
                winLossValue: forecastRenderer.renderWinLossNumber(winLoss, betTypeId + '-' + i)
            }));
        }

        html.push('</tr>');

        return html.join('');
    },

    liveMatchInfoTemplate: _.template('<td rowspan="<%= matchRowSpan %>" class="col-time text-center"><span class="match-score">\
                            <span class="home-score"><%= homeScore %> </span>-<span class="away-score"> <%= awayScore %></span></span><br />\
                            <span class="offer-live"><%= liveText %></span></td>'),
    deadballMatchInfoTemplate: _.template('<td rowspan="<%= matchRowSpan %>" class="col-time text-center">\
                                                <%= showTime %> \
                                            </td>'),
    matchNameTemplate: _.template('<td rowspan="<%= matchRowSpan %>" class="col-event"> \
            <span class="home-name"><%= homeName %></span> <br /> \
            <span class="away-name"><%= awayName %></span> \
        </td>'),
    renderMatchInfo: function (matchForecast) {
        var html = [];

        if (matchForecast.IsLive) {
            html.push(forecastRenderer.liveMatchInfoTemplate({
                matchRowSpan: matchForecast.MatchRowSpan,
                homeScore: matchForecast.LiveHomeScore,
                awayScore: matchForecast.LiveAwayScore,
                liveText: matchForecast.HasLive ? forecastRenderer.resources.mathcOfferLive : ''
            }));
        }
        else {
            html.push(forecastRenderer.deadballMatchInfoTemplate({
                matchRowSpan: matchForecast.MatchRowSpan,
                showTime: matchForecast.ShowTime
            }));
        }

        html.push(forecastRenderer.matchNameTemplate({
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
        var matchInfoHtml = forecastRenderer.renderMatchInfo(matchForecast),
            firstBetType = matchForecast.BetTypeForecasts[0],
            firstBetTypeTemplateData = {
                matchRowSpan: matchForecast.MatchRowSpan,
                matchId: matchForecast.Id,
                matchLive: matchForecast.IsLive ? 'match-live' : '',
                matchInfoHtml: firstBetType.IsOneXTwoBetType ? '' : matchInfoHtml,
                firstWinLossColumnNameHtml: forecastRenderer.renderWinLossColumnName(firstBetType),
                firstBetTypeForecastHtml: forecastRenderer.renderBetTypeForecast(firstBetType, true),
            };

        return forecastRenderer.firstBetTypeTemplate(firstBetTypeTemplateData);
    },

    firstWinLossColumnNameTemplate: _.template('<tr>\
            <td class="ex-col">\
                                            <span class="pull-left"><%= betTypeName %></span>\
                                        </td>\
        </tr>'),
    renderTotalRow: function () {

    },

    firstWinLossColumnNameTemplate: _.template('<tr><%= winLossColumnNameHtml %></tr>'),
    renderMatchForecast: function (matchForecast) {
        if (matchForecast.BetTypeForecasts.length === 0) {
            return forecastRenderer.renderNoInfo(matchForecast.Id);
        }

        var html = [];

        html.push(forecastRenderer.renderFirstBetTypeRow(matchForecast));

        for (var i = 1; i < matchForecast.BetTypeForecasts.length; i++) {
            var currentBetTypeForecast = matchForecast.BetTypeForecasts[i];

            html.push(forecastRenderer.firstWinLossColumnNameTemplate({
                winLossColumnNameHtml: forecastRenderer.renderWinLossColumnName(currentBetTypeForecast)
            }));

            html.push(forecastRenderer.renderBetTypeForecast(currentBetTypeForecast));
        }

        html.push('</tbody>');

        return html.join('');
    },

    highlightChanged: function (element) {
        if (!forecastRenderer.disableHighlight) {
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

    trackMatchInfoChanged: function (matchForecastElement, matchForecast) {
        var matchScore = matchForecastElement.find('.match-score'),
            homeScore = matchScore.find('.home-score'),
            awayScore = matchScore.find('.away-score'),
            forecastTotal = matchForecastElement.find('.forecast-total-value');

        if (parseInt(homeScore.text()) !== matchForecast.LiveHomeScore
            || parseInt(awayScore.text()) !== matchForecast.LiveAwayScore) {

            homeScore.text(matchForecast.LiveHomeScore);
            awayScore.text(matchForecast.LiveAwayScore);
            forecastRenderer.highlightChanged(matchScore);
        }

        forecastRenderer.checkChangedAndHighlight(forecastTotal, matchForecast.ForecastTotal);
    },

    checkChangedAndHighlight: function (element, value, formatString) {
        if (_.isNumber(value)) {
            value = forecastRenderer.formatNumber(value, formatString);
        }

        if (element.text().trim() !== value.trim()) {
            element.text(value);
            forecastRenderer.highlightChanged(element);
        }
    },

    updateMatchesCached: function (matchForecast) {
        var matchLength = forecastRenderer.matches.length,
            isExist = false;

        for (var matchIndex = 0; matchIndex < matchLength; matchIndex++) {
            if (forecastRenderer.matches[matchIndex].Id === matchForecast.Id) {
                forecastRenderer.matches[matchIndex] = matchForecast;
                isExist = true;
                break;
            }
        }

        if (!isExist) {
            forecastRenderer.matches.push(matchForecast);
        }
    },

    trackChangedAndUpdatedValues: function (matchForecast) {
        var matchForecastElement = $('#match-' + matchForecast.Id);

        if (matchForecastElement.length > 0) {

            var cachedMatchForecast = _.find(forecastRenderer.matches, function (match) { return match.Id === matchForecast.Id; });

            if (cachedMatchForecast.MatchRowSpan !== matchForecast.MatchRowSpan) {
                return false;
            }

            forecastRenderer.trackMatchInfoChanged(matchForecastElement, matchForecast);

            for (var betTypeIndex = 0; betTypeIndex < matchForecast.BetTypeForecasts.length; betTypeIndex++) {
                var currentBetType = matchForecast.BetTypeForecasts[betTypeIndex];

                for (var betChoiceIndex = 0; betChoiceIndex < currentBetType.BetChoices.length; betChoiceIndex++) {
                    var currentBetChoice = currentBetType.BetChoices[betChoiceIndex],
                        choiceIdentify = currentBetType.BetTypeId + '-' + betChoiceIndex;;

                    forecastRenderer.checkChangedAndHighlight(
                        matchForecastElement.find('.handicap-value-' + choiceIdentify),
                        currentBetChoice.HandicapValue);
                    forecastRenderer.checkChangedAndHighlight(
                        matchForecastElement.find('.bet-count-' + choiceIdentify),
                        currentBetChoice.BetCount, '0,0');
                    forecastRenderer.checkChangedAndHighlight(
                        matchForecastElement.find('.turnover-' + choiceIdentify),
                        currentBetChoice.Turnover);

                    for (var winLossIndex = 0; winLossIndex < currentBetChoice.WinLossValues.length; winLossIndex++) {
                        if (currentBetChoice.WinLossColumnNames.length > 0) {
                            forecastRenderer.checkChangedAndHighlight(
                                matchForecastElement.find('.winloss-name-' + currentBetType.BetTypeId + '-' + winLossIndex),
                                forecastRenderer.combineWinLossName(currentBetChoice.WinLossColumnNames[winLossIndex], currentBetType.IsOverUnder));
                        }

                        forecastRenderer.checkChangedAndHighlight(
                            matchForecastElement.find('.winloss-value-' + choiceIdentify + '-' + winLossIndex),
                            currentBetChoice.WinLossValues[winLossIndex]);
                    }
                }
            }

            return true;
        }

        return false;

////        [5:23:55 PM] Lan Dang - Natalie: #7e7f80 grey
////[5:24:08 PM] Lan Dang - Natalie: #f2ac52
////[5:24:14 PM] Lan Dang - Natalie: nè Ricky
////[5:24:41 PM] Lan Dang - Natalie: number red #e40000
    }
};