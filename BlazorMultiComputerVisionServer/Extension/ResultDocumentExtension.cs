using BlazorMultiComputerVisionServer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMultiComputerVisionServer.Extension
{
    public static class ResultDocumentExtension
    {
        public static bool IsAdultContent(this IResultDocument doc)
        {
            return doc.Results.Count(x => x.IsAdultContent) > doc.Results.Count / 2;
        }

        public static bool IsRacyContent(this IResultDocument doc)
        {
            return doc.Results.Count(x => x.IsRacyContent) > doc.Results.Count / 2;
        }
    }
}
