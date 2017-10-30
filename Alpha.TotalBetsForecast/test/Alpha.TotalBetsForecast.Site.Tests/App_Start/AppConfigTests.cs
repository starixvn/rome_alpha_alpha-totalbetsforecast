using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alpha.TotalBetsForecast.Site.App_Start;
using NUnit.Framework;

namespace Alpha.TotalBetsForecast.Site.Tests.App_Start
{
    public class AppConfigTests
    {
        [Test]
        public void DisableHighlighChange_NotConfig_ReturnFalse()
        {
            //// Arrange
            AppConfig.AppSettings = new NameValueCollection();

            //// Act
            var act = AppConfig.DisableHighlightChange;

            //// Assert
            Assert.IsFalse(act);
        }

        [Test]
        public void DisableHighlighChange_0_ReturnFalse()
        {
            //// Arrange
            AppConfig.AppSettings = new NameValueCollection { ["NewTBF:DisableHighlightChange"] = "0" };

            //// Act
            var act = AppConfig.DisableHighlightChange;

            //// Assert
            Assert.IsFalse(act);
        }

        [Test]
        public void DisableHighlighChange_1_ReturnTrue()
        {
            //// Arrange
            AppConfig.AppSettings = new NameValueCollection { ["NewTBF:DisableHighlightChange"] = "1" };

            //// Act
            var act = AppConfig.DisableHighlightChange;

            //// Assert
            Assert.IsTrue(act);
        }

        [Test]
        public void StoredProcedurePath_Always_ReturnCorrectText()
        {
            //// Arrange
            var expectedText = "abc";
            AppConfig.AppSettings = new NameValueCollection { ["StoredProcedurePath"] = expectedText };

            //// Act
            var act = AppConfig.StoredProcedurePath;

            //// Assert
            Assert.AreEqual(expectedText, act);
        }
    }
}
