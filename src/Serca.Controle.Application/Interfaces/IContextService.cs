using Serca.Controle.Core.Application.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application.Interfaces
{
    public interface IContextService
    {
        string Version { get; }
        UserViewModel? User { get; }
        SerigProfileViewModel? SerigProfile { get; }
        UtilisateurViewModel? Utilisateur { get; set; }
        public bool IsSerigInstance { get; }
        public bool IsPilote { get; }

        void SetRepositoryDefaultParameters();
        public event EventHandler? UtilisateurChanged;

        Task<bool> Login(string initializationCode);
        void Login(string userm, string password, string erp, string codeMachine);
        void Login(SerigProfileViewModel serigProfile, string userm, string password, int? utilisateurId, string? utilisateurName);
        Task<bool> Logout();
        Task ValidateAsync();
        bool Save();
        Task<bool> SaveAsync();
    }
}
