using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Serca.Controle.Core.Domain.Entities.Base;
using Serca.DataAccess.Repository.Abstractions;

namespace Serca.Controle.Core.Domain.Entities
{
    public class DeviceParametersEntity : BaseEntity<int>, IAggregateRoot
    {
        public new int Id => UtilisateurId;

        [JsonPropertyName("preparateur")]
        public int UtilisateurId { get; set; }

        [JsonPropertyName("pilote")]
        public bool IsPilote { get; set; }
    }
}
