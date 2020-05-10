using MultiComputerVisionService.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiComputerVisionService.Service.Application
{
    public interface IResultDocumentService
    {
        Task<IResultDocument> GetResult(Guid id);
        // XXX 時刻だと妥当なオフセットにならない。Idをソート可能にするなどすると良いかも知れない。
        Task<IList<IResultDocument>> GetResults(DateTimeOffset offset);
    }
}
