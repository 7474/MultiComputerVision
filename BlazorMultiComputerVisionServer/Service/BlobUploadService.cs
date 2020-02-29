using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.StaticFiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMultiComputerVisionServer.Service
{
    public class BlobUploadService : IUploadService
    {
        IContentTypeProvider contentTypeProvider;
        BlobContainerClient container;

        // XXX Config interface
        public BlobUploadService(string connectionString, string containerName)
        {
            container = new BlobContainerClient(connectionString, containerName);
            contentTypeProvider = new FileExtensionContentTypeProvider();
        }

        public Task<ICloudFile> UploadFile(Stream content, string fileName, bool detectContentType)
        {
            var id = Guid.NewGuid();
            string contentType = null;
            if (detectContentType)
            {
                contentTypeProvider.TryGetContentType(fileName, out contentType);
            }
            // XXX ディレクトリ指定できた方が管理上は良さそう。
            return Upload(content, id.ToString(), contentType ?? "application/octet-stream");
        }

        public async Task<ICloudFile> Upload(Stream content, string blobName, string contentType)
        {
            var blobClient = container.GetBlobClient(blobName);
            await blobClient.UploadAsync(content, new BlobHttpHeaders()
            {
                ContentType = contentType,
            });
            // .ConfigureAwait(true);

            return new Blob { BlobName = blobName, Uri = blobClient.Uri };
        }

        public ICloudFile GetInfo(string blobName)
        {
            var blobClient = container.GetBlobClient(blobName);
            return new Blob { BlobName = blobName, Uri = blobClient.Uri };
        }
    }

    public class Blob : ICloudFile
    {
        public string BlobName { get; set; }

        public Uri Uri { get; set; }
    }
}
