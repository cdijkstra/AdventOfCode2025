using System.Diagnostics;
using System.Text.RegularExpressions;
using Google.OrTools.LinearSolver;

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
        var sw = new Stopwatch();
        sw.Start();
        ReadData("testdata.txt");
        Debug.Assert(Part1() == 7);
        Debug.Assert(Part2() == 33);
        ReadData("data.txt");
        Console.WriteLine(Part1());
        Console.WriteLine(Part2());
        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);
    }

    private static void ReadData(string fileName)
    {
        _machines.Clear();
        foreach (var line in File.ReadAllLines(fileName))
        {
            // Extract content in []
            var machine = new Machine();
            var squareBracket = Regex.Match(line, @"\[(.*?)\]").Groups[1].Value;
            machine.DesiredDiagram = squareBracket.Select(c => c == '#').ToArray();

            // Extract all contents in ()
            machine.Buttons = Regex.Matches(line, @"\((.*?)\)")
                .Select(m => m.Groups[1].Value.Trim())
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => s.Split(',').Select(int.Parse).ToList())
                .ToList();

            // Extract content in {}
            machine.JoltageRequirements = Regex.Match(line, @"\{(.*?)\}")
                .Groups[1].Value.Split(',')
                .Select(s => int.Parse(s.Trim()))
                .ToList();

            _machines.Add(machine);
        }
    }

    private static long Part1()
    {
        var totalButtonsPressed = 0;
        var machineIdx = 0;
        foreach (var machine in _machines)
        {
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

                    pq.Enqueue((newDiagram, newButtonsPressed, newBitsOff),
                        newBitsOff + PrioWeightButtons * newButtonsPressed.Count);
                }
            }
        }

        return totalButtonsPressed;
    }

    private static long Part2()
    {
        // Use linear algebra to sovle this; Ax = B
        // [.##.]         (3) (1,3) (2) (2,3) (0,2) (0,1)      {3,5,4,7}
        // buttons:        0   1     2   3     4     5
        // Can be read as (with coefficient x0,x1,x2,x3,x4,x5 for the button presses)
        // e + f = 3
        // b + f = 5
        // c + d + e = 4
        // a + b + d = 7
        // Ax = B
        // [ 0 0 0 0 1 1][x_0] = [3]
        // [ 0 1 0 0 0 1][x_1] = [5]
        // [ 0 0 1 1 1 0][x_2] = [4]
        // [ 1 1 0 1 0 0][x_3] = [7]
        //               [x_4]
        //               [x_5]
        // 4 equations; 6 variables. Underdetermined system, damn.

        var totalButtonsPressed = 0;
        foreach (var machine in _machines)
        {
            var aHeight = machine.Buttons.Count;
            var aLength = machine.JoltageRequirements.Count;
            var aData = new double[aLength, aHeight];
            for (var buttonIdx = 0; buttonIdx != machine.Buttons.Count; buttonIdx++)
            {
                machine.Buttons[buttonIdx].ForEach(num => { aData[num, buttonIdx] = 1; });
            }

            var bData = machine.JoltageRequirements.Select(j => (double)j).ToArray();
            var buttonsPressed = SolveIntegerSystem(aData, bData);
            totalButtonsPressed += buttonsPressed.Sum();
        }

        return totalButtonsPressed;
    }

    public static int[] SolveIntegerSystem(double[,] aData, double[] bData)
    {
        // Make use of external library (Google.OrTools.LinearSolver) to solve the linear systems
        // https://developers.google.com/optimization
        int rows = aData.GetLength(0);
        int cols = aData.GetLength(1);
        var solver = Solver.CreateSolver("SCIP"); // Symbolic Constraint Integer Programming solver
        var x = new Variable[cols];
        for (var j = 0; j < cols; j++)
            x[j] = solver.MakeIntVar(0.0, double.PositiveInfinity, $"x{j}");

        for (var i = 0; i < rows; i++)
        {
            var ct = solver.MakeConstraint(bData[i], bData[i]);
            for (int j = 0; j < cols; j++)
                ct.SetCoefficient(x[j], aData[i, j]);
        }

        var objective = solver.Objective();
        for (var j = 0; j < cols; j++)
            objective.SetCoefficient(x[j], 1);
        objective.SetMinimization();

        var resultStatus = solver.Solve();
        if (resultStatus != Solver.ResultStatus.OPTIMAL) throw new Exception("No integer solution found");

        return x.Select(v => (int)v.SolutionValue()).ToArray();
    }
}