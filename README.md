# Introduction 
This is a sample implementation of the code for Microsoft Serverless OpenHack

## Challenge 3
You find the implementation of GetRating, GetRatings and CreateRating and if you want to test locally, you need to configure the local.settings.json file with the settings:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet",
    ...
    "CosmosDBConnection": "<cosmos db connectionstring>",
    "BackEndServiceUrl": "https://serverlessohapi.azurewebsites.net/api"
  }
}
```


## Challenge 6
If you want to test the entities locally, refer to this documentation page <a href="https://docs.microsoft.com/en-us/azure/azure-functions/functions-debug-event-grid-trigger-local" target="_blank">Azure Function Event Grid Trigger Local Debugging</a>.
You can use an HTTP call similar to this:

```http
POST /runtime/webhooks/EventGrid?functionName=BlobListener HTTP/1.1
Host: localhost:7071
aeg-event-type: Notification
Content-Type: application/json
Content-Length: 937

{
    "subject": "/blobServices/default/containers/container/20180518151300-OrderLineItems.csv",
    "eventType": "Microsoft.Storage.BlobCreated",
    "eventTime": "2017-06-26T18:41:00.9584103Z",
    "id": "831e1650-001e-001b-66ab-eeb76e069631",
    "data": {
        "api": "CreateFile",
        "clientRequestId": "6d79dbfb-0e37-4fc4-981f-442c9ca65760",
        "requestId": "831e1650-001e-001b-66ab-eeb76e000000",
        "eTag": "\"0x8D4BCC2E4835CD0\"",
        "contentType": "text/plain",
        "contentLength": 0,
        "contentOffset": 0,
        "blobType": "BlockBlob",
        "url": "https://my-storage-account.blob.core.windows.net/container/20180518151300-OrderLineItems.csv",
        "sequencer": "00000000000004420000000000028963",
        "storageDiagnostics": {
            "batchId": "b68529f3-68cd-4744-baa4-3c0498ec19f0"
        }
    },
    "dataVersion": "2",
    "metadataVersion": "1"
}
```

You can change the subject and the url in data property as you prefer to test the three different blobs you receive in the challenge.

