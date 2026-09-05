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

    [Fact]
    public void RendersAnExplicitTemplateWithTheGivenName()
    {
        Assert.Equal("Hi, Hannes!!", Greeting.For("Hannes", "Hi, {name}!!"));
    }

    [Fact]
    public void RendersAnExplicitTemplateWithTheDefaultNameWhenNoneIsGiven()
    {
        Assert.Equal("Hi, World!!", Greeting.For(null, "Hi, {name}!!"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void FallsBackToTheDefaultTemplateWhenTemplateIsNullOrWhitespace(string? template)
    {
        Assert.Equal("Hello, World!", Greeting.For(null, template));
        Assert.Equal("Hello, Hannes!", Greeting.For("Hannes", template));
    }

    [Fact]
    public void ReturnsATemplateWithoutThePlaceholderUnchanged()
    {
        Assert.Equal("Welcome aboard", Greeting.For("Hannes", "Welcome aboard"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void ResolveNameFallsBackToTheDefaultNameWhenNullOrWhitespace(string? name)
    {
        Assert.Equal("World", Greeting.ResolveName(name));
    }

    [Fact]
    public void ResolveNameTrimsSurroundingWhitespace()
    {
        Assert.Equal("Hannes", Greeting.ResolveName("  Hannes  "));
    }

    [Fact]
    public void ResolveNameReturnsANameThatNeedsNoResolutionUnchanged()
    {
        Assert.Equal("Hannes", Greeting.ResolveName("Hannes"));
    }
}
