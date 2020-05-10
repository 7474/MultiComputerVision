using MultiComputerVisionService.Models;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

namespace MultiComputerVisionService.Service.Application
{
    public abstract class AbstractUploadService : IUploadImageService
    {
        private readonly IResultRepositoryService resultRepositoryService;

        private readonly IUploadService uploadService;
        private readonly AzureImageDetectService azureImageDetectService;
        private readonly AwsImageDetectService awsImageDetectService;
        private readonly GcpImageDetectService gcpImageDetectService;

        public AbstractUploadService(
            IResultRepositoryService resultRepositoryService,
            IUploadService uploadService,
            AzureImageDetectService azureImageDetectService,
            AwsImageDetectService awsImageDetectService,
            GcpImageDetectService gcpImageDetectService
        )
        {
            this.resultRepositoryService = resultRepositoryService;

            this.uploadService = uploadService;
            this.azureImageDetectService = azureImageDetectService;
            this.awsImageDetectService = awsImageDetectService;
            this.gcpImageDetectService = gcpImageDetectService;
        }

        public abstract Task<IResultDocument> Upload(Stream content, string fileName, IPrincipal user);

        protected async Task<IResultDocument> UploadInternal(Stream content, string fileName, IPrincipal user)
        {
            var blob = await uploadService.UploadFile(content, fileName, true);
            var azureresult = await azureImageDetectService.Detect(blob.Uri.ToString());
            var awsresult = await awsImageDetectService.Detect(blob.Uri.ToString());
            var gcpresult = await gcpImageDetectService.Detect(blob.Uri.ToString());

            var doc = resultRepositoryService.BuildResult(user.Identity.Name, blob, new IImageDetectResult[] { azureresult, awsresult, gcpresult });
            await resultRepositoryService.Put(doc);

            return doc;
        }
    }
}
