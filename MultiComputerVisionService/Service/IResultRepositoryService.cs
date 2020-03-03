using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiComputerVisionService.Service
{
    public interface IResultRepositoryService
    {
        IResultDocument BuildResult(string ownerId, ICloudFile image, IEnumerable<IImageDetectResult> results);
        Task Put(IResultDocument doc);
        Task<IResultDocument> GetResult(Guid id);
        // XXX 時刻だと妥当なオフセットにならない。Idをソート可能にするなどすると良いかも知れない。
        Task<IList<IResultDocument>> GetResults(DateTimeOffset offset);
    }

    public interface IResultDocument
    {
        Guid Id { get; }
        string OwnerId { get; }

        ICloudFile Image { get; }
        ICollection<IImageDetectResult> Results { get; }

        DateTimeOffset CreatedAt { get; }
    }
}
