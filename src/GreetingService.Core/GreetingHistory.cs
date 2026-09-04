using System.Text;
using System.Text.Json;

namespace GreetingService.Core;

/// <summary>Records printed greetings to an append-only JSON Lines file.</summary>
public static class GreetingHistory
{
    /// <summary>The environment variable naming the history file's path.</summary>
    public const string PathVariable = "GREETING_HISTORY_PATH";

    /// <summary>The history path from <see cref="PathVariable"/>, or the <see cref="Environment.SpecialFolder.LocalApplicationData"/> default when unset.</summary>
    public static string ResolvePath() => ResolvePath(Environment.GetEnvironmentVariable(PathVariable));

    /// <summary><paramref name="configuredPath"/> when it is not null or whitespace, otherwise the <see cref="Environment.SpecialFolder.LocalApplicationData"/> default.</summary>
    public static string ResolvePath(string? configuredPath) =>
        string.IsNullOrWhiteSpace(configuredPath)
            ? Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GreetingService", "history.jsonl")
            : configuredPath;

    /// <summary>The JSON Lines representation of <paramref name="entry"/>, with its print time normalised to UTC.</summary>
    public static string ToLine(GreetingHistoryEntry entry) =>
        JsonSerializer.Serialize(entry with { PrintedAt = entry.PrintedAt.ToUniversalTime() });

    /// <summary>Appends <paramref name="entry"/> to the history file at <paramref name="path"/>, creating its containing directory if needed. Failures are swallowed so a recording problem never stops a greeting being printed.</summary>
    public static void Record(string path, GreetingHistoryEntry entry)
    {
        try
        {
            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.AppendAllText(path, ToLine(entry) + "\n", new UTF8Encoding(false));
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or NotSupportedException)
        {
        }
    }
}
