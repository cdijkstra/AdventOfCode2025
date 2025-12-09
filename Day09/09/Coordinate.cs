namespace _09;

enum TileType
{
    Red = '#',
    Green = 'X'
}

class Coordinates
{
    public TileType Type { get; set; }
    public long X { get; set; }
    public long Y { get; set; }

    public Coordinates(TileType type, long x, long y)
    {
        Type = type;
        X = x;
        Y = y;
    }
}