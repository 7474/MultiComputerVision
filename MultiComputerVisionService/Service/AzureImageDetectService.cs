using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MultiComputerVisionService.Service
{
    public sealed class AzureImageDetectService : IImageDetectService, IDisposable
    {
        private static readonly List<VisualFeatureTypes> features = new List<VisualFeatureTypes>()
        {
            VisualFeatureTypes.Adult,
        };

        IComputerVisionClient computerVisionClient;
        public AzureImageDetectService(string computerVisionSubscriptionKey, string computerVisionEndpoint)
        {
            computerVisionClient = new ComputerVisionClient(
                new ApiKeyServiceClientCredentials(computerVisionSubscriptionKey))
            {
                Endpoint = computerVisionEndpoint,
            };
        }

        public void Dispose()
        {
            computerVisionClient.Dispose();
        }

        public async Task<IImageDetectResult> Detect(string uri)
        {
            var analysis = await computerVisionClient.AnalyzeImageAsync(uri, features);

            return new AzureImageDetectResult()
            {
                IsAdultContent = analysis.Adult.IsAdultContent,
                IsRacyContent = analysis.Adult.IsRacyContent,
                AdultScore = analysis.Adult.AdultScore,
                RacyScore = analysis.Adult.RacyScore,
                RawResult = JsonConvert.SerializeObject(analysis),
            };
        }
    }

    public class AzureImageDetectResult : IImageDetectResult
    {
        public ImageDetector Detector => ImageDetector.AzureCognitiveServicesComputerVision;

        public bool IsAdultContent { get; set; }
        public bool IsRacyContent { get; set; }
        public double AdultScore { get; set; }
        public double RacyScore { get; set; }

        public string RawResult { get; set; }
    }
}
