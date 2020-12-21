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
    public class XmlParser : ISerrializer, IDeserializer
    {
        public T Deserialize<T>(string xml)
        {
            var objects = Parse(xml, true);
            return Deserialize<T>(objects);
        }

        public string Serialize(object obj)
        {
            return Serialize(obj, 0, "");
        }

        private T Deserialize<T>(List<DeserializableObject> objects)
        {
            T result;
            var type = typeof(T);

            if (objects.First().ObjType == DeserializableObject.ObjectType.Primitive
                && objects.Count == 1
                && objects.First().Key == "")
                return (T) Convert.ChangeType(objects.First().Value.Trim('\"'), type);
            if (ParserUtils.IsEnumerable(type) && objects.Count > 0)
            {
                objects.First().ObjType = DeserializableObject.ObjectType.Collection;
                return (T) GetEnumerableInstance(objects.First(), type);
            }

            result = (T) Activator.CreateInstance(type);
            foreach (var obj in objects)
            {
                var key = obj.Key;
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
                    if (ParserUtils.IsEnumerable(ParserUtils.GetMemberType(type, key)))
                    {
                        if (ParserUtils.IsEnumerable(memberType))
                            ParserUtils.SetMemberValue(result, key, GetEnumerableInstance(obj, memberType));
                    }
                    else
                    {
                        var parsed = typeof(XmlParser).GetMethod("Deserialize", BindingFlags.NonPublic)
                            .MakeGenericMethod(memberType)
                            .Invoke(this, new object[] {Parse(value, false)});
                        ParserUtils.SetMemberValue(result, key, parsed);
                    }
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

        private string Serialize(object obj, int depth, string key)
        {
            if (obj == null)
                return "";
            var type = obj.GetType();
            StringBuilder stringBuilderInstance;
            if (type.GetCustomAttribute(typeof(NonSerializedAttribute)) != null
                || type.GetCustomAttribute(typeof(XmlIgnore)) != null
                || type.GetCustomAttribute(typeof(ParseIgnore)) != null)
                return "";
            if (type.IsPrimitive
                || type.IsEnum
                || type == typeof(string))
            {
                key = key == "" ? type.Name : key;
                return new StringBuilder($"{new string('\t', depth)}<{key}>{obj}</{key}>\n").ToString();
            }

            if (ParserUtils.IsEnumerable(type))
            {
                var genericArgument = type.GenericTypeArguments.Length == 0
                    ? type.GetElementType()
                    : type.GenericTypeArguments[0];
                if (key == "")
                    key = $"{type.Name}_{genericArgument.Name}";
                stringBuilderInstance = new StringBuilder($"{new string('\t', depth)}<{key}>\n");
                foreach (var el in (IEnumerable) obj)
                    stringBuilderInstance.Append(Serialize(el, depth + 1, genericArgument.Name));
                stringBuilderInstance.AppendLine($"{new string('\t', depth)}</{key}>");
            }
            else
            {
                key = key == "" ? type.Name : key;
                stringBuilderInstance = new StringBuilder($"{new string('\t', depth)}<{key}>\n");
                MemberInfo[] members = type.GetFields();
                members = members.Concat(type.GetProperties()).ToArray();
                var counter = 0;
                foreach (var member in members)
                {
                    if (member.GetCustomAttribute(typeof(XmlIgnore)) != null || member.GetCustomAttribute(
                                                                                 typeof(ParseIgnore)) != null
                                                                             || member.GetCustomAttribute(
                                                                                 typeof(NonSerializedAttribute)) !=
                                                                             null || ParserUtils.GetMemberValue(obj,
                                                                                 member.Name) == null)
                        continue;
                    var value = Serialize(ParserUtils.GetMemberValue(obj, member.Name), depth + 1, member.Name);
                    if (counter == members.Length - 1)
                        value = value.TrimEnd('\t', '\n', ' ');
                    stringBuilderInstance.Append(value);
                    counter++;
                }

                stringBuilderInstance.AppendLine($"\n{new string('\t', depth)}</{key}>");
            }

            return stringBuilderInstance.ToString();
        }

        private List<DeserializableObject> Parse(string xml, bool trim)
        {
            xml = xml.Trim('\n', '\t', '\r', ' ');
            var objects = new List<DeserializableObject>();
            var values = new List<string>();
            string tagName;
            Match match;
            try
            {
                tagName = GetNextTag(xml, 0);
                if (trim)
                {
                    var trimming = new Regex($"^<{tagName}>(.*)</{tagName}>$", RegexOptions.Singleline);
                    match = trimming.Match(xml);
                    if (match.Success) xml = match.Groups[1].Value;
                }
            }
            catch
            {
                return new List<DeserializableObject>
                    {new DeserializableObject("", new List<string> {xml}, DeserializableObject.ObjectType.Primitive)};
            }

            var Tag = new Regex(@"<(/?.*)>");

            var keyValues = new Dictionary<string, List<string>>();
            string mainTag = "", tag = "";
            var deep = 0;
            bool isMainTag = true, isValue = false;
            var value = "";
            var quotes = false;
            foreach (var character in xml)
                if (character != '\t' && character != '\r' && character != '\n' || quotes)
                {
                    if (character == '\"') quotes = !quotes;
                    if (quotes)
                    {
                        value += character;
                        continue;
                    }

                    if (character == '<')
                    {
                        isValue = false;
                        if (!isMainTag) tag += character;
                    }
                    else if (character == '>')
                    {
                        if (isMainTag)
                        {
                            isMainTag = false;
                            isValue = true;
                            deep++;
                        }
                        else
                        {
                            tag += character;
                            match = Tag.Match(tag);

                            if (match.Success)
                            {
                                tagName = match.Groups[1].Value;
                                if (tagName[0] == '/')
                                {
                                    if ('/' + mainTag == tagName && deep == 1)
                                    {
                                        if (keyValues.ContainsKey(mainTag))
                                            keyValues[mainTag].Add(value);
                                        else
                                            keyValues.Add(mainTag, new List<string> {value});
                                        mainTag = "";
                                        tag = "";
                                        isMainTag = true;
                                        isValue = false;
                                        value = "";
                                    }
                                    else
                                    {
                                        value += tag;
                                        tag = "";
                                        isValue = true;
                                    }

                                    deep--;
                                }
                                else
                                {
                                    deep++;
                                    isValue = true;
                                    value += tag;
                                    tag = "";
                                }
                            }
                            else
                            {
                                throw new Exception("XML file was damaged");
                            }
                        }
                    }
                    else
                    {
                        if (isValue)
                            value += character;
                        else if (isMainTag)
                            mainTag += character;
                        else
                            tag += character;
                    }
                }

            if (mainTag != "")
                return new List<DeserializableObject>
                {
                    new DeserializableObject("", new List<string> {mainTag}, DeserializableObject.ObjectType.Primitive)
                };
            foreach (var pair in keyValues)
            {
                DeserializableObject.ObjectType type;
                if (pair.Value.First().Length > 0 && pair.Value.First()[0] == '<')
                    type = DeserializableObject.ObjectType.ComplexObject;
                else
                    type = DeserializableObject.ObjectType.Primitive;
                objects.Add(new DeserializableObject(pair.Key, pair.Value, type));
            }

            return objects;
        }

        private string GetNextTag(string xml, int startIndex)
        {
            var tag = new StringBuilder("");
            var isTag = false;
            for (var i = startIndex; i < xml.Length; i++)
            {
                var character = xml[i];
                if (character == '<')
                    isTag = true;
                else if ((character == '>' || character == ' ') && isTag)
                    return tag.ToString();
                else if (isTag) tag.Append(character);
            }

            throw new Exception($"No tags are left from current position {startIndex}");
        }

        private object GetEnumerableInstance(DeserializableObject obj, Type type)
        {
            var genericArgument = type.GenericTypeArguments.Length == 0
                ? type.GetElementType()
                : type.GenericTypeArguments[0];
            var objects = new List<object>();
            var listType = typeof(List<>).MakeGenericType(genericArgument);
            var list = Activator.CreateInstance(listType) as IList;
            if (obj.ObjType == DeserializableObject.ObjectType.ComplexObject)
                obj = Parse(obj.Value, false).First();
            foreach (var subVal in obj.Values)
                list.Add(typeof(XmlParser).GetMethod("Deserialize")
                    .MakeGenericMethod(genericArgument)
                    .Invoke(this, new object[] {subVal.Trim()}));
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