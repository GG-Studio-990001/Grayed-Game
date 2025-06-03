using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System;

namespace Runtime.ETC
{
    public class CSVReader
    {
        static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static char[] TRIM_CHARS = { '\"' };

        public static List<Dictionary<string, object>> Read(string file)
        {
            var list = new List<Dictionary<string, object>>();
            TextAsset data = Resources.Load(file) as TextAsset;

            if (data == null)
            {
                Debug.LogError($"Failed to load file: {file}");
                return list;
            }

            var lines = Regex.Split(data.text, LINE_SPLIT_RE);

            if (lines.Length <= 1) return list;

            var header = Regex.Split(lines[0], SPLIT_RE);
            for (var i = 1; i < lines.Length; i++)
            {
                var values = Regex.Split(lines[i], SPLIT_RE);
                if (values.Length == 0 || values[0] == "") continue;

                var entry = new Dictionary<string, object>();
                for (var j = 0; j < header.Length && j < values.Length; j++)
                {
                    string value = values[j];
                    value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
                    object finalvalue = value;

                    if (int.TryParse(value, out int n))
                    {
                        finalvalue = n;
                    }
                    else if (float.TryParse(value, out float f))
                    {
                        finalvalue = f;
                    }
                    entry[header[j]] = finalvalue;
                }
                list.Add(entry);
            }
            return list;
        }

        public static T[] ReadAndConvert<T>(string file) where T : new()
        {
            var data = Read(file);
            var result = new List<T>();

            foreach (var row in data)
            {
                var obj = new T();
                var properties = typeof(T).GetProperties();

                foreach (var property in properties)
                {
                    if (row.TryGetValue(property.Name, out object value))
                    {
                        try
                        {
                            if (value != null)
                            {
                                property.SetValue(obj, Convert.ChangeType(value, property.PropertyType));
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogWarning($"Failed to convert value for property {property.Name}: {e.Message}");
                        }
                    }
                }
                result.Add(obj);
            }

            return result.ToArray();
        }

        public static void SaveToJson<T>(T data, string filePath, bool prettyPrint = true)
        {
            try
            {
                string json = JsonUtility.ToJson(data, prettyPrint);
                string directory = Path.GetDirectoryName(filePath);
                
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save JSON file: {e.Message}");
            }
        }

        public static void SaveToJson<T>(T[] data, string filePath, bool prettyPrint = true)
        {
            try
            {
                string json = JsonUtility.ToJson(new { items = data }, prettyPrint);
                string directory = Path.GetDirectoryName(filePath);
                
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(filePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to save JSON file: {e.Message}");
            }
        }
    }
}