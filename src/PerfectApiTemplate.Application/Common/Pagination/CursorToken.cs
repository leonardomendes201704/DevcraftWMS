namespace PerfectApiTemplate.Application.Common.Pagination;

public sealed record CursorToken(IReadOnlyList<string> Parts)
{
    public static bool TryParse(string? cursor, out CursorToken token)
    {
        token = new CursorToken(Array.Empty<string>());

        if (string.IsNullOrWhiteSpace(cursor))
        {
            return false;
        }

        var parts = cursor.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
        {
            return false;
        }

        token = new CursorToken(parts);
        return true;
    }

    public static string Build(params object[] parts)
    {
        if (parts.Length == 0)
        {
            throw new ArgumentException("Cursor parts cannot be empty.", nameof(parts));
        }

        return string.Join('|', parts.Select(p => p?.ToString() ?? string.Empty));
    }
}
