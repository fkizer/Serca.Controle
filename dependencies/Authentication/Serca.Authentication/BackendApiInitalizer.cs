using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serca.Authentication.Abstractions;
using Serca.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Authentication
{
    public class BackendApiInitalizer : IInitalizer
    {
        public HttpClient HttpClient { get; private set; }
        public ILogger<BackendApiInitalizer> Logger { get; private set; }

        private string Endpoint = string.Empty;

        public BackendApiInitalizer(HttpClient httpClient, ILogger<BackendApiInitalizer> logger, IConfiguration configuration)
        {
            HttpClient = httpClient;
            Logger = logger;
            Endpoint = configuration["BackendApiInitializationEndpoint"] ?? string.Empty;
        }

        public BackendApiInitalizer(HttpClient httpClient, ILogger<BackendApiInitalizer> logger, string endpoint)
        {
            HttpClient = httpClient;
            Logger = logger;
            Endpoint = endpoint;
        }

        public async Task<CredentialsEntity?> GetCredentialsAsync(IInitializationParameters? initializationRequestParameters)
        {
            if (initializationRequestParameters == null)
            {
                throw new ArgumentException("Parameters should not be empty", nameof(initializationRequestParameters));
            }

            //Converting object to dictionnary
            var serializedInitializationRequestParameters = System.Text.Json.JsonSerializer.Serialize(initializationRequestParameters);
            var initializationRequestParametersAsDico = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(serializedInitializationRequestParameters);

            var endpointWithParams = string.Concat(Endpoint.TrimEnd('/'), "?", initializationRequestParametersAsDico?.ToParamAsString()) ;

            var response = await HttpClient.GetAsync(endpointWithParams);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<CredentialsEntity>();
        }
    }
}
