var betType = 
    {
        handicap: 1,
        handicap1H: 7,
        overUnder: 3,
        overUnder1H: 8,
        oneXTwo: 11,
        oneXTwo1H: 12
    }

var ForecastWinLoss = React.createClass({
    render: function () {
        var rows = [];
        if (this.props.betTypeForecast.betTypeId === betType.oneXTwo
            || this.props.betTypeForecast.betTypeId === betType.oneXTwo1H) {

        }
        else {

        }

        return (
            <div></div>
            );
    }
});