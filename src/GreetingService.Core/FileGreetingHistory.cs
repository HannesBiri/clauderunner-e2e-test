using System.Text.Json;

namespace GreetingService.Core;

/// <summary>An <see cref="IGreetingHistory"/> backed by a JSON-lines file, one record per line.</summary>
public sealed class FileGreetingHistory : IGreetingHistory
{
    private readonly string _path;
    private readonly TimeProvider _timeProvider;

    public FileGreetingHistory(string path, TimeProvider? timeProvider = null)
    {
        _path = path;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }

    /// <summary>Appends a record for <paramref name="name"/> and <paramref name="text"/>, stamping the current UTC time, creating the parent directory when it does not exist.</summary>
    public void Append(string name, string text)
    {
        var record = new GreetingRecord(_timeProvider.GetUtcNow().ToUniversalTime(), name, text);
        var line = JsonSerializer.Serialize(record);

        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.AppendAllText(_path, line + Environment.NewLine);
    }

    /// <summary>Reads every record from the file, oldest first, skipping blank or unparseable lines. Returns an empty list when the file does not exist.</summary>
    public IReadOnlyList<GreetingRecord> Read()
    {
        if (!File.Exists(_path))
        {
            return [];
        }

        var records = new List<GreetingRecord>();

        foreach (var line in File.ReadAllLines(_path))
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            GreetingRecord? record;
            try
            {
                record = JsonSerializer.Deserialize<GreetingRecord>(line);
            }
            catch (JsonException)
            {
                continue;
            }

            if (record is not null)
            {
                records.Add(record);
            }
        }

        return records;
    }
}
