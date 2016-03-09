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
        noDataText: 'noDataText',
        goal: 'Goal'
    },

    init: function () {
        var mainContent = $('#mainContent');

        forecastRenderer.resources.noDataText = mainContent.data('no-data-text');
        forecastRenderer.resources.mathcOfferLive = mainContent.data('offer-live');
        forecastRenderer.resources.goal = mainContent.data('goal');
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

    combineWinLossName: function (winlossName, isOverUnder) {
        return isOverUnder ? (winlossName + ' ' + forecastRenderer.resources.goal) : winlossName
    },
    winLossColumnNameTemplate: _.template('<td class="win-loss-name text-center ex-col head">\
                                            <span class="winloss-name-<%= betTypeId %>-<%= winLossIndex %>"><%= winLossName %></span>\
                                        </td>'),
    renderWinLossColumnName: function (betChoiceForecast, betTypeForecast) {
        var html = [];
        if (betChoiceForecast.WinLossColumnNames.length > 0) {

            html.push('<td class="ex-col">&nbsp;</td> \
                    <td class="ex-col">&nbsp;</td> \
                    <td class="ex-col">&nbsp;</td>');

            for (var i = 0; i < betChoiceForecast.WinLossColumnNames.length; i++) {
                var winlossName = betChoiceForecast.WinLossColumnNames[i];

                html.push(forecastRenderer.winLossColumnNameTemplate({
                    betTypeId: betTypeForecast.BetTypeId,
                    winLossIndex: i,
                    winLossName: forecastRenderer.combineWinLossName(winlossName, betTypeForecast.IsOverUnder)
                }));
            }
        }

        return html.join('');
    },

    renderWinLossNumber: function (winLoss, index) {
        return winLoss > 0
                ? _.template('<span class="positive winloss-value-<%= index %>"><%= winLoss %></span>')({ winLoss: forecastRenderer.formatNumber(winLoss), index: index })
                : _.template('<span class="winloss-value-<%= index %>"><%= winLoss %></span>')({ winLoss: forecastRenderer.formatNumber(winLoss), index: index });
    },

    betCountTemplate: _.template('<td class="ex-col text-right"><span class="bet-count-<%= choiceIdentify %>"><%= betCount %></span></td>'),
    turnoverTemplate: _.template('<td class="ex-col text-right"><span class="turnover-<%= choiceIdentify %>"><%= turnover %></span></td>'),
    winLossValueTemplate: _.template('<td class="ex-col <%= textAlign %>" <%= rowSpan %> >\
                                        <%= winLossValue %> \
                                    </td>'),
    renderBetChoiceForecast: function (betChoiceForecast, isOneXTwoBetType, choiceIdentify) {
        var html = [];
        html.push(forecastRenderer.betCountTemplate(
            {
                choiceIdentify: choiceIdentify,
                betCount: forecastRenderer.formatNumber(betChoiceForecast.BetCount, '0,0')
            }));

        html.push(forecastRenderer.turnoverTemplate(
            {
                choiceIdentify: choiceIdentify,
                turnover: forecastRenderer.formatNumber(betChoiceForecast.Turnover)
            }));

        for (var i = 0; i < betChoiceForecast.WinLossValues.length; i++) {
            var winLoss = betChoiceForecast.WinLossValues[i];

            html.push(forecastRenderer.winLossValueTemplate({
                textAlign: isOneXTwoBetType ? 'text-center' : 'text-right',
                rowSpan: isOneXTwoBetType ? 'colspan="3"' : '',
                winLossValue: forecastRenderer.renderWinLossNumber(winLoss, choiceIdentify + '-' + i)
            }));
        }

        return html.join('');
    },

    betTypeRowTemplate: _.template('<tr class="<%= even %> <%= lastRow %>" >'),
    handicapChoiceTemplate: _.template('<td class="ex-col">\
                                            <span class="pull-left"><%= betTypeName %></span>\
                                            <span class="pull-right ex-text text-right handicap-value-<%= choiceIdentify %>"><%= handicapValue %></span>\
                                        </td>'),
    overChoiceTemplate: _.template('<td class="ex-col"> \
                                        <span class="pull-left"><%= betTypeName %></span> \
                                        <span class="pull-right ex-text text-right handicap-value-<%= choiceIdentify %>"><%= handicapValue %></span> \
                                    </td>'),
    underChoiceTemplate: _.template('<td class="ex-col"> \
                                        <span class="pull-right ex-text text-right handicap-value-<%= choiceIdentify %>"><%= handicapValue %></span> \
                                    </td>'),
    oneXTwoMiddleChoiceTemplate: _.template('<td class="ex-col"> \
                                                <span class="pull-left"><%= betTypeName %></span> \
                                                <span class="pull-right ex-text text-right handicap-value-<%= choiceIdentify %>"><%= handicapValue %></span> \
                                            </td>'),
    oneXTwoNotMiddleChoiceTemplate: _.template('<td class="ex-col"> \
                                                    <span class="pull-right ex-text text-right handicap-value-<%= choiceIdentify %>"><%= handicapValue %></span> \
                                                </td>'),
    renderBetTypeForecast: function (betTypeForecast, isEven, firstRowWithoutWinLossName, forecastTotalHtml) {
        var html = [],
            totalBetChoice = betTypeForecast.BetChoices.length,
            betTypeId = betTypeForecast.BetTypeId;

        for (var i = 0; i < totalBetChoice; i++) {
            html.push(forecastRenderer.betTypeRowTemplate({
                even: isEven ? 'even ' : '',
                lastRow: i == totalBetChoice - 1 ? 'last-row' : ''
            }));

            if (i === 0
                && betTypeForecast.IsOneXTwoBetType
                && firstRowWithoutWinLossName)
            {
                html.push(firstRowWithoutWinLossName);
            }

            var betChoice = betTypeForecast.BetChoices[i],
                choiceIdentify = betTypeId + '-' + i,
                betChoiceTemplateData =
                {
                    betTypeName: betTypeForecast.BetTypeName,
                    choiceIdentify: choiceIdentify,
                    handicapValue: betChoice.HandicapValue
                };

            if (betTypeForecast.IsHandicap) {
                i == 0
                    ? html.push(forecastRenderer.handicapChoiceTemplate(betChoiceTemplateData))
                    : html.push('<td class="ex-col">&nbsp;</td>');
            }


            if (betTypeForecast.IsOverUnder) {
                i == 0
                    ? html.push(forecastRenderer.overChoiceTemplate(betChoiceTemplateData))
                    : html.push(forecastRenderer.underChoiceTemplate(betChoiceTemplateData))
            }


            if (betTypeForecast.IsOneXTwoBetType) {
                i == 1
                   ? html.push(forecastRenderer.oneXTwoMiddleChoiceTemplate(betChoiceTemplateData))
                   : html.push(forecastRenderer.oneXTwoNotMiddleChoiceTemplate(betChoiceTemplateData));
            }

            html.push(forecastRenderer.renderBetChoiceForecast(betChoice, betTypeForecast.IsOneXTwoBetType, choiceIdentify));

            if (i === 0
                && betTypeForecast.IsOneXTwoBetType
                && forecastTotalHtml) {
                html.push(forecastTotalHtml);
            }

            html.push('</tr>');
        }

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
                    <tr class="<%= evenClass %>"> \
                        <%= matchInfoHtml %>\
                        <%= firstWinLossColumnNameHtml %>\
                        <%= forecastTotalHtml %>\
                    </tr><%= firstBetTypeForecastHtml %>'),
    forecastTotalTemplate: _.template('<td rowspan="<%= matchRowSpan %>" class="text-center col-forecastTotal"><span class="forecast-total-value"><%= forecastTotal %></span></td>'),
    renderFirstBetTypeRow: function (matchForecast) {
        var matchInfoHtml = forecastRenderer.renderMatchInfo(matchForecast),
            forecastTotalHtml = forecastRenderer.forecastTotalTemplate({
                matchRowSpan: matchForecast.MatchRowSpan,
                forecastTotal: forecastRenderer.formatNumber(matchForecast.ForecastTotal),
            }),
            firstBetType = matchForecast.BetTypeForecasts[0],
            firstBetTypeTemplateData = {
            matchRowSpan: matchForecast.MatchRowSpan,
            matchId: matchForecast.MatchId,
            matchLive: matchForecast.IsLive ? 'match-live' : '',
            matchInfoHtml: firstBetType.IsOneXTwoBetType ? '' : matchInfoHtml,
            forecastTotalHtml: firstBetType.IsOneXTwoBetType ? '' : forecastTotalHtml,
            firstWinLossColumnNameHtml: forecastRenderer.renderWinLossColumnName(firstBetType.BetChoices[0], firstBetType),
            firstBetTypeForecastHtml: forecastRenderer.renderBetTypeForecast(firstBetType, true, matchInfoHtml, forecastTotalHtml),
            evenClass: firstBetType.IsOneXTwoBetType ? '' : 'even'
        };

        return forecastRenderer.firstBetTypeTemplate(firstBetTypeTemplateData);
    },

    firstWinLossColumnNameTemplate: _.template('<tr class="<%= even %>"><%= winLossColumnNameHtml %></tr>'),
    renderMatchForecast: function (matchForecast) {
        if (matchForecast.BetTypeForecasts.length === 0) {
            return forecastRenderer.renderNoInfo(matchForecast.MatchId);
        }

        var html = [];

        html.push(forecastRenderer.renderFirstBetTypeRow(matchForecast));

        for (var i = 1; i < matchForecast.BetTypeForecasts.length; i++) {
            var currentBetTypeForecast = matchForecast.BetTypeForecasts[i],
                isEven = i % 2 === 0,
                firstBetChoice = currentBetTypeForecast.BetChoices[0];

            if (firstBetChoice.WinLossColumnNames.length > 0) {
                html.push(forecastRenderer.firstWinLossColumnNameTemplate({
                    even: isEven ? "even" : '',
                    winLossColumnNameHtml: forecastRenderer.renderWinLossColumnName(firstBetChoice, currentBetTypeForecast)
                }));
            }

            html.push(forecastRenderer.renderBetTypeForecast(currentBetTypeForecast, isEven));
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

    checkChangedAndHighlight: function(element, value, formatString)
    {
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
            if (forecastRenderer.matches[matchIndex].MatchId === matchForecast.MatchId) {
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
        var matchForecastElement = $('#match-' + matchForecast.MatchId);

        if (matchForecastElement.length > 0) {

            var cachedMatchForecast = _.find(forecastRenderer.matches, function (match) { return match.MatchId === matchForecast.MatchId; });

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
    }
};