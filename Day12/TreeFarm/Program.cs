using System.Diagnostics;
using System.Numerics;
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
    private static readonly int PresentWidth = 3;
    private static readonly int PresentHeight = 3;
    
    static void Main()
    {
        var sw = new Stopwatch();
        sw.Start();
        ReadFile("testdata.txt");
        Debug.Assert(SolvePart1() == 2);
        ReadFile("data.txt");
        Console.WriteLine(SolvePart1());
        sw.Stop();
        Console.WriteLine($"Test passed in {sw.ElapsedMilliseconds} ms");
    }

    private static int SolvePart1_Alternative()
    {
        // Why does this work???
        var total = 0;
        for (var gridIdx = 0; gridIdx < _grids.Count; gridIdx++)
        {
            var numSpaces = _presentNums[gridIdx].Sum();
            var grid = _grids[gridIdx];
            var size = (grid.Height / 3) * (grid.Width / 3);
            if (size >= numSpaces)
            {
                total++;
            }
        }

        return total;
    }

    private static int SolvePart1()
    {
        var validGrids = 0;

        for (var gridIdx = 0; gridIdx < _grids.Count; gridIdx++)
        {
            var visited = new HashSet<string>();
            
            Console.WriteLine($"Considering idx {gridIdx}");
            var grid = _grids[gridIdx];
            var packageNums = Enumerable.Repeat(0, _presentNums[gridIdx].Count).ToList();
            var considerIndices = _presentNums[gridIdx]
                .Select((value, index) => (value, index))
                .Where(pair => pair.value > 0)
                .Select(pair => pair.index)
                .ToList();
            
            var pq = new PriorityQueue<(BitGrid grid, BitGrid package, int packageNum, int x, int y, List<int> packageNums), int>();
            
            foreach (var packageNum in considerIndices)
            {
                _permutations[_presents[packageNum]]
                    .ForEach(package => pq.Enqueue((grid, package, packageNum, 0, 0, packageNums), 0));
            }
            
            while (pq.Count > 0)
            {
                var (activeGrid, activePackage, packageNum, activeX, activeY, activePackageNums) = pq.Dequeue();
                var newGrid = PlacePackage(activeGrid, activePackage, activeX, activeY);
                var newPackageNums = new List<int>(activePackageNums);
                newPackageNums[packageNum]++;
                
                var stateKey = string.Join(",", newGrid.Rows) + "|" + string.Join(",", newPackageNums);
                if (!visited.Add(stateKey))
                {
                    continue;
                }

                if (newPackageNums.SequenceEqual(_presentNums[gridIdx]))
                {
                    Console.WriteLine($"Found a valid grid for {gridIdx}");
                    validGrids++;
                    break;
                }
                
                var placePackages = activePackageNums
                    .Zip(_presentNums[gridIdx], (a, t) => t - a)
                    .ToList();
                
                // Place new package in queue
                var validPackageIndices = activePackageNums
                    .Zip(_presentNums[gridIdx], (a, t) => t - a)
                    .Select((diff, i) => i)
                    .Where(i => _presentNums[gridIdx][i] - activePackageNums[i] > 0)
                    .Select(i => (Index: i, Package: _presents[i]))
                    .ToList();

                var packagesNeeded = activePackageNums
                    .Zip(_presentNums[gridIdx], (a, t) => t - a)
                    .Sum();
                
                var validPositions = CountValid3X3Areas(newGrid); // max amount of packages that can be placed in grid
                if (validPositions < packagesNeeded) continue;
                
                foreach (var validPackage in validPackageIndices)
                {
                    (int maxX, int maxY) = GetMaxSetBitPosition(newGrid);
                    for (var x = 0; x <= Math.Min(maxX, newGrid.Width - PresentWidth); x++)
                    {
                        for (var y = 0; y <= Math.Min(maxY, newGrid.Height - PresentHeight); y++)
                        {
                            if (BitSet(newGrid, x, y)) continue;
                            
                            var (canPlace, packages) = CanPlacePackage(newGrid, validPackage.Index, x, y);
                            
                            if (!canPlace) continue;
                            // Package can be placed
                            foreach (var package in packages)
                            {
                                // Only add if adjacent to set bits, or if this is the first present
                                if (!IsAdjacentToSetBits(newGrid, package, x, y))
                                    continue;

                                var score = packagesNeeded * 1000 - validPositions;                                pq.Enqueue((newGrid, package, validPackage.Index, x, y, newPackageNums), score);
                            }
                        }
                    }
                }
            }
        }
        
        return validGrids;
    }
    
    private static int CountValid3X3Areas(BitGrid grid)
    {
        int count = 0;
        for (int y = 0; y <= grid.Height - 3; y++)
        {
            for (int x = 0; x <= grid.Width - 3; x++)
            {
                bool valid = true;
                // Check each row in the 3x3 window
                for (int dy = 0; dy < 3 && valid; dy++)
                {
                    ulong row = (grid.Rows[y + dy] >> x) & 0b111UL;
                    if (row == 0b111UL) // All bits set in this row
                        valid = false;
                }
                // Check each column in the 3x3 window
                for (int dx = 0; dx < 3 && valid; dx++)
                {
                    bool allSet = true;
                    for (int dy = 0; dy < 3; dy++)
                    {
                        if (((grid.Rows[y + dy] >> (x + dx)) & 1UL) == 0)
                        {
                            allSet = false;
                            break;
                        }
                    }
                    if (allSet)
                        valid = false;
                }
                if (valid)
                    count++;
            }
        }
        return count;
    }

    private static bool BitSet(BitGrid grid, int x, int y) => ((grid.Rows[y] >> x) & 1UL) != 0;
    
    // Returns (maxX, maxY) for set bits in the grid
    private static (int maxX, int maxY) GetMaxSetBitPosition(BitGrid grid)
    {
        int maxX = -1;
        int maxY = -1;
        for (int y = 0; y < grid.Rows.Length; y++)
        {
            ulong row = grid.Rows[y];
            if (row != 0)
            {
                int rowMaxX = BitOperations.Log2(row);
                if (rowMaxX > maxX) maxX = rowMaxX;
                maxY = y;
            }
        }
        return (maxX, maxY);
    }
    
    
    // Returns true if any set bit in 'package' at (x, y) touches a set bit in 'grid'
    static bool IsAdjacentToSetBits(BitGrid grid, BitGrid package, int x, int y)
    {
        for (int i = 0; i < package.Height; i++)
        {
            ulong row = package.Rows[i] << x;
            int gridY = y + i;
            if (gridY < 0 || gridY >= grid.Height) continue;

            // Check above
            if (gridY > 0 && ((row & grid.Rows[gridY - 1]) != 0)) return true;
            // Check below
            if (gridY < grid.Height - 1 && ((row & grid.Rows[gridY + 1]) != 0)) return true;
            // Check left/right
            ulong left = (row >> 1) & ~0UL;
            ulong right = (row << 1) & ~0UL;
            if (((left | right) & grid.Rows[gridY]) != 0) return true;
        }
        return false;
    }

    private static int CountSetBits(ulong[] rows)
    {
        return rows.Sum(BitOperations.PopCount);
    }
    
    private static void FillPermutations()
    {
        _permutations.Clear();
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
        _presentNums = new();
        _grids = new();
        _permutations = new();
        
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
            "[0-9]+x[0-9]+"
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
        
        FillPermutations();
    }
    
    static int TouchScore(BitGrid grid, BitGrid package, int x, int y)
    {
        int touches = 0;

        for (int i = 0; i < package.Height; i++)
        {
            ulong row = package.Rows[i] << x;
            if (y + i == 0 || (grid.Rows[y + i - 1] & row) != 0)
                touches++;
        }

        return -touches; // more touches = better
    }

    static (bool, List<BitGrid>) CanPlacePackage(BitGrid grid, int package, int x, int y)
    {
        var packageBits = _presents[package];
        var permuts = _permutations[packageBits];
        var validPerms = new List<BitGrid>();

        foreach (var perm in permuts)
        {
            bool canPlace = true;
            for (int i = 0; i < perm.Height; i++)
            {
                ulong shiftedPackageRow = perm.Rows[i] << x;
                if ((grid.Rows[y + i] & shiftedPackageRow) != 0)
                {
                    canPlace = false;
                    break;
                }
            }
            if (canPlace)
                validPerms.Add(perm);
        }

        return (validPerms.Count > 0, validPerms);
    }
    
    static BitGrid PlacePackage(BitGrid grid, BitGrid package, int x, int y)
    {
        var newGrid = new BitGrid
        {
            Width  = grid.Width,
            Height = grid.Height,
            Rows   = (ulong[])grid.Rows.Clone()
        };
        
        for (int i = 0; i < package.Height; i++)
        {
            newGrid.Rows[y + i] |= (package.Rows[i] << x);
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