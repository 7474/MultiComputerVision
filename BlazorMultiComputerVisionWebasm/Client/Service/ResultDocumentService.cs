using MultiComputerVisionService.Models;
using MultiComputerVisionService.Service;
using MultiComputerVisionService.Service.Application;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace BlazorMultiComputerVisionWebasm.Client.Service
{
    public class ResultDocumentService : IResultDocumentService
    {
        private readonly HttpClient httpClient;

        public ResultDocumentService(AllowGuestHttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IResultDocument> GetResult(Guid id)
        {
            return await httpClient.GetFromJsonAsync<PlainResultDocument>($"Api/ResultDocument/{id}");
        }

        public async Task<IList<IResultDocument>> GetResults(DateTimeOffset offset)
        {
            return await httpClient.GetFromJsonAsync<PlainResultDocument[]>($"Api/ResultDocument?offset={offset.UtcTicks}");
        }
    }
}
