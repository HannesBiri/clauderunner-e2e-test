using GreetingService.Core;

namespace GreetingService.Core.Tests;

public class GreetingTests
{
    [Fact]
    public void GreetsTheWorldWhenNoNameIsGiven()
    {
        Assert.Equal("Hello, World!", Greeting.For(null));
    }

    [Theory]
    [InlineData("Hannes", "Hello, Hannes!")]
    [InlineData("  Hannes  ", "Hello, Hannes!")]
    public void GreetsTheNameItIsGiven(string name, string expected)
    {
        Assert.Equal(expected, Greeting.For(name));
    }
}
