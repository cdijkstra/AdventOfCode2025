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
        var width = MaxX() - MinX() + 1;
        var height = MaxY() - MinY() + 1;
        
        Console.WriteLine("Creating the grid part 1");
        _bitArray = Enumerable.Range(0, width)
            .Select(_ => new BitArray(height, false))
            .ToList();

        foreach (var coors in Data)
        {
            _bitArray[coors.X - MinX()][coors.Y - MinY()] = true;
        }
        Console.WriteLine("Creating the grid part 2");
        
        // Print();
        Console.WriteLine("Connecting the grid");
        var firstEntry = (from x in Enumerable.Range(0, _bitArray.Count)
            from y in Enumerable.Range(0, _bitArray[x].Count)
            where _bitArray[x][y]
            select new Coordinates(x, y)).FirstOrDefault();
        
        var queue = new Queue<Coordinates>();
        queue.Enqueue(firstEntry);
        HashSet<(Coordinates, Coordinates)> connectionSet = new();

        while (queue.Count > 0)
        {
            var entry = queue.Dequeue();
            var validNeighbors = GetValidNeighbors(entry);
            if (validNeighbors.Count == 0) continue;
            foreach (var neighbor in validNeighbors)
            {
                var conn = (entry, neighbor);
                // Canonical order: always (min, max)
                if (neighbor.CompareTo(entry) < 0)
                    conn = (neighbor, entry);

                if (connectionSet.Contains(conn))
                    continue; // Already processed

                connectionSet.Add(conn);
                _connections.Add(conn);
                
                if (neighbor.X != entry.X)
                {
                    int start = Math.Min(entry.X, neighbor.X) + 1;
                    int end = Math.Max(entry.X, neighbor.X) - 1;
                    for (int dx = start; dx <= end; dx++)
                    {
                        _bitArray[dx][entry.Y] = true;
                    }
                }

                if (neighbor.Y != entry.Y)
                {
                    int start = Math.Min(entry.Y, neighbor.Y) + 1;
                    int end = Math.Max(entry.Y, neighbor.Y) - 1;
                    for (int dy = start; dy <= end; dy++)
                    {
                        _bitArray[entry.X][dy] = true;
                    }
                }

                _connections.Add((entry, neighbor));
                queue.Enqueue(neighbor);
            }
        }

        // Print();
        Console.WriteLine("Flooding the grid");
        FillInterior();
        
        Console.WriteLine("Finished flooding the grid");
        Print();
    }
    
    private void FillInterior()
    {
        foreach (var row in _bitArray)
        {
            int first = -1, last = -1;
            for (var y = 0; y < row.Count; y++)
            {
                if (!row[y]) continue;
                
                if (first == -1) first = y;
                last = y;
            }
            
            if (first != -1 && last != -1 && last > first)
            {
                for (var y = first + 1; y < last; y++)
                {
                    row[y] = true;
                }
            }
        }
    }
    
    private List<Coordinates> GetValidNeighbors(Coordinates entry)
    {
        var neighbors = new List<Coordinates>();
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { 1, 0, -1, 0 };

        for (int dir = 0; dir < 4; dir++)
        {
            int nx = entry.X;
            int ny = entry.Y;
            while (true)
            {
                nx += dx[dir];
                ny += dy[dir];
                if (nx < 0 || ny < 0 || nx >= _bitArray.Count || ny >= _bitArray[0].Count)
                    break;
                
                var neighbor = new Coordinates(nx + MinX(), ny + MinY());
                if (Data.Contains(neighbor))
                {
                    var gridNeighbor = new Coordinates(nx, ny);
                    if (!_connections.Contains((entry, gridNeighbor)))
                        neighbors.Add(gridNeighbor);
                    break; // Stop at the first neighbor found in this direction
                }
            }
        }
        return neighbors;
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