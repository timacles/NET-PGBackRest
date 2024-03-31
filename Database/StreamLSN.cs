using NpgsqlTypes;

class StreamLSN
{
    public NpgsqlLogSequenceNumber LSN;
    
    public StreamLSN(NpgsqlLogSequenceNumber lsn) 
    {
        LSN = lsn; 
    } 
}