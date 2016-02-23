namespace Alpha.TotalBetsForecast.Site.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using App_GlobalResources;
    using Helpers;
    using MediatR;
    using Models;
    using Models.Requests;
    using Models.Views;
    using Services;
    using Sunplus.Agent.Utilities;
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

            var viewModel = new MatchForecastViewModel
            {
                MatchId = message.MatchId,
                LeagueName = league.Name,
                HomeName = match.HomeName,
                AwayName = match.AwayName,
                HasLive = match.HasLive,
                IsLive = message.IsLive,
                KickOffTime = match.KickOffTime,
                LiveHomeScore = match.LiveHomeScore,
                LiveAwayScore = match.LiveAwayScore,
                BetTypeForecasts = Enumerable.Empty<BetTypeForecast>()
            };

            try
            {
                var matchForecast = _forecastService.GetMatchForecast(message);
                viewModel.BetTypeForecasts = matchForecast.BetTypeForecasts.OrderBy(betType => betType.BetTypeId, new BetTypeComparer());
                viewModel.ForecastTotal = matchForecast.ForecastTotal;
            }
            catch (Exception ex)
            {
                Logs.FilterInfoWarnOrError(ex);
            }

            return viewModel;
        }

        public MatchesViewModel Handle(MatchesQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "message cannot be null!");
            }

            var matches = _eventService
                .GetMatches(new List<League> { new League { Id = message.LeagueId } }, message.IsLive)
                .OrderBy(match => match.Name);
            
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
                IsLive = message.IsLive,
                LiveValues = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = Resources.Deadball,
                        Value = "false",
                        Selected = !message.IsLive
                    },
                    new SelectListItem
                    {
                        Text = Resources.Live,
                        Value = "true",
                        Selected = message.IsLive
                    },
                },
                Leagues = leagues.Select(league =>
                {
                    return new SelectListItem
                    {
                        Text = league.Name,
                        Value = league.Id.ToString(),
                        Selected = message.LeagueId == league.Id
                    };
                }).OrderBy(selectItem => selectItem.Text)
            };
        }
    }
}