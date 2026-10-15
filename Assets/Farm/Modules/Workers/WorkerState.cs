namespace Orchard.Workers
{
    public enum WorkerState
    {
        Idle,
        WalkingToCrop,
        Harvesting,
        WaitingForCustomer,
        Delivering,
        Selling
    }
}
