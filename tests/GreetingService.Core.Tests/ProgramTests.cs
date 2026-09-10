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
    public void IgnoresExtraArguments()
    {
        Assert.Equal("Hello, Hannes!", Program.Compose(["Hannes", "extra"]).Greeting);
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

    [Fact]
    public void ComposeAllGreetsEachLineInFileOrder()
    {
        Func<string, string[]> readLines = _ => ["Hannes", "Ilse", "Werner"];

        var results = Program.ComposeAll(["--file", "names.txt"], null, readLines);

        Assert.Equal(["Hello, Hannes!", "Hello, Ilse!", "Hello, Werner!"], results.Select(r => r.Greeting));
    }

    [Fact]
    public void ComposeAllReturnsAnEmptyListForAnEmptyFile()
    {
        Func<string, string[]> readLines = _ => [];

        var results = Program.ComposeAll(["--file", "names.txt"], null, readLines);

        Assert.Empty(results);
    }

    [Fact]
    public void ComposeAllGreetsABlankLineAsTheDefaultWorldGreeting()
    {
        Func<string, string[]> readLines = _ => ["Hannes", "", "Werner"];

        var results = Program.ComposeAll(["--file", "names.txt"], null, readLines);

        Assert.Equal(["Hello, Hannes!", "Hello, World!", "Hello, Werner!"], results.Select(r => r.Greeting));
    }

    [Fact]
    public void ComposeAllReturnsAResultWithAnErrorForAnInvalidLine()
    {
        Func<string, string[]> readLines = _ => ["Hannes", "!!!"];

        var results = Program.ComposeAll(["--file", "names.txt"], null, readLines);

        Assert.Equal(2, results.Count);
        Assert.Equal("greet: a name must contain at least one letter or digit", results[1].Error);
    }

    [Fact]
    public void ComposeAllReturnsASingleErrorResultWhenTheFileCannotBeRead()
    {
        Func<string, string[]> readLines = _ => throw new FileNotFoundException();

        var results = Program.ComposeAll(["--file", "names.txt"], null, readLines);

        Assert.Single(results);
        Assert.Equal("greet: cannot read names file 'names.txt'", results[0].Error);
    }

    [Fact]
    public void ComposeAllAppliesTheGreetingOptionToEveryLine()
    {
        Func<string, string[]> readLines = _ => ["Hannes", "Ilse"];

        var results = Program.ComposeAll(["--file", "names.txt", "--greeting", "Hi, {name}!!"], null, readLines);

        Assert.Equal(["Hi, Hannes!!", "Hi, Ilse!!"], results.Select(r => r.Greeting));
    }

    [Fact]
    public void ComposeAllUsesTheEnvironmentTemplateWhenNoGreetingOptionIsGiven()
    {
        Func<string, string[]> readLines = _ => ["Hannes", "Ilse"];

        var results = Program.ComposeAll(["--file", "names.txt"], "Hi, {name}!!", readLines);

        Assert.Equal(["Hi, Hannes!!", "Hi, Ilse!!"], results.Select(r => r.Greeting));
    }

    [Fact]
    public void RunGreetsEveryNameInTheFileOnePerLineWithExitCode0()
    {
        var output = new StringWriter();
        var error = new StringWriter();
        Func<string, string[]> readLines = _ => ["Hannes", "Ilse", "Werner"];

        var exitCode = Program.Run(["--file", "names.txt"], null, output, error, readLines);

        Assert.Equal(0, exitCode);
        Assert.Equal(
            "Hello, Hannes!" + Environment.NewLine + "Hello, Ilse!" + Environment.NewLine + "Hello, Werner!" + Environment.NewLine,
            output.ToString());
        Assert.Equal("", error.ToString());
    }

    [Fact]
    public void RunProducesNoOutputAndExitCode0ForAnEmptyFile()
    {
        var output = new StringWriter();
        var error = new StringWriter();
        Func<string, string[]> readLines = _ => [];

        var exitCode = Program.Run(["--file", "names.txt"], null, output, error, readLines);

        Assert.Equal(0, exitCode);
        Assert.Equal("", output.ToString());
        Assert.Equal("", error.ToString());
    }

    [Fact]
    public void RunGreetsABlankLineInTheFileAsTheDefaultGreeting()
    {
        var output = new StringWriter();
        var error = new StringWriter();
        Func<string, string[]> readLines = _ => ["Hannes", ""];

        var exitCode = Program.Run(["--file", "names.txt"], null, output, error, readLines);

        Assert.Equal(0, exitCode);
        Assert.Equal("Hello, Hannes!" + Environment.NewLine + "Hello, World!" + Environment.NewLine, output.ToString());
        Assert.Equal("", error.ToString());
    }

    [Fact]
    public void RunRejectsAFileContainingAnInvalidNameWithExitCode2AndNoStandardOutput()
    {
        var output = new StringWriter();
        var error = new StringWriter();
        Func<string, string[]> readLines = _ => ["Hannes", "!!!", "Werner"];

        var exitCode = Program.Run(["--file", "names.txt"], null, output, error, readLines);

        Assert.Equal(2, exitCode);
        Assert.Equal("greet: a name must contain at least one letter or digit" + Environment.NewLine, error.ToString());
        Assert.Equal("", output.ToString());
    }

    [Fact]
    public void RunReportsAnUnreadableFileDistinctlyWithExitCode2AndNoStandardOutput()
    {
        var output = new StringWriter();
        var error = new StringWriter();
        Func<string, string[]> readLines = _ => throw new FileNotFoundException();

        var exitCode = Program.Run(["--file", "missing.txt"], null, output, error, readLines);

        Assert.Equal(2, exitCode);
        Assert.Equal("greet: cannot read names file 'missing.txt'" + Environment.NewLine, error.ToString());
        Assert.Equal("", output.ToString());
    }

    [Fact]
    public void RunPrefersTheFileOverAPositionalNameWhenBothAreGiven()
    {
        var output = new StringWriter();
        var error = new StringWriter();
        Func<string, string[]> readLines = _ => ["Ilse"];

        var exitCode = Program.Run(["Hannes", "--file", "names.txt"], null, output, error, readLines);

        Assert.Equal(0, exitCode);
        Assert.Equal("Hello, Ilse!" + Environment.NewLine, output.ToString());
        Assert.Equal("", error.ToString());
    }

    [Fact]
    public void RunTreatsAFileOptionWithNoFollowingValueAsUnset()
    {
        var output = new StringWriter();
        var error = new StringWriter();
        Func<string, string[]> readLines = _ => throw new InvalidOperationException("should not be called");

        var exitCode = Program.Run(["Hannes", "--file"], null, output, error, readLines);

        Assert.Equal(0, exitCode);
        Assert.Equal("Hello, Hannes!" + Environment.NewLine, output.ToString());
        Assert.Equal("", error.ToString());
    }
}
