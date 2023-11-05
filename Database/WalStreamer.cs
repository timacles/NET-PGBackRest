using Npgsql;
using Npgsql.Replication;

class WalStreamer
{
    public string replicationSlotName = "NETBackRest Slot";
    public string connectionString = "Host=localhost;Port=5446;Database=load;Username=postgres";

    public async Task Stream()
    {
        await using var conn = new PhysicalReplicationConnection(connectionString);
        await conn.Open();

        var cancellationTokenSource = new CancellationTokenSource();
        var cToken = cancellationTokenSource.Token;
        var sysId = await conn.IdentifySystem(cToken);
        Log.Info($"Sys ident: {sysId.XLogPos}");
        var slot = new PhysicalReplicationSlot(replicationSlotName);

        // start recieving replication messages
        await foreach (var message in conn.StartReplication(sysId.XLogPos, cToken, sysId.Timeline))
        {
            Console.WriteLine($"Message: {message.Data}");

            // Always call SetReplicationStatus() or assign LastAppliedLsn and LastFlushedLsn individually
            // so that Npgsql can inform the server which WAL files can be removed/recycled.
            conn.SetReplicationStatus(message.WalEnd);
            await conn.SendStatusUpdate(cToken);
        }
    }
}

/*
    static void Stream()
    {
        using (var replicationConnection = new PhysicalReplicationConnection(new NpgsqlConnectionStringBuilder(connectionString)))
        {
            replicationConnection.Open();

            // Create a replication slot if it doesn't exist.
            if (!replicationConnection.SlotExists(replicationSlotName))
            {
                replicationConnection.CreateReplicationSlot(replicationSlotName);
            }

            // Start replication.
            using (var replication = replicationConnection.StartReplication(replicationSlotName, PostgresLogSequenceNumber.Origin))
            {
                // Process changes.
                foreach (var change in replication)
                {
                    if (change is BeginMessage beginMessage)
                    {
                        Console.WriteLine($"Received BEGIN at LSN {beginMessage.NextLSN}");
                    }
                    else if (change is CommitMessage commitMessage)
                    {
                        Console.WriteLine($"Received COMMIT at LSN {commitMessage.NextLSN}");
                    }
                    else if (change is RelationalReplicationMessage relationalMessage)
                    {
                        // Process the actual data changes here.
                        Console.WriteLine($"Table: {relationalMessage.RelationId}, Operation: {relationalMessage.Kind}");
                    }
                }
            }
*/