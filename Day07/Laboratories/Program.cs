using System.Diagnostics;

public class Program
{
    private static List<List<char>> _grid = new();
    private static Dictionary<(int row, int col), int> _memo = new();

    // main function
    static void Main(string[] args)
    {
        ReadFile("testData.txt");
        Debug.Assert(SolvePart1() == 21);
        Debug.Assert(SolvePart2() == 40);
        ReadFile("data.txt");
        Console.WriteLine(SolvePart1());
        Console.WriteLine(SolvePart2());
    }

    private static void ReadFile(string fileName)
    {
        _grid = new();
        _grid = File.ReadAllLines(fileName).Select(line => line.ToList()).ToList();
    }
    
    private static int SolvePart1()
    {
        var startCoors = FindCoordinates('S');
        var beams = new List<(int row, int col)>();
        beams.Add((startCoors.row, startCoors.col));
        
        var fallingDistance = _grid.Count - startCoors.row - 1; // Check later

        var splits = 0;
        foreach (var fallStep in Enumerable.Range(1, fallingDistance))
        {
            var newBeams = new List<(int row, int col)>();
            foreach (var beam in beams)
            {
                // Examine beam.row + 1 and beam.col
                var gridElement = _grid[beam.row + 1][beam.col];
                if (gridElement == '^')
                {
                    splits++;
                    var leftSpit = (beam.row + 1, beam.col - 1);
                    var rightSpit = (beam.row + 1, beam.col + 1);
                    newBeams.AddRange(leftSpit, rightSpit);
                }
                else
                {
                    newBeams.Add((beam.row + 1, beam.col));
                }
            }

            beams = newBeams.Distinct().ToList();
        }
        
        return splits;
    }
    
    private static int SolvePart2()
    {
        _memo = new();
        var startCoors = FindCoordinates('S');
        var answer = CalculateContributionFrom(startCoors);
        Console.WriteLine(answer);
        return answer;
    }

    private static int CalculateContributionFrom((int row, int col) coors)
    {
        if (_memo.TryGetValue(coors, out var from))
        {
            return from;
        }
        
        if (coors.row == _grid.Count - 1)
        {
            return 1;
        }

        int result;
        if (_grid[coors.row + 1][coors.col] == '^')
        {
            result = CalculateContributionFrom((coors.row + 1, coors.col - 1))
                     + CalculateContributionFrom((coors.row + 1, coors.col + 1));
        }
        else
        {
            result = CalculateContributionFrom((coors.row + 1, coors.col));
        }
        
        _memo[coors] = result;
        return result;
    }

    private static (int row, int col) FindCoordinates(char element)
    {
        var coord = _grid
            .SelectMany((row, rowIndex) => row.Select((c, colIndex) => (row: rowIndex, col: colIndex, c)))
            .First(x => x.c == element);
        return (coord.row, coord.col);
    }
}