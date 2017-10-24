namespace Alpha.TotalBetsForecast.Site.Tests.Helpers
{
    using System;
    using System.Web.Mvc;
    using NUnit.Framework;
    using Site.Helpers;
    using Site.Models.Dtos;
    using Site.Models.Requests;

    [TestFixture]
    public class QueryEntityModelBinderTests : QueryEntityModelBinder
    {
        protected override object CreateInstanceOfUserSession(Type modelType)
        {
            return "CallCreateInstanceOfUserSession";
        }

        protected override object BaseCreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            return "CallBaseCreateModel";
        }

        [Test]
        public void CreateModel_ParameterIsIUserSession_CallCreateInstanceOfUserSession()
        {
            //// Arrange
            var query = typeof(MatchForecastsQuery);

            //// Act
            var act = CreateModel(null, null, query);

            //// Assert
            Assert.AreEqual("CallCreateInstanceOfUserSession", act);
        }

        [Test]
        public void CreateModel_ParameterIsNotIUserSession_CallBaseCreateInstance()
        {
            //// Arrange
            var dto = typeof(WinLossColumnNameDto);

            //// Act
            var act = CreateModel(null, null, dto);

            //// Assert
            Assert.AreEqual("CallBaseCreateModel", act);
        }
    }
}
