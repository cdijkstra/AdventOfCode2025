using System.Diagnostics;
using System.Text.RegularExpressions;

public class Machine
{
    public bool[] DesiredDiagram { get; set; }
    public List<List<int>> Buttons { get; set; }
    public List<int> JoltageRequirements { get; set;  }
}

public class Program
{
    private static List<Machine> _machines = new();
    private static readonly int PrioWeightButtons = 5;
    static void Main(string[] args)
    {
        ReadData("testdata.txt");
        Debug.Assert(Part1() == 7);
        ReadData("data.txt");
        Console.WriteLine(Part1());
    }
    
    private static void ReadData(string fileName)
    {
        _machines.Clear();
        foreach (var line in File.ReadAllLines(fileName))
        {
            // Extract content in []
            var machine = new Machine();
            var squareBracket = Regex.Match(line, @"\[(.*?)\]").Groups[1].Value;
            var boolArr = new bool[squareBracket.Length];
            for (int i = 0; i < squareBracket.Length; i++)
            {
                boolArr[i] = squareBracket[i] == '#';
            }
            machine.DesiredDiagram = boolArr;
            
            // Extract all contents in ()
            machine.Buttons = Regex.Matches(line, @"\((.*?)\)")
                .Select(m => m.Groups[1].Value)
                .Select(s => s.Split(',').Select(int.Parse).ToList())
                .ToList();
            
            // Extract content in {}
            machine.JoltageRequirements = Regex.Match(line, @"\{(.*?)\}").Groups[1].Value.Split(",").Select(int.Parse).ToList();
            _machines.Add(machine);
        }
    }

    private static long Part1()
    {
        var totalButtonsPressed = 0;
        var machineIdx = 0;
        foreach (var machine in _machines)
        {
            Console.WriteLine($"Considering machine {++machineIdx}; buttons = {totalButtonsPressed}");
            var currentDiagram = new bool[machine.DesiredDiagram.Length];
            var intialBitsOff = machine.DesiredDiagram
                .Zip(currentDiagram, (a, b) => a != b)
                .Count(b => b);

            var pq = new PriorityQueue<(bool[] diagram, List<List<int>> buttonsPressed, int bitsOff), int>();
            machine.Buttons.ForEach(but => pq.Enqueue((currentDiagram, new(), intialBitsOff), intialBitsOff));
            while (pq.Count > 0)
            {
                var (diagram, machineButtonsPressed, bitsOff) = pq.Dequeue();
                if (bitsOff == 0)
                {
                    totalButtonsPressed += machineButtonsPressed.Count;
                    break;
                }

                foreach (var button in machine.Buttons.Where(b => !machineButtonsPressed.Contains(b)))
                {
                    var newDiagram = (bool[])diagram.Clone(); // Switch bits at locations of the buttons
                    foreach (var idx in button)
                    {
                        newDiagram[idx] = !newDiagram[idx];
                    }

                    // Calculate bits off
                    var newBitsOff = machine.DesiredDiagram
                        .Zip(newDiagram, (a, b) => a != b)
                        .Count(b => b);

                    var newButtonsPressed = new List<List<int>>(machineButtonsPressed);
                    newButtonsPressed.AddRange(button);
                    
                    pq.Enqueue((newDiagram, newButtonsPressed, newBitsOff), newBitsOff + PrioWeightButtons * newButtonsPressed.Count);
                }
            }
        }

        return totalButtonsPressed;
    }
}