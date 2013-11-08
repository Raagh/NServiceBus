namespace NServiceBus.Distributor.ReadyMessages
{
    public struct Headers
    {
        public const string WorkerCapacityAvailable = "NServiceBus.Distributor.WorkerCapacityAvailable";
        public const string WorkerStarting = "NServiceBus.Distributor.WorkerStarting";
        public const string DisconnectWorker = "NServiceBus.Distributor.DisconnectWorker";
        public const string WorkerSessionId = "NServiceBus.Distributor.WorkerSessionId";
    }
}