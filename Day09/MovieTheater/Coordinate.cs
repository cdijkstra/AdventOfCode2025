namespace _09;

class Coordinates(int x, int y)
{
    public int X { get; set; } = x;
    public int Y { get; set; } = y;

    public override bool Equals(object? obj)
    {
        if (obj is not Coordinates other) return false;
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }
    
    public int CompareTo(Coordinates other)
    {
        return (X, Y).CompareTo((other.X, other.Y));
    }}