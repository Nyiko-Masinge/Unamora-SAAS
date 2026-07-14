using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Unamora.Application.Common.Interfaces;

namespace Unamora.Infrastructure.Storage;

public class AzureBlobStorageService(IConfiguration configuration) : IFileStorageService
{
    private BlobContainerClient GetContainer(string container)
    {
        var connectionString = configuration.GetConnectionString("AzureBlobStorage")
            ?? "UseDevelopmentStorage=true";
        var client = new BlobServiceClient(connectionString);
        var containerClient = client.GetBlobContainerClient(container);
        containerClient.CreateIfNotExists(PublicAccessType.None);
        return containerClient;
    }

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, string container, CancellationToken cancellationToken = default)
    {
        var containerClient = GetContainer(container);
        var blobName = $"{Guid.NewGuid()}-{fileName}";
        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType }, cancellationToken: cancellationToken);
        return blobClient.Uri.ToString();
    }

    public Task<string> GetSasUploadUrlAsync(string fileName, string container, CancellationToken cancellationToken = default)
    {
        var containerClient = GetContainer(container);
        var blobName = $"{Guid.NewGuid()}-{fileName}";
        var blobClient = containerClient.GetBlobClient(blobName);
        return Task.FromResult(blobClient.Uri.ToString());
    }

    public async Task DeleteAsync(string blobUrl, CancellationToken cancellationToken = default)
    {
        var uri = new Uri(blobUrl);
        var connectionString = configuration.GetConnectionString("AzureBlobStorage") ?? "UseDevelopmentStorage=true";
        var client = new BlobServiceClient(connectionString);
        var containerName = uri.Segments[1].TrimEnd('/');
        var blobName = string.Join("", uri.Segments.Skip(2));
        await client.GetBlobContainerClient(containerName).GetBlobClient(blobName).DeleteIfExistsAsync(cancellationToken: cancellationToken);
    }
}

public class LocalFileStorageService : IFileStorageService
{
    private readonly string _basePath = Path.Combine(Path.GetTempPath(), "unamora-uploads");

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, string container, CancellationToken cancellationToken = default)
    {
        var dir = Path.Combine(_basePath, container);
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, $"{Guid.NewGuid()}-{fileName}");
        await using var fs = File.Create(path);
        await stream.CopyToAsync(fs, cancellationToken);
        return path;
    }

    public Task<string> GetSasUploadUrlAsync(string fileName, string container, CancellationToken cancellationToken = default) =>
        Task.FromResult($"/uploads/{container}/{Guid.NewGuid()}-{fileName}");

    public Task DeleteAsync(string blobUrl, CancellationToken cancellationToken = default)
    {
        if (File.Exists(blobUrl)) File.Delete(blobUrl);
        return Task.CompletedTask;
    }
}
