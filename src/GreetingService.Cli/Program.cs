using GreetingService.Core;

namespace GreetingService.Cli;

public static class Program
{
    private const string LetterOrDigitRequiredMessage = "greet: a name must contain at least one letter or digit";
    private const string NameTooLongMessage = "greet: a name must be 64 characters or fewer";
    private const string TemplateMissingNamePlaceholderMessage = "greet: a --greeting template must contain {name}";
    private const int MaxNameLength = 64;

    public static ComposeResult Compose(string[] args) =>
        Compose(args, Environment.GetEnvironmentVariable("GREETING_TEMPLATE"));

    public static ComposeResult Compose(string[] args, string? environmentTemplate)
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

        if (!string.IsNullOrWhiteSpace(template) && !template.Contains("{name}", StringComparison.Ordinal))
        {
            return new ComposeResult(null, TemplateMissingNamePlaceholderMessage);
        }

        var resolvedTemplate = template ?? environmentTemplate;

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
