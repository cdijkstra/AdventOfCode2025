namespace _09;

public class HorizontalRange
{
    public long Y { get; }
    public long FromX { get; }
    public long ToX { get; }

    public HorizontalRange(long y, long fromX, long toX)
    {
        Y = y;
        FromX = fromX;
        ToX = toX;
    }
}