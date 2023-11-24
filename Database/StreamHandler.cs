
using Npgsql.Replication;
using NpgsqlTypes;


class StreamHandler
{
    public int SegmentSize = 16777216;
    public int Timeline;

    public int BytesLeft;

    public NpgsqlLogSequenceNumber PreviousLSN;
    public StreamLSN CurrentLSN;

    
    public async Task Process(XLogDataMessage message)
    {
        CurrentLSN = new StreamLSN(message.WalStart);

        Console.WriteLine($"LSN: {message.WalStart} Length: {message.Data.Length}");


        //Console.WriteLine($"  hash: {CurrentLSN.GetHashCode()} val: {(ulong)CurrentLSN}");

        //Console.WriteLine($"{(uint)(CurrentLSN >> 32):X}/{(uint)CurrentLSN:X}");

    }  
}

class StreamLSN
{
    public NpgsqlLogSequenceNumber LSN;
    public string WalFileName => GetWalFileName();
    
    public StreamLSN(NpgsqlLogSequenceNumber lsn) 
    {
        LSN = lsn; 
        Console.WriteLine($"{WalFileName}");
    } 

    public string GetWalFileName()
    {
        var value = (ulong)LSN;
        string result = String.Format(
            "{0:X8}_{1:X8}_{2:X8}",
            1, 
            (uint)(value >> 32),
            (byte)(value >> 24)
        );
        return result;
        //$"  wal file: {(uint)(value >> 32):X} {(uint)(value >> 24):X}";
    }

        // - split string by `/`
        // - Timeline, Hi Val, Low Val 
        // - 000000 000000 000000
        // - compare offset to LSN position?
}