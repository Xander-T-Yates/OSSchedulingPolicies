using System.Net.NetworkInformation;
using System.Text;

namespace SchedulingPolicies;

public class Process(int arrivalTime, int serviceTime)
{
    #region Attributes

    private static int _nextId = 'A';

    public readonly char Id = (char)_nextId++;
    public readonly int ArrivalTime = arrivalTime;
    public readonly int ServiceTime = serviceTime;

    public int PassedServiceTime { get; internal set; }
    public int FinishTime { get; internal set; }
    public int TurnaroundTime { get; internal set; }
    public float NormalisedTurnaround => MathF.Round((float)TurnaroundTime / ServiceTime, 3);

    #endregion

    #region Operations

    public override string ToString()
    {
        StringBuilder sb =
            new StringBuilder().AppendLine($"Process {Id} (passed service time: {PassedServiceTime:n0}) - Times: ");
        sb.Append($"Arrival: {ArrivalTime:n0} | ")
            .AppendLine($"Service: {ServiceTime:n0};");

        if (PassedServiceTime >= ServiceTime)
            sb.Append($"Finish: {FinishTime:n0} | ")
                .AppendLine($"Turnaround: {TurnaroundTime:n0};")
                .AppendLine($"Normalised turnaround: {NormalisedTurnaround:g4}");

        return sb.ToString();
    }

    #endregion
}