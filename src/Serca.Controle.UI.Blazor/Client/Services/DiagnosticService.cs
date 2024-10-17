using MediatR;
using Microsoft.Extensions.Options;
using Serca.Controle.Core.Application.Configuration;
using Serca.Controle.Core.Application.UseCases.DeviceParametersUseCases;
using Serca.Controle.UI.Blazor.Client.Managers;
using Serca.DataAccess.Repository.Abstractions;
using System.Configuration;

namespace Serca.Controle.UI.Blazor.Client.Services
{
    public class DiagnosticService
    {
        protected readonly IMediator Mediator;
        protected readonly IConfiguration Configuration;
        protected readonly NetworkConfiguration NetworkConfiguration;
        public List<DiagnosticConfiguration> DiagnosticConfigurations { get; private set; } = new List<DiagnosticConfiguration>();

        protected string? BasePathData;

        public DiagnosticService(IMediator mediator, IConfiguration configuration, NetworkConfiguration networkConfiguration, IOptions<ApiRepositoryOptions>? options)
        {
            Mediator = mediator;
            Configuration = configuration;
            NetworkConfiguration = networkConfiguration;
            BasePathData = options?.Value.Base?.Trim('/');
        }

        public void Refresh()
        {
            DiagnosticConfigurations.Clear();

            // From configuration
            Configuration.Bind("diagnostics", DiagnosticConfigurations);

            //Auto diag
            var autoDiag = GetDynamicDiags();
            DiagnosticConfigurations.ForEach(diagnostic =>
            {
                var match = autoDiag.FirstOrDefault(diag => diag.Name == diagnostic.Name);

                if (match?.Services == null)
                {
                    return;
                }

                if (diagnostic.Services != null)
                {
                    diagnostic.Services.AddRange(match.Services);
                }
                else
                {
                    diagnostic.Services = match.Services;
                }

                autoDiag.Remove(match);
            });

            DiagnosticConfigurations.AddRange(autoDiag);
        }

        private List<DiagnosticConfiguration> GetDynamicDiags()
        {
            var diags = new List<DiagnosticConfiguration>
            {
                new()
                {
                    Name = "Réseaux",
                    Desc = "Vérifier l'état de l'accès au réseau",
                    Services = new List<DiagnosticServiceConfiguration>()
                    {
                        new DiagnosticServiceConfiguration()
                        {
                            Name = "Application",
                            Url = NetworkConfiguration.BackendServerUrl, // Base address
                            HttpCodeShouldBeValid = false
                        },
                        new DiagnosticServiceConfiguration()
                        {
                            Name = "Données",
                            Url = $"{NetworkConfiguration.BackendServerUrl?.Trim('/')}/{BasePathData}",
                            HttpCodeShouldBeValid = false
                        }
                    }
                },
                new()
                {
                    Name = "Services",
                    Desc = "Vérifier l'état des services",
                    Services = new List<DiagnosticServiceConfiguration>()
                    {
                        new DiagnosticServiceConfiguration()
                        {
                            Name = "Authentification",
                            Url = $"{NetworkConfiguration.IdentityServerUrl?.Trim('/')}/.well-known/openid-configuration",
                            HttpCodeShouldBeValid = true
                        }
                    }
                }
            };

            return diags;
        }
    }
}
