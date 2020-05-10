using MultiComputerVisionService.Models;
using MultiComputerVisionService.Service;
using MultiComputerVisionService.Service.Application;
using System;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

namespace BlazorMultiComputerVisionServer.Service
{
    public class UploadImageService : AbstractUploadService
    {
        public UploadImageService(
            IResultRepositoryService resultRepositoryService,
            IUploadService uploadService,
            AzureImageDetectService azureImageDetectService,
            AwsImageDetectService awsImageDetectService,
            GcpImageDetectService gcpImageDetectService
            ) : base(resultRepositoryService, uploadService, azureImageDetectService, awsImageDetectService, gcpImageDetectService)
        {
        }

        public override async Task<IResultDocument> Upload(Stream content, string fileName, IPrincipal user)
        {
            return await UploadInternal(content, fileName, user);
        }
    }
}
