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
        var (name, template, _) = ParseArgs(args);

        return ComposeOne(name, template ?? environmentTemplate);
    }

    public static IReadOnlyList<ComposeResult> ComposeAll(
        string[] args,
        string? environmentTemplate,
        Func<string, string[]> readLines)
    {
        var (name, template, filePath) = ParseArgs(args);
        var resolvedTemplate = template ?? environmentTemplate;

        if (filePath is null)
        {
            return [ComposeOne(name, resolvedTemplate)];
        }

        string[] lines;

        try
        {
            lines = readLines(filePath);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException or NotSupportedException)
        {
            return [new ComposeResult(null, $"greet: cannot read names file '{filePath}'")];
        }

        return lines.Select(line => ComposeOne(line, resolvedTemplate)).ToList();
    }

    private static (string? Name, string? Template, string? FilePath) ParseArgs(string[] args)
    {
        string? name = null;
        string? template = null;
        string? filePath = null;

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
            else if (args[i] == "--file")
            {
                if (i + 1 < args.Length)
                {
                    filePath = args[i + 1];
                    i++;
                }
            }
            else if (name is null)
            {
                name = args[i];
            }
        }

        return (name, template, filePath);
    }

    private static ComposeResult ComposeOne(string? name, string? resolvedTemplate)
    {
        // `Greeting` is fully qualified throughout this method: the Greeting.Tools.Core package puts a `Greeting`
        // *namespace* in scope, which would otherwise win over the class of the same name.
        if (string.IsNullOrWhiteSpace(name))
        {
            return new ComposeResult(GreetingService.Core.Greeting.For(null, resolvedTemplate), null);
        }

        var trimmedName = name.Trim();

        if (!trimmedName.Any(char.IsLetterOrDigit))
        {
            return new ComposeResult(null, LetterOrDigitRequiredMessage);
        }

        if (trimmedName.Length > MaxNameLength)
        {
            return new ComposeResult(null, NameTooLongMessage);
        }

        return new ComposeResult(GreetingService.Core.Greeting.For(trimmedName, resolvedTemplate), null);
    }

    public static int Run(string[] args, string? environmentTemplate, TextWriter output, TextWriter error) =>
        Run(args, environmentTemplate, output, error, File.ReadAllLines);

    public static int Run(
        string[] args,
        string? environmentTemplate,
        TextWriter output,
        TextWriter error,
        Func<string, string[]> readLines)
    {
        var results = ComposeAll(args, environmentTemplate, readLines);
        var firstError = results.Select(r => r.Error).FirstOrDefault(e => e is not null);

        if (firstError is not null)
        {
            error.WriteLine(firstError);
            return 2;
        }

        foreach (var result in results)
        {
            output.WriteLine(result.Greeting);
        }

        return 0;
    }

    public static int Main(string[] args) =>
        Run(args, Environment.GetEnvironmentVariable("GREETING_TEMPLATE"), Console.Out, Console.Error);
}
