using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DevcraftWMS.Portaria.Infrastructure;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var name = value.ToString();
        var member = value.GetType().GetMember(name).FirstOrDefault();
        var attribute = member?.GetCustomAttribute<DisplayAttribute>();
        return attribute?.GetName() ?? name;
    }
}
