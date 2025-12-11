using System.Diagnostics;

public class Node
{
    public string Name { get; set; }
    public List<string> ConnectedTo { get; set; }
}

public class Program
{
    private static List<Node> _nodes = new();

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
    private static int Part2(string fileName)
    {
        ReadData(fileName);
        var node = _nodes.Single(n => n.Name == "svr");
        var routes = TraverseDacFft(node, new());
        Console.WriteLine(routes);
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
    
    private static int TraverseDacFft(Node node, List<string> visited)
    {
        var finished = 0;
        var validNeighbors = node.ConnectedTo.Where(n => !visited.Contains(n)).ToList();
        
        foreach (var neighbor in validNeighbors)
        {
            // Console.WriteLine($"{node.Name} -> {string.Join(',', visited)}; valid neighbors: {string.Join(',', validNeighbors)}");
            if (neighbor == "out")
            {
                return visited.Contains("fft") && visited.Contains("dac") ? 1 : 0;
            }
            
            var next = _nodes.Single(n => n.Name == neighbor);
            finished += TraverseDacFft(next, visited.Append(neighbor).ToList());
        }

        return finished;
    }
}