using GreetingService.Core;

namespace GreetingService.Cli;

public static class Program
{
    public static string Compose(string[] args) => Greeting.For(args.Length > 0 ? args[0] : null);

    public static void Main(string[] args) => Console.WriteLine(Compose(args));
}
