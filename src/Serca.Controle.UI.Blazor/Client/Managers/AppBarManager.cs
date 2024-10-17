using MudBlazor;

namespace Serca.Controle.UI.Blazor.Client.Managers
{
    public class AppBarManager
    {
        public string BackIconButton { get; private set; } = default!;
        public Action BackButtonAction { get; private set; } = default!;
        public Action? SyncButtonAction { get; private set; } = default!;
        public Action? InfoButtonAction { get; private set; } = default!;
        public Color InfoButtonColor { get; private set; } = Color.Inherit;
        public event EventHandler? ButtonsChanged;

        public NavigationManagerExtended NavigationManagerExtended { get; set; }

        public AppBarManager(NavigationManagerExtended navigationManagerExtended)
        {
            NavigationManagerExtended = navigationManagerExtended;
            DisplayDefaultBackButton();
        }

        public void DisplayDefaultBackButton()
        {
            BackIconButton = Icons.Material.Filled.ArrowBackIosNew;
            BackButtonAction = NavigationManagerExtended.GoBack;
            OnButtonChanged();
        }

        public void DisplayCustomBackButton(string icon, Action clickAction)
        {
            BackIconButton = icon;
            BackButtonAction = clickAction;
            OnButtonChanged();
        }

        public void AssignSyncButtonAction(Action clickAction)
        {
            SyncButtonAction = clickAction;
            OnButtonChanged();
        }

        public void RemoveSyncButtonAction()
        {
            SyncButtonAction = null;
            OnButtonChanged();
        }

        public void AssignInfoButtonAction(Action clickAction)
        {
            InfoButtonAction = clickAction;
            OnButtonChanged();
        }

        public void ChangeInfoButtonColor(Color color)
        {
            InfoButtonColor = color;
            OnButtonChanged();
        }

        public void RemoveInfoButtonAction()
        {
            InfoButtonAction = null;
            OnButtonChanged();
        }

        private void OnButtonChanged()
        {
            ButtonsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
