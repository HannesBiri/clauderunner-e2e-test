namespace GreetingService.Core.Tests;

public class GreetingBoardTests
{
    [Fact]
    public void RendersTheHeaderFollowedByANameAndGreetingRowPerPerson()
    {
        var expected = string.Join(Environment.NewLine,
            "Name    Greeting",
            "Hannes  Hello, Hannes!");

        Assert.Equal(expected, GreetingBoard.For(["Hannes"]));
    }

    [Fact]
    public void RendersOnlyTheHeaderWhenThereAreNoNames()
    {
        Assert.Equal("Name  Greeting", GreetingBoard.For([]));
    }

    [Fact]
    public void WidensTheHeaderToFitANameLongerThanIt()
    {
        var expected = string.Join(Environment.NewLine,
            "Name".PadRight("Chrysanthemum".Length) + "  Greeting",
            "Chrysanthemum  Hello, Chrysanthemum!");

        Assert.Equal(expected, GreetingBoard.For(["Chrysanthemum"]));
    }
}
