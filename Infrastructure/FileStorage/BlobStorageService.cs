using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.FileStorage
{
    public class BlobStorageService : IFileService
    {
        private readonly string _connectionString;
        private readonly string _containerName;


        public BlobStorageService(string connectionString, string containerName)
        {
            _connectionString = connectionString;   
            _containerName = containerName;
        }

        public async Task<string> CopyFileAsync(string source, string destination)
        {
            BlobClient sourceBlobClient = new BlobClient(new Uri(source));

            BlobClient destinationBlobClient = new BlobClient(_connectionString, _containerName, destination);


            // Start the copy operation
            Console.WriteLine($"Copying blob from {sourceBlobClient.Uri} to {destinationBlobClient.Uri}");
            var copyOperation = await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);

            var publicUri = destinationBlobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddDays(356));    

            return publicUri.AbsoluteUri;
        }

        public Task<string> CreateImageVariationAsync(string originalFilePath, int height)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SaveFileAsync(IFormFile file, Guid imageId)
        {
            // Get the blob client for uploading
            BlobServiceClient blobServiceClient = new BlobServiceClient(_connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);

            // Create a new block blob client
            BlockBlobClient blockBlobClient = containerClient.GetBlockBlobClient($"{imageId}/{file.FileName}");

            // Initialize list for block IDs (you will need these later to commit the blocks)
            var blockIds = new List<string>();

            // Define the size of each block (e.g., 4MB blocks)
            const int blockSize = 4 * 1024 * 1024; // 4MB chunk size

            // Read the file stream in chunks
            using (var fileStream = file.OpenReadStream())
            {
                int blockNumber = 0;
                byte[] buffer = new byte[blockSize];
                int bytesRead;
                while ((bytesRead = await fileStream.ReadAsync(buffer, 0, blockSize)) > 0)
                {
                    // Generate a unique block ID
                    string blockId = Convert.ToBase64String(BitConverter.GetBytes(blockNumber));
                    blockIds.Add(blockId);

                    // Upload the block
                    using (var memoryStream = new MemoryStream(buffer, 0, bytesRead))
                    {
                        await blockBlobClient.StageBlockAsync(blockId, memoryStream);
                    }

                    blockNumber++;
                }
            }

            // Commit the blocks to finalize the upload
            await blockBlobClient.CommitBlockListAsync(blockIds);

            var generatePublicUrl = blockBlobClient.GenerateSasUri(BlobSasPermissions.Read, DateTimeOffset.UtcNow.AddDays(365));

            return generatePublicUrl.AbsoluteUri.ToString();

        }

        public static Uri GetServiceSasUriForContainer(BlobClient blobClient, string storedPolicyName = null)
        {
            // Check whether this BlobClient object has been authorized with Shared Key.
            if (blobClient.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for one hour.
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };

                if (storedPolicyName == null)
                {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddDays(5);
                    sasBuilder.SetPermissions(BlobSasPermissions.Read |
                        BlobSasPermissions.Write);
                }
                else
                {
                    sasBuilder.Identifier = storedPolicyName;
                }

                Uri sasUri = blobClient.GenerateSasUri(sasBuilder);
                Console.WriteLine("SAS URI for blob is: {0}", sasUri);
                Console.WriteLine();

                return sasUri;
            }
            else
            {
                Console.WriteLine(@"BlobClient must be authorized with Shared Key 
                          credentials to create a service SAS.");
                return null;
            }
        }
    }
}
