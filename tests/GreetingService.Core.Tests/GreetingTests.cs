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

    [Theory]
    [InlineData("Hi, {name}!!", "Hi, Hannes!!")]
    [InlineData("Hi, {Name}!!", "Hi, Hannes!!")]
    [InlineData("HI, {NAME}!!", "HI, Hannes!!")]
    [InlineData("Hi, {nAmE}!!", "Hi, Hannes!!")]
    public void RendersAnExplicitTemplateWithTheGivenName(string template, string expected)
    {
        Assert.Equal(expected, Greeting.For("Hannes", template));
    }

    [Fact]
    public void KeepsTheCallerSCasingOfTheSubstitutedName()
    {
        Assert.Equal("Hi, McDonald!!", Greeting.For("McDonald", "Hi, {NAME}!!"));
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
}
