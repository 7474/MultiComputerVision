using System;
using Microsoft.MobileBlazorBindings;
using Microsoft.Extensions.Hosting;
using Xamarin.Essentials;
using Xamarin.Forms;
using Microsoft.Extensions.DependencyInjection;
using MultiComputerVisionService.Service.Application;
using MultiComputerVisionApp.Service;

namespace MultiComputerVisionApp
{
    public class App : Application
    {
        public App()
        {
            var host = MobileBlazorBindingsHost.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    // Register app-specific services
                    //services.AddSingleton<AppState>();
                    services.AddSingleton(new AllowGuestHttpClient { BaseAddress = new Uri("https://blazormulticomputervisionwebasmserver.azurewebsites.net/") });
                    services.AddSingleton<IResultDocumentService, ResultDocumentService>();

                    services.AddSingleton<AppState>();
                })
                .Build();

            MainPage = new TabbedPage();
            host.AddComponent<MCVApp>(parent: MainPage);
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
