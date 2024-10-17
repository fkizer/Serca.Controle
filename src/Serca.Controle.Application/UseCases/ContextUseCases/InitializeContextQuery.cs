using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Serca.Controle.Core.Application.Abstraction;
using Serca.Controle.Core.Application.Data;
using Serca.Controle.Core.Application.UseCases.Shared;
using Serca.Controle.Core.Application.ViewModels;
using Serca.Controle.Core.Domain.Entities;
using Serca.DataAccess.Repository.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application.UseCases.ContextUseCases
{
    public class InitializeContextQuery : IRequest<Result<UtilisateurViewModel>>
    {
        public int? UtilisateurId { get; set; }
    }

    public class InitializeContextQueryHandler : IRequestHandler<InitializeContextQuery, Result<UtilisateurViewModel>>
    {
        private readonly IDbContextExtendedFactory<ApplicationDbContext> _dbContextFactory;
        private readonly IGenericRepository<DeviceParametersEntity, int?> _deviceParametersRepository;
        private readonly ILogger<InitializeContextQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<UtilisateurEntity, int?> _preparateurRepository;

        public InitializeContextQueryHandler(
            IDbContextExtendedFactory<ApplicationDbContext> dbContextFactory,
            IGenericRepository<DeviceParametersEntity, int?> deviceParamtersRepository,
            ILogger<InitializeContextQueryHandler> logger,
            IMapper mapper,
            IGenericRepository<UtilisateurEntity, int?> preparateurRepository)
        {
            _dbContextFactory = dbContextFactory;
            _deviceParametersRepository = deviceParamtersRepository;
            _logger = logger;
            _mapper = mapper;
            _preparateurRepository = preparateurRepository;
        }

        public async Task<Result<UtilisateurViewModel>> Handle(InitializeContextQuery request, CancellationToken cancellationToken)
        {
            // Caller can force the id
            var deviceParameters = await RetrieveDeviceParameterAsync();
            var preparateurId = request.UtilisateurId ?? deviceParameters?.UtilisateurId ?? throw new Exception("Utilisateur ID cannot be determined.");
            UtilisateurEntity? preparateurEntity = null;

            try
            {
                _logger.LogInformation($"Retrieving preparateur {preparateurId} from repository...");
                preparateurEntity = await _preparateurRepository.GetByIdAsync(preparateurId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository error.");
            }

            if (preparateurEntity != null)
            {
                _logger.LogInformation($"Utilisateur was successfully retrieved!");
                await DbSave(preparateurEntity);
            }
            else
            {
                _logger.LogInformation("Utilisateur not found.");
                _logger.LogInformation("Retrieving from local...");
                using var ctx = await _dbContextFactory.CreateDbContextAsync();
                preparateurEntity = ctx.Utilisateurs.Where(x => x.UtilisateurId == preparateurId).FirstOrDefault();
            }

            if (preparateurEntity == null)
            {
                _logger.LogInformation("Context cannot be initialized.");
                return new Result<UtilisateurViewModel>() { Success = false };
            }

            return new Result<UtilisateurViewModel>() { Data = _mapper.Map<UtilisateurViewModel>(preparateurEntity), Success = true };
        }

        /// <summary>
        /// Retrieve device parameters from repository or local when not available.
        /// </summary>
        /// <param name="preparateurId"></param>
        /// <returns></returns>
        private async Task<DeviceParametersEntity?> RetrieveDeviceParameterAsync()
        {
            _logger.LogInformation("Starting research...");

            try
            {
                _logger.LogInformation("Retrieving from repository...");
                var deviceParameters = await _deviceParametersRepository.GetByIdAsync(null);

                if (deviceParameters != null)
                {
                    _logger.LogInformation($"Repository found. ID: {deviceParameters.UtilisateurId}");
                    await DbSave(deviceParameters);

                }
                else
                {
                    _logger.LogInformation("Repository not found.");
                    _logger.LogInformation("Retrieving from local...");
                    using var ctx = await _dbContextFactory.CreateDbContextAsync();
                    deviceParameters = ctx.DeviceParameters.Single();
                }

                return deviceParameters;
            }
            catch (Exception ex)
            {
                if (typeof(InvalidOperationException) == ex.GetType())
                {
                    _logger.LogInformation(ex, "No valid data found.");
                }
                else
                {
                    _logger.LogError(ex, "Repository error.");
                }

                return null;
            }
        }

        /// <summary>
        /// Save <see cref="DeviceParametersEntity"/> in Database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task DbSave(DeviceParametersEntity entity)
        {
            try
            {
                using var ctx = await _dbContextFactory.CreateDbContextAsync();
                ctx.DeviceParameters.RemoveRange(ctx.DeviceParameters.ToList());
                ctx.DeviceParameters.Add(entity);
                await ctx.SaveChangesAsync();
                _logger.LogInformation($"{nameof(DeviceParametersEntity)} saved!");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(DeviceParametersEntity)} save error.");
                throw;
            }
        }


        /// <summary>
        /// Save <see cref="UtilisateurEntity"/> in Database
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private async Task DbSave(UtilisateurEntity entity)
        {
            try
            {
                using var ctx = await _dbContextFactory.CreateDbContextAsync();
                var existant = ctx.Utilisateurs.SingleOrDefault(x => x.UtilisateurId == entity.UtilisateurId);

                if (existant != null)
                {
                    ctx.Utilisateurs.Remove(existant);
                }

                ctx.Utilisateurs.Add(entity);
                await ctx.SaveChangesAsync();
                _logger.LogInformation($"{nameof(UtilisateurEntity)} saved!");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{nameof(UtilisateurEntity)} save error.");
                throw;
            }
        }
    }
}
