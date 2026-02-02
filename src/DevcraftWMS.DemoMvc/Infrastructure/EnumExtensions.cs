using System.ComponentModel.DataAnnotations;

namespace DevcraftWMS.DemoMvc.Infrastructure;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (name is null)
        {
            return value.ToString();
        }

        var field = type.GetField(name);
        if (field is null)
        {
            return value.ToString();
        }

        var attribute = Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) as DisplayAttribute;
        return attribute?.Name ?? value.ToString();
    }
}
