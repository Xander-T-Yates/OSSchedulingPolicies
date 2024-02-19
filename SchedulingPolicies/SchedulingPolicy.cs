using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SchedulingPolicies;

public abstract class SchedulingPolicy
{
    #region Attributes

    protected readonly List<Process> _queuedProcesses = new();
    protected readonly List<Process> _runningProcesses = new();
    protected readonly List<Process> _finishedProcesses = new();
    
    public int RunTime { get; private set; }
    protected Process? ActiveProcess { get; set; }

    public bool IsFinished =>
        _queuedProcesses.Count is 0 && _runningProcesses.Count is 0 && _finishedProcesses.Count is not 0;

    #endregion

    #region Operations
    
    public void QueueProcess(Process process)
    {
        if (_queuedProcesses.Contains(process))
            return;
        
        _queuedProcesses.Add(process);
    }
    public void Run()
    {
        CheckFinishedProcesses();

        if (IsFinished)
        {
            --RunTime;
            return;
        }
        
        PrepareProcesses();

        if (_runningProcesses.Count is not 0)
            RunProcesses();

        if (ActiveProcess is not null)
            --ActiveProcess.RemainingServiceTime;
            
        ++RunTime;
        
        return;
        
        void PrepareProcesses()
        {
            List<Process> toUnqueue = new();
            foreach (Process process in _queuedProcesses)
            {
                if (!_runningProcesses.Contains(process) && RunTime >= process.ArrivalTime)
                {
                    _runningProcesses.Add(process);
                    toUnqueue.Add(process);
                }
            }

            if (toUnqueue.Count is not 0)
                _queuedProcesses.RemoveAll(toUnqueue.Contains);
        }
        void CheckFinishedProcesses()
        {
            if (_runningProcesses.Count is 0)
                return;
            
            List<Process> toFinish = new();
            foreach (Process process in _runningProcesses)
            {
                if (process.RemainingServiceTime > 0) 
                    continue;
                
                _finishedProcesses.Add(process);
                toFinish.Add(process);

                process.FinishTime = RunTime;
                process.TurnaroundTime = RunTime - process.ArrivalTime;
                process.NormalisedTurnaround = MathF.Round((float)process.TurnaroundTime / process.ServiceTime, 3);
            }

            if (toFinish.Count is not 0)
            {
                foreach (Process process in toFinish)
                    _runningProcesses.Remove(process);
            }
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder().AppendLine(GetType().Name);

        if (_finishedProcesses.Count is not 0)
        {
            sb.AppendLine("• Finished processes: ");
            foreach (Process process in _finishedProcesses)
                sb.AppendLine(process.ToString());
        }

        return sb.ToString();
    }

    protected abstract void RunProcesses();

    #endregion
}