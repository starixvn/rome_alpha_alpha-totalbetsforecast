namespace Alpha.TotalBetsForecast.Site.Tests.Helpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Site.Helpers;

    public class BetChoiceComparerTests
    {
        private BetChoiceComparer _betChoiceCompare;

        [TestCase("h", "a", false)]
        [TestCase("h", "1", false)]
        [TestCase("h", "x", false)]
        [TestCase("h", "2", false)]
        [TestCase("a", "1", false)]
        [TestCase("a", "x", false)]
        [TestCase("a", "2", false)]
        [TestCase("1", "2", false)]
        [TestCase("1", "x", false)]
        [TestCase("2", "x", false)]
        [TestCase("x", "2", true)]
        [TestCase("x", "x", false)]
        public void Compare_ReturnOrder(string firstBetChoice, string secondBetChoice, bool isSwitch)
        {
            //// Arrange
            _betChoiceCompare = new BetChoiceComparer();

            //// Act
            var isActualSwitch = _betChoiceCompare.Compare(firstBetChoice, secondBetChoice) > 0;

            //// Assert
            Assert.AreEqual(isSwitch, isActualSwitch);
        }
    }
}
