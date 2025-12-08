namespace Playground;

public class Coordinate
{
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
    public int DistanceTo(Coordinate other)
    {
        // Euclidean distance
        return (int)Math.Sqrt(
            Math.Pow(X - other.X, 2)
            + Math.Pow(Y - other.Y, 2)
            + Math.Pow(Z - other.Z, 2)
        );
    }
}