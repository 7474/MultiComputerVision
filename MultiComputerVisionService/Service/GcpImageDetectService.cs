using Google.Cloud.Vision.V1;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace MultiComputerVisionService.Service
{
    public class GcpImageDetectService : IImageDetectService
    {
        private ImageAnnotatorClient imageAnnotatorClient;

        public GcpImageDetectService(string jsonCredentials)
        {
            imageAnnotatorClient = new ImageAnnotatorClientBuilder()
            {
                JsonCredentials = jsonCredentials,
            }.Build();
        }

        public async Task<IImageDetectResult> Detect(string uri)
        {
            var image = Image.FromUri(uri);
            //var res = imageAnnotatorClient.DetectLabels(image);
            var res = await imageAnnotatorClient.DetectSafeSearchAsync(image);

            // ポッシブルはどっちやねん。
            return new GcpImageDetectResult()
            {
                IsAdultContent = IsLikly(res.Adult),
                IsRacyContent = IsLikly(res.Racy),
                AdultScore = res.AdultConfidence,
                RacyScore = res.RacyConfidence,
                RawResult = JsonConvert.SerializeObject(res),
            };
        }

        private bool IsLikly(Likelihood value)
        {
            return value == Likelihood.VeryLikely || value == Likelihood.Likely;
        }
    }

    public class GcpImageDetectResult : IImageDetectResult
    {
        public ImageDetector Detector => ImageDetector.GcpCloudVision;

        public bool IsAdultContent { get; set; }
        public bool IsRacyContent { get; set; }
        public double AdultScore { get; set; }
        public double RacyScore { get; set; }

        public string RawResult { get; set; }
    }
}
