namespace _09;

enum TileType
{
    Red = '#',
    Green = 'X'
}

class Coordinates
{
    public TileType Type { get; }
    public long X { get; set; }
    public long Y { get; set; }

    public Coordinates(TileType type, long x, long y)
    {
        Type = type;
        X = x;
        Y = y;
    }
    
    public override bool Equals(object? obj)
    {
        if (obj is not Coordinates other) return false;
        return X == other.X && Y == other.Y && Type == other.Type;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, Type);
    }
}