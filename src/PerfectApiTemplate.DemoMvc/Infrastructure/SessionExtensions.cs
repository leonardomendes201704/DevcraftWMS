using System.Text.Json;

namespace PerfectApiTemplate.DemoMvc.Infrastructure;

public static class SessionExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static void SetStringValue(this ISession session, string key, string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            session.Remove(key);
            return;
        }

        session.Set(key, System.Text.Encoding.UTF8.GetBytes(value));
    }

    public static string? GetStringValue(this ISession session, string key)
    {
        return session.TryGetValue(key, out var data) ? System.Text.Encoding.UTF8.GetString(data) : null;
    }

    public static void SetJson<T>(this ISession session, string key, T value)
    {
        var json = JsonSerializer.Serialize(value, JsonOptions);
        session.SetStringValue(key, json);
    }

    public static T? GetJson<T>(this ISession session, string key)
    {
        var json = session.GetStringValue(key);
        return string.IsNullOrWhiteSpace(json) ? default : JsonSerializer.Deserialize<T>(json, JsonOptions);
    }
}
