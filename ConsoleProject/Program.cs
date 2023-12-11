namespace ConsoleProject;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
    }

    public static int Sum(IEnumerable<object> values)
    {
        var sum = 0;
        foreach (var item in values)
        {
            switch(item)
            {
                case 0:
                    break;
                case int val:
                    sum += val;
                    break;
                case IEnumerable<object> subList when subList.Any():
                    sum += Sum(subList);
                    break;
            }
        }
        return sum;
    }
}
