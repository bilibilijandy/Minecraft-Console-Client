using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MinecraftClient.Scripting
{
    /// <summary>
    /// Minimal YAML subset parser/serializer for plugin config files.
    /// Supports scalar values (string, number, bool, null) and one level of
    /// nested maps, which is enough for typical plugin configuration.
    /// </summary>
    internal static class SimpleYaml
    {
        /// <summary>
        /// Parse YAML text into a map. Nested entries are stored as Dictionary&lt;string, object?&gt;.
        /// </summary>
        public static Dictionary<string, object?> Parse(string content)
        {
            Dictionary<string, object?> root = new();
            Dictionary<string, object?>? currentMap = null;

            foreach (string rawLine in content.Split('\n'))
            {
                string line = rawLine.TrimEnd('\r');
                string trimmed = line.TrimStart();
                if (trimmed.Length == 0 || trimmed.StartsWith("#"))
                    continue;

                int indent = line.Length - trimmed.Length;
                string withoutComment = StripComment(trimmed).TrimEnd();
                if (withoutComment.Length == 0)
                    continue;

                int colon = withoutComment.IndexOf(':');
                if (colon <= 0)
                    continue;

                string key = withoutComment[..colon].Trim();
                string value = withoutComment[(colon + 1)..].Trim();

                if (indent == 0)
                {
                    if (value.Length == 0)
                    {
                        currentMap = new Dictionary<string, object?>();
                        root[key] = currentMap;
                    }
                    else
                    {
                        root[key] = ParseScalar(value);
                        currentMap = null;
                    }
                }
                else
                {
                    // Nested entry, attach to the most recent parent map
                    if (currentMap is not null)
                    {
                        currentMap[key] = value.Length == 0
                            ? new Dictionary<string, object?>()
                            : ParseScalar(value);
                    }
                    // Deeper nesting is not supported and is silently ignored
                }
            }

            return root;
        }

        /// <summary>
        /// Get a value by key. Use "parent.child" for values nested one level deep.
        /// </summary>
        public static object? Get(Dictionary<string, object?> data, string key)
        {
            string[] parts = key.Split('.', 2);
            if (!data.TryGetValue(parts[0], out object? value))
                return null;
            if (parts.Length == 1)
                return value;
            return value is Dictionary<string, object?> nested && nested.TryGetValue(parts[1], out object? sub)
                ? sub
                : null;
        }

        /// <summary>
        /// Set a value by key. Use "parent.child" to create or update a nested value.
        /// </summary>
        public static void Set(Dictionary<string, object?> data, string key, object? value)
        {
            string[] parts = key.Split('.', 2);
            if (parts.Length == 1)
            {
                data[key] = value;
                return;
            }

            if (!data.TryGetValue(parts[0], out object? existing) || existing is not Dictionary<string, object?> nested)
            {
                nested = new Dictionary<string, object?>();
                data[parts[0]] = nested;
            }
            nested[parts[1]] = value;
        }

        /// <summary>
        /// Serialize a map back to YAML text.
        /// </summary>
        public static string Serialize(Dictionary<string, object?> data)
        {
            StringBuilder sb = new();
            foreach (KeyValuePair<string, object?> pair in data)
            {
                if (pair.Value is Dictionary<string, object?> nested)
                {
                    sb.Append(pair.Key).Append(':').Append('\n');
                    foreach (KeyValuePair<string, object?> nPair in nested)
                        sb.Append("  ").Append(nPair.Key).Append(": ").Append(FormatValue(nPair.Value)).Append('\n');
                }
                else
                {
                    sb.Append(pair.Key).Append(": ").Append(FormatValue(pair.Value)).Append('\n');
                }
            }
            return sb.ToString();
        }

        private static object? ParseScalar(string value)
        {
            string v = value.Trim();
            if (v.Length >= 2 && (v[0] == '"' || v[0] == '\'') && v[^1] == v[0])
                return v[1..^1];
            if (v.Equals("true", StringComparison.OrdinalIgnoreCase))
                return true;
            if (v.Equals("false", StringComparison.OrdinalIgnoreCase))
                return false;
            if (v.Equals("null", StringComparison.OrdinalIgnoreCase))
                return null;
            if (long.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out long l))
                return l;
            if (double.TryParse(v, NumberStyles.Float, CultureInfo.InvariantCulture, out double d))
                return d;
            return v;
        }

        private static string FormatValue(object? value)
        {
            return value switch
            {
                null => "null",
                bool b => b ? "true" : "false",
                string s => (s.Contains(' ') || s.Length == 0) ? "\"" + s + "\"" : s,
                _ => Convert.ToString(value, CultureInfo.InvariantCulture) ?? "null",
            };
        }

        private static string StripComment(string line)
        {
            bool inQuotes = false;
            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == '"' || c == '\'')
                    inQuotes = !inQuotes;
                else if (c == '#' && !inQuotes)
                    return line[..i];
            }
            return line;
        }
    }
}
