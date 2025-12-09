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
        long maxArea = 0;
        var redCoordinates = _grid.Data
            .Where(c => c.Type == TileType.Red)
            .ToList();
        
        
        for (var i = 0; i < redCoordinates.Count; i++)
        {
            var point1 = redCoordinates[i];
            for (var j = i + 1; j < redCoordinates.Count; j++)
            {
                var point2 = redCoordinates[j];
                long dx = point1.X - point2.X;
                long dy = point1.Y - point2.Y;

                if (dx != 0 && dy != 0)
                {
                    var minX = Math.Min(point1.X, point2.X);
                    var maxX = Math.Max(point1.X, point2.X);
                    var minY = Math.Min(point1.Y, point2.Y);
                    var maxY = Math.Max(point1.Y, point2.Y);

                    bool allFilled = true;
                    for (var x = minX; x <= maxX && allFilled; x++)
                    {
                        for (var y = minY; y <= maxY; y++)
                        {
                            if (!_grid.Data.Any(c => c.X == x && c.Y == y))
                            {
                                allFilled = false;
                                break;
                            }
                        }
                    }
                    if (!allFilled) continue;
                }
                
                var area = (Math.Abs(point1.X - point2.X) + 1) * (Math.Abs(point1.Y - point2.Y) + 1);
                if (area > maxArea)
                    maxArea = area;
            }
        }
        
        Console.WriteLine(maxArea);
        return maxArea;
    }
}