using GreetingService.Cli;

namespace GreetingService.Core.Tests;

public class ProgramTests
{
    [Fact]
    public void GreetsTheWorldWhenNoArgsAreGiven()
    {
        Assert.Equal("Hello, World!", Program.Compose([]).Greeting);
    }

    [Fact]
    public void GreetsTheNameGivenAsTheFirstArgument()
    {
        Assert.Equal("Hello, Hannes!", Program.Compose(["Hannes"]).Greeting);
    }

    [Fact]
    public void TrimsTheNameGivenAsTheFirstArgument()
    {
        Assert.Equal("Hello, Hannes!", Program.Compose(["  Hannes  "]).Greeting);
    }

    [Fact]
    public void TreatsEveryArgumentAsAName()
    {
        Assert.Equal(
            "Hello, Hannes!" + Environment.NewLine + "Hello, extra!",
            Program.Compose(["Hannes", "extra"]).Greeting);
    }

    [Fact]
    public void GreetsARepeatedNameOnceAtItsFirstPosition()
    {
        Assert.Equal(
            "Hello, Ada!" + Environment.NewLine + "Hello, Grace!",
            Program.Compose(["Ada", "Grace", "Ada"]).Greeting);
    }

    [Fact]
    public void GreetsNamesDifferingOnlyByCaseSeparately()
    {
        Assert.Equal(
            "Hello, Ada!" + Environment.NewLine + "Hello, ada!",
            Program.Compose(["Ada", "ada"]).Greeting);
    }

    [Fact]
    public void CollapsesARepeatThatDiffersOnlyByWhitespaceOntoTheEarlierOccurrence()
    {
        Assert.Equal(
            "Hello, Ada!" + Environment.NewLine + "Hello, Grace!",
            Program.Compose(["Ada", "Grace", "  Ada  "]).Greeting);
    }

    [Fact]
    public void DiscardsWhitespaceOnlyArgumentsAmongSeveralNames()
    {
        Assert.Equal(
            "Hello, Ada!" + Environment.NewLine + "Hello, Grace!",
            Program.Compose(["Ada", "   ", "Grace"]).Greeting);
    }

    [Fact]
    public void FailsTheWholeRunWhenAnyNameFailsValidation()
    {
        var result = Program.Compose(["Ada", "!!!"]);

        Assert.Null(result.Greeting);
        Assert.Equal("greet: a name must contain at least one letter or digit", result.Error);
    }

    [Fact]
    public void RunFailsTheWholeRunWhenAnyNameFailsValidation()
    {
        var output = new StringWriter();
        var error = new StringWriter();

        var exitCode = Program.Run(["Ada", "!!!"], null, output, error);

        Assert.Equal(2, exitCode);
        Assert.Equal("greet: a name must contain at least one letter or digit" + Environment.NewLine, error.ToString());
        Assert.Equal("", output.ToString());
    }

    [Fact]
    public void UsesTheGreetingOptionBeforeTheName()
    {
        Assert.Equal("Hi, Hannes!!", Program.Compose(["--greeting", "Hi, {name}!!", "Hannes"], null).Greeting);
    }

    [Fact]
    public void UsesTheGreetingOptionAfterTheName()
    {
        Assert.Equal("Hi, Hannes!!", Program.Compose(["Hannes", "--greeting", "Hi, {name}!!"], null).Greeting);
    }

    [Fact]
    public void TreatsAGreetingOptionWithNoFollowingValueAsUnset()
    {
        Assert.Equal("Hello, Hannes!", Program.Compose(["Hannes", "--greeting"], null).Greeting);
    }

    [Fact]
    public void UsesTheEnvironmentTemplateWhenNoGreetingOptionIsGiven()
    {
        Assert.Equal("Hi, Hannes!!", Program.Compose(["Hannes"], "Hi, {name}!!").Greeting);
    }

