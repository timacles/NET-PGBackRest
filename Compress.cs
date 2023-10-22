
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;


class CompressAndUpload
{
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
                        await blobClient.UploadAsync(compressedStream);
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