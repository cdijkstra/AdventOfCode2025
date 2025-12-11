using System.Diagnostics;

public class Node
{
    public string Name { get; set; }
    public List<string> ConnectedTo { get; set; }
}

public class Program
{
    private static List<Node> _nodes = new();
    private static Dictionary<(string nodeName, bool hasDac, bool hasFft), long> _nodeDict = new();

    static void Main(string[] args)
    {
        var sw = new Stopwatch();
        sw.Start();
        Debug.Assert(Part1("testdata.txt") == 5);
        Console.WriteLine(Part1("data.txt"));
        Debug.Assert(Part2("testdata2.txt") == 2);
        Console.WriteLine(Part2("data.txt"));
        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
    }

    private static void ReadData(string fileName)
    {
        _nodes.Clear();
        foreach (var line in File.ReadAllLines(fileName))
        {
            var node = line[..line.IndexOf(':')];
            var connectedTo = line[(line.IndexOf(':') + 2)..].Split();
            _nodes.Add(new Node { Name = node, ConnectedTo = connectedTo.ToList() });
        }
    }

    private static int Part1(string fileName)
    {
        ReadData(fileName);
        var node = _nodes.Single(n => n.Name == "you");
        var routes = Traverse(node, new());
        return routes;
        
    }
    private static long Part2(string fileName)
    {
        ReadData(fileName);
        var node = _nodes.Single(n => n.Name == "svr");
        var routes = TraverseDacFft(node, new());
        Console.WriteLine($"Found routes = {routes}");
        return routes;
    }

    private static int Traverse(Node node, List<string> visited)
    {
        var finished = 0;
        var validNeighbors = node.ConnectedTo.Where(n => !visited.Contains(n)).ToList();
        
        foreach (var neighbor in validNeighbors)
        {
            if (neighbor == "out")
            {
                return 1;
            }
            
            var next = _nodes.Single(n => n.Name == neighbor);
            finished += Traverse(next, visited.Append(neighbor).ToList());
        }

        return finished;
    }
    
    private static long TraverseDacFft(Node node, List<string> visited)
    {    
        var (hasDac, hasFft) = (visited.Contains("dac"), visited.Contains("fft"));
        var cacheKey = (node.Name, hasDac, hasFft);
        if (_nodeDict.TryGetValue(cacheKey, out long count))
            return count;
        
        long finished = 0;
        var validNeighbors = node.ConnectedTo.Where(n => !visited.Contains(n)).ToList();
        
        foreach (var neighbor in validNeighbors)
        {
            if (neighbor == "out")
                return hasDac && hasFft ? 1 : 0;
            
            var next = _nodes.Single(n => n.Name == neighbor);
            finished += TraverseDacFft(next, visited.Append(neighbor).ToList());
        }

        _nodeDict[cacheKey] = finished;
        return finished;
    }
}