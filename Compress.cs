using System.IO.Compression;

class CompressingReader : IFileReader, IDisposable
{
    private Stream fileStream;
    private MemoryStream compressedStream;
    private GZipStream gzipStream;

    public async Task<MemoryStream> ReadAsync(string filePath)
    {
        fileStream = File.OpenRead(filePath);
        compressedStream = new MemoryStream();
        gzipStream = new GZipStream(compressedStream, CompressionLevel.Optimal);

        await fileStream.CopyToAsync(gzipStream);
        await gzipStream.FlushAsync();
        compressedStream.Position = 0;
        return compressedStream;
    }

    public void Dispose()
    {
        fileStream.Dispose();
        gzipStream.Dispose();
        compressedStream.Dispose();
    }
}


/*
class CompressAndUpload
{
    public BlobStorageUploader uploader;

    public async Task ProcessCompressAndUploadFilesAsync(string blobContainerName, List<string> filePaths)
    {
        int maxDegreeOfParallelism = 8; // Maximum number of concurrent uploads

        using (var semaphoreSlim = new SemaphoreSlim(maxDegreeOfParallelism))
        {
            var tasks = filePaths.Select(async filePath =>
            {
                await semaphoreSlim.WaitAsync();
                try
                {
                    string compressedFileName = Path.GetFileNameWithoutExtension(filePath) + ".gz";
                    BlobClient blobClient = containerClient.GetBlobClient(compressedFileName);
                    using (FileStream fileStream = File.OpenRead(filePath))
                    using (MemoryStream compressedStream = new MemoryStream())
                    using (GZipStream gzipStream = new GZipStream(compressedStream, CompressionLevel.Optimal))
                    {
                        await fileStream.CopyToAsync(gzipStream);
                        await gzipStream.FlushAsync();
                        compressedStream.Position = 0;
                        await uploader.UploadAsync(compressedStream);
                        Console.WriteLine($"Uploaded {compressedFileName}");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Error uploading {Path.GetFileName(filePath)}: {ex.Message}");
                }
                finally
                {
                    semaphoreSlim.Release();
                }
            });

            await Task.WhenAll(tasks);
        }
    }
}
*/