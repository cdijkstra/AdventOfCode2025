using System.Diagnostics;

public class Program
{
    static void Main(string[] args)
    {
        var sw = new Stopwatch();
        sw.Start();
        List<(long low, long high)> testRanges, testRanges2, ranges;
        List<long> testIngredients, testIngredients2, ingredients;
        
        ReadData("testdata.txt", out testRanges, out testIngredients);
        ReadData("testdata2.txt", out testRanges2, out testIngredients2);
        ReadData("data.txt", out ranges, out ingredients);
        Debug.Assert(CalculateFreshness(testRanges, testIngredients) == 3);
        Console.WriteLine(CalculateFreshness(ranges, ingredients));
        Debug.Assert(CalculateFreshnessPart2(testRanges, testIngredients) == 14);
        Debug.Assert(CalculateFreshnessPart2(testRanges2, testIngredients2) == 20);
        Console.WriteLine(CalculateFreshnessPart2(ranges, ingredients));
        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
    }
    
    private static long CalculateFreshness(List<(long low, long high)> ranges, List<long> ingredients)
    {
        long freshIngredients = ingredients
            .Count(ingredient => 
                ranges.Any(range => ingredient >= range.low && ingredient <= range.high));
        return freshIngredients;
    }
    
    private static long CalculateFreshnessPart2(List<(long low, long high)> ranges, List<long> ingredients)
    {
        var sortedList = ranges.OrderBy(r => r.low).ToList();
        var merged = new List<(long low, long high)>();

        foreach (var range in sortedList)
        {
            if (merged.Count == 0)
            {
                merged.Add(range);
            }

            var last = merged[^1];
            if (merged[^1].high < range.low - 1) // First is if list is empty
            {
                merged.Add(range);
            }
            else
            {
                // At least there's one entry in merged
                merged[^1] = (last.low, Math.Max(last.high, range.high)); // Update the entry in merged
            }
        }

        return merged.Sum(r => r.high - r.low + 1);
    }

    private static void ReadData(string fileName, out List<(long low, long high)> ranges, out List<long> ingredients)
    {
        ranges = new();
        ingredients = new();
        foreach (var line in File.ReadAllLines(fileName).ToList())
        {
            if (line.Contains('-'))
            {
                var rangeParts = line.Split('-');
                ranges.Add((long.Parse(rangeParts[0]), long.Parse(rangeParts[1])));
            }
            else if (line.Length > 0)
            {
                ingredients.Add(long.Parse(line));
            }
        }
    }
}