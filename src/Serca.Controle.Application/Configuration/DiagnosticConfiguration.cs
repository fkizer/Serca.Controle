using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Core.Application.Configuration
{
    public class DiagnosticConfiguration
    {
        public string? Name { get; set; }
        public string? Desc { get; set; }
        public List<DiagnosticServiceConfiguration>? Services { get; set; }
    }

    public class DiagnosticServiceConfiguration
    {
        public string? Name { get; set; }
        public string? Url { get; set; }
        public bool HttpCodeShouldBeValid { get; set; }
    }
}
