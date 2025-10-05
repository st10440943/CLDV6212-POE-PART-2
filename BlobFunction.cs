using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace ABC_Retail_Functions
{
    public class BlobFunction
    {
        private readonly ILogger<BlobFunction> _logger;
        private readonly string _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private readonly string _containerName = "product-images"; // your blob container
        
        public BlobFunction(ILogger<BlobFunction> logger)
        {
            _logger = logger;
        }

        [Function("BlobFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "upload")] HttpRequestData req)
        {
            _logger.LogInformation("Blob upload function triggered.");

            // Check if request contains a file
            if (!req.Headers.TryGetValues("filename", out var filenames))
            {
                var badResponse = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Missing 'filename' header.");
                return badResponse;
            }

            string fileName = filenames.First(); // use filename from header
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = $"upload-{Guid.NewGuid()}"; // fallback to unique name

            // Upload to Blob
            var blobClient = new BlobClient(_connectionString, _containerName, fileName);
            using (var stream = new MemoryStream())
            {
                await req.Body.CopyToAsync(stream);
                stream.Position = 0; // reset stream
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            // Response
            var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
            await response.WriteStringAsync($"File '{fileName}' uploaded to container '{_containerName}'.");
            return response;
        }
    }
}
