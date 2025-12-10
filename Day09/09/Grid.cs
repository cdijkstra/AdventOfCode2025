namespace _09;

class Grid
{
    public List<Coordinates> Data = new();
    public Grid(List<Coordinates> data)
    {
        Data = new();
        Data = data;
    }

    public bool[,] _grid;
    
    public long MinX() => Data.Where(c => c.Type == TileType.Red).Min(c => c.X);
    public long MaxX() => Data.Where(c => c.Type == TileType.Red).Max(c => c.X);
    public long MinY() => Data.Where(c => c.Type == TileType.Red).Min(c => c.Y);
    public long MaxY() => Data.Where(c => c.Type == TileType.Red).Max(c => c.Y);

    public void Print()
    {
        for (int y = 0; y < _grid.GetLength(1); y++)
        {
            for (int x = 0; x < _grid.GetLength(0); x++)
            {
                Console.Write(_grid[x, y] ? "#" : ".");
            }
            Console.WriteLine();
        }
    }

    public void CreateConnectedGrid()
    {
        var entry = Data.First();
        List<Coordinates> visited = [];

        bool finised = false;
        while (!finised)
        {
            visited.Add(entry);
            // Console.WriteLine($"Visiting {entry.X}, {entry.Y}");
            var neighbors = FindNeighbors(entry);
            switch (neighbors.Count)
            {
                case 0:
                    Console.WriteLine($"No neighbors found for: {entry.X}, {entry.Y}");
                    return;
                case 1:
                    Console.WriteLine($"Neighbor found: {neighbors[0].X}, {neighbors[0].Y}");
                    AddGreen(neighbors[0], entry);
                    
                    entry = neighbors[0];
                    break;
                case 2:
                {
                    var newNeighbors = neighbors
                        .Where(n => !visited.Any(v => v.X == n.X && v.Y == n.Y))
                        .ToList();
                
                    Console.Write($"2 neighbors found for {entry.X}, {entry.Y}; unique = " + newNeighbors.Count());
                    foreach (var neighbor in newNeighbors)
                    {
                        Console.WriteLine($"({neighbor.X}, {neighbor.Y})");
                    }
                
                    if (newNeighbors.Count == 0)
                    {
                        Console.WriteLine($"No new neighbors found, finishing at; {entry.X}, {entry.Y}");
                        // Find the neighbor that is not connected by a green line and add it
                        foreach (var neighbor in neighbors)
                        {
                            // Check if a green connection exists
                            bool hasGreen = Data.Any(c =>
                                ((c.X == neighbor.X && c.Y == neighbor.Y) || (c.X == entry.X && c.Y == entry.Y)) &&
                                c.Type == TileType.Green);

                            if (!hasGreen)
                            {
                                AddGreen(neighbor, entry);
                            }
                        }
                        finised = true;
                    }
                    else
                    {
                        AddGreen(newNeighbors[0], entry);
                        entry = newNeighbors[0];
                    }

                    break;
                }
                case 3:
                    var newwNeighbors = neighbors
                        .Where(n => !visited.Any(v => v.X == n.X && v.Y == n.Y))
                        .ToList();
                    Console.Write($"3 neighbors found for {entry.X}, {entry.Y}; unique = " + newwNeighbors.Count);
                    return;
            }
        }
        
        Console.WriteLine("Now creating a grid");
        Console.WriteLine($"MinX = {MinX()}, MaxX = {MaxX()}, MinY = {MinY()}, MaxY = {MaxY()}");
        
        // Create 2d bool grid
        int width = (int)(MaxX() - MinX() + 3);
        int height = (int)(MaxY() - MinY() + 3);
        
        _grid = new bool[width, height];
        foreach (var c in Data)
        {
            var x = (int)(c.X - MinX()) + 1;
            var y = (int)(c.Y - MinY()) + 1;
            _grid[x, y] = true;
        }
        // Print();
        Console.WriteLine("Finished creating a grid");
        
        FloodFillInterior();
        Print();
        Console.WriteLine("Finished flooding grid");
    }
    
    public bool[] GetRowSubset(long minX, long maxX, long y)
    {
        var length = maxX - minX + 1;
        bool[] subset = new bool[length];
        for (long x = minX, i = 0; x <= maxX; x++, i++)
        {
            subset[i] = _grid[x, y];
        }
        return subset;
    }
    
    public bool[] GetColumnSubset(long x, long minY, long maxY)
    {
        var length = maxY - minY + 1;
        bool[] subset = new bool[length];
        for (long y = minY, i = 0; y <= maxY; y++, i++)
        {
            subset[i] = _grid[x, y];
        }
        return subset;
    }
    
    private void FloodFillInterior()
    {
        int width = _grid.GetLength(0);
        int height = _grid.GetLength(1);
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
                    if (!_grid[nx, ny] && !visited[nx, ny])
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
                    _grid[x, y] = true;
            }
        }
    }
    
    
    private void AddGreen(Coordinates neighbor, Coordinates entry)
    {
        long dx = neighbor.X - entry.X;
        long dy = neighbor.Y - entry.Y;

        if (dx != 0 && dy == 0)
        {
            int step = Math.Sign(dx);
            for (var x = entry.X + step; x != neighbor.X; x += step)
            {
                Data.Add(new Coordinates(TileType.Green, x, entry.Y));
            }
        }
        else if (dy != 0 && dx == 0)
        {
            int step = Math.Sign(dy);
            for (var y = entry.Y + step; y != neighbor.Y; y += step)
            {
                Data.Add(new Coordinates(TileType.Green, entry.X, y));
            }
        }
    }

    private List<Coordinates> FindNeighbors(Coordinates coors)
    {
        var neighbors = new List<Coordinates>();
        var leftNeighbor = Data
            .Where(c => c.Y == coors.Y && c.X < coors.X && c.Type == TileType.Red)
            .OrderByDescending(c => c.X)
            .FirstOrDefault();

        var rightNeighbor = Data
            .Where(c => c.Y == coors.Y && c.X > coors.X && c.Type == TileType.Red)
            .OrderBy(c => c.X)
            .FirstOrDefault();
        
        var upNeighbor = Data
            .Where(c => c.X == coors.X && c.Y > coors.Y && c.Type == TileType.Red)
            .OrderByDescending(c => c.X)
            .FirstOrDefault();

        var downNeighbor = Data
            .Where(c => c.X == coors.X && c.Y < coors.Y && c.Type == TileType.Red)
            .OrderBy(c => c.X)
            .FirstOrDefault();

        if (leftNeighbor != null) neighbors.Add(leftNeighbor);
        if (rightNeighbor != null) neighbors.Add(rightNeighbor);
        if (upNeighbor != null) neighbors.Add(upNeighbor);
        if (downNeighbor != null) neighbors.Add(downNeighbor);
        
        Console.WriteLine($"Found {neighbors.Count} neighbors for {coors.X}, {coors.Y}");

        return neighbors;
    }
    
}