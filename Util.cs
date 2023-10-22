public class Log 
{
    public static void Info(string msg)
    {
        Console.WriteLine(Log.BuildMessage("INFO", msg));        
    }

    public static void Error(string msg)
    {
        Console.WriteLine(Log.BuildMessage("ERROR", msg));        
    }

    public static void Fatal(string msg)
    {
        Console.WriteLine(Log.BuildMessage("FATAL", msg));        
        Environment.Exit(1); 
    }

    private static string BuildMessage(string level, string msg)
    {
        return $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {msg}";
    }
}