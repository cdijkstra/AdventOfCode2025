using System.Diagnostics;
using Playground;

public class Program
{
    private static List<Junction> _junctions = new();
    static void Main(string[] args)
    {
        var sw = new Stopwatch();
        sw.Start();
        ReadFile("testData.txt");
        Debug.Assert(SolvePart1(10) == 40);
        ReadFile("testData.txt");
        Debug.Assert(SolvePart2() == 25272);
        
        ReadFile("data.txt");
        Console.WriteLine(SolvePart1(1000));
        ReadFile("data.txt");
        Console.WriteLine(SolvePart2());
        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
    }

    private static int SolvePart1(int repeats, int multiplyLargestNum = 3)
    {
        var connected = 0;
        var topPairs = GetPairs(repeats, true);
        var idx = 0;
        while (connected != repeats)
        {
            // Creating the connections
            var A = topPairs[idx].A;
            var B = topPairs[idx].B;
            
            if (!A.ConnectedTo.Contains(B))
            {
                A.ConnectedTo.Add(B);
                B.ConnectedTo.Add(A);
                connected++;
            }
        
            idx++;
        }
        
        var circuits = FindCircuits();
        var largestNums = circuits
            .Select(x => x.Count)
            .OrderByDescending(x => x)
            .Take(multiplyLargestNum)
            .ToList();
        
        return largestNums.Aggregate(1, (a, b) => a * b);
    }
    
    private static long SolvePart2()
    {
        var sortedPairs = GetPairs(5000, true);
        var idx = -1;
        while (FindCircuits().Count != 1)
        {
            idx++;
            sortedPairs[idx].A.ConnectedTo.Add(sortedPairs[idx].B);
            sortedPairs[idx].B.ConnectedTo.Add(sortedPairs[idx].A);
        }
        
        long answer = (long) sortedPairs[idx].A.Coordinate.X * sortedPairs[idx].B.Coordinate.X;
        Console.WriteLine(answer);
        return answer;
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

    public static List<JunctionPair> GetPairs(int topN, bool orderPairs = true)
    {
        var pairs = new List<JunctionPair>();
        for (int i = 0; i < _junctions.Count - 1; i++)
        {
            for (int j = i + 1; j < _junctions.Count; j++)
            {
                var j1 = _junctions[i];
                var j2 = _junctions[j];
                int distance = j1.Coordinate.DistanceTo(j2.Coordinate);
                pairs.Add(new JunctionPair(j1, j2, distance));
            }
        }

        if (!orderPairs) return pairs;
        
        pairs.Sort((p1, p2) => p1.Distance.CompareTo(p2.Distance));
        
        // Take enough pairs for duplicates
        var topPairs = pairs
            .Take(2 * topN)
            .ToList();

        return topPairs;
    }


    private static void ReadFile(string fileName)
    {
        _junctions = new();
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
    
    // Not used anymore
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
}
