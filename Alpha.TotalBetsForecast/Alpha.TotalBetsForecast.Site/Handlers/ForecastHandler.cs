namespace Alpha.TotalBetsForecast.Site.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using App_GlobalResources;
    using MediatR;
    using Models;
    using Models.Requests;
    using Models.Views;
    using Services;
    using Sunplus.Agent.Utilities;

    public class ForecastHandler :
        IRequestHandler<MatchForecastsQuery, MatchForecastsViewModel>,
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
        public MatchForecastsViewModel Handle(MatchForecastsQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "message cannot be null!");
            }

            var viewModel = new MatchForecastsViewModel
            {
                LeagueName = (new League { Id = message.LeagueId }).Name,
            };

            try
            {
                var matchForecasts = _forecastService.GetMatchForecasts(message);
                var matchForecastViewModels = new List<MatchForecastViewModel>();

                foreach (var matchForecast in matchForecasts)
                {

                    var matchForecastViewModel = new MatchForecastViewModel
                    {
                        Id = matchForecast.Id,
                        HomeName = matchForecast.HomeName,
                        AwayName = matchForecast.AwayName,
                        LiveHomeScore = matchForecast.LiveHomeScore,
                        LiveAwayScore = matchForecast.LiveAwayScore,
                        IsLive = message.IsLive,
                        HasLive = matchForecast.HasLive,
                        ShowTime = matchForecast.ShowTime,
                        KickOffTime = matchForecast.KickOffTime,
                        BetTypeForecasts = matchForecast.BetTypeForecasts,
                        TotalBetCount = matchForecast.TotalBetCount,
                        TotalTurnover = matchForecast.TotalTurnover,
                        TotalForecastWinLoss = matchForecast.TotalForecastWinLoss,
                        MatchRowSpan = matchForecast.BetTypeForecasts.Count() * 2  //+ (matchForecast.MatchInfo.IsLive ? 1 : 0)
                    };

                    matchForecastViewModels.Add(matchForecastViewModel);
                }

                viewModel.MatchForecasts = matchForecastViewModels;
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