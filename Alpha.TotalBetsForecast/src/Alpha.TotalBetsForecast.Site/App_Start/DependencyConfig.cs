namespace Alpha.TotalBetsForecast.Site.App_Start
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;
    using System.Web.Mvc;
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using Handlers;
    using MediatR;
    using Services;
    using Services.Impl;
    using SimpleInjector;
    using SimpleInjector.Integration.Web;
    using SimpleInjector.Integration.Web.Mvc;
    using Sunplus.Agent.Model.Base;
    using Sunplus.Agent.Model.Entities;
    using TotalBetsForecast.Services;

    public static class DependencyConfig
    {
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Reviewed")]
        public static void Init()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();

            RegisterMediator(container);

            RegisterCrossCuttingConcerns(container);

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

        private static void RegisterCrossCuttingConcerns(Container container)
        {
            container.Register(() => CacheService.CreateInstance(CacheType.MemoryCache), Lifestyle.Singleton);
        }

        private static void RegisterServices(Container container)
        {
            container.RegisterSingleton<IUserSession, UserSessionExt>();
            container.Register<IEventService, EventService>(Lifestyle.Scoped);
            container.Register<IForecastService, ForecastService>(Lifestyle.Scoped);
            container.Register<BullfightingService, BullfightingServiceImpl>(Lifestyle.Scoped);
        }

        private static void RegisterDataAccesses(Container container)
        {
            container.Register<IDynamicRepository, DynamicRepository>(Lifestyle.Scoped);
        }
    }
}