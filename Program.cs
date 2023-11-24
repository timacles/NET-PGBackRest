/*
 * TODO: 
 *   - Implement logging library
 *   - Connect WAL stream
 *   - Download files (threaded)
 *   - check compression speed
 *   - Config File 
 */

using System.Diagnostics;

class Program
{
    static async Task Main(string[] args)
    {
        
        // Catch a SIGINT
        Console.CancelKeyPress += delegate {
            Log.Info("Program Interrupted. CTRL + C");     
        };

        // Timer 
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        //BackupManager backMan = new BackupManager();
        //await backMan.Backup();

        WalStreamer walStreamer = new WalStreamer();
        await walStreamer.Stream();

        /////////
        // END //
        /////////
        stopwatch.Stop();
        Log.Info($"Elapsed Time: {stopwatch.Elapsed}");
    }
}