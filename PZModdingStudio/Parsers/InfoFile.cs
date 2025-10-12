using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Property)]
public class InfoKeyAttribute : Attribute
{
    public string Key { get; private set; }
    public InfoKeyAttribute(string key) { Key = key; }
}

[AttributeUsage(AttributeTargets.Property)]
public class InfoRequiredAttribute : Attribute { }

public static class InfoFile
{
    private const string LineToken = "<LINE>";

    // --- Diccionario simple ---
    public static Dictionary<string, string> LoadToDictionary(string path, bool trimValues)
    {
        var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (!File.Exists(path)) return dict;

        foreach (var raw in File.ReadAllLines(path))
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            string line = raw.TrimEnd('\r');
            string t = line.TrimStart();
            if (t.StartsWith("#") || t.StartsWith(";")) continue;

            int idx = line.IndexOf('=');
            if (idx < 0) continue;
            string key = line.Substring(0, idx).Trim();
            string value = line.Substring(idx + 1);
            if (trimValues) value = value.Trim();
            dict[key] = value;
        }

        return dict;
    }

    public static void SaveFromDictionary(Dictionary<string, string> dict, string path, bool convertNewlinesToToken)
    {
        var lines = new List<string>();
        foreach (var kv in dict)
        {
            string val = kv.Value ?? "";
            if (convertNewlinesToToken) val = val.Replace(Environment.NewLine, LineToken);
            lines.Add(string.Format("{0}={1}", kv.Key, val));
        }
        File.WriteAllLines(path, lines);
    }

    // --- Mapeo genérico T <-> archivo .info ---
    public static T Deserialize<T>(string path, bool convertLineTokenToNewline, bool trimValues) where T : new()
    {
        var dict = LoadToDictionary(path, trimValues);
        var result = new T();

        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(delegate (PropertyInfo p) { return p.CanWrite && p.GetSetMethod() != null && p.GetSetMethod().IsPublic; });

        foreach (var p in props)
        {
            string key = GetKeyName(p);
            string rawVal;
            if (!dict.TryGetValue(key, out rawVal)) continue;

            string text = rawVal;
            if (convertLineTokenToNewline && text != null)
            {
                text = text.Replace(LineToken, Environment.NewLine);
            }

            SetPropertyFromString(result, p, text);
        }

        return result;
    }

    public static void Serialize<T>(T obj, string path, bool convertNewlineToLineToken, bool emitNulls)
    {
        var lines = new List<string>();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(delegate (PropertyInfo p) { return p.CanRead && p.GetGetMethod() != null && p.GetGetMethod().IsPublic; });

        foreach (var p in props)
        {
            string key = GetKeyName(p);
            object val = p.GetValue(obj, null);
            if (val == null && !emitNulls) continue;

            string text = ValueToString(val);
            if (convertNewlineToLineToken && text != null)
                text = text.Replace(Environment.NewLine, LineToken);

            lines.Add(string.Format("{0}={1}", key, text));
        }

        File.WriteAllLines(path, lines);
    }

    // --- Helpers ---
    private static string GetKeyName(PropertyInfo p)
    {
        var att = p.GetCustomAttributes(typeof(InfoKeyAttribute), false);
        if (att != null && att.Length > 0)
        {
            return ((InfoKeyAttribute)att[0]).Key;
        }
        return p.Name;
    }

    private static void SetPropertyFromString(object target, PropertyInfo p, string text)
    {
        Type type = p.PropertyType;
        Type nullableUnderlying = Nullable.GetUnderlyingType(type);
        if (nullableUnderlying != null)
        {
            if (string.IsNullOrEmpty(text))
            {
                p.SetValue(target, null, null);
                return;
            }
            type = nullableUnderlying;
        }

        object value = null;
        if (type == typeof(string))
        {
            value = text;
        }
        else if (type == typeof(bool))
        {
            value = bool.Parse(text);
        }
        else if (type == typeof(int))
        {
            value = int.Parse(text, CultureInfo.InvariantCulture);
        }
        else if (type == typeof(long))
        {
            value = long.Parse(text, CultureInfo.InvariantCulture);
        }
        else if (type == typeof(double))
        {
            value = double.Parse(text, CultureInfo.InvariantCulture);
        }
        else if (type == typeof(DateTime))
        {
            value = DateTime.Parse(text, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }
        else if (type.IsEnum)
        {
            value = Enum.Parse(type, text, true);
        }
        else if (IsGenericListOfT(type))
        {
            value = ParseList(type, text ?? "");
        }
        else
        {
            value = Convert.ChangeType(text, type, CultureInfo.InvariantCulture);
        }

        p.SetValue(target, value, null);
    }

    private static bool IsGenericListOfT(Type t)
    {
        if (!t.IsGenericType) return false;
        var def = t.GetGenericTypeDefinition();
        if (def == typeof(List<>)) return true;
        foreach (var i in t.GetInterfaces())
        {
            if (i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IList<>)) return true;
        }
        return false;
    }

    private static object ParseList(Type listType, string text)
    {
        Type itemType = listType.GetGenericArguments()[0];
        var parts = SplitRespectingEscapes(text).ToArray();
        var listTypeConcrete = typeof(List<>).MakeGenericType(itemType);
        var list = (IList)Activator.CreateInstance(listTypeConcrete);

        foreach (var raw in parts)
        {
            string cell = raw;
            object v = null;
            if (itemType == typeof(string)) v = cell;
            else if (itemType.IsEnum) v = Enum.Parse(itemType, cell, true);
            else v = Convert.ChangeType(cell, itemType, CultureInfo.InvariantCulture);

            list.Add(v);
        }

        if (listType.IsAssignableFrom(list.GetType())) return list;
        // intentar convertir a tipo declarado invocando Add
        object targetList = Activator.CreateInstance(listType);
        MethodInfo addMethod = listType.GetMethod("Add");
        foreach (var it in list) addMethod.Invoke(targetList, new object[] { it });
        return targetList;
    }

    private static IEnumerable<string> SplitRespectingEscapes(string s)
    {
        if (string.IsNullOrEmpty(s)) yield break;
        string cur = "";
        bool esc = false;
        foreach (char ch in s)
        {
            if (esc)
            {
                cur += ch;
                esc = false;
                continue;
            }
            if (ch == '\\') { esc = true; continue; }
            if (ch == ',') { yield return cur; cur = ""; continue; }
            cur += ch;
        }
        yield return cur;
    }

    private static string ValueToString(object value)
    {
        if (value == null) return "";
        if (value is string) return (string)value;
        if (value is DateTime)
        {
            return ((DateTime)value).ToString("o", CultureInfo.InvariantCulture);
        }
        Type t = value.GetType();
        if (IsGenericListOfT(t))
        {
            var items = new List<string>();
            foreach (var it in (IEnumerable)value)
            {
                items.Add(it == null ? "" : it.ToString());
            }
            return string.Join(",", items);
        }
        if (t.IsEnum) return value.ToString();
        var f = value as IFormattable;
        if (f != null) return f.ToString(null, CultureInfo.InvariantCulture);
        return value.ToString();
    }

    // Utilidad: lista de claves que faltan en archivo
    public static List<string> GetMissingKeys<T>(string path)
    {
        var dict = LoadToDictionary(path, true);
        var missing = new List<string>();
        var props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(delegate (PropertyInfo p) { return p.CanWrite && p.GetSetMethod() != null && p.GetSetMethod().IsPublic; });

        foreach (var p in props)
        {
            string key = GetKeyName(p);
            if (!dict.ContainsKey(key)) missing.Add(key);
        }
        return missing;
    }
}
