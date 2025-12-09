using System.Diagnostics;
using _09;


public class Program
{
    private static Grid _grid;
    static void Main(string[] args)
    {
        ReadData("testdata.txt");
        Console.WriteLine("Hi");
        Debug.Assert(Part1() == 50);
        ReadData("data.txt");
        Console.WriteLine(Part1());
        // 2147472128 not right
        // 2147462366 not right
    }
    
    private static void ReadData(string fileName)
    {
        var nums = File.ReadAllLines(fileName)
            .Select(line => line.Split(','))
            .Select(parts => new Coordinates(int.Parse(parts[1]), int.Parse(parts[0])))
            .ToList();
        _grid = new Grid(nums);
    }
    
    private static int Part1()
    {
        var pq = new PriorityQueue<(Coordinates left, Coordinates right), int>();

        var foundSizes = new List<int>();
        
        for (var gridIdx1 = 0; gridIdx1 != _grid.Data.Count - 1; gridIdx1++)
        {
            var point1 = _grid.Data[gridIdx1];
            for (var gridIdx2 = gridIdx1 + 1; gridIdx2 != _grid.Data.Count; gridIdx2++)
            {
                var point2 = _grid.Data[gridIdx2];
                
                var size = (Math.Abs(point1.X - point2.X) + 1) * (Math.Abs(point1.Y - point2.Y) + 1);
                foundSizes.Add(size);
            }
        }

        Console.WriteLine(foundSizes.Max());
        return foundSizes.Max();
        
        // while (pq.Count > 0) {
        //     pq.TryDequeue(out string task,
        //         out int priority);
        //     Console.WriteLine($"Dequeued: {task} with priority {priority}");
        // }
    }
}
