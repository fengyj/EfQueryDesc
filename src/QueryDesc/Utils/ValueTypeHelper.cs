using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace me.fengyj.QueryDesc.Utils
{
    public class ValueTypeHelper
    {
        public static object Parse(string str, Type targetType)
        {
            if (str == null) return null;

            if (str == string.Empty && targetType != typeof(string))
                return null;

            if (targetType == typeof(string)) return str;

            if (targetType.IsEnum)
                return Enum.Parse(targetType, str);
            if(targetType == typeof(TimeSpan))
                return TimeSpan.Parse(str);
            else if(targetType == typeof(Guid))
                return Guid.Parse(str);
            else if(targetType == typeof(bool))
            {
                if (string.Compare(str, "true", true) == 0
                    || string.Compare(str, "t", true) == 0
                    || string.Compare(str, "1", true) == 0
                    || string.Compare(str, "y", true) == 0
                    || string.Compare(str, "yes", true) == 0)
                {
                    return true;
                }
                else
                    return false;
            }
            else return Convert.ChangeType(str, targetType);
        }

        public static object ChangeType(object obj, Type targetType)
        {
            if (obj == null) return null;
            if (obj is string) return Parse(obj as string, targetType);

            return Convert.ChangeType(obj, targetType);
        }
    }
}
