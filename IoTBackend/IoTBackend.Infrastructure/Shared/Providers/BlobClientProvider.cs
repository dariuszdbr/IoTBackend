using Azure.Storage.Blobs;
using IoTBackend.Infrastructure.Shared.Models.Configurations;
using Microsoft.Extensions.Options;

namespace IoTBackend.Infrastructure.Shared.Providers
{
    public interface IBlobClientProvider
    {
        BlobClient GetClient(string blobPath);
    }

    public class BlobClientProvider : IBlobClientProvider
    {
        private readonly BlobContainerClient _blobContainerClient;

        public BlobClientProvider(IOptions<BlobConfiguration> configOptions)
        {
            _blobContainerClient = new BlobContainerClient(configOptions.Value.ConnectionString, configOptions.Value.ContainerName);
        }

        public BlobClient GetClient(string blobPath)
        {
            return _blobContainerClient.GetBlobClient(blobPath);
        }
    }
}