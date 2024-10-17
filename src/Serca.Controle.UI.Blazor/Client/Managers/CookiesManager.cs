using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Serca.Controle.UI.Blazor.Client.Managers
{
    public class CookiesManager
    {
        public const string CookiesIDSRVUserName = "idsrvuser";
        public const string CookiesIDSRVPasswordName = "idsrvpwd";
        public const string CookiesIDSRVErpName = "idsrverp";
        public const string CookiesIDSRVAppName = "idsrvapp";
        public const string CookiesIDSRVAppVersionName = "idsrvappversion";

        protected readonly IJSRuntime JSRuntime;
        protected readonly NavigationManager NavigationManager;

        private string _Domain;
        private int _ExpireIn = 5;

        public CookiesManager(IJSRuntime jSRuntime, NavigationManager navigationManager)
        {
            JSRuntime = jSRuntime;
            NavigationManager = navigationManager;
            GetDomain();
        }

        public async Task CreateCookie(string name, string value, int minutes, string _Domain)
        {
            await JSRuntime.InvokeAsync<string>("Cookies.CreateCookie", name, value, minutes, _Domain);
        }

        public async Task<string> GetCookies()
        {
            return await JSRuntime.InvokeAsync<string>("Cookies.GetCookies");
        }

        public async Task CreateInitCookie(string userm, string password, string erp, string app, string version)
        {
            await CreateCookie(CookiesIDSRVUserName, userm, _ExpireIn, _Domain);
            await CreateCookie(CookiesIDSRVPasswordName, password, _ExpireIn, _Domain);
            await CreateCookie(CookiesIDSRVErpName, erp, _ExpireIn, _Domain);
            await CreateCookie(CookiesIDSRVAppName, app, _ExpireIn, _Domain);
            await CreateCookie(CookiesIDSRVAppVersionName, version, _ExpireIn, _Domain);
        }

        public async Task DeleteInitCookie()
        {
            await CreateCookie(CookiesIDSRVUserName, string.Empty, _ExpireIn, _Domain);
            await CreateCookie(CookiesIDSRVPasswordName, string.Empty, _ExpireIn, _Domain);
            await CreateCookie(CookiesIDSRVErpName, string.Empty, _ExpireIn, _Domain);
            await CreateCookie(CookiesIDSRVAppName, string.Empty, _ExpireIn, _Domain);
            await CreateCookie(CookiesIDSRVAppVersionName, string.Empty, _ExpireIn, _Domain);
        }

        public async Task<bool> IsInitCoookieOk()
        {
            var cookieStr = await GetCookies();

            if (string.IsNullOrEmpty(cookieStr))
            {
                return false;
            }

            var cookiesRequired = new List<string>()
            {
                CookiesIDSRVUserName,
                CookiesIDSRVPasswordName,
                CookiesIDSRVErpName,
                CookiesIDSRVAppName,
                CookiesIDSRVAppVersionName
            };

            var currentCookies = CookiesStrToDico(cookieStr);

            foreach (var cookieRequire in cookiesRequired)
            {
                if (!currentCookies.ContainsKey(cookieRequire))
                {
                    return false;
                }
            }

            return true;
        }

        private Dictionary<string, string> CookiesStrToDico(string cookiesStr)
        {
            var currentCookie = new Dictionary<string, string>();

            var cookies = cookiesStr.Split(";");

            foreach (var cookie in cookies)
            {
                var KeyValue = cookie.Split('=');
                var key = KeyValue[0];
                var value = KeyValue[1];


                if (KeyValue.Length == 2)
                {
                    currentCookie.Add(key, value);
                }
            }

            return currentCookie;
        }

        private void GetDomain()
        {
            var uri = new Uri(NavigationManager.BaseUri);
            _Domain = uri.Host;

            var host_sub_parts = uri.Host.Split('.');
            if (host_sub_parts.Length > 1)
            {
                _Domain = string.Join(".", host_sub_parts.Skip(Math.Max(0, host_sub_parts.Count() - 2)));
            }
        }
    }
}
