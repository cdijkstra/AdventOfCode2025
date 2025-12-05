using System.Diagnostics;

public class Program
{
    static void Main(string[] args)
    {
        List<(int low, int high)> testRanges, ranges;
        List<int> testIngredients, ingredients;
        
        ReadData("testdata.txt", out testRanges, out testIngredients);
        ReadData("data.txt", out ranges, out ingredients);
        Debug.Assert(CalculateFreshness(testRanges, testIngredients) == 3);
        Console.WriteLine(CalculateFreshness(ranges, ingredients));
    }
    
    private static int CalculateFreshness(List<(int low, int high)> ranges, List<int> ingredients)
    {
        int freshIngredients = ingredients
            .Count(ingredient => 
                ranges.Any(range => ingredient >= range.low && ingredient <= range.high));
        return freshIngredients;
    }

    private static void ReadData(string fileName, out List<(int low, int high)> ranges, out List<int> ingredients)
    {
        ranges = new();
        ingredients = new();
        foreach (var line in File.ReadAllLines($"../../../{fileName}").ToList())
        {
            if (line.Contains("-"))
            {
                var rangeParts = line.Split('-');
                ranges.Add((int.Parse(rangeParts[0]), int.Parse(rangeParts[1])));
            }
            else if (line.Length > 0)
            {
                ingredients.Add(int.Parse(line));
            }
        }
    }
}