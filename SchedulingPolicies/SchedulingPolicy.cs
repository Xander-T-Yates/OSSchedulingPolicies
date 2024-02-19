using System.Collections.ObjectModel;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace SchedulingPolicies;

public abstract class SchedulingPolicy
{
    #region Enums

    protected enum FileSection : byte
    {
        Type,
        Id,
        ArrivalTime,
        ServiceTime,
        FinishTime,
        TurnaroundTime,
        NormalisedTurnaround,
        Extra1,

        Count
    }

    #endregion

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
        PreRunProcesses();
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
            ++ActiveProcess.PassedServiceTime;

        ++RunTime;

        return;

        void PrepareProcesses()
        {
            List<Process> toUnqueue = new();
            foreach (Process process in _queuedProcesses)
                if (!_runningProcesses.Contains(process) && RunTime >= process.ArrivalTime)
                {
                    _runningProcesses.Add(process);
                    toUnqueue.Add(process);
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
                if (process.PassedServiceTime < process.ServiceTime)
                    continue;

                _finishedProcesses.Add(process);
                toFinish.Add(process);

                process.FinishTime = RunTime;
                process.TurnaroundTime = RunTime - process.ArrivalTime;
            }

            if (toFinish.Count is not 0)
                foreach (Process process in toFinish)
                    _runningProcesses.Remove(process);
        }
    }
    public void SaveToFile(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);

        using (FileStream stream = File.Create(filePath)) { }

        using (StreamWriter writer = new(filePath))
        {
            for (int i = 0; i < (int)FileSection.Count; i++)
            {
                SaveProcess(writer, null, (FileSection)i, true);
                writer.Write(',');
            }

            writer.WriteLine();

            foreach (Process process in _finishedProcesses)
            {
                for (int i = 0; i < (int)FileSection.Count; i++)
                {
                    SaveProcess(writer, process, (FileSection)i, false);
                    writer.Write(',');
                }

                writer.WriteLine();
            }
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder().AppendLine(GetType().Name);

        if (_finishedProcesses.Count is not 0)
        {
            sb.AppendLine("- Finished processes: " + Environment.NewLine);
            foreach (Process process in _finishedProcesses)
                sb.AppendLine(process.ToString());
        }

        return sb.ToString();
    }

    protected virtual void PreRunProcesses() { }
    protected abstract void RunProcesses();
    protected virtual void SaveProcess(StreamWriter writer, Process? process, FileSection section, bool isHeader)
    {
        if (isHeader)
            switch (section)
            {
                case FileSection.Type:
                    writer.Write(GetType().Name);
                    break;

                case FileSection.Id:
                    writer.Write("ID");
                    break;

                case FileSection.ArrivalTime:
                    writer.Write("Arrival time");
                    break;

                case FileSection.ServiceTime:
                    writer.Write("Service time");
                    break;

                case FileSection.FinishTime:
                    writer.Write("Finish time");
                    break;

                case FileSection.TurnaroundTime:
                    writer.Write("Turnaround time");
                    break;

                case FileSection.NormalisedTurnaround:
                    writer.Write("Normalised turnaround");
                    break;
            }
        else
            switch (section)
            {
                case FileSection.Id:
                    writer.Write(process.Id);
                    break;

                case FileSection.ArrivalTime:
                    writer.Write(process.ArrivalTime.ToString("n0"));
                    break;

                case FileSection.ServiceTime:
                    writer.Write(process.ServiceTime.ToString("n0"));
                    break;

                case FileSection.FinishTime:
                    writer.Write(process.FinishTime.ToString("n0"));
                    break;

                case FileSection.TurnaroundTime:
                    writer.Write(process.TurnaroundTime.ToString("n0"));
                    break;

                case FileSection.NormalisedTurnaround:
                    writer.Write(process.NormalisedTurnaround.ToString("g2"));
                    break;
            }
    }

    #endregion
}