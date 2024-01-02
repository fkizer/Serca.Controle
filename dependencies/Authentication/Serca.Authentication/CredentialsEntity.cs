using System.Text.Json.Serialization;

namespace Serca.Authentication
{
    public class CredentialsEntity
    {
        [JsonPropertyName("userm")]
        public string? Userm { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }
    }
}
