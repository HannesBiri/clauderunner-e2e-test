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
}
