using System;
using System.IO;
using System.Threading.Tasks;

namespace MultiComputerVisionService.Models
{
    public interface ICloudFile
    {
        string BlobName { get; }
        Uri Uri { get; }
    }
}
