using NLog;
using NLog.Config;
using Serca.Controle.Core.Application;
using Serca.Controle.Core.Application.Abstraction.Services;
using Serca.Controle.Core.Application.Constants;
using Serca.Controle.Core.Application.Interfaces;
using Serca.Controle.Core.Application.Services;
using Serca.Controle.Core.Domain.Entities;
using Serca.Controle.Infrastructures.Logging;
using Serca.DataAccess.Repository.Abstractions;
using Serca.DataAccess.Repository.WebServices;
using Serca.Tools;
using Serca.Controle.UI.Blazor.Client.Infrastructure;

namespace Serca.Controle.UI.Blazor.Client.Extensions
{
    public static class AppServicesCollectionExtensions
    {
        public static IServiceCollection AddContextServices(this IServiceCollection services)
        {
            return services.AddSingleton<AssembliesInfo>()
                .AddScoped<IContextService, ContextService>()
                .AddScoped<IStorageService, LocalStorageService>()
                .AddScoped<ServerMigrationService, ServerMigrationService>();
        }

        public static IServiceCollection AddRepositories(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ApiRepositoryOptions>(configuration.GetSection("ApiRepositories"));

            return services
                .AddSingleton<IDefaultRepositoryParams, WebServiceRepositoryDefaultParameters>()
                .AddScoped(typeof(IGenericRepository<DeviceParametersEntity, int?>), typeof(GenericAPIRepository<DeviceParametersEntity, int?>))
                .AddScoped(typeof(IGenericRepository<UtilisateurEntity, int?>), typeof(GenericAPIRepository<UtilisateurEntity, int?>));

        }


        // MediatR and AutoMapper registration
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register MediatR services
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(
                typeof(ApplicationConstants).Assembly,
                typeof(Program).Assembly
            ));

            // Register AutoMapper services
            services.AddAutoMapper(typeof(ApplicationConstants).Assembly);

            return services;
        }

        public static IServiceCollection AddNLogCustomTargets(this IServiceCollection services, IConfiguration configuration)
        {
            // Get the current NLog configuration
            var config = LogManager.Configuration ?? new LoggingConfiguration();

            // Register SqliteTarget
            var sqliteTarget = new SqliteTarget(services.BuildServiceProvider(), configuration)
            {
                Name = "SqliteTarget"
            };
            config.AddTarget(sqliteTarget);
            config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Info, sqliteTarget));  // Adjust log level and logger rules as needed

            // Register MemoryFlushableTarget
            var memoryFlushableTarget = new MemoryFlushableTarget(services.BuildServiceProvider(), configuration)
            {
                Name = "MemoryFlushableTarget"
            };
            config.AddTarget(memoryFlushableTarget);
            config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Info, memoryFlushableTarget));

            // Apply the updated NLog configuration
            LogManager.Configuration = config;

            // Register these custom targets as singletons
            services.AddSingleton(sqliteTarget);
            services.AddSingleton(memoryFlushableTarget);

            return services;
        }
    }
}
