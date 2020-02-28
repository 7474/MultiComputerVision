using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMultiComputerVisionServer.Service
{
    public class BlobUploadService : IUploadService
    {
        BlobContainerClient container;
        // XXX Config interface
        public BlobUploadService(string connectionString, string containerName)
        {
            container = new BlobContainerClient(connectionString, containerName);
        }

        public async Task<ICloudFile> Upload(Stream content, string name, string contentType)
        {
            var ext = Path.GetExtension(name);
            var id = Guid.NewGuid();
            var key = id + ext;
            var blob = await container.UploadBlobAsync(key, content);

            return new Blob { Key = key, Uri = container.GetBlobClient(key).Uri };
        }
    }

    public class Blob : ICloudFile
    {
        public string Key { get; set; }

        public Uri Uri { get; set; }
    }
}
