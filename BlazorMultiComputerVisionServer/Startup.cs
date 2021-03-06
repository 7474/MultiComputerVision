using Amazon;
using Amazon.Runtime;
using BlazorMultiComputerVisionServer.Areas.Identity;
using MultiComputerVisionIdentityService.Data;
using MultiComputerVisionIdentityService.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MultiComputerVisionService.Service;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using MultiComputerVisionService.Service.Application;

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
            cosmos.Initialize(Configuration.GetValue<string>("Cosmos:DatabaseId"));
            services.AddSingleton<IResultRepositoryService>(cosmos);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddUserManager<UserManager<ApplicationUser>>(); ;

            services.AddHeadElementHelper();
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseHeadElementServerPrerendering();
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
