using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMultiComputerVisionServer.Service
{
    public interface IUploadService
    {
        Task<ICloudFile> UploadFile(Stream content, string fileName, bool detectContentType);
        Task<ICloudFile> Upload(Stream content, string blobName, string contentType);
        ICloudFile GetInfo(string blobName);
    }

    public interface ICloudFile
    {
        string BlobName { get; }
        Uri Uri { get; }
    }
}
