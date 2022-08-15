using System;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace BurnAfterReading
{
	public class CleanUp
	{
		[FunctionName("CleanUp")]
		public void Run([QueueTrigger("CleanupQueue")] string myQueueItem,
			[Blob("burn")] BlobContainerClient blobContainerClient,
			ILogger log)
		{
			log.LogInformation($"Cleaning up blob: {myQueueItem}");

			var blob = blobContainerClient.GetBlobClient(myQueueItem);

			blob.DeleteIfExists();
		}
	}
}
