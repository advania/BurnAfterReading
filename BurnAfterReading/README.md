# Burn after reading

Burn after reading is a simple web app that uses Azure Funtions and Azure Storage to allow users to upload data (files, json, etc.) and retrieve it once.
After the data is retrieved a clean up process runs and deletes the files.

## How does it work?

You upload your data in the body of a post request to the `/api/upload` endpoint, the Azure function takes has a blob binding with a random guid which is returned after the request body has been written to blob storage.
When the user wants to retrieve the data again, he makes a get request to `/api/download/{guid}` where `{guid}` is the id returned from the post request. The download Azure function also has a blob binding and a QueueClient binding and adds the id of the blob to a queue with a `visibilityTimeout` of one minute (to allow the download to finish safely), then returns the blob.
Once the item from the previous step appears in the queue the `CleanUp` function is triggered which then tries to delete the blob by the Id which triggered the function.