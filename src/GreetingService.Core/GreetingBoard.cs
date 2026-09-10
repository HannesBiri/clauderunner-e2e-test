using Greeting.Tools.Core;

namespace GreetingService.Core;

/// <summary>Greets several people at once, one per line.</summary>
public static class GreetingBoard
{
    /// <summary>
    /// A row per name — the name, then the greeting it produces — rendered by
    /// <see cref="TextTable"/> so every caller lays the columns out the same way.
    /// </summary>
    public static string For(IEnumerable<string> names, string? template = null) =>
        TextTable.Render(["Name", "Greeting"], names.Select(name => (IReadOnlyList<string>)[name, Greeting.For(name, template)]));
}
