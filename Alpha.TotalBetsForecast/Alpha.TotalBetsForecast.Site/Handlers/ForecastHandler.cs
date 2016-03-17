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
        IRequestHandler<MatchForecastQuery, LeagueForecastViewModel>,
        IRequestHandler<MatchesQuery, IEnumerable<Match>>,
        IRequestHandler<LeaguesQuery, IEnumerable<League>>,
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
        public LeagueForecastViewModel Handle(MatchForecastQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "message cannot be null!");
            }

            var viewModel = new LeagueForecastViewModel
            {
                LeagueName = (new League { Id = message.LeagueId }).Name,
            };

            try
            {
                var matchForecasts = _forecastService.GetMatchForecasts(message);

                foreach (var matchForecast in matchForecasts)
                {

                }

                viewModel.BetTypeForecasts = matchForecast.BetTypeForecasts.OrderBy(betType => betType.BetTypeId, new BetTypeComparer());
                viewModel.ForecastTotal = matchForecast.ForecastTotal;
                viewModel.MatchRowSpan = viewModel.BetTypeForecasts.Sum(betTypeForecast =>
                {
                    return betTypeForecast.BetChoices.Count() + (betTypeForecast.BetChoices.First().WinLossColumnNames.Any() ? 1 : 0);
                });
            }
            catch (Exception ex)
            {
                Logs.FilterInfoWarnOrError(ex);
            }

            return viewModel;
        }

        public IEnumerable<Match> Handle(MatchesQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "message cannot be null!");
            }

            var matches = _eventService
                .GetMatches(message.LeagueId, message.IsLive)
                .OrderByDescending(match => match.KickOffTime);

            return matches;
        }

        public IndexViewModel Handle(IndexQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "message cannot be null!");
            }

            return new IndexViewModel
            {
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
                }
            };
        }

        public IEnumerable<League> Handle(LeaguesQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "message cannot be null!");
            }

            var leagues = _eventService.GetLeagues(message.IsLive);

            return leagues.OrderBy(league => league.Name);
        }
    }
}