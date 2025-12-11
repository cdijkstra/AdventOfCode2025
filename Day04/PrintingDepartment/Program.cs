using System.Diagnostics;

public class Program
{
    static void Main(string[] args)
    {
        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var testGrid = ReadData("testdata.txt");
        var grid = ReadData("data.txt");
        Debug.Assert(CalculateAmountOfPaper(testGrid) == 13);
        Console.WriteLine("Part 1: " + CalculateAmountOfPaper(grid));
        Debug.Assert(CalculateAmountOfPaperPart2(testGrid) == 43);
        Console.WriteLine("Part 2: " + CalculateAmountOfPaperPart2(grid));
        stopWatch.Stop();
        Console.WriteLine(stopWatch.ElapsedMilliseconds);

    }

    private static List<string> ReadData(string fileName)
    {
        return File.ReadAllLines(fileName).ToList();
    }
    
    private static int CalculateAmountOfPaper(List<string> data)
    {
        var forkLiftPaper = 0;
        for (var row = 0; row != data.Count; row++)
        {
            for (var col = 0; col != data[row].Length; col++)
            {
                var charAtPosition = data[row][col];
                if (charAtPosition != '@') continue;

                var neighbors = GetNeighbors(row, col, data);
                if (neighbors.Count(x => x == '@') < 4) forkLiftPaper++;
            }
        }
        
        return forkLiftPaper;
    }
    
    private static int CalculateAmountOfPaperPart2(List<string> data)
    {
        var forkLiftPaper = 0;
        List<(int row, int col)> removedPaperInIteration = new();
        var iteration = 1;

        while ((removedPaperInIteration.Count != 0 && iteration > 1) || iteration == 1)
        {
            removedPaperInIteration = new();
            for (var row = 0; row != data.Count; row++)
            {
                for (var col = 0; col != data[row].Length; col++)
                {
                    var charAtPosition = data[row][col];
                    if (charAtPosition != '@') continue;

                    var neighbors = GetNeighbors(row, col, data);
                    if (neighbors.Count(x => x == '@') < 4)
                    {
                        forkLiftPaper++;
                        removedPaperInIteration.Add((row, col));
                    }
                }
            }
            
            // replace all entries of removedPaperInIteration in grid by '.'
            foreach (var (row, col) in removedPaperInIteration)
            {
                var charArray = data[row].ToCharArray();
                charArray[col] = '.';
                data[row] = new string(charArray);
            }
            
            iteration++;
        }
        
        return forkLiftPaper;
    }
    
    private static List<char> GetNeighbors(int row, int col, List<string> data)
    {
        List<char> neighbors = new();
        if (row > 0)
        {
            neighbors.Add(data[row - 1][col]);
            if (col > 0)
                neighbors.Add(data[row - 1][col - 1]); // Top left
            if (col < data[row].Length - 1)
                neighbors.Add(data[row - 1][col + 1]); // Top right
        }

        if (row < data.Count - 1)
        {
            neighbors.Add(data[row + 1][col]);
            if (col > 0)
                neighbors.Add(data[row + 1][col - 1]); // Bottom left
            if (col < data[row].Length - 1)
                neighbors.Add(data[row + 1][col + 1]); // Bottom right
        }

        if (col > 0)
        {
            neighbors.Add(data[row][col - 1]);
        }
        if (col < data[row].Length - 1)
        {
            neighbors.Add(data[row][col + 1]);
        }
        return neighbors;
    }
}

