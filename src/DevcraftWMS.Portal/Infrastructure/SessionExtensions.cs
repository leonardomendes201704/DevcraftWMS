using System.Text.Json;

namespace DevcraftWMS.Portal.Infrastructure;

public static class SessionExtensions
{
    public static void SetStringValue(this ISession session, string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            session.Remove(key);
            return;
        }

        session.SetString(key, value);
    }

    public static string? GetStringValue(this ISession session, string key)
        => session.GetString(key);

    public static void SetJson<T>(this ISession session, string key, T value)
    {
        var json = JsonSerializer.Serialize(value);
        session.SetStringValue(key, json);
    }

    public static T? GetJson<T>(this ISession session, string key)
    {
        var json = session.GetStringValue(key);
        return string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json);
    }
}
