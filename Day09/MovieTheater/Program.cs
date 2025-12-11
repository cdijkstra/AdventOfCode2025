using System.Diagnostics;
using _09;


public class Program
{
    private static Grid _grid;
    static void Main(string[] args)
    {
        // Debug.Assert(Part1("testdata.txt") == 50);
        // Debug.Assert(Part2("testdata.txt") == 24);
        // Console.WriteLine(Part1("data.txt"));
        Console.WriteLine(Part2("data.txt"));
        
        // 2930732777 too high
        // 2146031745 not right
    }
    
    private static void ReadData(string fileName)
    {
        var nums = File.ReadAllLines(fileName)
            .Select(line => line.Split(','))
            .Select(parts => new Coordinates(int.Parse(parts[0]), int.Parse(parts[1])))
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
        var reds = _grid.Data;
        
        var pq = new PriorityQueue<(Coordinates left, Coordinates right, long area), long>();
        for (var i = 0; i < reds.Count; i++)
        {
            var point1 = new Coordinates(reds[i].X, reds[i].Y);
            point1.X -= _grid.MinX();
            point1.Y -= _grid.MinY();
            for (var j = 0; j < reds.Count; j++)
            {
                var point2 = new Coordinates(reds[j].X, reds[j].Y);
                point2.X -= _grid.MinX();
                point2.Y -= _grid.MinY();
                
                var area = (Math.Abs(point1.X - point2.X) + 1) * (Math.Abs(point1.Y - point2.Y) + 1);
                pq.Enqueue((point1, point2, area), -area);
            }
        }
        
        Console.WriteLine("Start reading from queue");
        
        while (pq.Count > 0)
        {
            var pair = pq.Dequeue();
            long dx = pair.left.X - pair.right.X;
            long dy = pair.left.Y - pair.right.Y;
        
            if (dx == 0 || dy == 0) continue;
            
            var minX = Math.Min(pair.left.X, pair.right.X);
            var maxX = Math.Max(pair.left.X, pair.right.X);
            var minY = Math.Min(pair.left.Y, pair.right.Y);
            var maxY = Math.Max(pair.left.Y, pair.right.Y);

            bool allFilled = true;
            for (var y = minY; y <= maxY && allFilled; y++)
            {
                // Check if borders contain any element that is false
                if (_grid.GetRowSubset(minX, maxX, y).All(el => el)) continue;
                allFilled = false;
            }

            for (var x = minX; x <= maxX && allFilled; x++)
            {
                if (_grid.GetColumnSubset(minY, maxY, x).All(el => el)) continue;
                allFilled = false;
            }
            
            if (!allFilled) continue;
            
            Console.WriteLine($"Found a match: {pair.area}");
            return pair.area;
        }
        
        return 0;
    }
}