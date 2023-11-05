class BackupManager
{
    string rootDirectory = "/var/lib/pgsql/loa/tmp";

    public async Task Backup()
    {
        // Connect To Azure Storage
        AccountInfo account = new AccountInfo();
        BlobStorageUploader uploader = new BlobStorageUploader(account);   
        Log.Info($"Connected to {uploader.AccountName}");

        FileManager manager = new FileManager(uploader);
        FileScanner scanner = new FileScanner();
        scanner.ScanDirectory(rootDirectory); 

        await manager.ParallelCompressedUpload(scanner.Files);

        Log.Info($"ALL UPLOADS FINISHED.");
    }
}