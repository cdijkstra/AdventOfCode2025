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
    }

    private static long CalcPart1()
    {
        long result = 0;
        for (var col = 0; col < _nums[0].Count; col++)
        {
            var isAdd = _operations[col] == '+';
            long columnResult = isAdd ? 0 : 1;
            for (var row = 0; row < _nums.Count; row++)
            {
                if (isAdd)
                {
                    columnResult += _nums[row][col];
                }
                else // Multiply
                {
                    columnResult *= _nums[row][col];
                }
            }
            result += columnResult;
        }

        return result;
    }
    
    private static long CalcPart2()
    {
        long result = 0;
        for (var col = 0; col < _nums[0].Count; col++)
        {
            var isAdd = _operations[col] == '+';
            long columnResult = isAdd ? 0 : 1;

            List<string> entries = new();
            for (var row = 0; row < _nums.Count; row++)
            {
                entries.Add(_nums[row][col].ToString());
            }
            var maxLength = entries.Max(x => x.Length);
            
            for (var length = maxLength; length > 0; length--)
            {
                var relevantEntries = entries.Where(x => x.Length >= length).ToList();
                if (relevantEntries.Count == 0) continue;
                
                var newNumber = string.Empty;
                foreach (var entry in relevantEntries)
                {
                    var idx = entry.Length - length;
                    newNumber += entry[idx];
                }

                if (isAdd)
                {
                    columnResult += int.Parse(newNumber);
                }
                else
                {
                    columnResult *= int.Parse(newNumber);
                }
            }
            Console.WriteLine($"Obtained {columnResult}");
            result += columnResult;
        }

        Console.WriteLine(result);
        return result;
    }

    private static void ReadData(string filename)
    {
        _nums = new();
        _numStrings = new();
        _operations = new();
        foreach (var line in File.ReadAllLines($"../../../{filename}"))
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
        
        // For each column, determine the longest length of a number
        var lines = File.ReadAllLines($"../../../{filename}");
        // var offset = 0;
        
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
                Console.WriteLine($"Found {found}");
                newEntry.Add(found);
            }
            offset += maxLengths[col] + 1;
            _numStrings.Add(newEntry);
        }
    }
}