using System;
using System.ComponentModel;
using System.IO;


public class MyFile 
{
    public FileInfo FileInfo { get; } 
    public string FullPath { get; }

    public MyFile(string filePath)
    {
        FullPath = filePath;
        FileInfo = new FileInfo(filePath);
    }

    public string Name => FileInfo.Name;
    public long Length => FileInfo.Length;
}


class FileCollection 
{
    public List<FileInfo> Collection { get; set; }

    public void Add(FileInfo fi)
    {
        Collection.Add(fi);
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
                Console.WriteLine("Full Path: " + fileInfo.FullPath);
                Console.WriteLine("File Name: " + fileInfo.Name);
                Console.WriteLine("File Size: " + fileInfo.Length + " bytes");
                Console.WriteLine(); // Add a separator
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