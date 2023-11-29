using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Alpha.TotalBetsForecast.Site.Models
{
    public enum BetType
    {
        None = 0,
        Handicap = 1,
        Odd_Even = 2,
        Over_Under = 3,
        Correct_Score = 4,
        FT_1X2 = 5,
        Total_Goal = 6,
        _1st_Handicap = 7,
        _1st_Over_Under = 8,
        Mix_Parlay = 9,
        Outright = 10,
        Total_Corners = 11,
        _1st_Odd_Even = 12,
        Clean_Sheet = 13,
        First_Goal_Last_Goal = 14,
        FirstHalf_1X2 = 15,
        HT_FT = 16,
        _2nd_HDP = 17,
        _2nd_Over_Under = 18,
        Substitutes = 19,
        Money_Line = 20,
        _1st_Money_Line = 21,
        Next_Goal = 22,
        Next_Corner = 23,
        Double_Change = 24,
        Draw_No_Bet = 25,
        Both_One_Neither = 26,
        To_Win_To_Nill = 27,
        _3_Way_Handicap = 28,
        Combine_Mix_Parlay = 29,
        Win = 31,
        Place = 32,
        Win_Place = 33,
        Win_UK_Tote = 41,
        Place_UK_Tote = 42,
        Win_Place_UK_Tote = 43,
        _5_min_O_U = 51,
        _15_min_O_U = 52,
        _30_min_O_U = 53,
        _45_min_O_U = 54,
        _60_min_O_U = 55,
        End_of_day_O_U = 56,
        _5_min_OE = 61,
        _15_min_OE = 62,
        _30_min_OE = 63,
        _45_min_OE = 64,
        _60_min_OE = 65,
        End_of_day_OE = 66,
        Casino_Games = 71,
        Home_No_Bet = 121,
        Away_No_Bet = 122,
        Draw_NoDraw = 123,

        // Number Game.
        _1st_Ball_OU = 81,
        Last_Ball_OU = 82,
        _1st_Ball_OE = 83,
        Last_Ball_OE = 84,
        N_Over_Under = 85,
        N_Odd_Even = 86,
        Next_High_Low = 87,
        Warrior = 88,
        Next_Combo = 89,
        NumberWheel = 90,

        // Virtual Soccer.
        VS_Handicap = 1201,
        VS_OverUnder = 1203,
        VS_CorrectScore = 1204,
        VS_1X2 = 1205,
        VS_TotalGoal = 1206,
        VS_DoubleChange = 1224,

        // Virtual Horses Racing.
        VHR_Win = 1231,
        VHR_Place = 1232,
        VHR_WinPlace = 1233,
        VHR_Forecast = 1237,
        VHR_Tricast = 1238,

        // Virtual Tennis
        VT_PlayerWin = 1220,
        VT_ScoreBet = 1235,
        VT_TotalPoints = 1236,

        // Sporting Solution.
        MatchWinner = 1301,
        MatchCorrectScore = 1302,
        SetHandicap = 1303,
        TotalSets = 1304,
        MatchTotalGamesOddEven = 1305,
        MatchTotalGames2Way = 1306,
        MatchTotalGames3Way = 1307,
        MatchHandicapGames2Way = 1308,
        MatchHandicapGames3Way = 1309,
        PlayerXMatchTotalGames2Way = 1310,
        SetXWinner = 1311,
        SetXTotalGames2Way = 1312,
        SetXTotalGames3Way = 1313,
        SetXWillThereBeATiebreak = 1314,
        SetXPlayerToWinOnATiebreak = 1315,
        SetXGameHandicap = 1316,
        SetXCorrectScore = 1317,
        SetXTotalGamesOddEven = 1318,
        SetXRaceToYGames = 1319,
        SetXScoreAfter2Games = 1320,
        SetXScoreAfter4Games = 1321,
        SetXScoreAfter6Games = 1322,
        SetXTotalDeuceGame2Way = 1323,
        SetXGameYWinner = 1324,
        SetXGameYCorrectScore = 1325,
        SetXGameYPointHandicap = 1326,
        SetXGameYDeuceBetting = 1327,
        SetXGameYTotalPointsExact = 1328,
        SetXGameYTotalPoints2Way = 1329,
        SetXGameYTotalPoints3Way = 1330,
        SetXGameYTotalPointsOddEven = 1331,
        SetXGameYTotalPointZWinner = 1332,
        SetXGameYAndGameYAdd1Winner = 1333,
        SetXGameYAndGameYAdd1Points = 1334,

        // Max 19000 for Betting.
        Others = 19001,

        // 19001-32767 for Transfer
        FirstCorrectScore = 30,
        _1st_Total_Goal = 126,
        _1st_First_Goal_Last_Goal = 127,

        // AllBet.
        AllBetRangeStart = 2401,
        AllBetRangeEnd = 2499,

        // Voidbridge.
        VoidbridgeRangeStart = 4001,
        VoidbridgeRangeEnd = 7999,

        // Fishing World
        FishingWorldStart = 1011,
        FishingWorldEnd = 1020,

        Mix_Parlay_BetRadarVirtualSports = 2799,

        // SA Gaming.
        SaGamingRangeStart = 3001,
        SaGamingRangeEnd = 3099,
    }
}