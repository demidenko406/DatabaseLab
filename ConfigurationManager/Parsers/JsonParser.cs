using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using ConfigurationManager.Attributes;
using ConfigurationManager.Uitls;

namespace ConfigurationManager.Parsers
{
    public class JsonParser : ISerrializer, IDeserializer
    {
        public T Deserialize<T>(string json)
        {
            return Deserialize<T>(Parse(json));
        }

        public string Serialize(object obj)
        {
            return Serialize(obj, 0);
        }

        private static string Serialize(object obj, int depth)
        {
            if (obj == null)
                return "";
            var type = obj.GetType();
            StringBuilder stringBuilderInstance;
            if (type.GetCustomAttribute(typeof(JsonIgnore)) != null || type.GetCustomAttribute(typeof(ParseIgnore)) !=
                                                                    null
                                                                    || type.GetCustomAttribute(
                                                                        typeof(NonSerializedAttribute)) != null)
                return "";
            if (type.IsPrimitive || type.IsEnum)
                return $"{obj}";
            if (type == typeof(string))
                return $"\"{obj}\"";
            if (ParserUtils.IsEnumerable(type))
            {
                var isComplex = false;
                stringBuilderInstance = new StringBuilder("");
                var counter = 0;
                var collectionLength = ParserUtils.CollectionLength((IEnumerable) obj);
                foreach (var subObj in (IEnumerable) obj)
                {
                    if (subObj == null)
                        continue;
                    var value = $"{Serialize(subObj, depth + 1)}";
                    if (value.Trim('\t', '\n', ' ').First() == '{')
                        isComplex = true;
                    if (counter < collectionLength - 1)
                    {
                        value = value.TrimEnd('\n');
                        value += ',';
                    }

                    stringBuilderInstance.Append(value);
                    counter++;
                }

                if (isComplex)
                {
                    stringBuilderInstance.Insert(0, $"\n{new string('\t', depth)}[");
                    stringBuilderInstance.AppendLine();
                    stringBuilderInstance.Append($"{new string('\t', depth)}]");
                }
                else
                {
                    stringBuilderInstance.Insert(0, "[");
                    stringBuilderInstance.Append("]");
                }
            }
            else
            {
                stringBuilderInstance = new StringBuilder($"\n{new string('\t', depth)}{{\n");
                MemberInfo[] members = type.GetProperties();
                members = members.Concat(type.GetFields()).ToArray();
                var counter = 0;
                var length = members.Length;
                foreach (var member in members)
                {
                    if (member.GetCustomAttribute(typeof(JsonIgnore)) != null || member.GetCustomAttribute(
                                                                                  typeof(ParseIgnore)) != null
                                                                              || member.GetCustomAttribute(
                                                                                  typeof(NonSerializedAttribute)) !=
                                                                              null || ParserUtils.GetMemberValue(obj,
                                                                                  member.Name) == null)
                        continue;
                    var value = $"{Serialize(ParserUtils.GetMemberValue(obj, member.Name), depth + 1)}";
                    stringBuilderInstance.Append($"{new string('\t', depth + 1)}\"{member.Name}\" : {value}"
                        .TrimEnd('\n')); //           "name" : { field }
                    if (counter != length - 1)
                        stringBuilderInstance.Append(',');
                    stringBuilderInstance.AppendLine();
                    ++counter;
                }

                stringBuilderInstance.Append($"{new string('\t', depth)}}}\n");
            }

            return stringBuilderInstance.ToString();
        }

