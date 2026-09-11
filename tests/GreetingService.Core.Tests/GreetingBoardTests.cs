namespace GreetingService.Core.Tests;

public class GreetingBoardTests
{
    [Fact]
    public void RendersNameAndGreetingHeadersAboveOneRowPerName()
    {
        Assert.Equal(
            "Name  Greeting\r\nAnn   Hello, Ann!\r\nBo    Hello, Bo!",
            GreetingBoard.For(["Ann", "Bo"]));
    }

    [Fact]
    public void WidensTheNameColumnOnTheHeaderLineWhenANameIsLongerThanTheHeader()
    {
        Assert.Equal(
            "Name       Greeting\r\nChristina  Hello, Christina!",
            GreetingBoard.For(["Christina"]));
    }

    [Fact]
    public void RendersOnlyTheHeaderLineForAnEmptyListOfNames()
    {
        Assert.Equal("Name  Greeting", GreetingBoard.For([]));
    }
}
