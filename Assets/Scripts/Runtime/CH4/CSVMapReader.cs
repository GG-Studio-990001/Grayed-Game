using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Runtime.CH4
{
    public static class CSVMapReader
    {
        static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
        static string SPLIT_RE = @",";

        public static string[][] ReadRaw(string file)
        {
            TextAsset data = Resources.Load(file) as TextAsset;
            if (data == null)
            {
                Debug.LogError($"Failed to load file: {file}");
                return null;
            }

            var lines = Regex.Split(data.text, LINE_SPLIT_RE);
            List<string[]> result = new List<string[]>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var values = Regex.Split(line, SPLIT_RE);
                result.Add(values);
            }

            return result.ToArray();
        }
    }
}
