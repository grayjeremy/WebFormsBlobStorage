using Azure.Identity;
using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebFormsBlobStorage
{
    public static class Logger
    {
        internal static void AddLog(Exception ex, string empty)
        {
            byte[] byteArray = Encoding.ASCII.GetBytes(ex.Message);
            string fileName = Guid.NewGuid().ToString() + ".txt";
            string storageAccountName = ConfigurationManager.AppSettings["storageAccountName"].ToString();
            string storageContainerName = ConfigurationManager.AppSettings["storageContainerName"].ToString();

            using (MemoryStream stream = new MemoryStream(byteArray))
            {

                StreamToCloudStorageAsync(storageAccountName, storageContainerName, stream, fileName).GetAwaiter().GetResult();

            }
        }

        async static Task StreamToCloudStorageAsync(string accountName, string containerName, Stream sourceStream, string destinationFileName, bool overwrite = true, Dictionary<string, string> destinationFileMetadata = null)
        {
            // Construct the blob container endpoint from the arguments.
            string containerEndpoint = string.Format("https://{0}.blob.core.windows.net/{1}",
                                                    accountName,
                                                    containerName);

            // Get a credential and create a client object for the blob container.
            BlobContainerClient containerClient = new BlobContainerClient(new Uri(containerEndpoint), new DefaultAzureCredential());

            // Create the container if it does not exist.
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(destinationFileName);

            // Upload text to a new block blob.
            var blob = await blobClient.UploadAsync(sourceStream, overwrite);
            //await containerClient.UploadBlobAsync(destinationFileName, sourceStream);      

            if (null != destinationFileMetadata)
            {
                blobClient.SetMetadata(destinationFileMetadata);
            }
        }

        
    }
}