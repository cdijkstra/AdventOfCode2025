using System.Diagnostics;
using _09;


public class Program
{
    private static Grid _grid;
    static void Main(string[] args)
    {
        // ReadData("testdata.txt");
        // Debug.Assert(Part1() == 50);
        ReadData("data.txt");
        Console.WriteLine(Part1());
        // 2147472128 not right
        // 2147462366 not right
    }
    
    private static void ReadData(string fileName)
    {
        var nums = File.ReadAllLines(fileName)
            .Select(line => line.Split(','))
            .Select(parts => new Coordinates(int.Parse(parts[0]), int.Parse(parts[1])))
            .ToList();
        _grid = new Grid(nums);
    }
    
    private static long Part1()
    {
        long maxArea = 0;
        for (var i = 0; i < _grid.Data.Count; i++)
        {
            var point1 = _grid.Data[i];
            for (var j = i + 1; j < _grid.Data.Count; j++)
            {
                var point2 = _grid.Data[j];
                var area = (Math.Abs(point1.X - point2.X) + 1) * (Math.Abs(point1.Y - point2.Y) + 1);
                Console.WriteLine(area);
                if (area > maxArea)
                    maxArea = area;
            }
        }
        Console.WriteLine(maxArea);
        return maxArea;
    }
}