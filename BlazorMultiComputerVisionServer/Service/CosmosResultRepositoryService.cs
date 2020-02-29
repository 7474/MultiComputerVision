using Microsoft.Azure.Cosmos;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorMultiComputerVisionServer.Service
{
    public sealed class CosmosResultRepositoryService : IResultRepositoryService, IDisposable
    {
        private CosmosClient cosmosClient;
        private Database database;
        private Container resultContainer;

        public CosmosResultRepositoryService(string connectionString)
        {
            cosmosClient = new CosmosClient(connectionString);
        }

        public void Initialize(string databaseId)
        {
            database = cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId).Result;
            resultContainer = database.CreateContainerIfNotExistsAsync("ImageDetectResult", "/OwnerId").Result;
        }

        public void Dispose()
        {
            cosmosClient.Dispose();
        }
        public IResultDocument BuildResult(string ownerId, ICloudFile image, IEnumerable<IImageDetectResult> results)
        {
            return new ResultDocument()
            {
                Id = Guid.NewGuid(),
                OwnerId = ownerId,
                Image = image,
                Results = new List<IImageDetectResult>(results),
                CreatedAt = DateTimeOffset.UtcNow,
            };
        }

        public async Task Put(IResultDocument doc)
        {
            // XXX 他の具象クラスに対応
            var res = await resultContainer.CreateItemAsync(doc as ResultDocument);
        }

        public Task<IResultDocument> GetResult(Guid id)
        {
            var res = resultContainer.GetItemLinqQueryable<ResultDocument>().Where(x => x.Id == id).FirstOrDefault();
            return Task.FromResult(res as IResultDocument);
        }
    }

    public class ResultDocument : IResultDocument
    {
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        public string OwnerId { get; set; }
        [JsonConverter(typeof(ConcreteTypeConverter<CloudFile>))]
        public ICloudFile Image { get; set; }
        [JsonProperty(ItemConverterType = typeof(ConcreteTypeConverter<ImageDetectResult>))]
        public ICollection<IImageDetectResult> Results { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class CloudFile : ICloudFile
    {
        public string BlobName { get; set; }

        public Uri Uri { get; set; }
    }

    public class ImageDetectResult : IImageDetectResult
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ImageDetector Detector { get; set; }

        public bool IsAdultContent { get; set; }
        public bool IsRacyContent { get; set; }
        public double AdultScore { get; set; }
        public double RacyScore { get; set; }

        public string RawResult { get; set; }
    }

    // https://stackoverflow.com/questions/5780888/casting-interfaces-for-deserialization-in-json-net
    public class ConcreteTypeConverter<TConcrete> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            //assume we can convert to anything for now
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            //explicitly specify the concrete type we want to create
            return serializer.Deserialize<TConcrete>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            //use the default serialization - it works fine
            serializer.Serialize(writer, value);
        }
    }
}
