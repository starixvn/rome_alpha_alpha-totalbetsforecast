namespace Alpha.TotalBetsForecast.Site.Tests.Helpers
{
    using NUnit.Framework;
    using Site.Helpers;
    using Sunplus.Agent.Model.BL.Definitions;

    [TestFixture]
    public class BetTypeComparerTests
    {
        private BetTypeComparer _betTypeCompare;

        [TestCase(BetType.Handicap, BetType._1st_Handicap, false)]
        [TestCase(BetType.Handicap, BetType.Over_Under, false)]
        [TestCase(BetType.Handicap, BetType._1st_Over_Under, false)]
        [TestCase(BetType.Handicap, BetType.FT_1X2, false)]
        [TestCase(BetType.Handicap, BetType.FirstHalf_1X2, false)]
        [TestCase(BetType._1st_Handicap, BetType.Over_Under, false)]
        [TestCase(BetType._1st_Handicap, BetType._1st_Over_Under, false)]
        [TestCase(BetType._1st_Handicap, BetType.FT_1X2, false)]
        [TestCase(BetType._1st_Handicap, BetType.FirstHalf_1X2, false)]
        [TestCase(BetType.Over_Under, BetType._1st_Over_Under, false)]
        [TestCase(BetType.Over_Under, BetType.FT_1X2, false)]
        [TestCase(BetType.Over_Under, BetType.FirstHalf_1X2, false)]
        [TestCase(BetType._1st_Over_Under, BetType.FT_1X2, false)]
        [TestCase(BetType._1st_Over_Under, BetType.FirstHalf_1X2, false)]
        [TestCase(BetType.FT_1X2, BetType.FirstHalf_1X2, false)]
        [TestCase(BetType.FirstHalf_1X2, BetType.FT_1X2, true)]
        [TestCase(BetType.FirstHalf_1X2, BetType.FirstHalf_1X2, false)]
        public void Compare_ReturnOrder(int firstBetType, int secondBetType, bool isSwitch)
        {
            //// Arrange
            _betTypeCompare = new BetTypeComparer();

            //// Act
            var isActualSwitch = _betTypeCompare.Compare(firstBetType, secondBetType) > 0;

            //// Assert
            Assert.AreEqual(isSwitch, isActualSwitch);
        }
    }
}