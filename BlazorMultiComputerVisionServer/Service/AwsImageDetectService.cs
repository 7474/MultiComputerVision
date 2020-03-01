using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.Runtime;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BlazorMultiComputerVisionServer.Service
{
    public sealed class AwsImageDetectService : IImageDetectService, IDisposable
    {
        private AmazonRekognitionClient rekognitionClient;
        private HttpClient httpClient;

        public AwsImageDetectService(AWSCredentials credentials, RegionEndpoint regionEndpoint)
        {
            rekognitionClient = new AmazonRekognitionClient(credentials, regionEndpoint);
            httpClient = new HttpClient();
        }

        public void Dispose()
        {
            rekognitionClient.Dispose();
            httpClient.Dispose();
        }

        public async Task<IImageDetectResult> Detect(string uri)
        {
            // https://docs.aws.amazon.com/rekognition/latest/dg/procedure-moderate-images.html
            var req = new DetectModerationLabelsRequest()
            {
                // AWSの野郎S3最強だと思っていて他所からを受け入れない狭量さを発揮している。インタフェースがプア。好きではない。
                Image = new Image()
                {
                    Bytes = new System.IO.MemoryStream(await httpClient.GetByteArrayAsync(uri)),
                },
                MinConfidence = 0,
            };
            var res = await rekognitionClient.DetectModerationLabelsAsync(req);

            // https://docs.aws.amazon.com/rekognition/latest/dg/moderation.html
            // 取りあえずトップレベルだけ見る
            var adultLabel = res.ModerationLabels.Where(x => string.IsNullOrEmpty(x.ParentName) && x.Name == "Explicit Nudity").FirstOrDefault();
            var racyLabel = res.ModerationLabels.Where(x => string.IsNullOrEmpty(x.ParentName) && x.Name == "Suggestive").FirstOrDefault();
            return new AwsImageDetectResult()
            {
                // 50%がある種の閾らしいのでそれで
                IsAdultContent = adultLabel?.Confidence >= 50,
                IsRacyContent = racyLabel?.Confidence >= 50,
                AdultScore = (double)adultLabel?.Confidence / 100,
                RacyScore = (double)racyLabel?.Confidence / 100,
                RawResult = JsonConvert.SerializeObject(res),
            };
        }
    }

    public class AwsImageDetectResult : IImageDetectResult
    {
        public ImageDetector Detector => ImageDetector.AwsAmazonRekognition;

        public bool IsAdultContent { get; set; }
        public bool IsRacyContent { get; set; }
        public double AdultScore { get; set; }
        public double RacyScore { get; set; }

        public string RawResult { get; set; }
    }
}
