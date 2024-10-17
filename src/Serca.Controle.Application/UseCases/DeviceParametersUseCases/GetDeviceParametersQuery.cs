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

namespace Serca.Controle.Core.Application.UseCases.DeviceParametersUseCases
{
    public class GetDeviceParametersQuery : IRequest<Result<DeviceParameters>>
    {
        public bool GetFromCache { get; set; }
    }

    public class GetDeviceParametersQueryHandler : IRequestHandler<GetDeviceParametersQuery, Result<DeviceParameters>>
    {
        private readonly IGenericRepository<DeviceParametersEntity, int?> _deviceParametersRepository;
        private readonly IDbContextExtendedFactory<ApplicationDbContext> _dbContextFactory;
        private readonly ILogger<GetDeviceParametersQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetDeviceParametersQueryHandler(IGenericRepository<DeviceParametersEntity, int?> deviceParametersRepository,
            IDbContextExtendedFactory<ApplicationDbContext> dbContextFactory,
            ILogger<GetDeviceParametersQueryHandler> logger,
            IMapper mapper,
            IMediator mediator)
        {
            _deviceParametersRepository = deviceParametersRepository;
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<Result<DeviceParameters>> Handle(GetDeviceParametersQuery request, CancellationToken cancellationToken)
        {
            var deviceParameters = request.GetFromCache ? await GetDeviceParametersEntityFromCache() : await GetDeviceParametersEntity();

            if (deviceParameters == null)
            {
                return new Result<DeviceParameters>();
            }
            else
            {
                return new Result<DeviceParameters>()
                {
                    Success = true,
                    Data = _mapper.Map<DeviceParametersEntity, DeviceParameters>(deviceParameters)
                };

            }
        }

        private async Task<DeviceParametersEntity?> GetDeviceParametersEntity()
        {
            DeviceParametersEntity? deviceParameters = null;
            try
            {
                deviceParameters = await _deviceParametersRepository.GetByIdAsync(null);
                if (deviceParameters != null)
                {
                    await DbSave(deviceParameters);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Repository error.");
            }

            if (deviceParameters == null)
            {
                deviceParameters = await GetDeviceParametersEntityFromCache();
            }
            return deviceParameters;
        }

        private async Task<DeviceParametersEntity?> GetDeviceParametersEntityFromCache()
        {
            try
            {
                using var ctx = await _dbContextFactory.CreateDbContextAsync();
                return ctx.DeviceParameters.Single();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid data store");
            }

            return null;
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
    }
}