    [Fact]
    public void PrefersTheGreetingOptionOverTheEnvironmentTemplate()
    {
        Assert.Equal(
            "Hi, Hannes!!",
            Program.Compose(["Hannes", "--greeting", "Hi, {name}!!"], "Yo, {name}!!!").Greeting);
    }

    [Fact]
    public void FallsBackToWorldWhenNameIsEmpty()
    {
        Assert.Equal("Hello, World!", Program.Compose([""]).Greeting);
    }

    [Fact]
    public void FallsBackToWorldWhenNameIsWhitespaceOnly()
    {
        Assert.Equal("Hello, World!", Program.Compose(["   "]).Greeting);
    }

    [Fact]
    public void RejectsANameThatIsOnlyPunctuationOrSymbols()
    {
        var result = Program.Compose(["!!!"]);

        Assert.Null(result.Greeting);
        Assert.Equal("greet: a name must contain at least one letter or digit", result.Error);
    }

    [Fact]
    public void AcceptsATrimmedNameOfExactly64Characters()
    {
        var name = new string('a', 64);

        Assert.Equal($"Hello, {name}!", Program.Compose([name]).Greeting);
    }

    [Fact]
    public void RejectsATrimmedNameOf65Characters()
    {
        var result = Program.Compose([new string('a', 65)]);

        Assert.Null(result.Greeting);
        Assert.Equal("greet: a name must be 64 characters or fewer", result.Error);
    }

    [Fact]
    public void JudgesAnOverLengthNameOnItsTrimmedValue()
    {
        var name = " " + new string('a', 65) + " ";

        var result = Program.Compose([name]);

        Assert.Null(result.Greeting);
        Assert.Equal("greet: a name must be 64 characters or fewer", result.Error);
    }

    [Fact]
    public void AcceptsAPaddedNameThatFitsTheLimitOnceTrimmed()
    {
        var trimmed = new string('a', 64);
        var name = "  " + trimmed + "  ";

        Assert.Equal($"Hello, {trimmed}!", Program.Compose([name]).Greeting);
    }

    [Fact]
    public void JudgesAPunctuationOnlyNameOnItsTrimmedValue()
    {
        var result = Program.Compose(["  !!!  "]);

        Assert.Null(result.Greeting);
        Assert.Equal("greet: a name must contain at least one letter or digit", result.Error);
    }

    [Fact]
    public void RunGreetsTheWorldAndReturnsExitCode0WhenNameIsAbsent()
    {
        var output = new StringWriter();
        var error = new StringWriter();

        var exitCode = Program.Run([], null, output, error);

        Assert.Equal(0, exitCode);
        Assert.Equal("Hello, World!" + Environment.NewLine, output.ToString());
        Assert.Equal("", error.ToString());
    }

    [Fact]
    public void RunGreetsTheNameAndReturnsExitCode0ForAnAcceptedName()
    {
        var output = new StringWriter();
        var error = new StringWriter();

        var exitCode = Program.Run(["Hannes"], null, output, error);

        Assert.Equal(0, exitCode);
        Assert.Equal("Hello, Hannes!" + Environment.NewLine, output.ToString());
        Assert.Equal("", error.ToString());
    }

    [Fact]
    public void RunRejectsAPunctuationOnlyNameWithExitCode2AndNoStandardOutput()
    {
        var output = new StringWriter();
        var error = new StringWriter();

        var exitCode = Program.Run(["!!!"], null, output, error);

        Assert.Equal(2, exitCode);
        Assert.Equal("greet: a name must contain at least one letter or digit" + Environment.NewLine, error.ToString());
        Assert.Equal("", output.ToString());
    }

    [Fact]
    public void RunRejectsATooLongNameWithExitCode2AndNoStandardOutput()
    {
        var output = new StringWriter();
        var error = new StringWriter();

        var exitCode = Program.Run([new string('a', 65)], null, output, error);

        Assert.Equal(2, exitCode);
        Assert.Equal("greet: a name must be 64 characters or fewer" + Environment.NewLine, error.ToString());
        Assert.Equal("", output.ToString());
    }
}
