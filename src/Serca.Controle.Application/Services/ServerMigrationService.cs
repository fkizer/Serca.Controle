using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Serca.Controle.Core.Application.Configuration;
using Serca.Controle.Core.Application.Interfaces;
using Serca.Controle.Core.Application.UseCases.DeviceParametersUseCases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application.Services
{
    public class ServerMigrationService
    {
        protected readonly IConfiguration Configuration;
        protected readonly IContextService ContextService;
        protected readonly IMediator Mediator;
        protected readonly NavigationManager NavigationManager;

        protected List<MigrationConfiguration> MigrationConfigurations = new List<MigrationConfiguration>();

        public ServerMigrationService(IConfiguration configuration, IContextService contextService, IMediator mediator, NavigationManager navigationManager)
        {
            Configuration = configuration;
            ContextService = contextService;
            Mediator = mediator;
            NavigationManager = navigationManager;

            LoadDiagnostiqueConf();
        }

        protected void LoadDiagnostiqueConf()
        {
            Configuration.Bind("ServerMigration", MigrationConfigurations);
        }

        public async Task<MigrationConfiguration?> GetMigrationIfAvailable(string? src = null)
        {
            // Should be totaly auth
            if (!ContextService.User?.IsAuthenticated ?? false)
            {
                return null;
            }

            if (await IsDevicePilote())
            {

                return MigrationPiloteAvailable(src);
            }
            else
            {
                return MigrationAvailable(src);
            }
        }

        public MigrationConfiguration? MigrationAvailable(string? src = null)
        {
            return MigrationConfigurations.FirstOrDefault(x => x.IsMigrationEndpoint && (src != null ? x.Source != null && x.Source.Contains(src) : true));
        }

        public MigrationConfiguration? MigrationPiloteAvailable(string? src = null)
        {
            return MigrationConfigurations.FirstOrDefault(x => x.IsMigrationEndpoint && (src != null ? x.Source != null && x.Source.Contains(src) : true) && x.Pilote);
        }

        public async Task<bool> IsDevicePilote()
        {
            var getDeviceParametersResult = await Mediator.Send(new GetDeviceParametersQuery());
            return getDeviceParametersResult.Success && (getDeviceParametersResult.Data?.IsPilote ?? false);
        }

        /// <summary>
        /// Load and update diagnostic configuration for corresponding with target migration
        /// </summary>
        public List<DiagnosticConfiguration> LoadMigrationDiagnostic()
        {
            List<DiagnosticConfiguration> migrationDiagnostic = new List<DiagnosticConfiguration>();
            Configuration.Bind("Diagnostics", migrationDiagnostic);

            foreach (var diagGroupe in migrationDiagnostic)
            {
                if (diagGroupe.Services == null) continue;
                foreach (var service in diagGroupe.Services)
                {
                    foreach (var migration in MigrationConfigurations)
                    {
                        if (migration.Source == null) continue;
                        service.Url = service.Url?.Replace(migration.Source, migration.Destination);
                    }
                }
            }

            return migrationDiagnostic;
        }
    }
}
