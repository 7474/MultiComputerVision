using MultiComputerVisionService.Models;
using MultiComputerVisionService.Service;
using MultiComputerVisionService.Service.Application;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiComputerVisionService.Service.Application
{
    public class ServerSideResultDocumentService : IResultDocumentService
    {
        private readonly IResultRepositoryService resultRepositoryService;

        public ServerSideResultDocumentService(IResultRepositoryService resultRepositoryService)
        {
            this.resultRepositoryService = resultRepositoryService;
        }

        public async Task<IResultDocument> GetResult(Guid id)
        {
            return await resultRepositoryService.GetResult(id);
        }

        public async Task<IList<IResultDocument>> GetResults(DateTimeOffset offset)
        {
            return await resultRepositoryService.GetResults(offset);
        }
    }
}
