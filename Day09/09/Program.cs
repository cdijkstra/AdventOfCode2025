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
        Console.WriteLine(Part1("data.txt"));
        Console.WriteLine(Part2("data.txt"));
        
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
    
    private static long Part2(string fileName)
    {
        ReadData(fileName);

        _grid.CreateConnectedGrid();
        var redCoordinates = _grid.Data
            .Where(c => c.Type == TileType.Red)
            .ToList();
        
        Console.WriteLine("Starting priority queue");
        var pq = new PriorityQueue<(Coordinates left, Coordinates right, long area), long>();
        long maxLength = 0;
        
        for (var i = 0; i < redCoordinates.Count; i++)
        {
            var point1 = redCoordinates[i];
            for (var j = i + 1; j < redCoordinates.Count; j++)
            {
                var point2 = redCoordinates[j];
                
                var area = (Math.Abs(point1.X - point2.X) + 1) * (Math.Abs(point1.Y - point2.Y) + 1);
                if (area > 2930732777) continue;
                Console.WriteLine($"Considering ({point1.X},{point1.Y}) and ({point2.X},{point2.Y}) with area {area}");
                pq.Enqueue((point1, point2, area), -area);
            }
        }

        Console.WriteLine("Start reading from queue");
        long maxArea = 0;
        
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
            var interiorByY = _grid.Interior
                .GroupBy(r => r.Y)
                .ToDictionary(g => g.Key, g => g.ToList());
        
            bool allFilled = true;
            for (var x = minX; x <= maxX && allFilled; x++)
            {
                for (var y = minY; y <= maxY && allFilled; y++)
                {
                    if (dataSet.Contains((x, y))) continue;
        
                    if (interiorByY.TryGetValue(y, out var ranges) &&
                        ranges.Any(r => r.FromX <= x && r.ToX >= x)) continue;
        
                    allFilled = false;
                    break;
                }
            }

            if (!allFilled) continue;

            if (pair.area <= maxArea) continue;
            
            maxArea = pair.area;
            Console.WriteLine(maxArea);
            return maxArea;
        }
        
        Console.WriteLine(maxLength);
        return maxLength;
    }
}