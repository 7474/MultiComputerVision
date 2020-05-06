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

namespace BlazorMultiComputerVisionWebasm.Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly ILogger<UploadController> logger;
        private readonly IResultRepositoryService resultRepositoryService;

        private readonly IUploadService uploadService;
        private readonly AzureImageDetectService azureImageDetectService;
        private readonly AwsImageDetectService awsImageDetectService;
        private readonly GcpImageDetectService gcpImageDetectService;

        public UploadController(
            ILogger<UploadController> logger,
            IResultRepositoryService resultRepositoryService,
            IUploadService uploadService,
            AzureImageDetectService azureImageDetectService,
            AwsImageDetectService awsImageDetectService,
            GcpImageDetectService gcpImageDetectService
        )
        {
            this.logger = logger;
            this.resultRepositoryService = resultRepositoryService;

            this.uploadService = uploadService;
            this.azureImageDetectService = azureImageDetectService;
            this.awsImageDetectService = awsImageDetectService;
            this.gcpImageDetectService = gcpImageDetectService;
        }

        [HttpPost]
        public async Task<IResultDocument> PostAsync(UploadRequest req)
        {
            var blob = await uploadService.UploadFile(req.ToStream(), req.Name, true);
            var azureresult = await azureImageDetectService.Detect(blob.Uri.ToString());
            var awsresult = await awsImageDetectService.Detect(blob.Uri.ToString());
            var gcpresult = await gcpImageDetectService.Detect(blob.Uri.ToString());

            var doc = resultRepositoryService.BuildResult(User.Identity.Name, blob, new IImageDetectResult[] { azureresult, awsresult, gcpresult });
            await resultRepositoryService.Put(doc);

            return doc;
        }
    }

    public class UploadRequest
    {
        public string Name { get; set; }
        public string ImageDataUri { get; set; }

        public Stream ToStream()
        {
            var base64Data = Regex.Match(ImageDataUri, @"data:image/(?<type>.+?),(?<data>.+)").Groups["data"].Value;
            var binData = Convert.FromBase64String(base64Data);
            return new MemoryStream(binData);
        }
    }
}
