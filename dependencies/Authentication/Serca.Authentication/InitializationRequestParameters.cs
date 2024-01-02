using Serca.Authentication.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Serca.Authentication
{
    public class InitializationRequestParameters : IInitializationParameters
    {
        public string? App { get; set; }
        public string? CodeMachine { get; set; }
        public string? Erp { get; set; }
        public string? InitializationCode { get; set; }
        public string? Version { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals((InitializationRequestParameters?)obj);
        }

        public override int GetHashCode()
        {
            return App?.GetHashCode() ?? 0
                ^ CodeMachine?.GetHashCode() ?? 0
                ^ Erp?.GetHashCode() ?? 0
                ^ InitializationCode?.GetHashCode() ?? 0
                ^ Version?.GetHashCode() ?? 0;
        }

        public bool Equals(InitializationRequestParameters? other)
        {
            if (other == null) return false;

            return Equals(App, other.App)
                && Equals(CodeMachine, other.CodeMachine)
                && Equals(Erp, other.Erp)
                && Equals(InitializationCode, other.InitializationCode)
                && Equals(Version, other.Version);
        }
    }
}
