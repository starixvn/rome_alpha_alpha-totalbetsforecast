var matchForecastRenderer =
{
    matches: [],

    resources:
    {
        mathcOfferLive: 'mathcOfferLive',
        noDataText: 'noDataText',
        goal: 'Goal'
    },

    renderNoInfo: function (matchId) {
        return '<tbody id="no-info-body" data-match-id="match-' + matchId + '"> \
                    <tr class="no-info text-center"> \
                        <td colspan="9">' + matchForecast.resources.noDataText + '</td> \
                    </tr> \
                </tbody>';
    },

    renderWinLossColumnName: function (betChoiceForecast, isOverUnder) {
        var html = [];
        if (betChoiceForecast.WinLossColumnNames.length > 0) {

            html.push('<td class="ex-col">&nbsp;</td> \
                    <td class="ex-col">&nbsp;</td> \
                    <td class="ex-col">&nbsp;</td>');

            for (var i = 0; i < betChoiceForecast.WinLossColumnNames.length; i++) {
                var winlossName = betChoiceForecast.WinLossColumnNames[i];

                html.push('<td class="win-loss-name text-center ex-col head">'
                    + (isOverUnder ? (winlossName + ' ' + matchForecastRenderer.resources.goal) : winlossName)
                    + '</td>');
            }
        }

        return html.join('');
    },

    renderWinLossNumber: function (winLoss) {
        return winLoss > 0
                ? '<span class="positive">' + winLoss + '</span>'
                : '<span>' + winLoss + '</span>';
    },

    renderBetChoiceForecast: function (betChoiceForecast, isOneXTwoBetType) {
        var html = [];
        html.push('<td class="ex-col text-right">' + betChoiceForecast.BetCount + '</td>');
        html.push('<td class="ex-col text-right">' + betChoiceForecast.Turnover + '</td>');

        for (var i = 0; i < betChoiceForecast.WinLossValues.length; i++) {
            var winLoss = betChoiceForecast.WinLossValues[i];

            html.push('<td class="ex-col ' + (isOneXTwoBetType ? 'text-center' : 'text-right' ) + '"\
                        ' + (isOneXTwoBetType ? 'colspan="3"' : '') + '>'
                        + matchForecastRenderer.renderWinLossNumber(winLoss) +
                    '</td>');
        }
        
        return html.join('');
    },

    renderBetTypeForecast: function (betTypeForecast, isEven) {
        var html = [];
        var totalBetChoice = betTypeForecast.BetChoices.length;

        for (var i = 0; i < totalBetChoice; i++) {
            html.push('<tr class="' + (isEven ? 'even ' : '')
                + (i == totalBetChoice - 1 ? 'last-row' : '')
                + '">');

            var betChoice = betTypeForecast.BetChoices[i];
            if (betTypeForecast.IsHandicap) {
                i == 0
                    ? html.push('<td class="ex-col"> \
                                    <span class="pull-left">' + betTypeForecast.BetTypeName + '</span> \
                                    <span class="pull-right ex-text">' + betChoice.HandicapValue + '</span> \
                                </td>')
                    : html.push('<td class="ex-col">&nbsp;</td>');

            }


            if (betTypeForecast.IsOverUnder) {
                i == 0
                    ? html.push('<td class="ex-col"> \
                                    <span class="pull-left">' + betTypeForecast.BetTypeName + '</span> \
                                    <span class="pull-right ex-text">' + betChoice.HandicapValue + '</span> \
                                </td>')
                    : html.push('<td class="ex-col"> \
                                    <span class="pull-right ex-text">' + betChoice.HandicapValue + '</span> \
                                </td>')
            }


            if (betTypeForecast.IsOneXTwoBetType) {
                i == 1
                   ? html.push('<td class="ex-col"> \
                                <span class="pull-left">' + betTypeForecast.BetTypeName + '</span> \
                                <span class="pull-right ex-text">' + betChoice.HandicapValue + '</span> \
                            </td>')
                   : html.push('<td class="ex-col"> \
                                <span class="pull-right ex-text">' + betChoice.HandicapValue + '</span> \
                            </td>');
            }

            html.push(matchForecastRenderer.renderBetChoiceForecast(betChoice, betTypeForecast.IsOneXTwoBetType));
            html.push('</tr>');
        }

        return html.join('');
    },

    renderMatchInfo: function (matchForecast) {

        var html = [];
        html.push('<td rowspan="' + matchForecast.MatchRowSpan + '" class="col-time text-center">');

        if (matchForecast.IsLive) {
            html.push('<span class="match-score">'
                + matchForecast.LiveHomeScore
                + '-'
                + matchForecast.LiveAwayScore
                + '</span> <br />');

            html.push(matchForecast.HasLive
                ? ('<span class="offer-live">'
                    + matchForecastRenderer.resources.mathcOfferLive
                    + '</span>')
                : '');
        }
        else {
            html.push(matchForecast.HasLive ? '<span class="offer-live">' + matchForecastRenderer.resources.mathcOfferLive + '</span> <br />' : '');
            html.push('<span class="match-time">' + matchForecast.KickOffDateString + ' <br />' + matchForecast.KickOffTimeString + '</span>');
        }

        html.push('</td><td rowspan="' + matchForecast.MatchRowSpan + '" class="col-event"> \
            <span class="home-name">' + matchForecast.HomeName + '</span> <br /> \
            <span class="away-name">' + matchForecast.AwayName + '</span> \
        </td>');

        return html.join('');
    },

    renderFirstBetTypeRow: function (matchForecast) {
        var html = [];

        html.push('<tbody id="match-' + matchForecast.MatchId + '" class="' + (matchForecast.IsLive ? "match-live" : "") + '"><tr class="even">');

        html.push(matchForecastRenderer.renderMatchInfo(matchForecast));

        html.push(matchForecastRenderer.renderWinLossColumnName(
            matchForecast.BetTypeForecasts[0].BetChoices[0],
            matchForecast.BetTypeForecasts[0].IsOverUnder));

        html.push('<td rowspan="' + matchForecast.MatchRowSpan + '" class="text-center col-forecastTotal">' + matchForecast.ForecastTotal + '</td></tr>');

        html.push(matchForecastRenderer.renderBetTypeForecast(matchForecast.BetTypeForecasts[0], true));

        return html.join('');
    },

    renderMatchForecast: function (matchForecast) {
        var html = [];

        html.push(matchForecastRenderer.renderFirstBetTypeRow(matchForecast));

        for (var i = 1; i < matchForecast.BetTypeForecasts.length; i++) {
            var currentBetTypeForecast = matchForecast.BetTypeForecasts[i],
                isEven = i % 2 === 0,
                firstBetChoice = currentBetTypeForecast.BetChoices[0];

            if (firstBetChoice.WinLossColumnNames.length > 0) {
                html.push('<tr class="' + (isEven ? "even" : '') + '">'
                    + matchForecastRenderer.renderWinLossColumnName(firstBetChoice, currentBetTypeForecast.IsOverUnder)
                    + '</tr>');
            }

            html.push(matchForecastRenderer.renderBetTypeForecast(currentBetTypeForecast, isEven));
        }

        html.push('</tbody>');

        return html.join('');
    },

    trackChanged: function(matchForecast)
    {

    }
};