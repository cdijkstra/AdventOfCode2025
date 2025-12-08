namespace Playground;

public class Junction
{
    public string Name { get; set; }
    public Coordinate Coordinate { get; set; }
    public List<Junction> ConnectedTo = [];
}