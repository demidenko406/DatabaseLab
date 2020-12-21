using System;
using System.Collections;

namespace ConfigurationManager.Uitls
{
    internal class ParserUtils
    {
        public static Type GetMemberType(Type type, string memberName)
        {
            var type1 = type.GetProperty(memberName)?.PropertyType;
            if (type1 == null) type1 = type.GetField(memberName)?.FieldType;
            if (type1 == null) throw new Exception("This type doesn't contain member with this name");
            return type1;
        }

        public static bool IsEnumerable(Type type)
        {
            if (type == typeof(string))
                return false;
            return type.GetInterface(nameof(IEnumerable)) != null;
        }

        public static int CollectionLength(IEnumerable collection)
        {
            var i = 0;
            foreach (var e in collection)
                ++i;
            return i;
        }

        public static object GetMemberValue(object obj, string key)
        {
            var T = obj.GetType();
            if (T.GetProperty(key) != null)
                return T.GetProperty(key).GetValue(obj);
            if (T.GetField(key) != null)
                return T.GetField(key).GetValue(obj);
            throw new Exception("Wrong member was detected");
        }

        public static void SetMemberValue(object obj, string memberName, object value)
        {
            var type = obj.GetType();
            if (type.GetProperty(memberName) != null)
            {
                var info = type.GetProperty(memberName);
                info.SetValue(obj, value);
            }
            else if (type.GetField(memberName) != null)
            {
                var info = type.GetField(memberName);
                info.SetValue(obj, value);
            }
        }
    }
}