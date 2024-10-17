using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components;

namespace Serca.Controle.UI.Blazor.Client.Managers
{
    public class AuthManager
    {
        public NavigationManager NavigationManager { get; set; }
        public SignOutSessionStateManager SignOutManager { get; set; }

        public AuthManager(NavigationManager navigationManager, SignOutSessionStateManager signOutManager)
        {
            NavigationManager = navigationManager;
            SignOutManager = signOutManager;
        }

        public async Task Login()
        {
            await SignOutManager.SetSignOutState();
            NavigationManager.NavigateTo("authentication/login");
        }

        // Logout = Reset
        public async Task Logout()
        {
            await SignOutManager.SetSignOutState();
            NavigationManager.NavigateTo("authentication/logout");
        }
    }
}
