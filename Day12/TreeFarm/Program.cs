using System.Text.RegularExpressions;

public class BitGrid
{
    public int Width;
    public int Height;
    public ulong[] Rows; // bitmask rows
    
    public override bool Equals(object? obj)
    {
        if (obj is not BitGrid other) return false;
        if (Rows.Length != other.Rows.Length) return false;
        for (int i = 0; i < Rows.Length; i++)
            if (Rows[i] != other.Rows[i]) return false;
        return true;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            foreach (var row in Rows)
                hash = hash * 31 + row.GetHashCode();
            return hash;
        }
    }
}

public class Program
{
    private static Dictionary<BitGrid, List<BitGrid>> _permutations = new();
    private static List<BitGrid> _presents = new();
    private static List<List<int>> _presentNums = new();
    private static List<BitGrid> _grids = new();
    static void Main()
    {
        ReadFile("testdata.txt");
        FillPermutations();
        var validGrids = 0;
        
        var grid = _grids[0];
        var package = _presentNums[0].FindIndex(x => x > 0);

        var packageNum = _presentNums[0].Select(_ => 0).ToList();
        
        var pq = new PriorityQueue<(BitGrid grid, int package, int x, int y, List<int> packageNums), int>();
        pq.Enqueue((grid, package, 0, 0, packageNum), 0);
        
        while (pq.Count > 0)
        {
            var (activeGrid, activePackage, activeX, activeY, activePackageNums) = pq.Dequeue();
            var newGrid = PlacePackage(activeGrid, activePackage, activeX, activeY);
            
            activePackageNums[activePackage]++;
            if (activePackageNums.SequenceEqual(_presentNums[0]))
            {
                validGrids++;
                break;
            }
            
            // Place new package in queue
            var validPackageIndices = activePackageNums
                .Zip(_presentNums[0], (a, t) => t - a)
                .Select((diff, i) => i)
                .Where(i => _presentNums[0][i] - activePackageNums[i] > 0)
                .ToList();

            foreach (var validPackage in validPackageIndices)
            {
                for (var x = 0; x < newGrid.Width - 2; x++)
                {
                    for (var y = 0; y < newGrid.Height - 2; y++)
                    {
                        if (!CanPlacePackage(newGrid, validPackage, x, y)) continue;
                        // Package can be placed
                        var score = TouchScore(newGrid, validPackage, x, y); // Lower score = better
                        pq.Enqueue((newGrid, validPackage, x, y, activePackageNums), score);
                    }
                }
            }
        }
        
        Console.WriteLine(validGrids);
    }

    private static void FillPermutations()
    {
        foreach (var present in _presents)
        {
            var permutations = CreatePermutations(present);
            _permutations[present] = permutations;
        }
    }

    private static List<BitGrid> CreatePermutations(BitGrid bitMap)
    {
        var permutations = new List<BitGrid>();
        permutations.AddRange(Mirror(bitMap));
        var current = bitMap;
        for (int i = 0; i < 4; i++)
        {
            permutations.Add(current);
            permutations.AddRange(Mirror(current));

            current = Rotate90(current);
        }
        
        return permutations.Distinct().ToList();
    }
    
    private static BitGrid Rotate90(BitGrid bitMap)
    {
        var rotated = new BitGrid
        {
            Width = bitMap.Height,
            Height = bitMap.Width,
            Rows = new ulong[bitMap.Width]
        };
    
        for (int y = 0; y < bitMap.Height; y++)
        {
            for (int x = 0; x < bitMap.Width; x++)
            {
                if ((bitMap.Rows[y] & (1UL << x)) != 0)
                {
                    rotated.Rows[x] |= 1UL << (bitMap.Height - 1 - y);
                }
            }
        }
        return rotated;
    }
    
    private static List<BitGrid> Mirror(BitGrid bitMap)
    {
        // Mirror along X axis (vertical flip)
        List<BitGrid> permutations = new();
        var mirroredX = new BitGrid
        {
            Width = bitMap.Width,
            Height = bitMap.Height,
            Rows = new ulong[bitMap.Width]
        };
    
        for (int y = 0; y < bitMap.Height; y++)
        {
            mirroredX.Rows[y] = bitMap.Rows[bitMap.Height - 1 - y];
        }
        permutations.Add(mirroredX);
        
        // Mirror along Y axis (horizontal flip)
        var mirroredY = new BitGrid
        {
            Width = bitMap.Width,
            Height = bitMap.Height,
            Rows = new ulong[bitMap.Width]
        };
        for (int y = 0; y < bitMap.Height; y++)
        {
            ulong originalRow = bitMap.Rows[y];
            ulong mirroredRow = 0;
            for (int x = 0; x < bitMap.Width; x++)
            {
                if ((originalRow & (1UL << x)) != 0)
                {
                    mirroredRow |= 1UL << (bitMap.Width - 1 - x);
                }
            }
            mirroredY.Rows[y] = mirroredRow;
        }
        permutations.Add(mirroredY);
        
        return permutations;
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
    
    static int TouchScore(BitGrid grid, int package, int x, int y)
    {
        var packageBits = _presents[package];

        int touches = 0;

        for (int i = 0; i < packageBits.Height; i++)
        {
            ulong row = packageBits.Rows[i] << x;
            if (y + i == 0 || (grid.Rows[y + i - 1] & row) != 0)
                touches++;
        }

        return -touches; // more touches = better
    }

    static bool CanPlacePackage(BitGrid grid, int package, int x, int y)
    {
        var packageBits = _presents[package];

        for (int i = 0; i < packageBits.Height; i++)
        {
            ulong shiftedPackageRow = packageBits.Rows[i] << x;

            if ((grid.Rows[y + i] & shiftedPackageRow) != 0)
                return false; // collision found
        }

        return true; // space is free
    }
    
    static BitGrid PlacePackage(BitGrid grid, int package, int x, int y)
    {
        var packageBits = _presents[package];
        // Deep copy of the grid
        var newGrid = new BitGrid
        {
            Width  = grid.Width,
            Height = grid.Height,
            Rows   = (ulong[])grid.Rows.Clone()
        };
        
        for (int i = 0; i < packageBits.Height; i++)
        {
            newGrid.Rows[y + i] |= (packageBits.Rows[i] << x);
        }
        return newGrid;
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