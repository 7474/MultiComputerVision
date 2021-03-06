﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MultiComputerVisionService.Models;
using MultiComputerVisionService.Service;

namespace BlazorMultiComputerVisionWebasm.Server.Controllers.Api
{
    [ApiController]
    [Route("Api/[controller]")]
    public class ResultDocumentController : ControllerBase
    {
        private readonly ILogger<ResultDocumentController> logger;
        private readonly IResultRepositoryService resultRepositoryService;

        public ResultDocumentController(ILogger<ResultDocumentController> logger, IResultRepositoryService resultRepositoryService)
        {
            this.logger = logger;
            this.resultRepositoryService = resultRepositoryService;
        }

        [HttpGet]
        public async Task<IEnumerable<IResultDocument>> GetAsync(long offset = 0)
        {
            return await resultRepositoryService.GetResults(DateTimeOffset.FromUnixTimeMilliseconds(offset));
        }

        [HttpGet("{id}")]
        public async Task<IResultDocument> GetByIdAsync(Guid id)
        {
            var doc = await resultRepositoryService.GetResult(id);
            return doc;
            // XXX 404とか処理する
        }
    }
}
