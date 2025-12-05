using System.Diagnostics;

public class Program
{
    static void Main(string[] args)
    {
        List<(long low, long high)> testRanges, ranges;
        List<long> testIngredients, ingredients;
        
        ReadData("testdata.txt", out testRanges, out testIngredients);
        ReadData("data.txt", out ranges, out ingredients);
        Debug.Assert(CalculateFreshness(testRanges, testIngredients) == 3);
        Console.WriteLine(CalculateFreshness(ranges, ingredients));
        Debug.Assert(CalculateFreshnessPart2(testRanges, testIngredients) == 14);
        Console.WriteLine(CalculateFreshnessPart2(ranges, ingredients));
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
        var relevantRanges = ranges
            .Where(range => ingredients.Any(ingredient => ingredient >= range.low && ingredient <= range.high))
            .ToList();

        long freshIngredients = 0;
        
        // Find ranges that are not overlapping
        var nonOverlappingRanges = new List<(long low, long high)> ();
        var overlappingRanges = new List<(long low, long high)> ();
        
        foreach (var range in relevantRanges)
        {
            if (relevantRanges.Except(new[] {range})
                .All(otherRange => (otherRange.high < range.low && otherRange.low < range.low) || 
                                   (otherRange.low > range.high && otherRange.high > range.high)))
            {
                Console.WriteLine($"Found non-overlapping range: {range.low} - {range.high}");
                nonOverlappingRanges.Add(range);
            }
            else
            {
                Console.WriteLine($"Found overlapping range: {range.low} - {range.high}");
                overlappingRanges.Add(range);
            }
        }

        // Order sets
        var lowest = overlappingRanges.OrderBy(r => r.low).Select(r => r.low).First();
        var highest = overlappingRanges.OrderByDescending(r => r.low).Select(r => r.high).First();

        List<long> leftHandSides = overlappingRanges
            .SelectMany(r => new[] { r.low, r.high + 1 })
            .OrderBy(v => v)
            .Take(overlappingRanges.Count * 2 - 1)
            .ToList();
        
        List<long> rightHandSides = overlappingRanges
            .Select(r => r.high)
            .OrderBy(v => v)
            .ToList();

        // Print all combinations
        leftHandSides.ForEach(x => Console.WriteLine($"LHS {x}"));
        rightHandSides.ForEach(x => Console.WriteLine($"RHS {x}"));

        
        return freshIngredients;
    }

    private static void ReadData(string fileName, out List<(long low, long high)> ranges, out List<long> ingredients)
    {
        ranges = new();
        ingredients = new();
        foreach (var line in File.ReadAllLines($"../../../{fileName}").ToList())
        {
            if (line.Contains("-"))
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