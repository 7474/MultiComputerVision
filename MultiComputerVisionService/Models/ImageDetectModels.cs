using System.Threading.Tasks;

namespace MultiComputerVisionService.Models
{
    public enum ImageDetector
    {
        AzureCognitiveServicesComputerVision,
        AwsAmazonRekognition,
        GcpCloudVision
    }

    public interface IImageDetectResult
    {
        ImageDetector Detector { get; }

        bool IsAdultContent { get; }
        bool IsRacyContent { get; }
        double AdultScore { get; }
        double RacyScore { get; }

        string RawResult { get; }
    }
}
