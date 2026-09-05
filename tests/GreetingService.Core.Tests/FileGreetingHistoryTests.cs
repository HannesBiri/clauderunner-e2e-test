namespace GreetingService.Core.Tests;

public class FileGreetingHistoryTests : IDisposable
{
    private readonly string _directory =
        Path.Combine(Path.GetTempPath(), "FileGreetingHistoryTests_" + Guid.NewGuid());

    private string HistoryPath => Path.Combine(_directory, "history.jsonl");

    [Fact]
    public void ReadingAHistoryWhoseFileHasNeverBeenWrittenYieldsAnEmptyList()
    {
        var history = new FileGreetingHistory(HistoryPath);

        Assert.Empty(history.Read());
    }

    [Fact]
    public void AppendedRecordsAreWrittenAsOneJsonObjectPerLineWithTheExpectedPropertyNames()
    {
        var time = new DateTimeOffset(2026, 9, 5, 12, 34, 56, TimeSpan.Zero);
        var history = new FileGreetingHistory(HistoryPath, new FixedTimeProvider(time));

        history.Append("Hannes", "Hello, Hannes!");
        history.Append("World", "Hello, World!");

        var lines = File.ReadAllLines(HistoryPath);
        Assert.Equal(2, lines.Length);
        Assert.Equal(
            """{"time":"2026-09-05T12:34:56+00:00","name":"Hannes","text":"Hello, Hannes!"}""",
            lines[0]);
        Assert.Equal(
            """{"time":"2026-09-05T12:34:56+00:00","name":"World","text":"Hello, World!"}""",
            lines[1]);
    }

    [Fact]
    public void AppendedRecordsReadBackByAFreshFileGreetingHistoryMatchWhatWasWritten()
    {
        var time = new DateTimeOffset(2026, 9, 5, 12, 34, 56, TimeSpan.Zero);
        var writer = new FileGreetingHistory(HistoryPath, new FixedTimeProvider(time));
        writer.Append("Hannes", "Hello, Hannes!");

        var reader = new FileGreetingHistory(HistoryPath);
        var records = reader.Read();

        var record = Assert.Single(records);
        Assert.Equal(time, record.Time);
        Assert.Equal("Hannes", record.Name);
        Assert.Equal("Hello, Hannes!", record.Text);
    }

    [Fact]
    public void ABlankOrHalfWrittenTrailingLineIsSkippedAndTheRemainingRecordsStillReadBack()
    {
        Directory.CreateDirectory(_directory);
        File.WriteAllText(
            HistoryPath,
            """
            {"time":"2026-09-05T12:34:56+00:00","name":"Hannes","text":"Hello, Hannes!"}

            {"time":"2026-09-05T12:35:0
            """);

        var history = new FileGreetingHistory(HistoryPath);
        var records = history.Read();

        var record = Assert.Single(records);
        Assert.Equal("Hannes", record.Name);
        Assert.Equal("Hello, Hannes!", record.Text);
    }

    [Fact]
    public void SeveralAppendsReadBackOldestFirst()
    {
        var history = new FileGreetingHistory(
            HistoryPath,
            new FixedTimeProvider(new DateTimeOffset(2026, 9, 5, 12, 0, 0, TimeSpan.Zero)));

        history.Append("First", "Hello, First!");
        history.Append("Second", "Hello, Second!");
        history.Append("Third", "Hello, Third!");

        var records = history.Read();

        Assert.Equal(["First", "Second", "Third"], records.Select(r => r.Name));
    }

    [Fact]
    public void AppendingToAPathWhoseParentDirectoryDoesNotExistCreatesTheDirectory()
    {
        var nestedPath = Path.Combine(_directory, "nested", "sub", "history.jsonl");
        var history = new FileGreetingHistory(nestedPath);

        history.Append("Hannes", "Hello, Hannes!");

        Assert.True(File.Exists(nestedPath));
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }

    private sealed class FixedTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }
}
