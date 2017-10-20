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
        IRequestHandler<MatchForecastsQuery, IEnumerable<MatchForecastViewModel>>,
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
        public IEnumerable<MatchForecastViewModel> Handle(MatchForecastsQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "message cannot be null!");
            }

            var matchForecastViewModels = new List<MatchForecastViewModel>();

            try
            {
                var matchForecasts = _forecastService.GetMatchForecasts(message);

                foreach (var matchForecast in matchForecasts)
                {
                    var showTimeString = string.Empty;
                    if (!string.IsNullOrWhiteSpace(matchForecast.ShowTime))
                    {
                        showTimeString = BuildShowTime(matchForecast);
                    }

                    var matchForecastViewModel = new MatchForecastViewModel
                    {
                        Id = matchForecast.Id,
                        HomeName = matchForecast.HomeName,
                        AwayName = matchForecast.AwayName,
                        LiveHomeScore = matchForecast.LiveHomeScore,
                        LiveAwayScore = matchForecast.LiveAwayScore,
                        IsLive = message.IsLive,
                        HasLive = matchForecast.HasLive,
                        ShowTime = showTimeString,
                        KickOffTime = matchForecast.KickOffTime,
                        BetTypeForecasts = matchForecast.BetTypeForecasts,
                        TotalBetCount = matchForecast.TotalBetCount,
                        TotalTurnover = matchForecast.TotalTurnover,
                        TotalForecastWinLoss = matchForecast.TotalForecastWinLoss,
                        MatchRowSpan = (matchForecast.BetTypeForecasts.Count() * 2) + 1
                    };

                    matchForecastViewModels.Add(matchForecastViewModel);
                }
            }
            catch (Exception ex)
            {
                Logs.FilterInfoWarnOrError(ex);
            }

            return matchForecastViewModels;
        }

        private static string BuildShowTime(MatchForecast matchForecast)
        {
            var showTimeStrings = matchForecast.ShowTime.Trim().Split(' ');
            var totalShowTimeStrings = showTimeStrings.Count();

            if (totalShowTimeStrings > 2)
            {
                return string.Join(" ", showTimeStrings.Take(totalShowTimeStrings - 1))
                    + "<br />"
                    + showTimeStrings.Last();
            }

            return string.Join("<br />", showTimeStrings);
        }

        public IEnumerable<Match> Handle(MatchesQuery message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message), "message cannot be null!");
            }

            var matches = _eventService
                .GetMatches(message.LeagueId, message.IsLive, message.IsLocalTest)
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
                        Text = Resources.NonLive,
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

            var leagues = _eventService.GetLeagues(message.IsLive, message.IsLocalTest);

            return leagues.OrderBy(league => league.Name);
        }
    }
}