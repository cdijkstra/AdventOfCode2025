using System.Diagnostics;

namespace GiftShop;

internal class Program
{
    private static void Main(string[] args)
    {
        var testdata = ReadData("testdata.txt");
        var data = ReadData("data.txt");
        Debug.Assert(CalcDoubleId(testdata) == 1227775554);
        Console.WriteLine($"Part 1: {CalcDoubleId(data)}");
        Debug.Assert(CalcRepeatedId(testdata) == 4174379265);
        Console.WriteLine($"Part 2: {CalcRepeatedId(data)}");
    }

    private static long CalcDoubleId(string[] data)
    {
        long totalValue = 0;
        foreach (var td in data)
        {
            var min = long.Parse(td.Split('-')[0]);
            var max = long.Parse(td.Split('-')[1]);
            for (var val = min; val <= max; val++)
            {
                var str = val.ToString();
                var length = str.Length;
                if (length % 2 != 0) continue;

                if (str.Substring(0, length / 2) == str.Substring(length / 2, length / 2))
                {
                    totalValue += val;
                }
            }
        }

        return totalValue;
    }
    
    private static long CalcRepeatedId(string[] data)
    {
        List<long> values = new();
        foreach (var td in data)
        {
            var min = long.Parse(td.Split('-')[0]);
            var max = long.Parse(td.Split('-')[1]);
            for (long val = min; val <= max; val++)
            {
                var str = val.ToString();
                var length = str.Length;

                for (var substringLength = 1; substringLength <= length / 2; substringLength++)
                {
                    if (length % substringLength != 0) continue;
                    var repeats = length / substringLength;
                    var pattern = str.Substring(0, substringLength);
                    
                    if (string.Concat(Enumerable.Repeat(pattern, repeats)) == str)
                    {
                        values.Add(val);
                    }
                }
            }
        }

        return values.Distinct().Sum();
    }

    private static string[] ReadData(string fileName)
    {
        return File.ReadAllText($"../../../{fileName}").Split(',');
    }
}