using System.Collections.Generic;
using UnityEngine;
using Runtime.ETC;

namespace Runtime.CH3.Dancepace
{
    public enum LanguageEnum
    {
        Dev,
        Korean,
        English,
        Japan,
        China
    }

    public static class StringTableManager
    {
        private static Dictionary<string, string> _table;
        private const string CsvPath = "Data/Dancepace/CH3_Dancepace_StringTable";
        private const string KeyColumn = "Key";
        private static LanguageEnum _currentLanguage = LanguageEnum.Korean; // 기본값

        public static string Get(string key)
        {
            return Get(key, _currentLanguage);
        }

        public static string Get(string key, LanguageEnum language)
        {
            if (_table == null)
                LoadTable();

            key = key?.Trim();
            if (_table.TryGetValue(key, out var value))
                return value;
            return key; // fallback: 키 그대로 반환
        }

        public static void SetLanguage(LanguageEnum language)
        {
            _currentLanguage = language;
            _table = null; // 테이블을 다시 로드하도록 초기화
        }

        private static void LoadTable()
        {
            _table = new Dictionary<string, string>();
            var csv = CSVReader.Read(CsvPath); // List<Dictionary<string, object>> 반환 가정
            string valueColumn = _currentLanguage.ToString();
            
            Debug.Log($"Loading StringTable with language: {valueColumn}");
            Debug.Log($"CSV row count: {csv.Count}");
            
            foreach (var row in csv)
            {
                if (row.ContainsKey(KeyColumn) && row.ContainsKey(valueColumn))
                {
                    var key = row[KeyColumn]?.ToString().Trim();
                    var value = row[valueColumn]?.ToString().Trim();
                    
                    // 빈 키나 값은 건너뛰기
                    if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                    {
                        _table[key] = value;
                    }
                }
            }
        }
    }
} 