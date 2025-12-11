using System.Collections;
using System.Diagnostics;

namespace _09;

class Grid
{
    public Grid(List<Coordinates> data)
    {
        Data = data;
    }
    
    public List<Coordinates> Data = new();
    public List<BitArray> _bitArray;
    private List<(Coordinates from, Coordinates to)> _connections = [];
    
    public int MinX() => Data.Min(c => c.X);
    public int MaxX() => Data.Max(c => c.X);
    public int MinY() => Data.Min(c => c.Y);
    public int MaxY() => Data.Max(c => c.Y);

    public void Print()
    {
        for (int y = 0; y < _bitArray[0].Count; y++)
        {
            for (int x = 0; x < _bitArray.Count; x++)
            {
                Console.Write(_bitArray[x][y] ? "#" : ".");
            }
            Console.WriteLine();
        }
    }

    public void CreateConnectedGrid()
    {
        _connections = [];
        _bitArray = new List<BitArray>();
        var width = MaxX()+ 1;
        var height = MaxY() + 1;
        
        Console.WriteLine("Creating the grid part 1");
        _bitArray = Enumerable.Range(0, width)
            .Select(_ => new BitArray(height, false))
            .ToList();

        foreach (var coors in Data)
        {
            _bitArray[coors.X][coors.Y] = true;
        }
        Console.WriteLine("Creating the grid part 2");
        for (var dataIdx = 0; dataIdx < Data.Count; dataIdx++)
        {
            var entry = Data[dataIdx];
            var next = Data[(dataIdx + 1) % Data.Count];
            if (next.X != entry.X)
            {
                var step = Math.Sign(next.X - entry.X);
                for (var x = entry.X + step; x != next.X; x += step)
                {
                    _bitArray[x][entry.Y] = true;
                }
            }
            else
            {
                var step = Math.Sign(next.Y - entry.Y);
                for (var y = entry.Y + step; y != next.Y; y += step)
                {
                    _bitArray[entry.X][y] = true;
                }
            }
        }
        
        Console.WriteLine("Flooding the grid");
        FillInterior();
        Console.WriteLine("Finished flooding the grid");
    }

    private (int x, int y) FindInterior()
    {
        int width = _bitArray.Count;
        int height = _bitArray[0].Count;

        for (int y = 0; y < height; y++)
        {
            var row = _bitArray.Select(col => col[y]).ToArray();
            bool hasConsecutiveTrue = row.Zip(row.Skip(1), (a, b) => a && b).Any(x => x);

            if (hasConsecutiveTrue)
                continue;

            for (int x = 0; x < width - 2; x++)
            {
                if (!row[x] && row[x + 1] && !row[x + 2])
                    return (x + 2, y);
            }
        }

        throw new Exception("No interior found");
    }

    private void FillInterior()
    {
        int width = _bitArray.Count;
        int height = _bitArray[0].Count;
        var visited = Enumerable.Range(0, width)
            .Select(_ => new BitArray(height, false))
            .ToList();

        var queue = new Queue<(int x, int y)>();

        // Enqueue a single interior point
        var interior = FindInterior();
        queue.Enqueue((interior.x, interior.y));

        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { 1, 0, -1, 0 };

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();
            if (x < 0 || y < 0 || x >= width || y >= height) continue;
            if (visited[x][y] || _bitArray[x][y]) continue; // Don't cross walls
            visited[x][y] = true;
            for (int dir = 0; dir < 4; dir++)
            {
                queue.Enqueue((x + dx[dir], y + dy[dir]));
            }
        }

        // Fill all visited cells (interior)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (visited[x][y])
                {
                    _bitArray[x][y] = true;
                }
            }
        }
    }
    
    public bool[] GetRowSubset(long minX, long maxX, long y)
    {
        var length = maxX - minX + 1;
        bool[] subset = new bool[length];
        for (long x = minX, i = 0; x <= maxX; x++, i++)
        {
            subset[i] = _bitArray[(int)x][(int)y];
        }
        return subset;
    }

    public bool[] GetColumnSubset(long minY, long maxY, long x)
    {
        var length = maxY - minY + 1;
        bool[] subset = new bool[length];
        for (long y = minY, i = 0; y <= maxY; y++, i++)
        {
            subset[i] = _bitArray[(int)x][(int)y];
        }
        return subset;
    }
}