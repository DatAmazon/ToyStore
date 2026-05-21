using System.ComponentModel;
using System.Reflection;

namespace ToyStoreManagement.Common.Extensions
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            //var field = value.GetType().GetField(value.ToString());
            //var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            //return attribute == null ? value.ToString() : attribute.Description;

            if (value == null)
                return string.Empty;

            var field = value.GetType().GetField(value.ToString());

            if (field == null)
                return value.ToString();

            var attribute = field.GetCustomAttribute<DescriptionAttribute>();

            return attribute?.Description ?? value.ToString();
        }
    }
}