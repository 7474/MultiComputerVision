using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiComputerVisionService.Models
{
    public interface IResultDocument
    {
        Guid Id { get; }
        string OwnerId { get; }

        ICloudFile Image { get; }
        ICollection<IImageDetectResult> Results { get; }

        DateTimeOffset CreatedAt { get; }
    }
}
