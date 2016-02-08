namespace Alpha.TotalBetsForecast.Site.Handlers
{
    using System;
    using System.Linq;
    using MediatR;
    using Models;
    using Models.Requests;
    using Models.Views;
    using Services;

    public class ForecastHandler :
        IRequestHandler<MatchForecastQuery, MatchForecast>,
        IRequestHandler<MatchesAndLeaguesQuery, MatchesAndLeaguesViewModel>
    {
        private IForecastService _forecastService;
        private IEventService _eventService;

        public ForecastHandler(IForecastService forecastService, IEventService eventService)
        {
            _forecastService = forecastService;
            _eventService = eventService;
        }

        public MatchForecast Handle(MatchForecastQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "message cannot be null!");
            }

            var match = _eventService.GetMatchInfo(message.IsLive, message.MatchId);
            var league = new League
            {
                Id = match.LeagueId
            };

            var matchForecast = _forecastService.GetMatchForecast(message);
            matchForecast.Match = match;
            matchForecast.League = league;

            return matchForecast;
        }

        public MatchesAndLeaguesViewModel Handle(MatchesAndLeaguesQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "message cannot be null!");
            }

            var matches = _eventService.GetMatches(Enumerable.Empty<League>(), message.IsLive);
            var leagues = _eventService.GetLeagues(message.IsLive);

            return new MatchesAndLeaguesViewModel
            {
                LeagueId = message.LeagueId,
                MatchId = message.MatchId,
                Matches = matches,
                Leagues = leagues
            };
        }
    }
}