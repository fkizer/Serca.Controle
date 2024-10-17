using Serca.Tools.Errors;

namespace Serca.Controle.UI.Blazor.Client.Managers
{
    public class NotificationManager
    {
        public Action<ErrorViewModel>? ProcessNewWebservicesFailOver { get; set; }
    }
}
