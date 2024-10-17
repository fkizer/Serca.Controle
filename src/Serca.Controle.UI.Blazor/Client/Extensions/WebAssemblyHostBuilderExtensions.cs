using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serca.Authentication;
using Serca.Authentication.Abstractions;
using Serca.Controle.Core.Application;
using Serca.Controle.Infrastructures.API.Fakers;
using Serca.Controle.UI.Blazor.Client.Extensions;

namespace Serca.Controle.UI.Blazor.Client.Extensions
{
    public static class WebAssemblyHostBuilderExtensions
    {
        public static WebAssemblyHostBuilder AddAuthenticationsServices(this WebAssemblyHostBuilder builder, IConfiguration configuration)
        {
            // Adding serca authentication service
            builder.Services.AddAuthenticationsServices();

            DevelopmentOptions developmentOptions = new DevelopmentOptions();

            // Loading development options
            if (builder.HostEnvironment.IsDevelopment())
            {
                configuration.GetSection("DevelopmentOptions").Bind(developmentOptions);
            }

            // Override Initiliazer
            if (developmentOptions.UseFakeAuthentication)
            {
                builder.Services.AddFakeInitializerServices();
            }

            return builder;
        }

        private static IServiceCollection AddFakeInitializerServices(this IServiceCollection services)
        {
            return services
                .AddTransient<IInitalizer, FakeBackendApiInitializer>();
        }
    }
}
