using MultiComputerVisionService.Models;
using System.Threading.Tasks;

namespace MultiComputerVisionService.Service
{
    public interface IImageDetectService
    {
        Task<IImageDetectResult> Detect(string uri);
    }
}
