/*
 * - Implement logging library
 * - Connect WAL stream
 * - Upload files in ASYNC
 */



class Program
{
    static void Main(string[] args)
    {
        AccountInfo account = new AccountInfo();
        Console.WriteLine(account);

        BlobStorageUploader uploader = new BlobStorageUploader(account);   
        var aname = uploader.containerClient.AccountName;

        Console.WriteLine($"Connected to {aname}");
    }
}