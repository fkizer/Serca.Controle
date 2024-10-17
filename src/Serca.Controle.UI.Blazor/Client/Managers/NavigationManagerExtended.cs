using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components;
using System.Text;

namespace Serca.Controle.UI.Blazor.Client.Managers
{
    public class NavigationManagerExtended
    {
        protected readonly ILogger<NavigationManagerExtended> Logger;
        protected readonly NavigationManager NavigationManager;
        protected List<string> PreviousPages = new();
        protected List<string> ProhibitedPages = new();

        public NavigationManagerExtended(NavigationManager navigationManager, ILogger<NavigationManagerExtended> logger)
        {
            Logger = logger;
            NavigationManager = navigationManager;
            NavigationManager.LocationChanged += NavigationManager_LocationChanged;
        }

        private void NavigationManager_LocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            var currentRoute = UriToRoute(e.Location);
            // If it is the initialization or authentication page, disable the history by cleaning it
            if (currentRoute.StartsWith(Routes.InitialisationPage.TrimStart('/')) || currentRoute.StartsWith(Routes.AuthenticationPage.TrimStart('/')))
            {
                PreviousPages.Clear();
                return;
            }

            //Catch navigator back action
            if (PreviousPages.Count() > 1 && PreviousPages[PreviousPages.Count() - 2].Equals(e.Location))
            {
                PopBackPage();
                return;
            }

            AddPageToHistory(e.Location);
        }

        public void AddPageToHistory(string pageName)
        {
            if (ProhibitedPages.Contains(pageName) || (PreviousPages.LastOrDefault()?.Equals(pageName) ?? false))
            {
                return;
            }

            // Check wildcard prohibited
            foreach (var prohibitedPage in ProhibitedPages.Where(x => x.EndsWith('*')))
            {
                if (pageName.StartsWith(prohibitedPage.TrimEnd('*')))
                {
                    return;
                }
            }

            if (pageName.EndsWith('*'))
            {
                PreviousPages.RemoveAll(x => x.StartsWith(pageName.TrimEnd('*')));
            }

            PreviousPages.Add(pageName);
            Logger.LogInformation(System.Text.Json.JsonSerializer.Serialize(PreviousPages));
        }

        public void AddProhibitedPage(string pageName)
        {
            if (ProhibitedPages.Contains(pageName)) return;

            ProhibitedPages.Add(pageName);
            Logger.LogInformation(System.Text.Json.JsonSerializer.Serialize(ProhibitedPages));

            //Checking if page is already in history (support wildcard exemple: authentication/*)
            if (pageName.EndsWith('*'))
            {
                PreviousPages.RemoveAll(x => x.StartsWith(pageName.TrimEnd('*')));
            }
            else
            {
                PreviousPages.Remove(pageName);
            }
        }

        public string PopBackPage()
        {
            if (PreviousPages.Count > 1)
            {
                if (!ProhibitedPages.Contains(NavigationManager.Uri))
                {
                    PreviousPages.RemoveAt(PreviousPages.Count - 1);
                }

                return PreviousPages.ElementAt(PreviousPages.Count - 1);
            }

            // Can't go back because you didn't navigate enough
            return PreviousPages.FirstOrDefault() ?? string.Empty;
        }

        public bool CanGoBack()
        {
            if (PreviousPages.Count == 0) return false;

            return (PreviousPages.Count == 1 && ProhibitedPages.Contains(NavigationManager.Uri)) || PreviousPages.Count > 1;
        }

        public void GoBack()
        {
            if (!CanGoBack())
            {
                Logger.LogWarning("Navigation arrière impossible");
                return;
            }

            var pageBack = PopBackPage();
            Logger.LogInformation(System.Text.Json.JsonSerializer.Serialize(PreviousPages));
            NavigationManager.NavigateTo(pageBack);
        }

        public void Dispose()
        {
            NavigationManager.LocationChanged -= NavigationManager_LocationChanged;
        }

        public string UriToRoute(string uri)
        {
            return uri.Replace(NavigationManager.BaseUri, "");
        }

        public override string ToString()
        {
            var sb = new StringBuilder("Page History :");

            foreach (var pageName in PreviousPages)
            {
                sb.AppendLine(pageName);
            }

            return sb.ToString();

        }
    }
}
