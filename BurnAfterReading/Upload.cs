using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Azure.Storage.Blobs;

namespace BurnAfterReading
{
    public static class Upload
    {
        [FunctionName("Upload")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Blob("burn/{rand-guid}")] BlobClient blob,
            ILogger log)
        {
            if (req.Form.Files["File"] is not IFormFile file) return new BadRequestResult();

            await blob.UploadAsync(file.OpenReadStream());

            var path = req;

            return new CreatedResult($"{req.Host}/api/Download/{blob.Name}", blob.Name);
        }
    }
}
