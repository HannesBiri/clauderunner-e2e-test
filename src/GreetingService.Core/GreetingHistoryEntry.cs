using System.Text.Json.Serialization;

namespace GreetingService.Core;

/// <summary>A single recorded greeting: the text as printed and the instant it was printed.</summary>
public sealed record GreetingHistoryEntry(
    [property: JsonPropertyName("text")] string Text,
    [property: JsonPropertyName("printedAt")] DateTimeOffset PrintedAt);
