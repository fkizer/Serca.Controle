using Serca.Authentication;
using Serca.Authentication.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serca.Controle.Infrastructures.API.Fakers
{
    public class FakeBackendApiInitializer : IInitalizer
    {
        public HttpClient HttpClient { get; private set; }

        public FakeBackendApiInitializer(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public async Task<CredentialsEntity?> GetCredentialsAsync(IInitializationParameters? initializationRequestParameters)
        {
            return await Task.Run(() =>
            {
                return new CredentialsEntity() { Userm = "EXTQUAP", Password = "bjYTDWLajxnIGAf" };
            });
        }
    }
}
