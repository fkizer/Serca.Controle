using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Serca.Authentication.Abstractions
{
    public interface IInitializationParameters
    {
        [JsonPropertyName("app")]
        public string? App { get; set; }
        [JsonPropertyName("codeMachine")]
        public string? CodeMachine { get; set; }
        [JsonPropertyName("erp")]
        public string? Erp { get; set; }
        [JsonPropertyName("codeInit")]
        public string? InitializationCode { get; set; }
        [JsonPropertyName("version")]
        public string? Version { get; set; }
    }
}
