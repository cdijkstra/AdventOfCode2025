using System.Reflection.PortableExecutable;
using System.Security.AccessControl;

namespace _09;

class Grid
{
    public List<Coordinates> Data = new();
    public Grid(List<Coordinates> data)
    {
        Data = new();
        Data = data;
    }
    
    public long MinX() => Data.Where(c => c.Type == TileType.Red).Min(c => c.X);
    public long MaxX() => Data.Where(c => c.Type == TileType.Red).Max(c => c.X);
    public long MinY() => Data.Where(c => c.Type == TileType.Red).Min(c => c.Y);
    public long MaxY() => Data.Where(c => c.Type == TileType.Red).Max(c => c.Y);

    public void Print()
    {
        // Create grid from MinX - 1 to MaxX + 1, same for Y
        for (var idx = MinX() - 1; idx != MaxX() + 1; idx++)
        {
            for (var idy = MinY() - 1; idy != MaxY() + 1; idy++)
            {
                Console.Write(    Data.FirstOrDefault(c => c.X == idx && c.Y == idy) is { } found
                    ? (char)found.Type
                    : '.');
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
                    Console.WriteLine($"3 neighbors found for {entry.X}, {entry.Y}; Oh noooo");
                    return;
            }
        }

        FloodFillInterior();
        Print();
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
    
    private void FloodFillInterior()
    {
        // Find a starting interior point
        Coordinates? start = null;
        for (var x = MinX() + 1; x < MaxX(); x++)
        {
            for (var y = MinY() + 1; y < MaxY(); y++)
            {
                if (Data.All(c => c.X != x || c.Y != y))
                {
                    start = new Coordinates(TileType.Red, x, y);
                    break;
                }
            }
            if (start != null) break;
        }
        if (start == null) return; // No interior found

        var visited = new HashSet<(long, long)>();
        var queue = new Queue<Coordinates>();
        queue.Enqueue(start);

        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            if (visited.Contains((current.X, current.Y))) continue;
            visited.Add((current.X, current.Y));

            // Only fill empty cells
            if (Data.All(c => c.X != current.X || c.Y != current.Y))
            {
                Data.Add(new Coordinates(TileType.Green, current.X, current.Y));
            }

            var directions = new (long dx, long dy)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
            foreach (var (dx, dy) in directions)
            {
                var nx = current.X + dx;
                var ny = current.Y + dy;
                if (nx <= MinX() || nx >= MaxX() || ny <= MinY() || ny >= MaxY()) continue;
                
                if (Data.All(c => c.X != nx || c.Y != ny) && !visited.Contains((nx, ny)))
                {
                    queue.Enqueue(new Coordinates(TileType.Green, nx, ny));
                }
            }
        }
    }

    private List<Coordinates> FindNeighbors(Coordinates coors)
    {
        List<Coordinates> neighbors = new();

        // Assume DataSet is a HashSet<Coordinates> built from Data
        var dataSet = new HashSet<Coordinates>(Data);

        var directions = new (long dx, long dy)[] { (-1, 0), (1, 0), (0, -1), (0, 1) };
        foreach (var (dx, dy) in directions)
        {
            var x = coors.X + dx;
            var y = coors.Y + dy;
            while (x >= MinX() && x <= MaxX() && y >= MinY() && y <= MaxY())
            {
                var neighbor = new Coordinates(TileType.Red, x, y);
                if (dataSet.Contains(neighbor))
                {
                    neighbors.Add(neighbor);
                    break;
                }
                x += dx;
                y += dy;
            }
        }

        return neighbors;
    }
}