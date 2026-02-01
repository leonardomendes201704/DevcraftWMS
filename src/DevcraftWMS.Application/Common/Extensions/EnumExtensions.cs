using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DevcraftWMS.Application.Common.Extensions;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
        var attribute = member?.GetCustomAttribute<DisplayAttribute>();
        if (attribute?.Name is null)
        {
            throw new InvalidOperationException($"DisplayAttribute is missing for enum member '{value}'.");
        }

        return attribute.Name;
    }
}
