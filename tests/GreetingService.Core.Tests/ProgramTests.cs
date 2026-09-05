using System.Text.RegularExpressions;
using GreetingService.Cli;

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
    public void ReportsItsVersion()
    {
        Assert.Equal(Program.Version, Program.Compose(["--version"], null));
    }

    [Fact]
    public void PrefersTheVersionOptionOverAName()
    {
        Assert.Equal(Program.Version, Program.Compose(["Hannes", "--version"], null));
    }

    [Fact]
    public void PrefersTheVersionOptionOverTheGreetingOption()
    {
        Assert.Equal(Program.Version, Program.Compose(["--version", "--greeting", "Hi, {name}!!"], null));
    }

    [Fact]
    public void TreatsTheVersionOptionAsTheGreetingValueWhenItFollowsTheGreetingOption()
    {
        Assert.Equal("--version", Program.Compose(["Hannes", "--greeting", "--version"], null));
    }

    [Fact]
    public void HasAVersionMatchingSemanticVersioningFormat()
    {
        Assert.Matches(new Regex(@"^\d+\.\d+\.\d+$"), Program.Version);
    }
}
