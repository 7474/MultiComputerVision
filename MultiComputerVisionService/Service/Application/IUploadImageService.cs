using MultiComputerVisionService.Models;
using System;
using System.IO;
using System.Security.Principal;
using System.Threading.Tasks;

namespace MultiComputerVisionService.Service.Application
{
    public interface IUploadImageService
    {
        Task<IResultDocument> Upload(Stream content, string fileName, IPrincipal user);
    }
}
