using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application.Configuration
{
    public class MigrationConfiguration
    {
        public string? Source { get; set; }
        public string? Destination { get; set; }
        public bool IsMigrationEndpoint { get; set; }
        public bool Pilote { get; set; }
        public bool Auto { get; set; }
    }
}
