namespace Alpha.TotalBetsForecast.Site.Helpers
{
    using AlphaPlus.User.Models;
    using System;
    using System.Web.Mvc;

    public class QueryEntityModelBinder : DefaultModelBinder
    {
        private static readonly Type[] IUserSessionType = { typeof(IUserSession) };

        protected override object CreateModel(
            ControllerContext controllerContext,
            ModelBindingContext bindingContext,
            Type modelType)
        {
            if (modelType != null
                && modelType.GetConstructor(IUserSessionType) != null)
            {
                return CreateInstanceOfUserSession(modelType);
            }

            return BaseCreateModel(controllerContext, bindingContext, modelType);
        }

        protected virtual object CreateInstanceOfUserSession(Type modelType)
            => Activator.CreateInstance(modelType, DependencyResolver.Current.GetService<IUserSession>());

        protected virtual object BaseCreateModel(
           ControllerContext controllerContext,
           ModelBindingContext bindingContext,
           Type modelType) => base.CreateModel(controllerContext, bindingContext, modelType);
    }
}