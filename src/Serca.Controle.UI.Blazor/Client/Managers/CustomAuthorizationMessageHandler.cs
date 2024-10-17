using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System.Net.Http.Headers;

namespace Serca.Controle.UI.Blazor.Client.Managers
{
    public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public CustomAuthorizationMessageHandler(IAccessTokenProvider provider, NavigationManager navigationManager, NetworkConfiguration configure)
            : base(provider, navigationManager)
        {
            ConfigureHandler(configure.AsEnumerable());
        }
    }

    public class NetworkConfiguration
    {
        public string? BackendServerUrl { get; set; }
        public string? IdentityServerUrl { get; set; }

        private List<string> AttachedAuthrizationMessageHandler = new List<string>();

        public IEnumerable<string?> AsEnumerable() => AttachedAuthrizationMessageHandler;

        public void AttachAuthorizationMessageHandler(string? url)
        {
            if (string.IsNullOrEmpty(url) || AttachedAuthrizationMessageHandler.Contains(url)) return;

            AttachedAuthrizationMessageHandler.Add(url);
        }
    }
}
