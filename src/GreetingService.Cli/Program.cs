using GreetingService.Core;

namespace GreetingService.Cli;

public static class Program
{
    public static string Compose(string[] args) =>
        Compose(args, Environment.GetEnvironmentVariable("GREETING_TEMPLATE"));

    public static string Compose(string[] args, string? environmentTemplate)
    {
        string? name = null;
        string? template = null;

        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "--greeting")
            {
                if (i + 1 < args.Length)
                {
                    template = args[i + 1];
                    i++;
                }
            }
            else if (name is null)
            {
                name = args[i];
            }
        }

        return Greeting.For(name, template ?? environmentTemplate);
    }

    public static void Main(string[] args) => Console.WriteLine(Compose(args));
}
