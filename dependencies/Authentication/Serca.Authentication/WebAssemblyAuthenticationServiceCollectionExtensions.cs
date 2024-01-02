using Microsoft.Extensions.DependencyInjection;
using Serca.Authentication.Abstractions;

namespace Serca.Authentication
{
    public static class WebAssemblyAuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddAuthenticationsServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IInitalizer, BackendApiInitalizer>()
                .AddAuthenticationManager();
        }

        private static IServiceCollection AddAuthenticationManager(this IServiceCollection services)
        {
            return services.AddTransient<IAuthenticationManager, AuthenticationManager>();
        }
    }
}
