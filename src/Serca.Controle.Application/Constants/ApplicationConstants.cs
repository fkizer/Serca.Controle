using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application.Constants
{
    public class ApplicationConstants
    {
        public const string ApplicationName = "Controle";
        public const int DefaultHistoryRetentionDays = 30;
        
        public const string RegexCodeArticle = @"^\$C;([0-9].{0,6})\$$";

        #region HttpClient

        public const string DiagHttpClientName = "diag";

        #endregion HttpClient

        public class StorageKey
        {
            public const string User = "User";
            public const string Utilisateur = "Utilisateur";
            public const string SerigProfile = "SerigProfile";
        }

        public class RandomString
        {
            public const string UnknowError = "Erreur Inconnue";
        }
    }
}
