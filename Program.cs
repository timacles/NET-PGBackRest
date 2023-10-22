/*
 * - Implement logging library
 * - Connect WAL stream
 * - Upload files in ASYNC
 * - Config File 
 */


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
        var aname = uploader.containerClient.AccountName;
        Console.WriteLine($"Connected to {aname}");

        // Scan Dir
        FileScanner scanner = new FileScanner();
        scanner.ScanDirectory(rootDirectory); 

        foreach (MyFile file in scanner.Files)
        {
            await uploader.UploadBlob(file);
        }
    }
}