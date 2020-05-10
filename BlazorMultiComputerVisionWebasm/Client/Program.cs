using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MultiComputerVisionService.Service.Application;
using BlazorMultiComputerVisionWebasm.Client.Service;
using Toolbelt.Blazor.Extensions.DependencyInjection;

namespace BlazorMultiComputerVisionWebasm.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            // XXX これだと認証されたユーザーのリクエストからもパラメータが無くなっちゃうんだよね。困るね。
            builder.Services.AddSingleton(new AllowGuestHttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

            builder.Services.AddHttpClient<AuthHttpClient>("BlazorMultiComputerVisionWebasm.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("BlazorMultiComputerVisionWebasm.ServerAPI"));

            builder.Services.AddSingleton<IResultDocumentService, ResultDocumentService>();
            builder.Services.AddSingleton<IUploadImageService, UploadImageService>();

            builder.Services.AddApiAuthorization();

            builder.Services.AddHeadElementHelper();

            ConfigureCommonServices(builder.Services);

            await builder.Build().RunAsync();
        }

        public static void ConfigureCommonServices(IServiceCollection services)
        {
            // Ref. https://docs.microsoft.com/ja-jp/aspnet/core/security/blazor/webassembly/additional-scenarios?view=aspnetcore-3.1#support-prerendering-with-authentication
            // Common service registrations
        }
    }
}
