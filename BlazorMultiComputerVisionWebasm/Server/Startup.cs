using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Linq;
using MultiComputerVisionIdentityService.Data;
using MultiComputerVisionIdentityService.Models;
using MultiComputerVisionService.Service;
using Amazon;
using Amazon.Runtime;
using MultiComputerVisionService.Service.Application;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BlazorMultiComputerVisionWebasm.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var cosmos = new CosmosResultRepositoryService(Configuration.GetConnectionString("AzureCosmos"));
            cosmos.Initialize(Configuration.GetValue<string>("Cosmos:DatabaseId"));
            services.AddSingleton<IResultRepositoryService>(cosmos);

            services.AddSingleton<IUploadService>(new BlobUploadService(
                            Configuration.GetConnectionString("BlobStorage"),
                            Configuration.GetValue<string>("Blob:ContainerName")
                            ));
            services.AddSingleton(new AzureImageDetectService(
                            Configuration.GetValue<string>("AzureCognitiveConfig:ComputerVisionSubscriptionKey"),
                            Configuration.GetValue<string>("AzureCognitiveConfig:ComputerVisionEndpoint")
                            ));
            services.AddSingleton(new AwsImageDetectService(new BasicAWSCredentials(
                            Configuration.GetValue<string>("AWS:AccessKeyID"),
                            Configuration.GetValue<string>("AWS:SecretAccessKey")
                ), RegionEndpoint.GetBySystemName(Configuration.GetValue<string>("AWS:Region"))));
            services.AddSingleton(new GcpImageDetectService(
                Configuration.GetValue<string>("GCP:JsonCredentials")
                ));

            services.AddSingleton<IResultDocumentService, ServerSideResultDocumentService>();
            services.AddSingleton<IUploadImageService, ServerSideUploadImageService>();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<ApplicationUser, ApplicationDbContext>();

            // IdentityServer: https://identityserver4-ja.readthedocs.io/ja/latest/topics/signin_external_providers.html
            // XamarinEssentials: https://docs.microsoft.com/ja-jp/mobile-blazor-bindings/advanced/xamarin-essentials?tabs=windows%2Candroid
            // WebAuthenticator: https://docs.microsoft.com/ja-jp/xamarin/essentials/web-authenticator?tabs=android
            // IdentityServer .net: https://docs.microsoft.com/ja-jp/aspnet/core/security/authentication/identity-api-authorization?view=aspnetcore-3.1#create-an-app-with-api-authorization-support
            // Profile: https://github.com/dotnet/aspnetcore/issues/20248
            services.AddAuthentication()
                .AddIdentityServerJwt()
                .AddTwitter(twitterOptions =>
                {
                    twitterOptions.ConsumerKey = Configuration["Authentication:Twitter:ConsumerAPIKey"];
                    twitterOptions.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
                    twitterOptions.RetrieveUserDetails = true;
                })
                .AddCookie(o =>
                {
                    o.LoginPath = "/Identity/Account/Login";
                })
                ;

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
            services.AddScoped<SignOutSessionStateManager>();
            services.AddHeadElementHelper();

            Client.Program.ConfigureCommonServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();

            app.UseHeadElementServerPrerendering();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
