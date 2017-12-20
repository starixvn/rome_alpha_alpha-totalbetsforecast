namespace Alpha.TotalBetsForecast.Models
{
    using System;
    using Constants;
    using RemotingRef;

    public class BullfightingModel
    {
        public BullfightingModel(
            DateTime eventDate,
            int matchId,
            int leagueId,
            string leagueCode,
            bool hasLive,
            int homeId,
            int awayId,
            int betCount,
            decimal turnover,
            decimal banker,
            decimal player)
        {
            EventDate = eventDate;
            MatchId = matchId;
            LeagueId = leagueId;
            LeagueCode = leagueCode;
            HasLive = hasLive;
            HomeId = homeId;
            AwayId = awayId;
            BetCount = betCount;
            Turnover = turnover;
            Banker = banker;
            Player = player;
        }

        public DateTime EventDate { get; }

        public string EventDateToString => EventDate.ToString("MM/dd");

        public int MatchId { get; }

        public int LeagueId { get; }

        public string LeagueCode { get; }

        public bool HasLive { get; }        

        public int HomeId { get; }

        public string HomeName => CacheRefFactory.CacheRef.GetTeamName3rd(HomeId);

        public int AwayId { get; }

        public string AwayName => CacheRefFactory.CacheRef.GetTeamName3rd(AwayId);

        public int BetCount { get; }

        public decimal Turnover { get; }

        public string TurnoverToDislay => Turnover.ToString(NumberFormat.DecimalFormat);

        public decimal Banker { get; }

        public string BankerToDisplay => Banker.ToString(NumberFormat.DecimalFormat);

        public decimal Player { get; }

        public string PlayerToDisplay => Player.ToString(NumberFormat.DecimalFormat);
    }
}
