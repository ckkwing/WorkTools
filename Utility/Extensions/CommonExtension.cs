using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Extensions
{
    public static class CommonExtension
    {
        public static bool IsNull(this object obj)
        {
            bool result = obj == null;
            return result;
        }

        public static bool IsNullOrEmpty(this string str)
        {
            bool result = string.IsNullOrEmpty(str);
            return result;
        }

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            bool result = list == null || !list.Any();
            return result;
        }

        public static string GetDescription(this Enum value)
        {
            if (null == value)
            {
                throw new ArgumentException("value");
            }

            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes =
                  (DescriptionAttribute[])fi.GetCustomAttributes(
                  typeof(DescriptionAttribute), false);
            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
    }
}
