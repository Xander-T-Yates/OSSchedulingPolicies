namespace SchedulingPolicies;

public sealed class FCFS : SchedulingPolicy
{
    #region Operations

    protected override void RunProcesses()
    {
        ActiveProcess = _runningProcesses.First();
    }

    #endregion
}