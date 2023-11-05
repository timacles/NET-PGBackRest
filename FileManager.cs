using System.Diagnostics;

public interface IFileReader : IDisposable
{
    Task<MemoryStream> ReadAsync(string filePath);
}

public interface IFileUploader
{
    Task UploadAsync(Stream fileStream, 
                            string destinationPath, 
                            bool overwrite);
}

class FileManager {
    public IFileUploader uploader; 

    public int maxDegreeOfParallelism = 8;

    public FileManager(IFileUploader uploader)
    {
        this.uploader = uploader;
    }

    public async Task CompressAndUploadAsync(MyFile myFile)
    {
        Log.Info($"Starting Compressing Upload: {myFile.Name}");
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        using (IFileReader reader = new CompressingReader()) 
        {
            MemoryStream stream = await reader.ReadAsync(myFile.FullPath);
            await uploader.UploadAsync(stream, myFile.Name, true);
        }

        stopwatch.Stop();
        Log.Info($"   Upload Done: {myFile.Name}, timing: {stopwatch.Elapsed}");
    }


    public async Task ParallelCompressedUpload(List<MyFile> Files)
    {

        using (var semaphoreSlim = new SemaphoreSlim(maxDegreeOfParallelism))
        {
            var tasks = Files.Select(async myFile =>
            {
                await semaphoreSlim.WaitAsync();
                try
                {
                    await CompressAndUploadAsync(myFile);
                }
                catch (Exception ex)
                {
                    Log.Error($"Error parallel uploading {myFile.Name}: {ex.Message}");
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