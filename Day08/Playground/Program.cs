using System.Diagnostics;
using Playground;

public class Program
{
    private static List<Junction> _junctions = new();
    static void Main(string[] args)
    {
        ReadFile("testData.txt");
        Debug.Assert(SolvePart1(10) == 40);
        ReadFile("data.txt");
        Console.WriteLine(SolvePart1(1000));
        
        // 38688 Is too low
    }

    private static int SolvePart1(int repeats, int multiplyLargestNum = 3)
    {
        foreach (var _ in Enumerable.Range(0, repeats))
        {
            ConnectJolts();
        }

        var circuits = FindCircuits();
        var largestNums = circuits
            .Select(x => x.Count)
            .OrderByDescending(x => x)
            .Take(multiplyLargestNum)
            .ToList();
        return largestNums.Aggregate(1, (a, b) => a * b);
    }
    
    private static List<List<Junction>> FindCircuits()
    {
        List<List<Junction>> circuits = new();
        foreach (var junction in _junctions)
        {
            if (circuits.SelectMany(x => 
                    x.Select(y => y.Name))
                .Contains(junction.Name)) continue;
            
            var circuit = CreateCircuit(junction);
            circuits.Add(circuit);
        }
        
        Console.WriteLine(circuits.Count);
        return circuits;
    }

    private static List<Junction> CreateCircuit(Junction start)
    {
        var visited = new HashSet<Junction>();
        ExploreCircuit(start, visited);
        return visited.ToList();
    }
    
    private static void ExploreCircuit(Junction j, HashSet<Junction> visited)
    {
        if (!visited.Add(j))
            return;

        foreach (var next in j.ConnectedTo)
            ExploreCircuit(next, visited);
    }

   private static void ConnectJolts()
    {
        int minDistance = int.MaxValue, leftJunctionIdx = int.MaxValue, rightJunctionIdx = int.MaxValue;
        
        for (var junctionIdx1 = 0; junctionIdx1 != _junctions.Count - 1; junctionIdx1++)
        {
            var junction1 = _junctions[junctionIdx1];
            for (var junctionIdx2 = junctionIdx1 + 1; junctionIdx2 != _junctions.Count; junctionIdx2++)
            {
                var junction2 = _junctions[junctionIdx2];
                var distance = junction1.Coordinate.DistanceTo(junction2.Coordinate);
                
                if (distance > minDistance)
                    continue;
                
                if (junction1.ConnectedTo.Contains(junction2))
                    continue;
                
                minDistance = distance;
                leftJunctionIdx = junctionIdx1;
                rightJunctionIdx = junctionIdx2;
            }
        }
        
        _junctions[leftJunctionIdx].ConnectedTo.Add(_junctions[rightJunctionIdx]);
        _junctions[rightJunctionIdx].ConnectedTo.Add(_junctions[leftJunctionIdx]);
    }

    private static void ReadFile(string fileName)
    {
        File.ReadAllLines(fileName).ToList().ForEach(line =>
        {
            var parts = line.Split(',').Select(int.Parse).ToArray();
            _junctions.Add(new Junction()
            {
                Name = string.Join(",", parts),
                Coordinate = new()
                {
                    X = parts[0],
                    Y = parts[1],
                    Z = parts[2]
                },
                ConnectedTo = []
            });
        });
    }
}
