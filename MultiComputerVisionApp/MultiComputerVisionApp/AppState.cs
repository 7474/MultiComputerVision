using Google.Api;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MultiComputerVisionApp
{
    // https://github.com/xamarin/MobileBlazorBindings/blob/master/samples/MobileBlazorBindingsTodoSample/MobileBlazorBindingsTodo/AppState.cs
    public class AppState
    {
        private readonly string baseUri = "https://blazormulticomputervisionwebasmserver.azurewebsites.net/";
        //private readonly string baseUri = "https://10.0.2.2:5001/";
        private readonly string authScheme = "Identity.Application";

        private WebAuthenticatorResult auth;
        public WebAuthenticatorResult Auth => auth;
        public async Task SetAuthAsync(WebAuthenticatorResult auth)
        {
            this.auth = auth;
            await NotifyStateChanged();
        }
        public bool IsLogged => Auth != null;

        public async Task LoginAsync()
        {
            var authResult = await WebAuthenticator.AuthenticateAsync(
                new Uri($"{baseUri}mobileauth/{authScheme}"),
                new Uri("multicomputervision://"));

            await SetAuthAsync(authResult);
        }

        public async Task LogoutAsync()
        {
            // XXX ちゃんとログアウトする       
            await SetAuthAsync(null);
        }

        private string GetAuthCookie()
        {
            if (Auth != null)
            {
                return WebUtility.UrlDecode(Auth.Properties["Cookie"]);
            }
            return null;
        }

        public HttpClient AuthHttpClient
        {
            get
            {
                var http = new HttpClient();
                http.BaseAddress = new Uri(baseUri);
                var cookie = GetAuthCookie();
                if (!string.IsNullOrEmpty(cookie))
                {
                    http.DefaultRequestHeaders.Add("Cookie", cookie);
                }
                return http;
            }
        }

        public event Func<Task> OnChange;

        private async Task NotifyStateChanged()
        {
            if (OnChange != null)
            {
                await OnChange.Invoke();
            }
        }
    }
}
