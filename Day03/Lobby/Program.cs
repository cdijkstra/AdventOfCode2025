using System.Diagnostics;
namespace Lobby;

internal abstract class Program
{
    static void Main(string[] args)
    {
        var testBanks = ReadData("testdata.txt");
        var banks = ReadData("data.txt");
        Debug.Assert(CalculateDoubleJolt(testBanks) == 357);
        Console.WriteLine(CalculateDoubleJolt(banks));
        Debug.Assert(CalculateMultipleJolt(testBanks) == 3121910778619);
        Console.WriteLine(CalculateMultipleJolt(banks));
    }

    private static string[] ReadData(string fileName)
    {
        return File.ReadAllLines(fileName);
    }

    private static int CalculateDoubleJolt(string[] banks)
    {
        var joltage = 0;
        foreach (var bank in banks)
        {
            var firstJolt = bank.Substring(0, bank.Length - 1)
                .Select((c, i) => new { Digit = int.Parse(c.ToString()), Index = i })
                .OrderByDescending(x => x.Digit)
                .ThenBy(x => x.Index)
                .First(); // Choose first match
            
            Console.WriteLine("First : " + firstJolt.Digit + " at " + firstJolt.Index);
            
            // Loop from pos until bank.length
            var secondJolt = bank.Substring(firstJolt.Index + 1)
                .Select((c, i) => new { Digit = int.Parse(c.ToString()), Index = i })
                .OrderByDescending(x => x.Digit)
                .First();

            var jolt = int.Parse(firstJolt.Digit.ToString() + secondJolt.Digit);
            joltage += jolt;
        }
        return joltage;
    }
    
    private static long CalculateMultipleJolt(string[] banks, int elements = 12)
    {
        long joltage = 0;
        foreach (var bank in banks)
        {
            var jolt = string.Empty;
            var index = -1;
            var elementsLeft = elements;

            foreach (var _ in Enumerable.Range(0, elements))
            {
                var newJolt = bank[(index + 1)..^(elementsLeft - 1)]
                    .Select((c, i) => new { Digit = int.Parse(c.ToString()), Index = i })
                    .OrderByDescending(x => x.Digit)
                    .ThenBy(x => x.Index)
                    .First();
                
                elementsLeft--;
                index += newJolt.Index + 1;
                jolt += newJolt.Digit;
            }
            
            joltage += long.Parse(jolt);
        }
        return joltage;
    }
}