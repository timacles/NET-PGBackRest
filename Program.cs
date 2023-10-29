/*
 * - Implement logging library
 * - Connect WAL stream
 * - Upload files in ASYNC
 * - Config File 
 */

using System.Diagnostics;

class Program
{
    static async Task Main(string[] args)
    {
        string rootDirectory = "/var/lib/pgsql/loa/tmp";

        // Create Connection String
        AccountInfo account = new AccountInfo();
        Console.WriteLine(account);

        // Connect to account
        BlobStorageUploader uploader = new BlobStorageUploader(account);   
        Console.WriteLine($"Connected to {uploader.AccountName}");

        FileManager manager = new FileManager(uploader);
        FileScanner scanner = new FileScanner();

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        scanner.ScanDirectory(rootDirectory); 
        
        await manager.ParallelUpload(scanner.Files);

        stopwatch.Stop();
        Log.Info($"ALL UPLOADS FINISHED. Timing: {stopwatch.Elapsed}");
    }
}