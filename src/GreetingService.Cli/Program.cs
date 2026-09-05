using System.Reflection;
using GreetingService.Core;

namespace GreetingService.Cli;

public static class Program
{
    public static readonly string Version = ReadVersion();

    private static string ReadVersion()
    {
        var informationalVersion = typeof(Program).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;

        if (informationalVersion is not null)
        {
            return informationalVersion.Split('+')[0];
        }

        return typeof(Program).Assembly.GetName().Version?.ToString(3) ?? "0.0.0";
    }

    public static string Compose(string[] args) =>
        Compose(args, Environment.GetEnvironmentVariable("GREETING_TEMPLATE"));

    public static string Compose(string[] args, string? environmentTemplate)
    {
        string? name = null;
        string? template = null;

        for (var i = 0; i < args.Length; i++)
        {
            if (args[i] == "--version")
            {
                return Version;
            }

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
