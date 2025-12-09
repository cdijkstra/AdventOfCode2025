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
    
    public long MinX() => Data.Min(c => c.X);
    public long MaxX() => Data.Max(c => c.X);
    public long MinY() => Data.Min(c => c.Y);
    public long MaxY() => Data.Max(c => c.Y);

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
                    break;
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
                    Console.WriteLine("3 neighbors found; Oh noooo");
                    Console.Write($"3 neighbors found for {entry.X}, {entry.Y}");
                    var newwNeighbors = neighbors
                        .Where(n => !visited.Any(v => v.X == n.X && v.Y == n.Y))
                        .ToList();
                    foreach (var neighbor in newwNeighbors)
                    {
                        Console.WriteLine($"({neighbor.X}, {neighbor.Y})");
                    }
                    return;
                    break;
            }
        }

        Print();

    }

    private void AddGreen(Coordinates neighbor, Coordinates entry)
    {
        if (neighbor.X - entry.X != 0)
        {
            if (neighbor.X - entry.X > 0)
            {
                for (var idx = entry.X + 1; idx < neighbor.X; idx++)
                {
                    Data.Add(new Coordinates(TileType.Green, idx, entry.Y));
                }
            }
            else
            {
                for (var idx = entry.X - 1; idx > neighbor.X; idx--)
                {
                    Data.Add(new Coordinates(TileType.Green, idx, entry.Y));
                }
            }
        }
        else if (neighbor.Y - entry.Y != 0)
        {
            if (neighbor.Y - entry.Y > 0)
            {
                for (var idy = entry.Y + 1; idy < neighbor.Y; idy++)
                {
                    Data.Add(new Coordinates(TileType.Green, entry.X, idy));
                }
            }
            else
            {
                for (var idy = entry.Y - 1; idy > neighbor.Y; idy--)
                {
                    Data.Add(new Coordinates(TileType.Green, entry.X, idy));
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
        
        // List<Coordinates> neighbors = new();
        //
        // // Find the first left neighbor on the same row
        // for (var x = coors.X - 1; x >= MinX(); x--)
        // {
        //     if (Data.Any(c => c.X == x && c.Y == coors.Y && c.Type == TileType.Red))
        //     {
        //         neighbors.Add(Data.Single(c => c.X == x && c.Y == coors.Y));
        //         break;
        //     }
        // }
        //
        // // Find the first right neighbor on the same row
        // for (var x = coors.X + 1; x <= MaxX(); x++)
        // {
        //     if (Data.Any(c => c.X == x && c.Y == coors.Y && c.Type == TileType.Red))
        //     {
        //         neighbors.Add(Data.Single(c => c.X == x && c.Y == coors.Y));
        //         break;
        //     }
        // }
        //
        // // Find the first up neighbor in the same column
        // for (var y = coors.Y - 1; y >= MinY(); y--)
        // {
        //     if (Data.Any(c => c.X == coors.X && c.Y == y && c.Type == TileType.Red))
        //     {
        //         neighbors.Add(Data.Single(c => c.X == coors.X && c.Y == y));
        //         break;
        //     }
        // }
        //
        // // Find the first down neighbor in the same column
        // for (var y = coors.Y + 1; y <= MaxY(); y++)
        // {
        //     if (Data.Any(c => c.X == coors.X && c.Y == y && c.Type == TileType.Red))
        //     {
        //         neighbors.Add(Data.Single(c => c.X == coors.X && c.Y == y));
        //         break;
        //     }
        // }

        return neighbors;
    }
}