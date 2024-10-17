using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Serca.Controle.Core.Application.Constants;
using System;
using System.Security.Claims;

namespace Serca.Controle.UI.Blazor.Client.Pages
{
    public partial class Authentication
    {
        [Parameter] public string? Action { get; set; }

        [Inject] public ILogger<Authentication>? Logger { get; set; }


        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationState { get; set; }

        public bool ShouldInitialize => (!ContextService?.User?.IsInitialized ?? false) && Action != "logout" && Action != "logged-out";
        protected bool Initializing;

        protected override void OnInitialized()
        {
            if (ShouldInitialize)
            {
                NavigationManager.NavigateTo(Routes.InitialisationPage);
                return;
            }

            if (string.IsNullOrWhiteSpace(Action))
            {
                if (!ContextService?.User?.IsAuthenticated ?? true)
                {
                    Action = "login";
                }
                else
                {
                    NavigationManager.NavigateTo(Routes.Index);
                }
            }
        }
        protected override async Task OnParametersSetAsync()
        {
            if (Action == "login" && AuthenticationState != null)
            {
                Initializing = true;
                var authState = await AuthenticationState;
                if (!authState?.User?.Identity?.IsAuthenticated ?? false)
                {
                    await EnsureAuthCookieCreated();
                }
                Initializing = false;
            }
        }

        public async Task OnLogInSucceeded()
        {
            await CookieManager.DeleteInitCookie();

            if (AuthenticationState == null || ContextService.IsSerigInstance)
            {
                return;
            }

            var authState = await AuthenticationState;
            if (authState.User?.Identity?.IsAuthenticated ?? false)
            {
                await UpdateContextServiceAsync(authState.User?.Claims);
            }
        }

        protected void OnLogOutSucceeded(RemoteAuthenticationState remoteAuthenticationState)
        {
            ContextService.Logout();
            BrowserCache.DeleteAsync("app.sqlite3");
            NavigationManager.NavigateTo("/", forceLoad: true, replace: true);
        }

        protected async Task EnsureAuthCookieCreated()
        {
            if (await CookieManager.IsInitCoookieOk())
            {
                return;
            }

            // Store cookies for oidc login form
            await CookieManager.CreateInitCookie(ContextService.User!.Userm!,
                ContextService.User!.Password!,
                ContextService.User!.Erp!,
                ApplicationConstants.ApplicationName,
                ContextService.Version);
        }

        protected async Task UpdateContextServiceAsync(IEnumerable<Claim>? claims)
        {
            if (claims == null || !claims.Any())
            {
                Logger?.LogWarning("No claims");
                return;
            }

            if (ContextService.User == null)
            {
                Logger?.LogWarning("User not defined in ContextService");
                return;
            }

            foreach (var item in claims)
            {
                switch (item.Type)
                {
                    case "ste":
                        ContextService.User.Ste = ushort.Parse(item.Value);
                        break;
                    case "depo":
                        ContextService.User.Depo = ushort.Parse(item.Value);
                        break;
                    default:
                        Logger?.LogInformation($"Not mapped value : {item.Type} - {item.Value}");
                        break;
                }
            }

            await ContextService.SaveAsync();
        }
    }
}
