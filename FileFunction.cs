using Azure;
using Azure.Storage.Files.Shares;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ABC_Retail_Functions
{
    public class FileFunction
    {
        private readonly ILogger<FileFunction> _logger;
        private readonly string _connectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private readonly string _shareName = "orders-files"; // name of your file share

        public FileFunction(ILogger<FileFunction> logger)
        {
            _logger = logger;
        }

        [Function("FileFunction")]
        public async Task<HttpResponseData> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", "get", Route = "file/{filename?}")] HttpRequestData req,
            string filename)
        {
            _logger.LogInformation("FileFunction triggered.");

            var share = new ShareClient(_connectionString, _shareName);
            await share.CreateIfNotExistsAsync();

            var rootDir = share.GetRootDirectoryClient();

            if (req.Method == "POST")
            {
                // Upload sample text file to file share
                filename ??= $"OrderSummary_{Guid.NewGuid()}.txt";
                var fileClient = rootDir.GetFileClient(filename);

                string content = $"Order file generated at {DateTime.UtcNow}";
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(content);

                using (var stream = new MemoryStream(bytes))
                {
                    await fileClient.CreateAsync(stream.Length);
                    await fileClient.UploadRangeAsync(new HttpRange(0, stream.Length), stream);
                }

                var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                await response.WriteStringAsync($"File '{filename}' uploaded to Azure File Share '{_shareName}'.");
                return response;
            }
            else if (req.Method == "GET" && !string.IsNullOrEmpty(filename))
            {
                // Download file content
                var fileClient = rootDir.GetFileClient(filename);
                if (await fileClient.ExistsAsync())
                {
                    var download = await fileClient.DownloadAsync();
                    using var reader = new StreamReader(download.Value.Content);
                    var text = await reader.ReadToEndAsync();

                    var response = req.CreateResponse(System.Net.HttpStatusCode.OK);
                    await response.WriteStringAsync($"Contents of '{filename}':\n{text}");
                    return response;
                }
                else
                {
                    var response = req.CreateResponse(System.Net.HttpStatusCode.NotFound);
                    await response.WriteStringAsync($"File '{filename}' not found.");
                    return response;
                }
            }
            else
            {
                var response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
                await response.WriteStringAsync("Invalid request. Use POST to upload or GET /file/{filename} to download.");
                return response;
            }
        }
    }
}
