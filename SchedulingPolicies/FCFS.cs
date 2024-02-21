namespace SchedulingPolicies;

public sealed class FCFS : SchedulingPolicy
{
    protected override void PreRunProcesses()
    {
        PrepareProcesses();
        if (_runningProcesses.Count is not 0)
            ActiveProcess = _runningProcesses[0];
    }
    protected override void RunProcesses() { }
}