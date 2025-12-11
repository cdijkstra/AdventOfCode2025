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
        Debug.Assert(Part1("testdata.txt") == 5);
        Console.WriteLine(Part1("data.txt"));
        Debug.Assert(Part2("testdata2.txt") == 2);
        Console.WriteLine(Part2("data.txt"));
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
        var routes = TraverseDacFft();
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
    
    private static long TraverseDacFft()
    {
        long finished = 0;
        var firstNode = _nodes.Single(n => n.Name == "svr");
        HashSet<string> PassBy = new() { "fft", "dac" };
        
        var pq = new PriorityQueue<(Node node, HashSet<string> visited), int>();
        firstNode.ConnectedTo.ForEach(n => pq.Enqueue((_nodes.Single(nn => nn.Name == n), new()), 0));
        while (pq.Count > 0)
        {
            var (node, visited) = pq.Dequeue();
            bool hasDac = visited.Contains("dac");
            bool hasFft = visited.Contains("fft");
            var cacheKey = (node.Name, hasDac, hasFft);
            
            if (_nodeDict.TryGetValue(cacheKey, out long count))
            {
                Console.WriteLine($"FOUND IN CACHE! Count = {count}");
                finished += count;
                continue;
            }
            
            var validNeighbors = node.ConnectedTo.Where(n => !visited.Contains(n)).ToList();
            int passByCount = PassBy.Count(item => visited.Contains(item));
            int localFinished = 0;

            foreach (var neighbor in validNeighbors)
            {
                if (neighbor == "out")
                {
                    if (passByCount == 2)
                    {
                        Console.WriteLine($"FINISHED!; Count = {++finished}");
                    }
                }
                else
                {
                    var next = _nodes.Single(n => n.Name == neighbor);
                    var newVisited = new HashSet<string>(visited) { neighbor };
                    pq.Enqueue((next, newVisited), -passByCount);
                }
            }
            
            _nodeDict[cacheKey] = finished;
        }
        
        
        return finished;
    }
}