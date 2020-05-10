using MultiComputerVisionService.Models;
using MultiComputerVisionService.Service;
using MultiComputerVisionService.Service.Application;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Principal;
using System.Text.Json;
using System.Threading.Tasks;

namespace BlazorMultiComputerVisionWebasm.Client.Service
{
    public class UploadImageService : IUploadImageService
    {
        private readonly HttpClient httpClient;

        public UploadImageService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<IResultDocument> Upload(Stream content, string fileName, IPrincipal user)
        {
            // XXX サーバサイドでこれ見てないこと知ってるから適当でもいい状態だけれど明らかにダメ実装
            // BlazorInputFile前提なら一貫してクライアントサイドで画像処理してもいいかも
            //var format = "image/jpeg";
            //var imageFile = await rawFile.ToImageFileAsync(format, 1920, 1920);
            //var ms = new System.IO.MemoryStream();
            //await imageFile.Data.CopyToAsync(ms);
            //var imageDataUri = $"data:{format};base64,{Convert.ToBase64String(ms.ToArray())}";

            var format = "image/jpeg";
            var ms = new MemoryStream();
            await content.CopyToAsync(ms);
            var imageDataUri = $"data:{format};base64,{Convert.ToBase64String(ms.ToArray())}";

            var res = await httpClient.PostAsJsonAsync("Api/Upload", new
            {
                Name = fileName,
                ImageDataUri = imageDataUri,
            });
            var doc = await res.Content.ReadFromJsonAsync<PlainResultDocument>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true, });

            return doc;
        }
    }
}
