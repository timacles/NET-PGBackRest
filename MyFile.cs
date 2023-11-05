public class MyFile 
{
    public FileInfo FileInfo { get; } 
    public string FullPath { get; }
    public string Name => FileInfo.Name;
    public long Length => FileInfo.Length;

    public Stream stream { get; set; }  

    public MyFile(string filePath)
    {
        FullPath = filePath;
        FileInfo = new FileInfo(filePath);
    }
}


class FileScanner
{
    public string directory;
    public List<MyFile> Files = new List<MyFile>();

    public void ScanDirectory(string directory)
    {
        try
        {
            foreach (string filePath in Directory.GetFiles(directory))
            {
                MyFile fileInfo = new MyFile(filePath);
                Files.Add(fileInfo);
            }

            foreach (string subDirectory in Directory.GetDirectories(directory))
            {
                Console.WriteLine("Scanning directory: " + subDirectory);
                ScanDirectory(subDirectory); // Recursively scan subdirectories
            }
        }
        catch (UnauthorizedAccessException)
        {
            Log.Fatal("Access denied to directory: " + directory);
        }
        catch (DirectoryNotFoundException)
        {
            Log.Fatal("Directory not found: " + directory);
        }
    }
}