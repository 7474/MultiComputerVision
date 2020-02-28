using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMultiComputerVisionServer.Service
{
    public interface IUploadService
    {
        Task<ICloudFile> Upload(Stream content, string name, string contentType);
    }

    public interface ICloudFile
    {
        string Key { get; }
        Uri Uri { get; }
    }
}
