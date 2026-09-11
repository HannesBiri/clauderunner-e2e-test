namespace GreetingService.Cli;

/// <summary>The outcome of <see cref="Program.ComposeMany"/>: <see cref="Greetings"/> is empty whenever <see cref="Error"/> is non-null.</summary>
public readonly record struct ComposeManyResult(IReadOnlyList<string> Greetings, string? Error);
