using GreetingService.Core;

namespace GreetingService.Cli;

public static class Program
{
    private static readonly string DefaultHistoryPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "GreetingService",
        "history.jsonl");

    public static string Compose(string[] args) =>
        Compose(
            args,
            Environment.GetEnvironmentVariable("GREETING_TEMPLATE"),
            new FileGreetingHistory(Environment.GetEnvironmentVariable("GREETING_HISTORY_PATH") ?? DefaultHistoryPath));

    /// <summary>Composes a greeting without recording it to any history.</summary>
    public static string Compose(string[] args, string? environmentTemplate) =>
        Compose(args, environmentTemplate, GreetingHistory.None);

    public static string Compose(string[] args, string? environmentTemplate, IGreetingHistory history)
    {
        string? name = null;
        string? template = null;
        var showHistory = false;

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
            else if (args[i] == "--history")
            {
                showHistory = true;
            }
            else if (args[i] == "--table")
            {
                // Only meaningful alongside --history, which is not yet handled here.
            }
            else if (name is null)
            {
                name = args[i];
            }
        }

        if (showHistory)
        {
            return string.Join(Environment.NewLine, history.Read().Select(record => record.Text));
        }

        var greeting = Greeting.For(name, template ?? environmentTemplate);
        history.Append(Greeting.ResolveName(name), greeting);
        return greeting;
    }

    public static void Main(string[] args) => Console.WriteLine(Compose(args));
}
