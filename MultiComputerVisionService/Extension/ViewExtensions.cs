using MultiComputerVisionService.Models;
using MultiComputerVisionService.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiComputerVisionService.Extension
{
    public static class ResultDocumentViewExtension
    {
        public static string GetTitle(this IResultDocument doc)
        {
            return "判定　" + (doc.IsAdultContent() ? "性的！" : doc.IsRacyContent() ? "えっち！" : "健全！");
        }

        public static string GetDescription(this IResultDocument doc)
        {
            return string.Join(", ", doc.Results.Select(
                x => $"{x.Detector.GetNickName()}: {(x.IsAdultContent ? "❌" : "✔")} {(x.IsRacyContent ? "❌" : "✔")}")
            );
        }
    }

    public static class ImageDetectorViewExtension
    {
        public static string GetNickName(this ImageDetector detector)
        {
            switch (detector)
            {
                case ImageDetector.AzureCognitiveServicesComputerVision:
                    return "Mさん";
                case ImageDetector.AwsAmazonRekognition:
                    return "Aさん";
                case ImageDetector.GcpCloudVision:
                    return "Gさん";
                default:
                    return "Uさん";
            }
        }
    }
}
