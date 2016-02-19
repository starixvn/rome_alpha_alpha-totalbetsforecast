namespace Alpha.TotalBetsForecast.Site.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Helpers;
    using MediatR;
    using Models;
    using Models.Requests;
    using Models.Views;
    using Services;

    public class ForecastHandler :
        IRequestHandler<MatchForecastQuery, MatchForecastViewModel>,
        IRequestHandler<MatchesQuery, MatchesViewModel>,
        IRequestHandler<IndexQuery, IndexViewModel>
    {
        private IForecastService _forecastService;
        private IEventService _eventService;

        public ForecastHandler(IForecastService forecastService, IEventService eventService)
        {
            _forecastService = forecastService;
            _eventService = eventService;
        }

        public MatchForecastViewModel Handle(MatchForecastQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "message cannot be null!");
            }

            var match = _eventService.GetMatchInfo(message.IsLive, message.MatchId);

            if (match == null)
            {
                throw new ArgumentNullException(nameof(match), "match cannot be null!");
            }

            var league = new League
            {
                Id = match.LeagueId
            };

            var matchForecast = _forecastService.GetMatchForecast(message);

            return new MatchForecastViewModel
            {
                MatchId = match.Id,
                LeagueName = league.Name,
                HomeName = match.HomeName,
                AwayName = match.AwayName,
                IsLive = match.IsLive,
                KickOffTime = match.KickOffTime,
                LiveHomeScore = match.LiveHomeScore,
                LiveAwayScore = match.LiveAwayScore,
                BetTypeForecasts = matchForecast.BetTypeForecasts.OrderBy(betType => betType.BetTypeId, new BetTypeComparer()),
                ForecastTotal = matchForecast.ForecastTotal
            };
        }

        public MatchesViewModel Handle(MatchesQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "message cannot be null!");
            }

            var matches = _eventService.GetMatches(new List<League> { new League { Id = message.LeagueId } }, message.IsLive);
            
            return new MatchesViewModel
            {
                LeagueId = message.LeagueId,
                MatchId = message.MatchId,
                Matches = matches
            };
        }

        public IndexViewModel Handle(IndexQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "message cannot be null!");
            }

            var leagues = _eventService.GetLeagues(message.IsLive);

            return new IndexViewModel
            {
                LeagueId = message.LeagueId,
                Leagues = leagues.Select(league =>
                {
                    return new SelectListItem
                    {
                        Text = league.Name,
                        Value = league.Id.ToString(),
                        Selected = message.LeagueId == league.Id
                    };
                })
            };
        }
    }
}