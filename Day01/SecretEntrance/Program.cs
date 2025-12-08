using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        var testdata = ReadData("dummydata.txt");
        var data = ReadData("data.txt");
        Debug.Assert(CalculateTimesAtZero(testdata) == 3);
        Debug.Assert(CalculateTimesPassingZero(testdata) == 6);
        Console.WriteLine("Part 1: " + CalculateTimesAtZero(data));
        Console.WriteLine("Part 2: " + CalculateTimesPassingZero(data));
    }

    private static int CalculateTimesAtZero(string[] data)
    {
        var num = 50;
        var timesAtZero = 0;
        foreach (var line in data)
        {
            var leftRight = (Direction)Enum.Parse(typeof(Direction), line[0].ToString());
            var rotation = int.Parse(line[1..]);
            num = leftRight == Direction.L ? 
                (num - rotation) % 100 :
                (num + rotation) % 100;
    
            if (num == 0) timesAtZero++;
        }
        return timesAtZero;
    }
    
    private static int CalculateTimesPassingZero(string[] data)
    {
        var num = 50;
        var timesPassingZero = 0;
        foreach (var line in data)
        {
            var leftRight = (Direction)Enum.Parse(typeof(Direction), line[0].ToString());
            var rotation = int.Parse(line[1..]);
            switch (leftRight)
            {
                case Direction.L:
                {
                    var initialZero = num == 0 ? 1 : 0;
                    num -= rotation;
                    if (num <= 0)
                    {
                        timesPassingZero += Math.Abs(num) / 100 + 1 - initialZero;
                        num = ((num % 100) + 100) % 100;
                    }
                    break;
                }
                case Direction.R:
                {
                    num += rotation;
                    if (num >= 100)
                    {
                        timesPassingZero += num / 100;
                        num %= 100;
                    }
                    break;
                }
            }
        }
        Console.WriteLine(timesPassingZero);
        return timesPassingZero;
    }

    private static string[] ReadData(string fileName)
    {
        return File.ReadAllLines(fileName);
    }
}

enum Direction
{
    L,
    R
};


