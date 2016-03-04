namespace Alpha.TotalBetsForecast.Site.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
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
        private readonly IForecastService _forecastService;
        private readonly IEventService _eventService;

        public ForecastHandler(IForecastService forecastService, IEventService eventService)
        {
            _forecastService = forecastService;
            _eventService = eventService;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Reviewed!")]
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
                KickOffTimeString = match.KickOffTime.ToString("MM/dd/yyyy hh:mm tt"),
                LiveHomeScore = match.LiveHomeScore,
                LiveAwayScore = match.LiveAwayScore,
                BetTypeForecasts = Enumerable.Empty<BetTypeForecast>()
            };

            try
            {
                message.LiveHomeScore = match.LiveHomeScore;
                message.LiveAwayScore = match.LiveAwayScore;

                var matchForecast = _forecastService.GetMatchForecast(message, match.KickOffTime);

                viewModel.BetTypeForecasts = matchForecast.BetTypeForecasts.OrderBy(betType => betType.BetTypeId, new BetTypeComparer());
                viewModel.ForecastTotal = matchForecast.ForecastTotal;
                viewModel.MatchRowSpan = viewModel.BetTypeForecasts.Sum(betTypeForecast =>
                {
                    return betTypeForecast.BetChoices.Count()
                    + (betTypeForecast.BetChoices.First().WinLossColumnNames.Any() ? 1 : 0);
                });
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
                .GetMatches(message.LeagueId, message.IsLive)
                .OrderByDescending(match => match.KickOffTime);

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
                        Text = Resources.MatchOfferLive,
                        Value = "true",
                        Selected = message.IsLive
                    },
                    new SelectListItem
                    {
                        Text = Resources.Deadball,
                        Value = "false",
                        Selected = !message.IsLive
                    }
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