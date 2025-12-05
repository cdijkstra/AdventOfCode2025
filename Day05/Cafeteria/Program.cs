using System.Diagnostics;

public class Program
{
    static void Main(string[] args)
    {
        List<(long low, long high)> testRanges, testRanges2, ranges;
        List<long> testIngredients, testIngredients2, ingredients;
        
        ReadData("testdata.txt", out testRanges, out testIngredients);
        ReadData("testdata2.txt", out testRanges2, out testIngredients2);
        ReadData("data.txt", out ranges, out ingredients);
        Debug.Assert(CalculateFreshness(testRanges, testIngredients) == 3);
        Console.WriteLine(CalculateFreshness(ranges, ingredients));
        // Debug.Assert(CalculateFreshnessPart2(testRanges, testIngredients) == 14);
        Debug.Assert(CalculateFreshnessPart2(testRanges2, testIngredients2) == 23);
        Console.WriteLine(CalculateFreshnessPart2(ranges, ingredients));
        
        // 726964187350064 is too high
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
            Console.WriteLine($"Checking range: {range.low} - {range.high}");
            
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
        long thresHold = 0;
        long rhs = 0;
        long maxNumber = overlappingRanges.Select(x => x.high).Max();
        while (rhs != maxNumber)
        {
            var lhs = overlappingRanges.SelectMany(r => new[] { r.low, r.high + 1 })
                .Where(v => v > thresHold)
                .Min();

            rhs = overlappingRanges
                .SelectMany(r => new[] { r.low - 1, r.high })
                .Where(v => v > lhs)
                .Min();

            thresHold = rhs;
            Console.WriteLine($"LHS: {lhs}, RHS: {rhs}");
            nonOverlappingRanges.Add((lhs, rhs));
        }
        
        return nonOverlappingRanges.Select(r => r.high - r.low + 1).Sum();
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