using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serca.Authentication;
using Serca.Authentication.Abstractions;
using Serca.Controle.Core.Application.Abstraction.Services;
using Serca.Controle.Core.Application.Constants;
using Serca.Controle.Core.Application.Interfaces;
using Serca.Controle.Core.Application.UseCases.ContextUseCases;
using Serca.Controle.Core.Application.UseCases.DeviceParametersUseCases;
using Serca.Controle.Core.Application.ViewModels;
using Serca.Controle.Core.Domain;
using Serca.DataAccess.Repository.Abstractions;
using Serca.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application.Services
{
    public class ContextService : IContextService
    {
        private readonly AssembliesInfo _assembliesInfo;
        private readonly IAuthenticationManager _authenticationManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ContextService> _logger;
        private readonly IMediator _mediator;
        private readonly IStorageService _storageService;
        private readonly WebServiceRepositoryDefaultParameters? WebServiceRepositoryDefaultParameters;


        private UtilisateurViewModel? _Utilisateur;
        public UtilisateurViewModel? Utilisateur
        {
            get
            {
                if (!_isContextValidateCall) throw new Exception($"Valider le context en appellant la méthode {nameof(ValidateAsync)} avant d'acceder à {nameof(Utilisateur)}");

                return _Utilisateur;
            }
            set
            {
                _Utilisateur = value;
                UtilisateurChanged?.Invoke(this, EventArgs.Empty);
                WebServiceRepositoryDefaultParameters?.UpdateUtilisateur(_Utilisateur?.UtilisateurId.ToString());
            }
        }

        public UserViewModel User { get; private set; }
        public SerigProfileViewModel SerigProfile { get; private set; }
        public string Version { get; }
        public bool IsSerigInstance { get; }
        public bool IsPilote { get; protected set; }

        // Flag use on save 
        private bool _userChanged;
        private bool _isContextValidated;
        private bool _isContextValidateCall;

        public event EventHandler? UtilisateurChanged;

        public ContextService(AssembliesInfo assembliesInfo,
            IAuthenticationManager authenticationManager,
            ILogger<ContextService> logger,
            IMediator mediator,
            IStorageService storageService,
            IDefaultRepositoryParams? defaultRepositoryParams,
            IConfiguration configuration)
        {
            _authenticationManager = authenticationManager;
            _assembliesInfo = assembliesInfo;
            _configuration = configuration;
            _logger = logger;
            _mediator = mediator;
            _storageService = storageService;
            WebServiceRepositoryDefaultParameters = (WebServiceRepositoryDefaultParameters?)defaultRepositoryParams;

            Version = _assembliesInfo.GetVersion(this).First().Value;
            User = _storageService.Get<UserViewModel>(ApplicationConstants.StorageKey.User) ?? new UserViewModel();
            User.PropertyChanged += (s, e) => { _userChanged = true; };
            SerigProfile = _storageService.Get<SerigProfileViewModel>(ApplicationConstants.StorageKey.SerigProfile) ?? new SerigProfileViewModel();
            Utilisateur = _storageService.Get<UtilisateurViewModel>(ApplicationConstants.StorageKey.Utilisateur);
            IsSerigInstance = configuration.GetValue<bool>("IsSerigInstance");
            SetRepositoryDefaultParameters();
            _ = Task.Run(async () =>
            {
                var getDeviceParametersResult = await mediator.Send(new GetDeviceParametersQuery());
                IsPilote = getDeviceParametersResult.Success && (getDeviceParametersResult.Data?.IsPilote ?? false);
            });
        }

        public async Task ValidateAsync()
        {
            _isContextValidateCall = true;
            if (!User.IsAuthenticated || _isContextValidated) return;

            try
            {
                var response = await _mediator.Send(new InitializeContextQuery() { UtilisateurId = _Utilisateur?.UtilisateurId });

                if (response.Success)
                {
                    Utilisateur = response.Data;
                }
                _isContextValidated = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Validation Utilisateur error");
            }

            await SaveAsync();
        }

        public void SetRepositoryDefaultParameters()
        {
            var headers = new Dictionary<string, string>();

            if (IsSerigInstance)
            {
                headers.Add("X-AppId", "b0t1zudb4tkk8k6bck4hg48ep3syqrcc7hnyuyvzfkqawuj9thn7tn7f3zwtzyt3");
            }

            WebServiceRepositoryDefaultParameters?.Initiliaze(
                ApplicationConstants.ApplicationName,
                User.CodeMachine.ToString(),
                User.Erp,
                Version,
                User.Ste.ToString(),
                User.Depo.ToString(),
                User.Userm,
                User.Password,
                _Utilisateur?.UtilisateurId.ToString(),
                headers
            );
        }

        public async Task<bool> Login(string initializationCode)
        {
            if (User.IsInitialized)
            {
                return true;
            }

            try
            {
                var erp = initializationCode.ExtractErpId();
                var codeMachine = Guid.NewGuid();

                var initializationRequestParameters = new InitializationRequestParameters()
                {
                    InitializationCode = initializationCode,
                    App = ApplicationConstants.ApplicationName,
                    CodeMachine = codeMachine.ToString(),
                    Erp = erp,
                    Version = Version
                };

                var credentials = await _authenticationManager.InitializedAsync(initializationRequestParameters);

                // Store User info
                User.CodeMachine = codeMachine;
                User.Userm = credentials?.Userm;
                User.Password = credentials?.Password;
                User.Erp = erp;

                // Persistance storage
                _storageService.Save(User, ApplicationConstants.StorageKey.User);
            }
            catch (Exception)
            {
                // Reset user
                User = new UserViewModel();

                // Persistance storage
                _storageService.Save(User, ApplicationConstants.StorageKey.User);

                throw;
            }

            return true;
        }

        public void Login(SerigProfileViewModel serigProfile, string userm, string password, int? utilisateurId, string? utilisateurName)
        {
            if (User.IsInitialized)
            {
                return;
            }

            try
            {
                var codeMachine = Guid.NewGuid();

                var initializationRequestParameters = new InitializationRequestParameters()
                {
                    InitializationCode = "",
                    App = ApplicationConstants.ApplicationName,
                    CodeMachine = codeMachine.ToString(),
                    Erp = "***", // FAEL __> Add Erp name
                    Version = Version
                };

                //User Info
                User.CodeMachine = codeMachine;
                User.Userm = userm;
                User.Password = password;
                User.Erp = "***"; // FAEL __> Add Erp name
                UInt16.TryParse(serigProfile.SocCode!, out ushort ste);
                User.Ste = ste;
                User.Depo = UInt16.Parse(serigProfile.Depo!);

                _storageService.Save(User, ApplicationConstants.StorageKey.User);

                //Serig profile info
                SerigProfile.ServerHost = serigProfile.ServerHost;
                SerigProfile.ServerPort = serigProfile.ServerPort;
                SerigProfile.ServerProtocol = serigProfile.ServerProtocol;
                SerigProfile.ServerPath = serigProfile.ServerPath;
                SerigProfile.ServerName = serigProfile.ServerName;
                SerigProfile.ClientId = serigProfile.ClientId;
                SerigProfile.SocCode = serigProfile.SocCode;
                SerigProfile.Depo = serigProfile.Depo;

                _storageService.Save(SerigProfile, ApplicationConstants.StorageKey.SerigProfile);

                //Utilisateur
                if (Utilisateur == null)
                {
                    Utilisateur = new UtilisateurViewModel() { UtilisateurId = utilisateurId, Nom = utilisateurName };
                    _storageService.Save(Utilisateur, ApplicationConstants.StorageKey.Utilisateur);
                }
            }
            catch (Exception)
            {
                // Reset user
                User = new UserViewModel();
                // Persistance storage
                _storageService.Save(User, ApplicationConstants.StorageKey.User);

                //reset serig profile 

                SerigProfile = new SerigProfileViewModel();
                _storageService.Save(SerigProfile, ApplicationConstants.StorageKey.SerigProfile);
                throw;
            }
        }

        public void Login(string userm, string password, string erp, string codeMachine)
        {
            try
            {
                // Store User info
                User.CodeMachine = new Guid(codeMachine);
                User.Userm = userm;
                User.Password = password;
                User.Erp = erp;

                // Persistance storage
                _storageService.Save(User, ApplicationConstants.StorageKey.User);
            }
            catch (Exception)
            {
                // Reset user
                User = new UserViewModel();

                // Persistance storage
                _storageService.Save(User, ApplicationConstants.StorageKey.User);

                throw;
            }
        }

        public async Task<bool> Logout()
        {
            if (!User.IsInitialized)
            {
                return false;
            }

            User = new UserViewModel();
            Utilisateur = null;

            return await ResetSerigProfileIfRequired()
                && await _storageService.SaveAsync(User, ApplicationConstants.StorageKey.User)
                && await _storageService.SaveAsync(Utilisateur, ApplicationConstants.StorageKey.Utilisateur);
        }

        private async Task<bool> ResetSerigProfileIfRequired()
        {
            if (!IsSerigInstance)
            {
                return true;
            }

            SerigProfile = new SerigProfileViewModel();
            return await _storageService.SaveAsync(SerigProfile, ApplicationConstants.StorageKey.SerigProfile);
        }

        //QUAP__>TODO: It' should be a delegate
        public bool Save()
        {
            if (_userChanged)
            {
                _userChanged = false;
                SetRepositoryDefaultParameters();
                Task.Run(async () => { await ValidateAsync(); });
            }

            return SaveSerigProfileIfRequired() && _storageService.Save(User, ApplicationConstants.StorageKey.User)
                && _storageService.Save(Utilisateur, ApplicationConstants.StorageKey.Utilisateur);
        }

        public async Task<bool> SaveAsync()
        {
            if (_userChanged)
            {
                _userChanged = false;
                SetRepositoryDefaultParameters();
                await ValidateAsync();
            }

            return await SaveSerigProfileIfRequiredAsync()
                && await _storageService.SaveAsync(User, ApplicationConstants.StorageKey.User)
                && await _storageService.SaveAsync(Utilisateur, ApplicationConstants.StorageKey.Utilisateur);
        }

        private bool SaveSerigProfileIfRequired()
        {
            if (!IsSerigInstance)
            {
                return true;
            }

            return _storageService.Save(SerigProfile, ApplicationConstants.StorageKey.SerigProfile);
        }

        private async Task<bool> SaveSerigProfileIfRequiredAsync()
        {
            if (!IsSerigInstance)
            {
                return true;
            }

            return await _storageService.SaveAsync(SerigProfile, ApplicationConstants.StorageKey.SerigProfile);
        }
    }
}
