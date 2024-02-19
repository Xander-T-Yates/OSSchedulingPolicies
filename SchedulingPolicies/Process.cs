using System.Text;

namespace SchedulingPolicies;

public class Process(int arrivalTime, int serviceTime)
{
    #region Attributes

    private static int _nextId;

    public readonly char Id = (char)_nextId++;
    public readonly int ArrivalTime = arrivalTime;
    public readonly int ServiceTime = serviceTime;

    public int RemainingServiceTime { get; internal set; }
    public int FinishTime { get; internal set; }
    public int TurnaroundTime { get; internal set; }
    public float NormalisedTurnaround { get; internal set; }
    
    #endregion

    #region Operations

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder().AppendLine($"Process {Id} - Times: ");
        sb.Append($"Arrival: {ArrivalTime:n0} | ")
            .AppendLine($"Service: {ServiceTime:n0};");

        if (RemainingServiceTime <= 0)
        {
            sb.Append($"Finish: {FinishTime:n0} | ")
                .AppendLine($"Turnaround: {TurnaroundTime:n0};")
                .AppendLine($"Normalised turnaround: {NormalisedTurnaround:g2}");
        }
        
        return sb.ToString();
    }

    #endregion
}