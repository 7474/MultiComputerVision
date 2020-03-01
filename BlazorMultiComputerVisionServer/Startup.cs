using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using BlazorMultiComputerVisionServer.Areas.Identity;
using BlazorMultiComputerVisionServer.Data;
using BlazorMultiComputerVisionServer.Service;
using Amazon.Runtime;

namespace BlazorMultiComputerVisionServer
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
            cosmos.Initialize("MultiComputerVision");
            services.AddSingleton<IResultRepositoryService>(cosmos);
            var cosmosDbContext = new CosmosDbContext(Configuration.GetConnectionString("AzureCosmos"));
            cosmosDbContext.Initialize("MultiComputerVision");
            services.AddSingleton(cosmosDbContext);
            services.AddSingleton<IUserStore<ApplicationUser>, CosmosUserStore>();

            services.AddDefaultIdentity<ApplicationUser>()
                .AddUserStore<CosmosUserStore>()
                .AddUserManager<UserManager<ApplicationUser>>();

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            services.AddSingleton<WeatherForecastService>();

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
                )));
            services.AddSingleton(new GcpImageDetectService(
                Configuration.GetValue<string>("GCP:JsonCredentials")
                ));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
