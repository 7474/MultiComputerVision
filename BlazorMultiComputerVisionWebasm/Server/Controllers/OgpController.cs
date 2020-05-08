using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MultiComputerVisionService.Service;
using System.IO;
using System.Text.RegularExpressions;
using IdentityServer4.Extensions;
using BlazorMultiComputerVisionWebasm.Server.ViewModels;
using MultiComputerVisionService.Extension;

namespace BlazorMultiComputerVisionWebasm.Server.Controllers
{
    public class OgpController : Controller
    {
        private readonly ILogger<OgpController> logger;
        private readonly IResultRepositoryService resultRepositoryService;

        public OgpController(
            ILogger<OgpController> logger,
            IResultRepositoryService resultRepositoryService
        )
        {
            this.logger = logger;
            this.resultRepositoryService = resultRepositoryService;
        }

        [HttpGet("/results/{id}")]
        public async Task<IActionResult> GetResultDocumentByIdAsync(Guid id)
        {
            var doc = await resultRepositoryService.GetResult(id);
            var ogp = new Ogp
            {
                Title = doc.GetTitle(),
                Description = doc.GetDescription(),
                Image = doc.Image.Uri,
            };
            return View("Ogp", ogp);
            // XXX 404とか処理する
        }
    }

}
