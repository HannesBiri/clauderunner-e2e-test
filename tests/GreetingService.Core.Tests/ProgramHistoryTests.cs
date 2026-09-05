using GreetingService.Cli;

namespace GreetingService.Core.Tests;

public class ProgramHistoryTests
{
    [Fact]
    public void HistoryWithNothingRecordedGivesAnEmptyString()
    {
        var history = new RecordingGreetingHistory();

        Assert.Equal(string.Empty, Program.Compose(["--history"], null, history));
    }

    [Fact]
    public void AHistoryRunRecordsNothing()
    {
        var history = new RecordingGreetingHistory(
            new GreetingRecord(DateTimeOffset.UnixEpoch, "Hannes", "Hello, Hannes!"));

        Program.Compose(["--history"], null, history);

        Assert.Empty(history.Appended);
    }

    [Fact]
    public void HistoryWithRecordsPrintsTheirTextOnePerLineOldestFirst()
    {
        var history = new RecordingGreetingHistory(
            new GreetingRecord(DateTimeOffset.UnixEpoch, "Hannes", "Hello, Hannes!"),
            new GreetingRecord(DateTimeOffset.UnixEpoch.AddMinutes(1), "World", "Hello, World!"));

        var result = Program.Compose(["--history"], null, history);

        Assert.Equal($"Hello, Hannes!{Environment.NewLine}Hello, World!", result);
    }

    [Fact]
    public void AGreetingRunAppendsExactlyOneRecordCarryingTheResolvedNameAndTheComposedText()
    {
        var history = new RecordingGreetingHistory();

        var result = Program.Compose(["Hannes"], null, history);

        Assert.Equal("Hello, Hannes!", result);
        var appended = Assert.Single(history.Appended);
        Assert.Equal("Hannes", appended.Name);
        Assert.Equal("Hello, Hannes!", appended.Text);
    }

    [Fact]
    public void AGreetingRunWithNoNameGivenAppendsARecordForWorld()
    {
        var history = new RecordingGreetingHistory();

        var result = Program.Compose([], null, history);

        Assert.Equal("Hello, World!", result);
        var appended = Assert.Single(history.Appended);
        Assert.Equal("World", appended.Name);
        Assert.Equal("Hello, World!", appended.Text);
    }

    [Fact]
    public void TableAndHistoryFlagsBeforeANameAreNotConsumedAsTheName()
    {
        var history = new RecordingGreetingHistory();

        Assert.Equal("Hello, Hannes!", Program.Compose(["--table", "Hannes"], null, history));
    }

    [Fact]
    public void TableWithoutHistoryGreetsNormally()
    {
        var history = new RecordingGreetingHistory();

        Assert.Equal("Hello, Hannes!", Program.Compose(["Hannes", "--table"], null, history));
    }

    [Fact]
    public void TheTwoArgumentOverloadRecordsNothing()
    {
        Assert.Equal("Hello, Hannes!", Program.Compose(["Hannes"], null));
    }

    private sealed class RecordingGreetingHistory : IGreetingHistory
    {
        private readonly List<GreetingRecord> _records;

        public RecordingGreetingHistory(params GreetingRecord[] records)
        {
            _records = [.. records];
        }

        public List<GreetingRecord> Appended { get; } = [];

        public IReadOnlyList<GreetingRecord> Read() => _records;

        public void Append(string name, string text) => Appended.Add(new GreetingRecord(default, name, text));
    }
}
