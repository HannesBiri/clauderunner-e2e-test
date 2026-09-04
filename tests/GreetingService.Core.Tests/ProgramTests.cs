using System.Text.Json;
using GreetingService.Cli;
using GreetingService.Core;

namespace GreetingService.Core.Tests;

public class ProgramTests
{
    [Fact]
    public void GreetsTheWorldWhenNoArgsAreGiven()
    {
        Assert.Equal("Hello, World!", Program.Compose([]));
    }

    [Fact]
    public void GreetsTheNameGivenAsTheFirstArgument()
    {
        Assert.Equal("Hello, Hannes!", Program.Compose(["Hannes"]));
    }

    [Fact]
    public void TrimsTheNameGivenAsTheFirstArgument()
    {
        Assert.Equal("Hello, Hannes!", Program.Compose(["  Hannes  "]));
    }

    [Fact]
    public void IgnoresExtraArguments()
    {
        Assert.Equal("Hello, Hannes!", Program.Compose(["Hannes", "extra"]));
    }

    [Fact]
    public void UsesTheGreetingOptionBeforeTheName()
    {
        Assert.Equal("Hi, Hannes!!", Program.Compose(["--greeting", "Hi, {name}!!", "Hannes"], null));
    }

    [Fact]
    public void UsesTheGreetingOptionAfterTheName()
    {
        Assert.Equal("Hi, Hannes!!", Program.Compose(["Hannes", "--greeting", "Hi, {name}!!"], null));
    }

    [Fact]
    public void TreatsAGreetingOptionWithNoFollowingValueAsUnset()
    {
        Assert.Equal("Hello, Hannes!", Program.Compose(["Hannes", "--greeting"], null));
    }

    [Fact]
    public void UsesTheEnvironmentTemplateWhenNoGreetingOptionIsGiven()
    {
        Assert.Equal("Hi, Hannes!!", Program.Compose(["Hannes"], "Hi, {name}!!"));
    }

    [Fact]
    public void PrefersTheGreetingOptionOverTheEnvironmentTemplate()
    {
        Assert.Equal(
            "Hi, Hannes!!",
            Program.Compose(["Hannes", "--greeting", "Hi, {name}!!"], "Yo, {name}!!!"));
    }

    [Fact]
    public void RunRecordsExactlyOneHistoryEntryForTheFirstGreeting()
    {
        var directory = Path.Combine(Path.GetTempPath(), "ProgramTests_" + Guid.NewGuid());
        var path = Path.Combine(directory, "history.jsonl");

        try
        {
            Assert.False(File.Exists(path));

            var printedAt = DateTimeOffset.Parse("2026-09-04T12:34:56+00:00");
            var greeting = Program.Run(["Hannes"], null, path, printedAt);

            Assert.Equal("Hello, Hannes!", greeting);

            var line = Assert.Single(File.ReadAllLines(path));
            var entry = JsonSerializer.Deserialize<GreetingHistoryEntry>(line);
            Assert.Equal(greeting, entry!.Text);
            Assert.Equal(printedAt, entry.PrintedAt);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void RunRecordsOneEntryPerCallIncludingRepeatedGreetingText()
    {
        var directory = Path.Combine(Path.GetTempPath(), "ProgramTests_" + Guid.NewGuid());
        var path = Path.Combine(directory, "history.jsonl");

        try
        {
            var first = DateTimeOffset.Parse("2026-09-04T12:00:00+00:00");
            var second = DateTimeOffset.Parse("2026-09-04T12:00:01+00:00");
            var third = DateTimeOffset.Parse("2026-09-04T12:00:02+00:00");

            Program.Run([], null, path, first);
            Program.Run([], null, path, second);
            Program.Run([], null, path, third);

            var lines = File.ReadAllLines(path);
            Assert.Equal(3, lines.Length);

            var entries = lines.Select(line => JsonSerializer.Deserialize<GreetingHistoryEntry>(line)!).ToArray();
            Assert.All(entries, e => Assert.Equal("Hello, World!", e.Text));
            Assert.Equal([first, second, third], entries.Select(e => e.PrintedAt));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void RunReturnsTheGreetingAndDoesNotThrowWhenTheHistoryPathIsUnwritable()
    {
        var directory = Path.Combine(Path.GetTempPath(), "ProgramTests_" + Guid.NewGuid());

        try
        {
            Directory.CreateDirectory(directory);
            var filePath = Path.Combine(directory, "not-a-directory");
            File.WriteAllText(filePath, "occupied");
            var unwritablePath = Path.Combine(filePath, "history.jsonl");

            var greeting = Program.Run(["Hannes"], null, unwritablePath, DateTimeOffset.UtcNow);

            Assert.Equal("Hello, Hannes!", greeting);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public void RunRecordsTheDefaultGreetingWhenNoNameArgumentIsSupplied()
    {
        var directory = Path.Combine(Path.GetTempPath(), "ProgramTests_" + Guid.NewGuid());
        var path = Path.Combine(directory, "history.jsonl");

        try
        {
            var printedAt = DateTimeOffset.Parse("2026-09-04T12:34:56+00:00");
            var greeting = Program.Run([], null, path, printedAt);

            Assert.Equal("Hello, World!", greeting);

            var line = Assert.Single(File.ReadAllLines(path));
            var entry = JsonSerializer.Deserialize<GreetingHistoryEntry>(line);
            Assert.Equal("Hello, World!", entry!.Text);
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }
}
