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
        const string callbackScheme = "multicomputervision";

        [HttpGet("{scheme}")]
        public async Task Get([FromRoute]string scheme)
        {
            var auth = await Request.HttpContext.AuthenticateAsync(scheme);

            if (!auth.Succeeded
                || auth?.Principal == null
                || !auth.Principal.Identities.Any(id => id.IsAuthenticated)
                // XXX アクセストークンの取り方がイマイチ分からない。
                // Profile NativeApp のサポートが厚くなったら再考しよう。
                //|| string.IsNullOrEmpty(auth.Properties.GetTokenValue("access_token"))
                )
            {
                // Not authenticated, challenge
                await Request.HttpContext.ChallengeAsync(scheme);
            }
            else
            {
                // Get parameters to send back to the callback
                var qs = new Dictionary<string, string>();
                auth.Properties.Items.Keys.ToList().ForEach(
                    k => qs[k] = auth.Properties.Items[k]
                );

                qs["Cookie"] = Request.Headers["Cookie"].FirstOrDefault()?.ToString();
                qs["access_token"] = auth.Properties.GetTokenValue("access_token");
                qs["refresh_token"] = auth.Properties.GetTokenValue("refresh_token") ?? string.Empty;
                qs["expires"] = (auth.Properties.ExpiresUtc?.ToUnixTimeSeconds() ?? -1).ToString();

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
