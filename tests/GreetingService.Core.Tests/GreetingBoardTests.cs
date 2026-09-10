namespace GreetingService.Core.Tests;

public class GreetingBoardTests
{
    [Fact]
    public void PrintsNameAndGreetingHeadersAboveTheRowsWideningTheNameColumnWhenNeeded()
    {
        var expected = string.Join(Environment.NewLine,
            "Name   Greeting",
            "Al     Hello, Al!",
            "Maria  Hello, Maria!");

        Assert.Equal(expected, GreetingBoard.For(["Al", "Maria"]));
    }

    [Fact]
    public void PrintsOnlyTheHeaderRowWhenNoNamesAreGiven()
    {
        Assert.Equal("Name  Greeting", GreetingBoard.For([]));
    }
}
