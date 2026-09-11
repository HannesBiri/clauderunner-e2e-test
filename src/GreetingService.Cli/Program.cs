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
        ParseArgs(args, out var name, out var template, out _);

        return ComposeOne(name, template ?? environmentTemplate);
    }

    public static ComposeManyResult ComposeMany(string[] args, string? environmentTemplate, Func<string, string[]> readAllLines)
    {
        ParseArgs(args, out _, out var template, out var namesFile);
        var resolvedTemplate = template ?? environmentTemplate;

        if (namesFile is null)
        {
            return new ComposeManyResult([], "greet: --names-file requires a path");
        }

        string[] lines;

        try
        {
            lines = readAllLines(namesFile);
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
        {
            return new ComposeManyResult([], $"greet: cannot read names file '{namesFile}': {ex.Message}");
        }

        var greetings = new List<string>();

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var oneResult = ComposeOne(line, resolvedTemplate);

            if (oneResult.Error is not null)
            {
                return new ComposeManyResult([], oneResult.Error);
            }

            greetings.Add(oneResult.Greeting!);
        }

        return new ComposeManyResult(greetings, null);
    }

    private static void ParseArgs(string[] args, out string? name, out string? template, out string? namesFile)
    {
        name = null;
        template = null;
        namesFile = null;

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
            else if (args[i] == "--names-file")
            {
                if (i + 1 < args.Length)
                {
                    namesFile = args[i + 1];
                    i++;
                }
            }
            else if (name is null)
            {
                name = args[i];
            }
        }
    }

    // `Greeting` is fully qualified throughout this method: the Greeting.Tools.Core package puts a `Greeting`
    // *namespace* in scope, which would otherwise win over the class of the same name.
    private static ComposeResult ComposeOne(string? name, string? resolvedTemplate)
    {
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

    public static int Run(string[] args, string? environmentTemplate, TextWriter output, TextWriter error, Func<string, string[]> readAllLines)
    {
        ParseArgs(args, out _, out _, out var namesFile);

        if (namesFile is not null)
        {
            var manyResult = ComposeMany(args, environmentTemplate, readAllLines);

            if (manyResult.Error is not null)
            {
                error.WriteLine(manyResult.Error);
                return 2;
            }

            foreach (var greeting in manyResult.Greetings)
            {
                output.WriteLine(greeting);
            }

            return 0;
        }

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
