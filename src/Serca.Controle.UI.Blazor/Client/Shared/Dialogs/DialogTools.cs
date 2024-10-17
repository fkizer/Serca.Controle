using MudBlazor;

namespace Serca.Controle.UI.Blazor.Client.Shared.Dialogs
{
    public static class DialogTools
    {
        public static IDialogReference DisplayMessageBox(this IDialogService dialogService, string title, string text,
                                                         Color color = Color.Default, string buttonText = "Ok", string cancelButtonText = "Annuler")
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", text);
            parameters.Add("ButtonText", buttonText);
            parameters.Add("Color", color);
            parameters.Add("CancelButtonText", cancelButtonText);

            return dialogService.Show<ConfirmDialog>(title, parameters);
        }

        public static IDialogReference DisplayMessageBox(this IDialogService dialogService, DialogOptions dialogOptions, string title, string text,
                                                         Color color = Color.Default, string buttonText = "Ok", string cancelButtonText = "Annuler")
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", text);
            parameters.Add("ButtonText", buttonText);
            parameters.Add("Color", color);
            parameters.Add("CancelButtonText", cancelButtonText);

            return dialogService.Show<ConfirmDialog>(title, parameters, dialogOptions);
        }

        public static IDialogReference DisplayInfoBox(this IDialogService dialogService, DialogOptions dialogOptions, string title, string text)
        {
            var parameters = new DialogParameters();
            parameters.Add("ContentText", text);

            return dialogService.Show<InfoDialog>(title, parameters, dialogOptions);
        }
    }
}
