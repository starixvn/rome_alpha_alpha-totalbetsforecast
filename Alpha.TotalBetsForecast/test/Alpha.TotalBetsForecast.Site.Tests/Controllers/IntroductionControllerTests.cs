namespace Alpha.TotalBetsForecast.Site.Tests.Controllers
{
    using System.Web.Mvc;
    using Alpha.TotalBetsForecast.Site.Controllers;
    using AlphaPlus.User.Models;
    using MediatR;
    using NSubstitute;
    using NUnit.Framework;

    public class IntroductionControllerTests
    {
        private IntroductionController controler;
        private IMediator mediator;
        private IUserSession user;

        [SetUp]
        public void Setup()
        {
            mediator = Substitute.For<IMediator>();
            user = Substitute.For<IUserSession>();
            controler = new IntroductionController(mediator, user);
        }

        [TestCase("en-US", "Index.en-US")]
        [TestCase("ja-JP", "Index.ja-JP")]
        [TestCase("ko-KR", "Index.ko-KR")]
        [TestCase("th-TH", "Index.th-TH")]
        [TestCase("zh-CN", "Index.zh-CN")]
        [TestCase("zh-TW", "Index.zh-TW")]
        public void Index_Always_ReturnCorrectView(string language, string viewName)
        {
            //// Arrange
            user.Language.Returns(language);

            //// Act
            var act = controler.Index() as ViewResult;

            //// Assert
            Assert.AreEqual(viewName, act.ViewName);
        }

        [Test]
        public void QuickIntroduction_Always_ReturnCorrectView()
        {
            //// Arrange

            //// Act
            var act = controler.QuickIntroduction() as ViewResult;

            //// Assert
            Assert.AreEqual("QuickIntroduction", act.ViewName);
        }
    }
}
