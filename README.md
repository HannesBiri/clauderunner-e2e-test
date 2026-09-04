# GreetingService

Prints a greeting and records it to an append-only history file, one entry per printed greeting.

## History file

**Path**: the `GREETING_HISTORY_PATH` environment variable when set and non-whitespace; otherwise
`<LocalApplicationData>/GreetingService/history.jsonl`, where `<LocalApplicationData>` is
`Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)`. The containing
directory is created if it does not exist. Recording failures (an unwritable path, a locked file)
are swallowed - the greeting is still printed.

**Format**: JSON Lines, UTF-8 without a byte-order mark. Each line is one `System.Text.Json` object
with exactly two properties:

- `text` - the greeting string exactly as printed.
- `printedAt` - the print time as a `DateTimeOffset`, normalised to UTC, in the default ISO-8601
  round-trip form.

Example line:

```
{"text":"Hello, World!","printedAt":"2026-09-04T12:34:56.7891234+00:00"}
```

`System.Text.Json` trims trailing zeros from the fractional seconds, so a whole-second value
serialises as `2026-09-04T12:34:56+00:00`; both forms occur.
