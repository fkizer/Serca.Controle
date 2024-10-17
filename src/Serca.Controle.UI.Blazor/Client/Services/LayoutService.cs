using MudBlazor;

namespace Serca.Controle.UI.Blazor.Client.Services
{
    public class LayoutService
    {
        //public bool IsDarkMode { get; private set; } = false;

        public MudTheme CurrentTheme { get; private set; }

        public void SetBaseTheme(MudTheme theme)
        {
            CurrentTheme = theme;
        }
    }
}
