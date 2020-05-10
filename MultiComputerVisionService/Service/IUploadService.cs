using MultiComputerVisionService.Models;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MultiComputerVisionService.Service
{
    public interface IUploadService
    {
        Task<ICloudFile> UploadFile(Stream content, string fileName, bool detectContentType);
        Task<ICloudFile> Upload(Stream content, string blobName, string contentType);
        ICloudFile GetInfo(string blobName);
    }
}
