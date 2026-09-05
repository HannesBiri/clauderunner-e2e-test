using System.Text.Json.Serialization;

namespace GreetingService.Core;

/// <summary>A single recorded greeting.</summary>
public sealed record GreetingRecord(
    [property: JsonPropertyName("time")] DateTimeOffset Time,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("text")] string Text);

/// <summary>A store of recorded greetings.</summary>
public interface IGreetingHistory
{
    /// <summary>All recorded greetings, oldest first.</summary>
    IReadOnlyList<GreetingRecord> Read();

    /// <summary>Records a greeting for <paramref name="name"/> with text <paramref name="text"/>, stamping the current time.</summary>
    void Append(string name, string text);
}

/// <summary>Well-known <see cref="IGreetingHistory"/> implementations.</summary>
public static class GreetingHistory
{
    /// <summary>A history that records nothing and never has anything to read.</summary>
    public static IGreetingHistory None { get; } = new NoOpGreetingHistory();

    private sealed class NoOpGreetingHistory : IGreetingHistory
    {
        public IReadOnlyList<GreetingRecord> Read() => [];

        public void Append(string name, string text)
        {
        }
    }
}
