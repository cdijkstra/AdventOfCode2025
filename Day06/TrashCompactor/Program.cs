using System.Diagnostics;
using System.Text.RegularExpressions;

public class Program
{
    private static List<List<long>> _nums = new();
    private static List<List<string>> _numStrings = new();
    private static List<char> _operations = new();

    static void Main(string[] args)
    {
        ReadData("testdata.txt");
        Debug.Assert(CalcPart1() == 4277556);
        Debug.Assert(CalcPart2() == 3263827);
        ReadData("data.txt");
        Console.WriteLine(CalcPart1());
        Console.WriteLine(CalcPart2());
    }

    private static long CalcPart1()
    {
        long result = 0;
        for (var col = 0; col < _nums[0].Count; col++)
        {
            var isAdd = _operations[col] == '+';
            long columnResult = isAdd ? 0 : 1; // If add, start with 0, multiplication start with 1
            for (var row = 0; row < _nums.Count; row++)
            {
                columnResult = isAdd
                    ? columnResult + _nums[row][col]
                    : columnResult * _nums[row][col];
            }
            result += columnResult;
        }

        return result;
    }
    
    private static long CalcPart2()
    {
        long result = 0;
        var maxLengths = _numStrings.Select(row => row.Max(x => x.Length)).ToList();
        
        for (var col = 0; col < _numStrings.Count; col++)
        {
            var isAdd = _operations[col] == '+';
            long columnResult = isAdd ? 0 : 1;

            for (var length = maxLengths[col]; length > 0; length--)
            {
                var newEntry = string.Empty;
                for (var row = 0; row < _numStrings[0].Count; row++)
                {
                    var entry = _numStrings[col][row];
                    var newNum = _numStrings[col][row][entry.Length - length];
                    if (newNum == ' ') continue;
                    
                    newEntry += newNum;
                }
                
                columnResult = isAdd
                    ? columnResult + long.Parse(newEntry)
                    : columnResult * long.Parse(newEntry);
            }

            result += columnResult;
        }
        
        return result;
    }

    private static void ReadData(string filename)
    {
        _nums = new();
        _numStrings = new();
        _operations = new();
        
        foreach (var line in File.ReadAllLines(filename))
        {
            var entries = Regex.Split(line.Trim(), @"\s+");
            if (long.TryParse(entries[0], out long _))
            {
                _nums.Add(entries.Select(long.Parse).ToList());
            }
            else
            {
                _operations.AddRange(entries.Select(char.Parse).ToList());
            }
        }
        
        // For part 2; Obtain a List of string preserving whitespaces. Strings can be left or right aligned
        var lines = File.ReadAllLines(filename);
        
        var colCount = _nums[0].Count;
        var maxLengths = new int[colCount];
        for (int col = 0; col < colCount; col++)
            maxLengths[col] = _nums.Select(row => row[col].ToString().Length).Max();

        var offset = 0;
        for (int col = 0; col < colCount; col++)
        {
            List<string> newEntry = new();
            for (int lineIdx = 0; lineIdx < lines.Length - 1; lineIdx++)
            {
                var line = lines[lineIdx];
                var found = line.Substring(offset, maxLengths[col]);
                newEntry.Add(found);
            }
            offset += maxLengths[col] + 1; // Keep track of where we should start scanning a new entry
            _numStrings.Add(newEntry);
        }
        // The columns have become rows in the List<List<string>>
    }
}