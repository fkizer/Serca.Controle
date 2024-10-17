using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application.ViewModels
{
    public class DeviceParameters
    {
        public int? UtilisateurId { get; set; }
        public bool IsPilote { get; set; }
    }
}
