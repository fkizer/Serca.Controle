using Serca.DataAccess.Repository.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application
{
    public class WebServiceRepositoryDefaultParameters : IDefaultRepositoryParams
    {
        public string? App { get; private set; }
        public string? CodeMachine { get; private set; }
        public string? Erp { get; private set; }
        public string? Version { get; private set; }
        public string? Ste { get; private set; }
        public string? Depo { get; private set; }
        public string? Userm { get; private set; }
        public string? Password { get; private set; }
        public string? Utilisateur { get; private set; }
        public Dictionary<string, string>? Headers { get; private set; }


        public Dictionary<string, string> ToDictionnary()
        {
            return new Dictionary<string, string>()
            {
                {"app", App ?? string.Empty },
                {"machine", CodeMachine ?? string.Empty },
                {"erp", Erp ?? string.Empty },
                {"ste", Ste ?? string.Empty },
                {"depo", Depo ?? string.Empty },
                {"userm", Userm ?? string.Empty },
                {"pwd", Password ?? string.Empty },
                {"version", Version ?? string.Empty },
                {"utilisateur", Utilisateur ?? string.Empty }
            };
        }

        public void Initiliaze(string? app, string? codeMachine, string? erp, string? version, string? ste, string? depo, string? userm, string? password,
                                string? utilisateur = null, Dictionary<string, string>? headers = null)
        {
            App = app;
            CodeMachine = codeMachine;
            Erp = erp;
            Version = version;
            Ste = ste;
            Depo = depo;
            Userm = userm;
            Password = password;
            Utilisateur = utilisateur;
            Headers = headers;
        }

        public void UpdateUtilisateur(string? utilisateur)
        {
            if (Utilisateur != null && !string.IsNullOrWhiteSpace(utilisateur))
                Utilisateur = utilisateur;
        }
    }
}
