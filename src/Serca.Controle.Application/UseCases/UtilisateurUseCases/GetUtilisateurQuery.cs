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

namespace Serca.Controle.Core.Application.UseCases.UtilisateurUseCases
{
    public class GetUtilisateurQuery : IRequest<CachableResult<UtilisateurViewModel>>
    {
        public bool GetFromRepo { get; set; } = true;
        public bool GetFromCache { get; set; }
        public int PreparaterId { get; set; }
        public bool SaveInCache { get; set; }
    }

    public class GetUtilisateurQueryHandler : IRequestHandler<GetUtilisateurQuery, CachableResult<UtilisateurViewModel>>
    {
        private readonly IDbContextExtendedFactory<ApplicationDbContext> _dbContextFactory;
        private readonly ILogger<GetUtilisateurQueryHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<UtilisateurEntity, int?> _repository;

        public GetUtilisateurQueryHandler(IDbContextExtendedFactory<ApplicationDbContext> dbContextFactory, ILogger<GetUtilisateurQueryHandler> logger, IMapper mapper, IGenericRepository<UtilisateurEntity, int?> repository)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CachableResult<UtilisateurViewModel>> Handle(GetUtilisateurQuery request, CancellationToken cancellationToken)
        {
            UtilisateurEntity? utilisateurEntity = null;
            var result = new CachableResult<UtilisateurViewModel>();

            try
            {
                _logger.LogInformation("Getting from repository...");
                utilisateurEntity = await _repository.GetByIdAsync(request.PreparaterId);

                if (request.SaveInCache)
                {
                    await DbSave(request.PreparaterId, utilisateurEntity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on trying to get utilisateurs list from repository.");
            }

            if (utilisateurEntity == null && request.GetFromCache)
            {
                utilisateurEntity = await GetFromCache(request.PreparaterId);
                result.FromCache = true;
            }

            if (utilisateurEntity != null)
            {
                result.Success = true;
                result.Data = _mapper.Map<UtilisateurViewModel>(utilisateurEntity);
            }

            return result;
        }

        private async Task<UtilisateurEntity?> GetFromCache(int id)
        {
            _logger.LogInformation("Getting from cache...");

            try
            {
                using var ctx = await _dbContextFactory.CreateDbContextAsync();
                return ctx.Utilisateurs.SingleOrDefault(x => x.UtilisateurId == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on trying to get utilisateur from cache.");
            }

            return null;
        }


        /// <summary>
        /// Save entity in database
        /// </summary>
        /// <param name="id">Require for deletion</param>
        /// <param name="entity">Entity to save</param>
        /// <returns></returns>
        private async Task DbSave(int id, UtilisateurEntity? entity)
        {
            try
            {
                using var ctx = await _dbContextFactory.CreateDbContextAsync();
                var existUtilisateur = ctx.Utilisateurs.SingleOrDefault(x => x.UtilisateurId == id);

                if (existUtilisateur != null)
                {
                    ctx.Utilisateurs.Remove(existUtilisateur);
                }

                if (entity != null)
                {
                    ctx.Utilisateurs.Add(entity);
                }

                await ctx.SaveChangesAsync();
                _logger.LogInformation("Utilisateur saved in cache!");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on trying to save utilisateur in cache.");
            }
        }
    }
}
