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
    public class GetAllUtilisateursQuery : IRequest<CachableResult<List<UtilisateurViewModel>>>
    {
        public bool GetFromRepo { get; set; } = true;
        public bool GetFromCache { get; set; }
        public bool SaveInCache { get; set; }
    }

    public class GetAllPreprateursRequestHandler : IRequestHandler<GetAllUtilisateursQuery, CachableResult<List<UtilisateurViewModel>>>
    {
        private readonly IDbContextExtendedFactory<ApplicationDbContext> _dbContextFactory;
        private readonly ILogger<GetAllPreprateursRequestHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<UtilisateurEntity, int?> _repository;

        public GetAllPreprateursRequestHandler(IDbContextExtendedFactory<ApplicationDbContext> dbContextFactory, ILogger<GetAllPreprateursRequestHandler> logger, IMapper mapper, IGenericRepository<UtilisateurEntity, int?> repository)
        {
            _dbContextFactory = dbContextFactory;
            _logger = logger;
            _mapper = mapper;
            _repository = repository;
        }

        public async Task<CachableResult<List<UtilisateurViewModel>>> Handle(GetAllUtilisateursQuery request, CancellationToken cancellationToken)
        {
            var result = new CachableResult<List<UtilisateurViewModel>>();
            IReadOnlyList<UtilisateurEntity>? utilisateursEntities = null;

            try
            {
                _logger.LogInformation("Getting from repository...");
                utilisateursEntities = await _repository.GetAllAsync();

                if (request.SaveInCache)
                {
                    await DbSave(utilisateursEntities);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on trying to get utilisateurs list from repository.");
            }

            if (utilisateursEntities == null && request.GetFromCache)
            {
                utilisateursEntities = await GetFromCache();
                result.FromCache = true;
            }

            if (utilisateursEntities != null)
            {
                result.Success = true;
                result.Data = _mapper.Map<IEnumerable<UtilisateurViewModel>>(utilisateursEntities).ToList();
            }
            else
            {
                result.Message = "L'application n'a pas pu récupérer la liste des préparateurs";
            }

            return result;
        }

        private async Task<IReadOnlyList<UtilisateurEntity>?> GetFromCache()
        {
            _logger.LogInformation("Getting from cache...");

            try
            {
                using var ctx = await _dbContextFactory.CreateDbContextAsync();
                return ctx.Utilisateurs.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on trying to get utilisateurs list from cache.");
            }

            return null;
        }


        /// <summary>
        /// Save entity in database
        /// </summary>
        /// <param name="id">Require for deletion</param>
        /// <param name="entity">Entity to save</param>
        /// <returns></returns>
        private async Task DbSave(IEnumerable<UtilisateurEntity>? entity)
        {
            try
            {
                using var ctx = await _dbContextFactory.CreateDbContextAsync();
                ctx.Utilisateurs.RemoveRange(ctx.Utilisateurs.ToList());

                if (entity != null)
                {
                    ctx.Utilisateurs.AddRange(entity);
                }

                await ctx.SaveChangesAsync();
                _logger.LogInformation("Utilisateurs list saved in cache!");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error on trying to save utilisateurs list in cache.");
            }
        }
    }
}
