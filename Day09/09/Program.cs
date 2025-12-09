using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using _09;


public class Program
{
    private static Grid _grid;
    static void Main(string[] args)
    {
        Debug.Assert(Part1("testdata.txt") == 50);
        Debug.Assert(Part2("testdata.txt") == 24);
        // Console.WriteLine(Part1("data.txt"));
        // Console.WriteLine(Part2("data.txt"));
        
        // 2930732777 too high
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
    
    private static bool IsInsideLoop(long x, long y)
    {
        // Ray casting algorithm for point-in-polygon
        int count = 0;
        for (int i = 0, j = _grid.Data.Count - 1; i < _grid.Data.Count; j = i++)
        {
            var xi = _grid.Data[i].X;
            var yi = _grid.Data[i].Y;
            var xj = _grid.Data[j].X;
            var yj = _grid.Data[j].Y;

            if ((yi > y) != (yj > y) &&
                (x < (xj - xi) * (y - yi) / (double)(yj - yi) + xi))
            {
                count++;
            }
        }
        return (count % 2) == 1;
    }
    
    private static long Part2(string fileName)
    {
        ReadData(fileName);

        _grid.CreateConnectedGrid();
        var reds = _grid.Data
            .Where(c => c.Type == TileType.Red)
            .ToList();
        
        long maxArea = 0;
        var pq = new PriorityQueue<(Coordinates left, Coordinates right, long area), long>();
        for (var i = 0; i < reds.Count; i++)
        {
            var point1 = reds[i];
            for (var j = i + 1; j < reds.Count; j++)
            {
                var point2 = reds[j];
                
                var area = (Math.Abs(point1.X - point2.X) + 1) * (Math.Abs(point1.Y - point2.Y) + 1);
                if (area > 2930732777) continue;
                Console.WriteLine($"Considering ({point1.X},{point1.Y}) and ({point2.X},{point2.Y}) with area {area}");
                pq.Enqueue((point1, point2, area), -area);
            }
        }
        
        Console.WriteLine("Start reading from queue");
        
        while (pq.Count > 0)
        {
            var pair = pq.Dequeue();
            // Console.WriteLine($"Processing ({pair.left.X},{pair.left.Y}) and ({pair.right.X},{pair.right.Y}) with area {pair.area}");
            long dx = pair.left.X - pair.right.X;
            long dy = pair.left.Y - pair.right.Y;
        
            if (dx == 0 || dy == 0) continue;
            
            var minX = Math.Min(pair.left.X, pair.right.X);
            var maxX = Math.Max(pair.left.X, pair.right.X);
            var minY = Math.Min(pair.left.Y, pair.right.Y);
            var maxY = Math.Max(pair.left.Y, pair.right.Y);
            
            var dataSet = new HashSet<(long, long)>(_grid.Data.Select(c => (c.X, c.Y)));
        }
        
        return 0;
    }
    
    private static List<Coordinates> FindAllNeighbors(Coordinates coors)
    {
        var neighbors = new List<Coordinates>();
        var leftNeighbor = _grid.Data
            .Where(c => c.Y == coors.Y && c.X < coors.X)
            .OrderByDescending(c => c.X)
            .FirstOrDefault();

        var rightNeighbor = _grid.Data
            .Where(c => c.Y == coors.Y && c.X > coors.X)
            .OrderBy(c => c.X)
            .FirstOrDefault();
        
        var upNeighbor = _grid.Data
            .Where(c => c.X == coors.X && c.Y > coors.Y)
            .OrderByDescending(c => c.X)
            .FirstOrDefault();

        var downNeighbor = _grid.Data
            .Where(c => c.X == coors.X && c.Y < coors.Y)
            .OrderBy(c => c.X)
            .FirstOrDefault();

        if (leftNeighbor != null) neighbors.Add(leftNeighbor);
        if (rightNeighbor != null) neighbors.Add(rightNeighbor);
        if (upNeighbor != null) neighbors.Add(upNeighbor);
        if (downNeighbor != null) neighbors.Add(downNeighbor);
        
        return neighbors;
    }
}