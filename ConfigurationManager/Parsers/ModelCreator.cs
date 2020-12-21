using System;
using System.Collections.Generic;
using ConfigurationManager.Uitls;

namespace ConfigurationManager.Parsers
{
    public class ModelCreator
    {
        public static T CreateInstanse<T>(Dictionary<string, object> pairs)
        {
            var result = (T) Activator.CreateInstance(typeof(T));
            foreach (var pair in pairs)
                if (pair.Value.GetType() == typeof(DBNull))
                    ParserUtils.SetMemberValue(result, pair.Key, null);
                else
                    ParserUtils.SetMemberValue(result, pair.Key, pair.Value);
            return result;
        }
    }
}