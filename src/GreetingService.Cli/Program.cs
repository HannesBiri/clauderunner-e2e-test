using GreetingService.Core;

namespace GreetingService.Cli;

public static class Program
{
    private const string LetterOrDigitRequiredMessage = "greet: a name must contain at least one letter or digit";
    private const string NameTooLongMessage = "greet: a name must be 64 characters or fewer";
    private const int MaxNameLength = 64;

    public static ComposeResult Compose(string[] args) =>
        Compose(args, Environment.GetEnvironmentVariable("GREETING_TEMPLATE"));

    public static ComposeResult Compose(string[] args, string? environmentTemplate)
    {
        var names = new List<string>();
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
            else
            {
                names.Add(args[i]);
            }
        }

        var resolvedTemplate = template ?? environmentTemplate;

        var distinctNames = new List<string>();
        foreach (var name in names)
        {
            var trimmedName = name.Trim();

            if (trimmedName.Length == 0)
            {
                continue;
            }

            if (!distinctNames.Contains(trimmedName, StringComparer.Ordinal))
            {
                distinctNames.Add(trimmedName);
            }
        }

        if (distinctNames.Count == 0)
        {
            return new ComposeResult(Greeting.For(null, resolvedTemplate), null);
        }

        foreach (var trimmedName in distinctNames)
        {
            if (!trimmedName.Any(char.IsLetterOrDigit))
            {
                return new ComposeResult(null, LetterOrDigitRequiredMessage);
            }

            if (trimmedName.Length > MaxNameLength)
            {
                return new ComposeResult(null, NameTooLongMessage);
            }
        }

        var greeting = string.Join(
            Environment.NewLine,
            distinctNames.Select(trimmedName => Greeting.For(trimmedName, resolvedTemplate)));

        return new ComposeResult(greeting, null);
    }

    public static int Run(string[] args, string? environmentTemplate, TextWriter output, TextWriter error)
    {
        var result = Compose(args, environmentTemplate);

        if (result.Error is not null)
        {
            error.WriteLine(result.Error);
            return 2;
        }

        output.WriteLine(result.Greeting);
        return 0;
    }

    public static int Main(string[] args) =>
        Run(args, Environment.GetEnvironmentVariable("GREETING_TEMPLATE"), Console.Out, Console.Error);
}
