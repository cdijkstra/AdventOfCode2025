namespace Playground;

public class JunctionPair
{
    public Junction A { get; set; }
    public Junction B { get; set; }
    public int Distance { get; set; }

    public JunctionPair(Junction a, Junction b, int distance)
    {
        A = a;
        B = b;
        Distance = distance;
    }

    public override string ToString()
    {
        return $"{A.Name} <-> {B.Name} : {Distance}";
    }
}