using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMultiComputerVisionServer.Service
{
    public interface IImageDetectService
    {
        Task<IImageDetectResult> Detect(string uri);
    }

    public interface IImageDetectResult
    {
        bool IsAdultContent { get; }
        bool IsRacyContent { get; }
        double AdultScore { get; }
        double RacyScore { get; }

        string RawResult { get; }
    }
}
