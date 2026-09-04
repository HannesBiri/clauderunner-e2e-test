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

    public static string Run(string[] args, string? environmentTemplate, string historyPath, DateTimeOffset printedAt)
    {
        var greeting = Compose(args, environmentTemplate);
        GreetingHistory.Record(historyPath, new GreetingHistoryEntry(greeting, printedAt));
        return greeting;
    }

    public static void Main(string[] args) => Console.WriteLine(Run(
        args,
        Environment.GetEnvironmentVariable("GREETING_TEMPLATE"),
        GreetingHistory.ResolvePath(),
        DateTimeOffset.UtcNow));
}
