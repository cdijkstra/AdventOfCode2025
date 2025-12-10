using System.Collections;

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
        
        Console.WriteLine("Creating the grid");
        var coordSet = new HashSet<(int x, int y)>(Data.Select(c => (c.X, c.Y)));
        for (var x = 0; x < width; x++)
        {
            var array = new BitArray(height);
            for (var y = 0; y < height; y++)
            {
                if (coordSet.Contains((x + MinX(), y + MinY())))
                    array[y] = true;
            }
            _bitArray.Add(array);
        }
        
        // Print();
        Console.WriteLine("Filling the grid");
        var firstEntry = (from x in Enumerable.Range(0, _bitArray.Count)
            from y in Enumerable.Range(0, _bitArray[x].Count)
            where _bitArray[x][y]
            select new Coordinates(x, y)).FirstOrDefault();
        
        var queue = new Queue<Coordinates>();
        queue.Enqueue(firstEntry);
        
        while (queue.Count > 0)
        {
            var entry = queue.Dequeue();
            var validNeighbors = GetValidNeighbors(entry);
            if (validNeighbors.Count == 0) continue;
            foreach (var neighbor in validNeighbors)
            {
                if (neighbor.X != entry.X)
                {
                    var step = Math.Sign(neighbor.X - entry.X);
                    for (var dx = entry.X + step; dx != neighbor.X; dx += step)
                    {
                        _bitArray[dx][entry.Y] = true;
                    }
                }
                
                if (neighbor.Y != entry.Y)
                {
                    var step = Math.Sign(neighbor.Y - entry.Y);
                    for (var dy = entry.Y + step; dy != neighbor.Y; dy += step)
                    {
                        _bitArray[entry.X][dy] = true;
                    }
                }
                
                _connections.Add((entry, neighbor));
                queue.Enqueue(neighbor);
            }
            
            // Find neighbors
        }
        Print();
        
        Console.WriteLine("Finished creating a grid");
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
                var neighbor = new Coordinates(nx, ny);
                if (_bitArray[nx][ny])
                {
                    if (!_connections.Contains((entry, neighbor)))
                        neighbors.Add(neighbor);
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

    public bool[] GetColumnSubset(long x, long minY, long maxY)
    {
        var length = maxY - minY + 1;
        bool[] subset = new bool[length];
        for (long y = minY, i = 0; y <= maxY; y++, i++)
        {
            subset[i] = _bitArray[(int)x][(int)y];
        }
        return subset;
    }
    
    private void FloodFillInterior()
    {
        int height = _bitArray.Count;
        int width = _bitArray[0].Length;
        bool[,] visited = new bool[width, height];
    
        var queue = new Queue<(int x, int y)>();
        queue.Enqueue((0, 0));
        visited[0, 0] = true;
    
        int[] dx = { 0, 1, 0, -1 };
        int[] dy = { 1, 0, -1, 0 };
    
        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();
            for (int dir = 0; dir < 4; dir++)
            {
                int nx = x + dx[dir];
                int ny = y + dy[dir];
                if (nx >= 0 && ny >= 0 && nx < width && ny < height)
                {
                    if (!_bitArray[ny][nx] && !visited[nx, ny])
                    {
                        visited[nx, ny] = true;
                        queue.Enqueue((nx, ny));
                    }
                }
            }
        }
    
        // Set all non-visited cells to true (interior)
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (!visited[x, y])
                    _bitArray[y][x] = true;
            }
        }
    }
    

    // private List<Coordinates> FindNeighbors(Coordinates coors)
    // {
    //     var neighbors = new List<Coordinates>();
    //     var leftNeighbor = Data
    //         .Where(c => c.Y == coors.Y && c.X < coors.X && c.Type == TileType.Red)
    //         .OrderByDescending(c => c.X)
    //         .FirstOrDefault();
    //
    //     var rightNeighbor = Data
    //         .Where(c => c.Y == coors.Y && c.X > coors.X && c.Type == TileType.Red)
    //         .OrderBy(c => c.X)
    //         .FirstOrDefault();
    //     
    //     var upNeighbor = Data
    //         .Where(c => c.X == coors.X && c.Y > coors.Y && c.Type == TileType.Red)
    //         .OrderByDescending(c => c.X)
    //         .FirstOrDefault();
    //
    //     var downNeighbor = Data
    //         .Where(c => c.X == coors.X && c.Y < coors.Y && c.Type == TileType.Red)
    //         .OrderBy(c => c.X)
    //         .FirstOrDefault();
    //
    //     if (leftNeighbor != null) neighbors.Add(leftNeighbor);
    //     if (rightNeighbor != null) neighbors.Add(rightNeighbor);
    //     if (upNeighbor != null) neighbors.Add(upNeighbor);
    //     if (downNeighbor != null) neighbors.Add(downNeighbor);
    //     
    //     Console.WriteLine($"Found {neighbors.Count} neighbors for {coors.X}, {coors.Y}");
    //
    //     return neighbors;
    // }
    
}