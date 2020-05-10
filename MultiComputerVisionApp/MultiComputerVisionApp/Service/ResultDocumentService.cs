using MultiComputerVisionService.Models;
using MultiComputerVisionService.Service;
using MultiComputerVisionService.Service.Application;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace MultiComputerVisionApp.Service
{
    public class ResultDocumentService : IResultDocumentService
    {
        private readonly HttpClient httpClient;

        public ResultDocumentService(AllowGuestHttpClient httpClient)
        {
            this.httpClient = httpClient;
            System.Diagnostics.Debug.WriteLine($"new ResultDocumentService()");
        }

        public async Task<IResultDocument> GetResult(Guid id)
        {
            System.Diagnostics.Debug.WriteLine($"ResultDocumentService.GetResult({id})");
            return await httpClient.GetFromJsonAsync<PlainResultDocument>($"Api/ResultDocument/{id}");
        }

        public async Task<IList<IResultDocument>> GetResults(DateTimeOffset offset)
        {
            System.Diagnostics.Debug.WriteLine($"ResultDocumentService.GetResults({offset})");
            return await httpClient.GetFromJsonAsync<PlainResultDocument[]>($"Api/ResultDocument?offset={offset.UtcTicks}");
        }
    }
}
