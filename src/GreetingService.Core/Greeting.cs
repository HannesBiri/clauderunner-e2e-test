namespace GreetingService.Core;

/// <summary>Builds the text the application greets somebody with.</summary>
public static class Greeting
{
    /// <summary>The greeting shown when nothing else has been configured.</summary>
    public const string Default = "Hello, World!";

    /// <summary>The template used when no other template has been configured.</summary>
    public const string DefaultTemplate = "Hello, {name}!";

    /// <summary>The name used when no other name has been given.</summary>
    public const string DefaultName = "World";

    /// <summary>The greeting for <paramref name="name"/>, or <see cref="Default"/> when no name is given.</summary>
    public static string For(string? name) => For(name, null);

    /// <summary>The greeting for <paramref name="name"/> rendered from <paramref name="template"/>, falling back to <see cref="DefaultTemplate"/> and <see cref="DefaultName"/> when either is null or whitespace.</summary>
    public static string For(string? name, string? template)
    {
        var resolvedTemplate = string.IsNullOrWhiteSpace(template) ? DefaultTemplate : template;
        var resolvedName = string.IsNullOrWhiteSpace(name) ? DefaultName : name.Trim();
        return resolvedTemplate.Replace("{name}", resolvedName, StringComparison.OrdinalIgnoreCase);
    }
}