        private static List<DeserializableObject> Parse(string json)
        {
            var objects = new List<DeserializableObject>();
            var values = new List<string>();
            string key = "", value = "";
            int braces = 0, squares = 0;
            var InQuotes = false;
            var InKey = true;
            var trimming = new Regex("^\\s*{(?<object>.*)}\\s*$", RegexOptions.Singleline);
            var match = trimming.Match(json);
            if (match.Success) json = match.Groups["object"].Value;
            foreach (var character in json)
                if (char.IsPunctuation(character) || char.IsLetterOrDigit(character) || InQuotes)
                {
                    if (character == '\"') InQuotes = !InQuotes;
                    if (InQuotes)
                    {
                        if (InKey)
                            key += character;
                        else
                            value += character;
                        continue;
                    }

                    if (character == '{')
                    {
                        braces++;
                    }
                    else if (character == '}')
                    {
                        braces--;
                    }
                    else if (character == '[' && braces == 0)
                    {
                        squares++;
                        if (squares == 1) continue;
                    }
                    else if (character == ']' && braces == 0)
                    {
                        squares--;
                        if (squares == 0) continue;
                    }
                    else if (character == ':' && braces == 0 && squares == 0)
                    {
                        InKey = false;
                        continue;
                    }
                    else if (character == ',' && braces == 0)
                    {
                        if (InKey)
                        {
                            value = key;
                            key = "";
                        }

                        values.Add(value);
                        value = "";
                        if (squares == 0)
                        {
                            objects.Add(new DeserializableObject(key, values));
                            values = new List<string>();
                            key = "";
                            value = "";
                            InKey = true;
                        }

                        continue;
                    }

                    if (InKey)
                        key += character;
                    else
                        value += character;
                }

            if (braces == 0 && squares == 0)
            {
                if (!(key == "" && value == ""))
                {
                    if (InKey)
                    {
                        values.Add(key);
                        key = "";
                    }
                    else
                    {
                        values.Add(value);
                    }

                    objects.Add(new DeserializableObject(key, values));
                }
            }
            else
            {
                throw new Exception("Json read failure");
            }

            return objects;
        }

        private static T Deserialize<T>(List<DeserializableObject> objects)
        {
            T result;
            var type = typeof(T);

            if (objects.Count == 1 && objects.First().Key == "")
            {
                if (objects.First().ObjType == DeserializableObject.ObjectType.Primitive)
                    return (T) Convert.ChangeType(objects.First().Value.Trim('\"'), type);
                if (objects.First().ObjType == DeserializableObject.ObjectType.Collection)
                    if (ParserUtils.IsEnumerable(type))
                        return (T) GetEnumerableInstance(objects.First(), type);
            }

            result = (T) Activator.CreateInstance(type);
            foreach (var obj in objects)
            {
                var key = obj.Key.Trim('\"');
                var value = obj.Value.Trim('\"');
                var memberType = ParserUtils.GetMemberType(type, key);
                if (obj.ObjType == DeserializableObject.ObjectType.Primitive)
                {
                    object converted;
                    if (memberType.IsEnum)
                        converted = Enum.Parse(memberType, value);
                    else
                        converted = Convert.ChangeType(value, memberType);
                    ParserUtils.SetMemberValue(result, key, converted);
                }
                else if (obj.ObjType == DeserializableObject.ObjectType.ComplexObject)
                {
                    var parsed = typeof(JsonParser)
                        .GetMethod("Deserialize")
                        .MakeGenericMethod(memberType)
                        .Invoke(null, new object[] {Parse(value)});
                    ParserUtils.SetMemberValue(result, key, parsed);
                }
                else
                {
                    if (ParserUtils.IsEnumerable(memberType))
                        ParserUtils.SetMemberValue(result, key, GetEnumerableInstance(obj, memberType));
                    else
                        throw new Exception("Invalid deserialization");
                }
            }

            return result;
        }

        private static object GetEnumerableInstance(DeserializableObject obj, Type type)
        {
            var genericArgument = type.GenericTypeArguments.Length == 0
                ? type.GetElementType()
                : type.GenericTypeArguments[0];
            var objects = new List<object>();
            var listType = typeof(List<>).MakeGenericType(genericArgument);
            var list = Activator.CreateInstance(listType) as IList;
            foreach (var subValue in obj.Values)
                list.Add(typeof(JsonParser)
                    .GetMethod("Deserialize")
                    .MakeGenericMethod(genericArgument)
                    .Invoke(null, new object[] {subValue.Trim()}));
            if (type.IsArray)
            {
                var array = Array.CreateInstance(genericArgument, list.Count);
                list.CopyTo(array, 0);
                return array;
            }

            var IEnumerableGenericType = typeof(IEnumerable<>).MakeGenericType(genericArgument);
            var info = type.GetConstructor(new[] {IEnumerableGenericType});
            if (info != null)
                return Activator.CreateInstance(type, list);
            throw new Exception("Type is not a collection");
        }
    }
}