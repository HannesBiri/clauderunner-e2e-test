namespace GreetingService.Core.Tests;

/// <summary>
/// Seeded deliberately red for the ClaudeRunner end-to-end test, 2026-09-11. ADR 0021 says a gate that was
/// already red parks the run before it pays for a fix session, and that park is the case under test.
///
/// It asserts a greeting style this repository does not use and no ticket asks for, so that no ticket's own
/// work can accidentally repair it — which is exactly what happened to the first seed. The red one in
/// Greeting.Tools (ColumnWidthTests) was "TextTable pads nothing", and the ticket under test was "make
/// TextTable pad", so the implementation turned it green and the run never parked.
///
/// Delete this file to end the park case.
/// </summary>
public class SeededRedTests
{
    [Fact]
    public void GreetsWithAnExclamationlessSalutation()
    {
        Assert.Equal("Greetings, World", Greeting.For(null));
    }
}
