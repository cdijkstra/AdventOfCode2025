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
                Console.Write(Data.Any(c => c.X == idx && c.Y == idy) ? "#" : ".");
            }
            Console.WriteLine();
        }
    }
}