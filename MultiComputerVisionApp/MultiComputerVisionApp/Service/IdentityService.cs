using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiComputerVisionApp.Service
{
    public class IdentityService : IIdentityService
    {
        const string NativeAppClientRedirectUri = "urn:ietf:wg:oauth:2.0:oob";
        const string BaseUri = "https://localhost:5001/";
        public string CreateAuthorizationRequest()
        {
            // Create URI to authorization endpoint
            const string IdentityEndpoint = BaseUri + "Identity/Account/Login";
            var authorizeRequest = new RequestUrl(IdentityEndpoint);

            // Dictionary with values for the authorize request
            var dic = new Dictionary<string, string>();
            dic.Add("client_id", "MultiComputerVisionApp");
            dic.Add("response_type", "id_token token");
            dic.Add("scope", "openid profile");

            dic.Add("redirect_uri", NativeAppClientRedirectUri);
            dic.Add("nonce", Guid.NewGuid().ToString("N"));

            // Add CSRF token to protect against cross-site request forgery attacks.
            var currentCSRFToken = Guid.NewGuid().ToString("N");
            dic.Add("state", currentCSRFToken);

            var authorizeUri = authorizeRequest.Create(dic);
            return authorizeUri;
        }

        public string CreateLogoutRequest(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return string.Empty;
            }

            const string LogoutEndpoint = BaseUri + "Identity/Account/Logout";
            return string.Format("{0}?id_token_hint={1}&post_logout_redirect_uri={2}",
                LogoutEndpoint,
                token,
                NativeAppClientRedirectUri);
        }

        public bool IsLoginCallback(string url)
        {
            return NativeAppClientRedirectUri.Equals(url);
        }

        public bool IsLogoutCallback(string url)
        {
            return NativeAppClientRedirectUri.Equals(url);
        }
    }
}
