using System.Diagnostics;
using _09;


public class Program
{
    private static Grid _grid;
    static void Main(string[] args)
    {
        Debug.Assert(Part1("testdata.txt") == 50);
        Debug.Assert(Part2("testdata.txt") == 24);
        Console.WriteLine(Part1("data.txt"));
        Console.WriteLine(Part2("data.txt"));
    }
    
    private static void ReadData(string fileName)
    {
        var nums = File.ReadAllLines(fileName)
            .Select(line => line.Split(','))
            .Select(parts => new Coordinates(TileType.Red, int.Parse(parts[0]), int.Parse(parts[1])))
            .ToList();
        _grid = new Grid(nums);
    }
    
    private static long Part1(string fileName)
    {
        ReadData(fileName);
        long maxArea = 0;
        for (var i = 0; i < _grid.Data.Count; i++)
        {
            var point1 = _grid.Data[i];
            for (var j = i + 1; j < _grid.Data.Count; j++)
            {
                var point2 = _grid.Data[j];
                var area = (Math.Abs(point1.X - point2.X) + 1) * (Math.Abs(point1.Y - point2.Y) + 1);
                if (area > maxArea)
                    maxArea = area;
            }
        }
        return maxArea;
    }
    
    private static long Part2(string fileName)
    {
        ReadData(fileName);

        _grid.CreateConnectedGrid();
        var redCoordinates = _grid.Data
            .Where(c => c.Type == TileType.Red)
            .ToList();
        
        Console.WriteLine("Starting priority queue");
        var pq = new PriorityQueue<(Coordinates left, Coordinates right, long area), long>();
        
        for (var i = 0; i < redCoordinates.Count; i++)
        {
            var point1 = redCoordinates[i];
            for (var j = i + 1; j < redCoordinates.Count; j++)
            {
                var point2 = redCoordinates[j];
                
                var area = (Math.Abs(point1.X - point2.X) + 1) * (Math.Abs(point1.Y - point2.Y) + 1);
                pq.Enqueue((point1, point2,area), -area);
            }
        }

        Console.WriteLine("Start reading from queue");
        long maxArea = 0;
        
        while (pq.Count > 0)
        {
            var pair = pq.Dequeue();
            long dx = pair.left.X - pair.right.X;
            long dy = pair.left.Y - pair.right.Y;
            
            if (dx != 0 && dy != 0)
            {
                var minX = Math.Min(pair.left.X, pair.right.X);
                var maxX = Math.Max(pair.left.X, pair.right.X);
                var minY = Math.Min(pair.left.Y, pair.right.Y);
                var maxY = Math.Max(pair.left.Y, pair.right.Y);
            
                bool allFilled = true;
                for (var x = minX; x <= maxX && allFilled; x++)
                {
                    for (var y = minY; y <= maxY; y++)
                    {
                        // If we find any point not in Data or in Interior range, we stop
                        if (!(_grid.Data.Any(c => c.X == x && c.Y == y) ||
                            _grid.Interior.Any(inter => inter.Y == y && 
                                                         inter.FromX <= x && 
                                                         inter.ToX >= x)))
                        {
                            allFilled = false;
                            break;
                        }
                    }
                }
                if (!allFilled) continue;

                if (pair.area > maxArea)
                {
                    maxArea = pair.area;
                    Console.WriteLine(maxArea);
                }
            }
        }
        
        Console.WriteLine(maxArea);
        return maxArea;
    }
}