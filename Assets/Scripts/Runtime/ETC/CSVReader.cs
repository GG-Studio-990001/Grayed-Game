using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

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

                    // 값의 시작과 끝에 있는 따옴표만 제거
                    if (value.StartsWith("\"") && value.EndsWith("\""))
                    {
                        value = value.Substring(1, value.Length - 2);
                    }

                    // 값 내부에 있는 이중 따옴표를 단일 따옴표로 변경
                    value = value.Replace("\"\"", "\"");

                    value = value.Replace("\\n", "\n")
                                 .Replace("\\t", "\t")
                                 .Replace("\\r", "\r")
                                 .Replace("\\\\", "\\");

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
    }
}