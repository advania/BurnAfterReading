using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Azure.Storage.Blobs;
using Azure.Storage.Queues;

namespace BurnAfterReading
{
    public static class Download
    {
        [FunctionName("Download")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "Download/{id:guid}")] HttpRequest req,
            [Blob("burn/{id}")] BlobClient blob,
            [Queue("CleanupQueue")] QueueClient outputQueueItem,
            string id,
            ILogger log)
        {
            log.LogInformation($"Returning blob: {id}");

            if (!await blob.ExistsAsync()) return new BadRequestResult();

            req.HttpContext.Response.Headers.Add(new ("Content-Disposition", "attachment"));

            outputQueueItem.SendMessage(id, visibilityTimeout: TimeSpan.FromMinutes(1));

            return new FileStreamResult(blob.OpenRead(), "application/octet-stream");
        }
    }
}
