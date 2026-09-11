namespace GreetingService.Core.Tests;

public class GreetingBoardTests
{
    [Fact]
    public void RendersTheColumnHeadersAboveTheRowsAndWidensTheNameColumnToTheHeaderWhenEveryNameIsShorter()
    {
        var expected = "Name  Greeting" + Environment.NewLine + "Al    Hello, Al!";

        Assert.Equal(expected, GreetingBoard.For(["Al"]));
    }

    [Fact]
    public void RendersOnlyTheHeadersWhenNoNamesAreGiven()
    {
        Assert.Equal("Name  Greeting", GreetingBoard.For([]));
    }
}
