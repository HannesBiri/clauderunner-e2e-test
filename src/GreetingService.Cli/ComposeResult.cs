namespace GreetingService.Cli;

/// <summary>The outcome of <see cref="Program.Compose(string[])"/>: exactly one of <see cref="Greeting"/> or <see cref="Error"/> is non-null.</summary>
public readonly record struct ComposeResult(string? Greeting, string? Error);
