using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MultiComputerVisionIdentityService.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BlazorMultiComputerVisionWebasm.Server.Controllers
{
    [Route("mobileauth")]
    [ApiController]
    public class MobileAuthController : ControllerBase
    {
        private readonly ILogger<MobileAuthController> logger;
        private readonly UserManager<ApplicationUser> userManager;
        public MobileAuthController(ILogger<MobileAuthController> logger, UserManager<ApplicationUser> userManager)
        {
            this.logger = logger;
            this.userManager = userManager;
        }
        const string callbackScheme = "xamarinessentials";

        [HttpGet("{scheme}")]
        public async Task Get([FromRoute]string scheme)
        {
            var auth = await Request.HttpContext.AuthenticateAsync(scheme);
            var jwtAuth = await Request.HttpContext.AuthenticateAsync(IdentityServerJwtConstants.IdentityServerJwtScheme);
            var appAuth = await Request.HttpContext.AuthenticateAsync(IdentityConstants.ApplicationScheme);
            var access_token = await Request.HttpContext.GetTokenAsync(IdentityConstants.ApplicationScheme, OpenIdConnectParameterNames.AccessToken);
            var refresh_token = await Request.HttpContext.GetTokenAsync(IdentityConstants.ApplicationScheme, OpenIdConnectParameterNames.RefreshToken);
            var expires = await Request.HttpContext.GetTokenAsync(IdentityConstants.ApplicationScheme, OpenIdConnectParameterNames.ExpiresIn);
            if (auth != null && auth.Principal != null)
            {
                var user = await userManager.GetUserAsync(auth.Principal);
            }
            //var token = await userManager.GetAuthenticationTokenAsync(user, )
            //logger.LogDebug(JsonConvert.SerializeObject(auth, Formatting.Indented));

            if (!auth.Succeeded
                || auth?.Principal == null
                || !auth.Principal.Identities.Any(id => id.IsAuthenticated)
                || string.IsNullOrEmpty(auth.Properties.GetTokenValue("access_token")))
            {
                // Not authenticated, challenge
                //await Request.HttpContext.ChallengeAsync(scheme);
                if (scheme == "Cookies")
                {
                    var authProps = new AuthenticationProperties(); 
                    authProps.SetParameter("response_mode", "query");
                    authProps.SetParameter("response_type", "id_token token");
                    authProps.SetParameter("client_id", "MultiComputerVisionApp");
                    authProps.SetParameter("scope", "BlazorMultiComputerVisionWebasm.ServerAPI openid profile");

                    await Request.HttpContext.ChallengeAsync(scheme, authProps);
                }
                else
                {
                    await Request.HttpContext.ChallengeAsync(scheme);
                }
            }
            else
            {
                // Get parameters to send back to the callback
                var qs = new Dictionary<string, string>
            {
                { "access_token", auth.Properties.GetTokenValue("access_token") },
                { "refresh_token", auth.Properties.GetTokenValue("refresh_token") ?? string.Empty },
                { "expires", (auth.Properties.ExpiresUtc?.ToUnixTimeSeconds() ?? -1).ToString() }
            };

                // Build the result url
                var url = callbackScheme + "://#" + string.Join(
                    "&",
                    qs.Where(kvp => !string.IsNullOrEmpty(kvp.Value) && kvp.Value != "-1")
                    .Select(kvp => $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

                // Redirect to final url
                Request.HttpContext.Response.Redirect(url);
            }
        }
    }
}
