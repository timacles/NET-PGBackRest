using System;
using System.IO;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using Azure.Identity;
using System.Threading.Tasks;

public class AccountInfo
{
    public string Account = "devtestbackrest";
    public string Container = "testdbbackups";
    public string ConnString 
    { 
        get
        {
            return $"https://{Account}.blob.core.windows.net";
        } 
    }

    public override string ToString()
    {
        return $"Account: {Account} Container: {Container} ConnString: {ConnString}";
    }
}


public class BlobStorageUploader
{
    public AccountInfo account;
    public BlobServiceClient blobServiceClient;
    public BlobContainerClient containerClient;
    public BlobClient blobClient;

    public BlobStorageUploader(AccountInfo inAccount)
    {
        account = inAccount;
        blobServiceClient = new BlobServiceClient(
                new Uri(account.ConnString),
                new DefaultAzureCredential());
        containerClient = blobServiceClient.GetBlobContainerClient(account.Container);
    }

    public async Task UploadBlob(MyFile file)
    {
        blobClient = containerClient.GetBlobClient(file.Name);
        try
        {
            using (FileStream fs = File.OpenRead(file.FullPath))
            {
                await blobClient.UploadAsync(fs, true);
            }
            Log.Info($"Uploaded {file.FullPath} => {file.Name}");
        }
        catch (Exception ex)
        {
            Log.Error($"Error uploading blob: {ex.Message}");
            // Handle the exception as needed
        }
    }
}
