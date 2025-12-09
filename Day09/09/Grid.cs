using System.Reflection.PortableExecutable;
using System.Security.AccessControl;

namespace _09;

class Grid
{
    public List<Coordinates> Data = new();
    public List<HorizontalRange> Interior = new();
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
                    var newwNeighbors = neighbors
                        .Where(n => !visited.Any(v => v.X == n.X && v.Y == n.Y))
                        .ToList();
                    Console.Write($"3 neighbors found for {entry.X}, {entry.Y}; unique = " + newwNeighbors.Count());


                    return;
            }
        }

        Console.WriteLine("Finished creating connected grid");
        FloodFillInterior();
        Console.WriteLine("Finished flooding");
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

    private void FloodExterior()
    {
        Interior = new();

        for (var y = MinY() + 1; y < MaxY(); y++)
        {
            // Get all filled Xs for this row
            var xs = Data.Where(c => c.Y == y).Select(c => c.X).OrderBy(x => x).ToList();
            if (xs.Count < 2) continue; // Need at least two to form an interior

            for (int i = 0; i < xs.Count - 1; i++)
            {
                long fromX = xs[i] + 1;
                long toX = xs[i + 1] - 1;
                if (fromX <= toX)
                {
                    Interior.Add(new HorizontalRange(y, fromX, toX));
                }
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