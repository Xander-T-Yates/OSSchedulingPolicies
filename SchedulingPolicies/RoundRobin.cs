using System.Text;

namespace SchedulingPolicies;

public sealed class RoundRobin(int quantumSize) : SchedulingPolicy
{
    #region Attributes

    public readonly int QuantumSize = quantumSize;

    private int _passedQuantum;
    private char _lastId;

    #endregion

    #region Operations

    protected override void PreRunProcesses()
    {
        if (ActiveProcess is not null && ActiveProcess.Id == _lastId)
            return;

        _passedQuantum = 0;

        if (ActiveProcess is not null)
            _lastId = ActiveProcess.Id;
    }
    protected override void RunProcesses()
    {
        ActiveProcess = _runningProcesses.First();

        if (++_passedQuantum > QuantumSize && _runningProcesses.Count is > 1)
        {
            _runningProcesses.Remove(ActiveProcess);
            _runningProcesses.Add(ActiveProcess);
        }
    }
    protected override void SaveProcess(StreamWriter writer, Process? process, FileSection section, bool isHeader)
    {
        if (section is not FileSection.Extra1 || !isHeader)
        {
            base.SaveProcess(writer, process, section, isHeader);
            return;
        }

        writer.Write($"Quantum size: {QuantumSize:n0}");
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder().AppendLine($"Quantum Size: {QuantumSize:n0}");
        sb.AppendLine(base.ToString());

        return sb.ToString();
    }

    #endregion
}