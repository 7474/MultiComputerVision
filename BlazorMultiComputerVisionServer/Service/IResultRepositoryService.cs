using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMultiComputerVisionServer.Service
{
    public interface IResultRepositoryService
    {
        IResultDocument BuildResult(string ownerId, ICloudFile image, IEnumerable<IImageDetectResult> results);
        Task Put(IResultDocument doc);
        Task<IResultDocument> GetResult(Guid id);
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
