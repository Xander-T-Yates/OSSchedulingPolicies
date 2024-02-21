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
        PrepareProcesses();
        if (_runningProcesses.Count is not 0)
            ActiveProcess = _runningProcesses[0];

        if (ActiveProcess is null)
        {
            if (_lastId is not (char)0)
            {
                _passedQuantum = 0;
                _lastId = (char)0;
            }
        }
        else
        {
            if (_lastId != ActiveProcess.Id)
            {
                if (_lastId is not (char)0)
                    _passedQuantum = 0;

                _lastId = ActiveProcess.Id;
            }

            if (++_passedQuantum >= QuantumSize)
            {
                _runningProcesses.Remove(ActiveProcess);
                _runningProcesses.Add(ActiveProcess);

                ActiveProcess = _runningProcesses[0];
            }
        }
    }
    protected override void RunProcesses() { }
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