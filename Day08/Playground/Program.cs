
using System.Diagnostics;

public class Junction
{
    public string Name { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
}

public class Program
{
    private static List<(string name, int x, int y, int z)> _junctions = new();
    static void Main(string[] args)
    {
        ReadFile("testData.txt");
        Debug.Assert(SolvePart1() == 21);
    }

    private static int SolvePart1()
    {
        return 1;
    }

    private static void ReadFile(string fileName)
    {
        File.ReadAllLines(fileName).ToList().ForEach(line =>
        {
            var parts = line.Split(',').Select(int.Parse).ToArray();
            _junctions.Add((string.Join(",", parts), parts[0], parts[1], parts[2]));
        });
    }
}