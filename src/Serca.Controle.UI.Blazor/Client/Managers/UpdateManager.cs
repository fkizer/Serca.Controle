using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace Serca.Controle.UI.Blazor.Client.Managers
{
    public class UpdateManager
    {
        protected readonly ILogger<UpdateManager> Logger;
        protected readonly IJSRuntime JS;
        protected readonly ISnackbar Snackbar;
        protected readonly NavigationManager NavigationManager;

        public bool UpdateAvailable { get; private set; }

        public event EventHandler? UpdateDetected;

        public UpdateManager(IJSRuntime js, ISnackbar snackbar, NavigationManager navigationManager, ILogger<UpdateManager> logger)
        {
            Logger = logger;
            JS = js;
            Snackbar = snackbar;
            NavigationManager = navigationManager;
        }

        public async Task CheckForUpdate()
        {
            await RegisterForUpdateAvailableNotification();
        }

        private async Task RegisterForUpdateAvailableNotification()
        {
            await JS.InvokeAsync<object>(
                identifier: "registerForUpdateAvailableNotification",
                DotNetObjectReference.Create(this),
                nameof(OnUpdateAvailable));
        }

        [JSInvokable(nameof(OnUpdateAvailable))]
        public Task OnUpdateAvailable()
        {
            SetUpdateAvailable();
            Snackbar.Clear();
            Snackbar.Configuration.PositionClass = Defaults.Classes.Position.TopCenter;
            Snackbar.Add("Nouvelle version disponible. Cliquez ici pour l'installer", Severity.Warning, conf =>
            {
                conf.Onclick = (b) =>
                {
                    return Task.Run(() =>
                    {
                        UpdateVersion();

                    });
                };
                conf.ShowCloseIcon = true;
                conf.RequireInteraction = true;
            });


            return Task.CompletedTask;
        }

        public void SetUpdateAvailable()
        {
            UpdateAvailable = true;
            OnUpdateDetected();
        }

        public void UpdateVersion()
        {
            NavigationManager.NavigateTo(NavigationManager.Uri, forceLoad: true);
        }

        protected void OnUpdateDetected()
        {
            EventHandler? handler = UpdateDetected;
            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}
