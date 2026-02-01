using System.Text.Json;
using System.Text.Json.Nodes;

namespace PerfectApiTemplate.Application.Common.Logging;

public static class LogMasking
{
    public static string? SanitizeHeaders(
        IEnumerable<KeyValuePair<string, string>> headers,
        string[] allowList,
        string[] denyList)
    {
        var filtered = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var allowSet = allowList.Length > 0 ? new HashSet<string>(allowList, StringComparer.OrdinalIgnoreCase) : null;
        var denySet = denyList.Length > 0 ? new HashSet<string>(denyList, StringComparer.OrdinalIgnoreCase) : null;

        foreach (var (key, value) in headers)
        {
            if (allowSet is not null && !allowSet.Contains(key))
            {
                continue;
            }

            if (denySet is not null && denySet.Contains(key))
            {
                filtered[key] = "***";
                continue;
            }

            filtered[key] = value;
        }

        return JsonSerializer.Serialize(filtered);
    }

    public static string? SanitizeJson(string? json, string[] jsonKeys, string[] jsonPaths, bool enabled)
    {
        if (!enabled || string.IsNullOrWhiteSpace(json))
        {
            return json;
        }

        try
        {
            var node = JsonNode.Parse(json);
            if (node is null)
            {
                return json;
            }

            var keySet = new HashSet<string>(jsonKeys, StringComparer.OrdinalIgnoreCase);
            MaskByKeys(node, keySet);
            MaskByPaths(node, jsonPaths);

            return node.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
        }
        catch
        {
            return json;
        }
    }

    private static void MaskByKeys(JsonNode node, HashSet<string> keys)
    {
        if (node is JsonObject obj)
        {
            foreach (var property in obj.ToList())
            {
                if (keys.Contains(property.Key))
                {
                    obj[property.Key] = "***";
                }
                else if (property.Value is not null)
                {
                    MaskByKeys(property.Value, keys);
                }
            }
        }
        else if (node is JsonArray array)
        {
            foreach (var item in array)
            {
                if (item is not null)
                {
                    MaskByKeys(item, keys);
                }
            }
        }
    }

    private static void MaskByPaths(JsonNode node, string[] jsonPaths)
    {
        foreach (var path in jsonPaths)
        {
            if (!path.StartsWith("$.", StringComparison.Ordinal))
            {
                continue;
            }

            var segments = path[2..].Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            ApplyPathMask(node, segments, 0);
        }
    }

    private static void ApplyPathMask(JsonNode node, string[] segments, int index)
    {
        if (index >= segments.Length)
        {
            return;
        }

        if (node is JsonObject obj)
        {
            var key = segments[index];
            if (!obj.TryGetPropertyValue(key, out var child) || child is null)
            {
                return;
            }

            if (index == segments.Length - 1)
            {
                obj[key] = "***";
                return;
            }

            ApplyPathMask(child, segments, index + 1);
        }
        else if (node is JsonArray array)
        {
            foreach (var item in array)
            {
                if (item is not null)
                {
                    ApplyPathMask(item, segments, index);
                }
            }
        }
    }
}

