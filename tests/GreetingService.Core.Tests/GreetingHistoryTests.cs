using System.Text;

namespace GreetingService.Core.Tests;

public class GreetingHistoryTests
{
    [Fact]
    public void ResolvePathReturnsTheConfiguredPathWhenGiven()
    {
        Assert.Equal(@"C:\configured\history.jsonl", GreetingHistory.ResolvePath(@"C:\configured\history.jsonl"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ResolvePathFallsBackToLocalApplicationDataWhenConfiguredPathIsNullEmptyOrWhitespace(string? configuredPath)
    {
        var expected = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "GreetingService",
            "history.jsonl");

        Assert.Equal(expected, GreetingHistory.ResolvePath(configuredPath));
    }

    [Fact]
    public void ToLineProducesTheGoldenFixtureLine()
    {
        var entry = new GreetingHistoryEntry("Hello, World!", DateTimeOffset.Parse("2026-09-04T12:34:56.7891234+00:00"));

        Assert.Equal(
            "{\"text\":\"Hello, World!\",\"printedAt\":\"2026-09-04T12:34:56.7891234+00:00\"}",
            GreetingHistory.ToLine(entry));
    }

    [Fact]
    public void ToLineSerialisesAWholeSecondPrintTimeWithoutTrailingFractionalZeros()
    {
        var entry = new GreetingHistoryEntry("Hi", DateTimeOffset.Parse("2026-09-04T12:34:56+00:00"));

        var line = GreetingHistory.ToLine(entry);

        Assert.Equal("{\"text\":\"Hi\",\"printedAt\":\"2026-09-04T12:34:56+00:00\"}", line);
        Assert.DoesNotContain('.', line);
    }

    [Fact]
    public void ToLineNormalisesANonUtcOffsetWithoutShiftingTheInstant()
    {
        var entry = new GreetingHistoryEntry("Hi", DateTimeOffset.Parse("2026-09-04T14:34:56+02:00"));

        var line = GreetingHistory.ToLine(entry);

        Assert.Equal("{\"text\":\"Hi\",\"printedAt\":\"2026-09-04T12:34:56+00:00\"}", line);
    }

    [Fact]
    public void RecordAppendsTheGoldenLinePlusNewlineWithoutABom()
    {
        var directory = Path.Combine(Path.GetTempPath(), "GreetingHistoryTests_" + Guid.NewGuid());
        var path = Path.Combine(directory, "history.jsonl");

        try
        {
            var entry = new GreetingHistoryEntry("Hello, World!", DateTimeOffset.Parse("2026-09-04T12:34:56.7891234+00:00"));
            GreetingHistory.Record(path, entry);

            var bytes = File.ReadAllBytes(path);
            var bom = new UTF8Encoding(true).GetPreamble();
            Assert.False(bytes.Length >= bom.Length && bytes.AsSpan(0, bom.Length).SequenceEqual(bom));

            var text = File.ReadAllText(path, new UTF8Encoding(false));
            Assert.Equal(
                "{\"text\":\"Hello, World!\",\"printedAt\":\"2026-09-04T12:34:56.7891234+00:00\"}\n",
                text);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void RecordSwallowsFailuresWhenThePathIsUnwritable()
    {
        var directory = Path.Combine(Path.GetTempPath(), "GreetingHistoryTests_" + Guid.NewGuid());

        try
        {
            Directory.CreateDirectory(directory);
            var filePath = Path.Combine(directory, "not-a-directory");
            File.WriteAllText(filePath, "occupied");
            var unwritablePath = Path.Combine(filePath, "history.jsonl");

            var exception = Capture(() => GreetingHistory.Record(unwritablePath, new GreetingHistoryEntry("Hi", DateTimeOffset.UtcNow)));

            Assert.Null(exception);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static Exception? Capture(Action action)
    {
        try
        {
            action();
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}
