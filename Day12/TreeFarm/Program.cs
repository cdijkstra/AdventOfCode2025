using System.Text.RegularExpressions;

public class BitGrid
{
    public int Width;
    public int Height;
    public ulong[] Rows; // bitmask rows
}

public class Program
{
    private static List<BitGrid> _presents = new();
    private static List<List<int>> _presentNums = new();
    private static List<BitGrid> _grids = new();
    static void Main()
    {
        ReadFile("testdata.txt");
        var grid = _grids[0];
        var package = _presents[0];
        PlacePackage(grid, package, 0, 0);
    }
    
    private static void ReadFile(string fileName)
    {
        _presents = new();
        var text = File.ReadAllText(fileName);
        var presents = Regex.Matches(
            text, 
            "([#.]{3}\\n){2}[#.]{3}");
        foreach (Match match in presents)
        {
            _presents.Add(ParseShape(match.Value.Split("\n")));
        }

        var grids = Regex.Matches(
            text,
            "[0-9]+x[0-9]"
        );
        foreach (Match grid in grids)
        {
            var parts = grid.Value.Split('x');
            var width = int.Parse(parts[0]);   // 4
            var height = int.Parse(parts[1]);  // 4
            _grids.Add(new BitGrid
            {
                Width = width,
                Height = height,
                Rows = new ulong[height]
            });
        }
        
        var presentNums = Regex.Matches(
            text,
            "([0-9]+ ){5}[0-9]+"
        );
        foreach (Match presentNum in presentNums)
        {
            List<int> presentNumList = new();
            presentNumList.AddRange(presentNum.Value.Split().Select(int.Parse));
            _presentNums.Add(presentNumList);
        }
        Console.WriteLine(_presents.Count);
    }
    
    static void PlacePackage(BitGrid grid, BitGrid package, int x, int y)
    {
        for (int i = 0; i <= package.Width; i++)
        {
            grid.Rows[y + i] |= (package.Rows[i] << x);
        }
    }
    
    static BitGrid ParseShape(string[] lines)
    {
        int height = lines.Length;
        int width = lines[0].Length;
        ulong[] mask = new ulong[height];

        var bitgrid = new BitGrid
        {
            Width = width,
            Height = height,
            Rows = mask
        };

        for (var y = 0; y < height; y++)
        {
            ulong rowMask = 0;

            for (var x = 0; x < width; x++)
            {
                if (lines[y][x] == '#')
                {
                    rowMask |= 1UL << x; // Set the bit at position x
                }
            }

            bitgrid.Rows[y] = rowMask;
        }

        return bitgrid;
    }
}