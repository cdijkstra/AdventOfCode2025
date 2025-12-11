namespace _09;

class Coordinates
{
    public int X { get; set; }
    public int Y { get; set; }

    public Coordinates(int x, int y)
    {
        X = x;
        Y = y;
    }
    
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