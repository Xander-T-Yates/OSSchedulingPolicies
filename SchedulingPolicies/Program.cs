﻿namespace SchedulingPolicies;

public class Program
{
    private static void Main(string[] args)
    {
        SchedulingPolicy policy = new FCFS();
        policy = new RoundRobin(6);

        const bool optimiseArrivalTime = false;

        if (!optimiseArrivalTime)
        {
            policy.QueueProcess(new(0, 3));
            policy.QueueProcess(new(2, 6));
            policy.QueueProcess(new(5, 5));
            policy.QueueProcess(new(6, 3));
            policy.QueueProcess(new(8, 6));
            policy.QueueProcess(new(9, 2));
            policy.QueueProcess(new(10, 6));
        }
        else
        {
            policy.QueueProcess(new(2, 3));
            policy.QueueProcess(new(9, 6));
            policy.QueueProcess(new(6, 5));
            policy.QueueProcess(new(5, 3));
            policy.QueueProcess(new(8, 6));
            policy.QueueProcess(new(0, 2));
            policy.QueueProcess(new(10, 6));
        }

        while (!policy.IsFinished)
            policy.Run();

        policy.SaveToFile("Output.csv");

        Console.WriteLine(policy.ToString());
    }
}