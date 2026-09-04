namespace GreetingService.Core;

/// <summary>Builds the text the application greets somebody with.</summary>
public static class Greeting
{
    /// <summary>The greeting shown when nothing else has been configured.</summary>
    public const string Default = "Hello, World!";

    /// <summary>The greeting for <paramref name="name"/>, or <see cref="Default"/> when no name is given.</summary>
    public static string For(string? name) =>
        string.IsNullOrWhiteSpace(name) ? Default : $"Hello, {name.Trim()}!";
}
