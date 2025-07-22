using System.Collections.Generic;
using UnityEngine;
using Runtime.ETC;

namespace Runtime.CH3.Dancepace
{
    public static class StringTableManager
    {
        private static Dictionary<string, string> _table;
        private const string CsvPath = "Data/Dancepace/CH3_Dancepace_StringTable";
        private const string KeyColumn = "Key";
        private const string ValueColumn = "Korean"; // 언어별로 바꿀 수 있음

        public static string Get(string key)
        {
            if (_table == null)
                LoadTable();

            key = key?.Trim();
            if (_table.TryGetValue(key, out var value))
                return value;
            return key; // fallback: 키 그대로 반환
        }

        private static void LoadTable()
        {
            _table = new Dictionary<string, string>();
            var csv = CSVReader.Read(CsvPath); // List<Dictionary<string, object>> 반환 가정
            foreach (var row in csv)
            {
                if (row.ContainsKey(KeyColumn) && row.ContainsKey(ValueColumn))
                {
                    var key = row[KeyColumn]?.ToString().Trim();
                    var value = row[ValueColumn]?.ToString().Trim();
                    if (!string.IsNullOrEmpty(key))
                        _table[key] = value;
                }
            }
        }
    }
} 