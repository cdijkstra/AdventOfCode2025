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
                    Console.Write($"3 neighbors found for {entry.X}, {entry.Y}; unique = " + newwNeighbors.Count());


                    return;
            }
        }

        Print();
        FloodFillInterior();
        Print();

    }
    
    public void FloodFillInterior()
    {
        // Create a set of existing coordinates for fast lookup
        var existingCoords = new HashSet<(long, long)>(Data.Select(c => (c.X, c.Y)));
        
        // Find a point inside the loop to start flood fill
        var startPoint = FindInteriorPoint(existingCoords);
        if (startPoint == null)
        {
            Console.WriteLine("Could not find interior point");
            return;
        }
        
        // Flood fill from the interior point
        var queue = new Queue<(long X, long Y)>();
        var visited = new HashSet<(long, long)>();
        
        queue.Enqueue(startPoint.Value);
        visited.Add(startPoint.Value);
        
        // Directions: up, down, left, right
        var directions = new[] { (0, 1), (0, -1), (-1, 0), (1, 0) };
        
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            
            // Add this point as green if it's not already in Data
            if (!existingCoords.Contains(current))
            {
                Data.Add(new Coordinates(TileType.Green, current.X, current.Y));
                existingCoords.Add(current);
            }
            
            // Check all 4 directions
            foreach (var (dx, dy) in directions)
            {
                var newX = current.X + dx;
                var newY = current.Y + dy;
                var newPoint = (newX, newY);
                
                // Skip if already visited
                if (visited.Contains(newPoint)) continue;
                
                // Skip if it's a boundary point (red coordinate)
                if (Data.Any(c => c.X == newX && c.Y == newY && c.Type == TileType.Red)) continue;
                
                // Check if this point is inside the loop using ray casting
                if (IsInsideLoop(newX, newY))
                {
                    visited.Add(newPoint);
                    queue.Enqueue(newPoint);
                }
            }
        }
        
        Console.WriteLine("Finished flooding");
    }

    private (long X, long Y)? FindInteriorPoint(HashSet<(long, long)> existingCoords)
    {
        // Find a point that's already green (connecting line) as starting point
        var greenPoint = Data.FirstOrDefault(c => c.Type == TileType.Green);
        if (greenPoint != null)
        {
            return (greenPoint.X, greenPoint.Y);
        }
        
        // Alternative: scan horizontally and find a point between red boundaries
        for (long y = MinY(); y <= MaxY(); y++)
        {
            var redPointsInRow = Data.Where(c => c.Y == y && c.Type == TileType.Red)
                                    .OrderBy(c => c.X).ToList();
            
            if (redPointsInRow.Count >= 2)
            {
                // Find a point between first two red points
                long midX = (redPointsInRow[0].X + redPointsInRow[1].X) / 2;
                if (IsInsideLoop(midX, y))
                {
                    return (midX, y);
                }
            }
        }
        
        return null;
    }

    private bool IsInsideLoop(long x, long y)
    {
        // Ray casting algorithm - cast ray to the right and count intersections
        var redPoints = Data.Where(c => c.Type == TileType.Red).ToList();
        int intersections = 0;
        
        for (int i = 0; i < redPoints.Count; i++)
        {
            var p1 = redPoints[i];
            var p2 = redPoints[(i + 1) % redPoints.Count];
            
            // Check if ray crosses this edge
            if ((p1.Y > y) != (p2.Y > y))
            {
                // Calculate intersection x-coordinate
                double intersectX = p1.X + (double)(p2.X - p1.X) * (y - p1.Y) / (p2.Y - p1.Y);
                if (x < intersectX)
                {
                    intersections++;
                }
            }
        }
        
        return (intersections % 2) == 1;
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