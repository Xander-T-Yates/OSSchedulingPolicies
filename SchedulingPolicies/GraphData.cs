using System.Drawing;
using Spire.Xls;

namespace SchedulingPolicies;

public sealed class GraphData
{
    #region Attributes

    private List<KeyValuePair<char, List<int>>> _nodes = new();
    internal int endTime;

    #endregion

    #region Operations

    public void CreateNode(int runTime, char processId)
    {
        int nodeId = _nodes.FindIndex(o => o.Key == processId);
        if (nodeId is not -1)
            _nodes[nodeId].Value.Add(runTime);
        else
            _nodes.Add(new(processId, new() { runTime }));
    }
    public void SaveToFile(string fileName)
    {
        if (File.Exists(fileName))
            File.Delete(fileName);

        using (FileStream stream = File.Create(fileName))
        {
        }

        Workbook workbook = new();
        workbook.Worksheets.Clear();
        Worksheet? worksheet = workbook.Worksheets.Add("Graph");

        for (var i = 0; i < endTime; i++)
            worksheet.Range[1, i + 2].Value = (i + 1).ToString("n0");

        for (var i = 0; i < _nodes.Count; i++)
        {
            KeyValuePair<char, List<int>> node = _nodes[i];

            worksheet.Range[i + 2, 1].Value = node.Key.ToString();

            for (var j = 0; j < endTime; j++)
                if (node.Value.Contains(j))
                    worksheet.Range[i + 2, j + 2].Style.Color = Color.Gray;
        }

        worksheet.AllocatedRange.RowHeight = (worksheet.AllocatedRange.ColumnWidth = 3) * 6;
        workbook.SaveToFile(fileName);
    }

    #endregion
}