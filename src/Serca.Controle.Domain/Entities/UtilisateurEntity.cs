using Serca.Controle.Core.Domain.Entities.Base;
using Serca.DataAccess.Repository.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Domain.Entities
{
    public class UtilisateurEntity : BaseEntity<int>, IAggregateRoot
    {
        public new int Id => UtilisateurId;

        [JsonPropertyName("utilisateur")]
        public int UtilisateurId { get; set; }

        [JsonPropertyName("nom")]
        public string? Nom { get; set; }
    }
}
