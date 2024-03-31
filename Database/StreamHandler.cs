using Npgsql.Replication;
using NpgsqlTypes;


class StreamHandler
{
    public uint SegmentSize = 16777216;
    public uint Timeline;

    public int BytesLeft;

    public NpgsqlLogSequenceNumber PreviousLSN;
    public NpgsqlLogSequenceNumber CurrentLSN;
    public NpgsqlLogSequenceNumber StartLSN;
    public NpgsqlLogSequenceNumber EndLSN;

    public StreamHandler(ReplicationSystemIdentification sysId)
    {
        Timeline = sysId.Timeline;
        StartLSN = GetWALStart(sysId.XLogPos);
    }
    
    public async Task Process(XLogDataMessage message)
    {
        CurrentLSN = message.WalStart;
        Console.WriteLine($"  LSN: {CurrentLSN} Length: {message.Data.Length} Wal File: {GetWalFileName(CurrentLSN)} Start: {GetWALStart(CurrentLSN)}");
    }  

    // Return an LSN without the offset.
    public NpgsqlLogSequenceNumber GetWALStart(NpgsqlLogSequenceNumber lsn)
    {
        ulong value = (ulong)lsn;
        return new NpgsqlLogSequenceNumber(((value >> 24) << 24 ));
    }

    // -------------------------------------------------------------------------------------
    // Converting an LSN to a WAL File name.
    // -------------------------------------------------------------------------------------
    // A WAL file is made up of three 8 character segments
    //      - IE: 00000000 00000000 00000000
    //      - 1st: Timeline, 2nd: High Value, 3rd: Low Value
    //
    // An LSN is just a Postgres specific implementation of a Ulong
    //      IE. an 64 bit unsigned integer. 
    // It looks like this: C281/14636948
    // This is a ulong value represented in HEX split into a High and Low Value by a slash
    //  The last 6 digits are the offset within the WAL file.
    //
    // To convert it to a WAL file name, for each segment:
    //      1) Timeline: We should know the current timeline, from the SYSIDENT
    //         and we'll hope it never changes, because I don`t know what to do then.a
    //          The value is stored in `uint` and cast to HEX.
    //      2) High Value: This shifts the ulong 32 bits and convert it to uint, 
    //          effectively extracting the upper 32 bits of the value. 
    //      3) Low Value: The last 6 digits of an LSN are the offset within the specific
    //          WAL file which should not be represented. 
    //         To get this, we will shift the value 24 bits, then truncate the upper bits 
    //          by casting it to a `byte`. This effectively extracts bits 32 to 39 of the
    //          64-bit integer.  
    // The resulting HEX values are padded with 0s to have 8 characters
    // -------------------------------------------------------------------------------------
    public string GetWalFileName(NpgsqlLogSequenceNumber lsn)
    {
        var value = (ulong)lsn;
        return string.Format("{0:X8}{1:X8}{2:X8}",
            Timeline, 
            (uint)(value >> 32),
            (byte)(value >> 24)
        );
    }
}
