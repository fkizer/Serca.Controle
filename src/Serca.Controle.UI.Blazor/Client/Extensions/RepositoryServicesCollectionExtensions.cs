using Serca.Controle.Core.Application;
using Serca.Controle.Core.Domain.Entities;
using Serca.DataAccess.Repository.Abstractions;
using Serca.DataAccess.Repository.WebServices;

namespace Serca.Controle.UI.Blazor.Client.Extensions
{
    public static class RepositoryServicesCollectionExtensions
    {
        public static IServiceCollection AddRepositoryServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions();
            services.Configure<ApiRepositoryOptions>(configuration.GetSection("ApiRepositories"));

            return services
                .AddSingleton<IDefaultRepositoryParams, WebServiceRepositoryDefaultParameters>()
                .AddScoped(typeof(IGenericRepository<DeviceParametersEntity, int?>), typeof(GenericAPIRepository<DeviceParametersEntity, int?>))
                .AddScoped(typeof(IGenericRepository<UtilisateurEntity, int?>), typeof(GenericAPIRepository<UtilisateurEntity, int?>));
        }
    }
}
