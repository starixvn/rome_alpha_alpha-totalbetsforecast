namespace Alpha.TotalBetsForecast.Site.App_Start
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Web.Mvc;
    using Alpha.PartialUM.Helpers;
    using Alpha.Web.Helpers;
    using AlphaPlus.Configurations;
    using AlphaPlus.PartialUM.DataAccess;
    using AlphaPlus.PartialUM.Helpers;
    using AlphaPlus.PartialUM.Services;
    using AlphaPlus.User.Models;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Handlers;
    using MediatR;
    using Services;
    using Services.Impl;
    using SimpleInjector;
    using SimpleInjector.Integration.Web;
    using SimpleInjector.Integration.Web.Mvc;

    public static class DependencyConfig
    {
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed")]
        public static void Init()
        {
            ConfigurationManager.Init(new ConfigurationHelper());
            UserSession.Init(new HttpContextHelper());

            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            RegisterMediator(container);

            RegisterDataAccesses(container);

            RegisterServices(container);

            // This is an extension method from the integration package.
            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            // This is an extension method from the integration package as well.
            container.RegisterMvcIntegratedFilterProvider();

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        private static void RegisterMediator(Container container)
        {
            var assemblies = GetAssemblies();
            container.RegisterSingleton<IMediator, Mediator>();
            container.Register(typeof(IRequestHandler<,>), assemblies);
            container.Register(typeof(IAsyncRequestHandler<,>), assemblies);
            container.RegisterCollection(typeof(INotificationHandler<>), assemblies);
            container.RegisterCollection(typeof(IAsyncNotificationHandler<>), assemblies);
            container.RegisterSingleton(new SingleInstanceFactory(container.GetInstance));
            container.RegisterSingleton(new MultiInstanceFactory(container.GetAllInstances));
        }

        private static IEnumerable<Assembly> GetAssemblies()
        {
            yield return typeof(ForecastHandler).Assembly;
        }

        private static void RegisterServices(Container container)
        {
            container.RegisterSingleton<ICacheService>(new CacheService());
            container.RegisterSingleton<IUserSession, UserSessionExt>();
            container.Register<IEventService, EventService>(Lifestyle.Scoped);
            container.Register<IForecastService, ForecastService>(Lifestyle.Scoped);
            container.RegisterSingleton<IPartialUMService, PartialUMService>();
            container.RegisterSingleton<IPartialUMSectionHelper, PartialUMSectionHelper>();
        }

        private static void RegisterDataAccesses(Container container)
        {
            container.RegisterSingleton<IDynamicRepository, DynamicRepository>();
            container.RegisterSingleton<IPartialUMDataAccess, PartialUMDataAccess>();
        }
    }
}