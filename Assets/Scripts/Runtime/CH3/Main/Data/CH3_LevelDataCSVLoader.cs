using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Runtime.CH3.Main
{
    public static class CH3_LevelDataCSVLoader
    {
        private const string CSV_PATH = "Data/CH3/CH3_LevelData";
        private static Dictionary<string, CH3_LevelData> _dataCache;

        public static void ClearCache()
        {
            _dataCache = null;
        }

        public static Dictionary<string, CH3_LevelData> LoadAllFromCSV()
        {
            if (_dataCache != null) return _dataCache;

            _dataCache = new Dictionary<string, CH3_LevelData>();
            TextAsset csvFile = Resources.Load<TextAsset>(CSV_PATH);

            if (csvFile == null)
            {
                Debug.LogError($"CSV 파일을 찾을 수 없습니다: {CSV_PATH}");
                return _dataCache;
            }

            string[] lines = Regex.Split(csvFile.text, @"\r\n|\n\r|\n|\r");
            if (lines.Length < 3) return _dataCache;

            string[] headers = ParseCSVLine(lines[1]);

            for (int i = 2; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] values = ParseCSVLine(lines[i]);
                if (values.Length == 0 || string.IsNullOrEmpty(values[0])) continue;

                CH3_LevelData data = ParseLevelData(headers, values);
                if (data != null && !string.IsNullOrEmpty(data.id))
                {
                    _dataCache[data.id] = data;
                }
            }

            return _dataCache;
        }

        public static List<CH3_LevelData> GetBuildableItems()
        {
            var allData = LoadAllFromCSV();
            return allData.Values.Where(data => data.isBuilding).ToList();
        }

        private static string[] ParseCSVLine(string line)
        {
            List<string> result = new List<string>();
            bool inQuotes = false;
            string current = "";

            foreach (char c in line)
            {
                if (c == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (c == ',' && !inQuotes)
                {
                    result.Add(current.Trim());
                    current = "";
                }
                else
                {
                    current += c;
                }
            }
            result.Add(current.Trim());

            return result.ToArray();
        }

        private static CH3_LevelData ParseLevelData(string[] headers, string[] values)
        {
            CH3_LevelData data = ScriptableObject.CreateInstance<CH3_LevelData>();

            int idIndex = GetHeaderIndex(headers, "id");
            int devIndex = GetHeaderIndex(headers, "dev");
            int sizeXIndex = GetHeaderIndex(headers, "sizeX");
            int sizeYIndex = GetHeaderIndex(headers, "sizeY");
            int isBuildingIndex = GetHeaderIndex(headers, "isBuilding");
            int maxBuildIndex = GetHeaderIndex(headers, "maxBuild");
            int isBreakableIndex = GetHeaderIndex(headers, "isBreakable");
            int maxDropIndex = GetHeaderIndex(headers, "maxDrop");
            int isRespawnIndex = GetHeaderIndex(headers, "isRespawn");

            data.id = GetValue(values, idIndex);
            data.dev = GetValue(values, devIndex);
            data.sizeX = GetIntValue(values, sizeXIndex, 1);
            data.sizeY = GetIntValue(values, sizeYIndex, 1);
            data.isBuilding = GetBoolValue(values, isBuildingIndex, false);
            data.maxBuild = GetIntValue(values, maxBuildIndex, 1);
            data.isBreakable = GetBoolValue(values, isBreakableIndex, false);
            data.maxDrop = GetIntValue(values, maxDropIndex, 0);
            data.isRespawn = GetBoolValue(values, isRespawnIndex, false);

            data.dropCurrency = new List<CurrencyData>();
            int currencyBaseIndex = GetHeaderIndex(headers, "currency");
            if (currencyBaseIndex >= 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    int currencyIndex = currencyBaseIndex + (i * 3);
                    int amountIndex = currencyIndex + 1;
                    int ratioIndex = currencyIndex + 2;

                    if (currencyIndex < values.Length && !string.IsNullOrEmpty(values[currencyIndex]))
                    {
                        string currencyStr = values[currencyIndex];
                        int amount = GetIntValue(values, amountIndex, 0);
                        float ratio = GetFloatValue(values, ratioIndex, 1f);

                        if (TryParseCurrency(currencyStr, out ECurrencyData currency))
                        {
                            data.dropCurrency.Add(new CurrencyData(currency, amount, ratio));
                        }
                    }
                }
            }

            data.buildCurrency = new List<CurrencyData>();
            int buildCurrencyBaseIndex = GetHeaderIndex(headers, "buildCurrency");
            if (buildCurrencyBaseIndex >= 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    int currencyIndex = buildCurrencyBaseIndex + (i * 2);
                    int amountIndex = currencyIndex + 1;

                    if (currencyIndex < values.Length && !string.IsNullOrEmpty(values[currencyIndex]))
                    {
                        string currencyStr = values[currencyIndex];
                        int amount = GetIntValue(values, amountIndex, 0);

                        if (TryParseCurrency(currencyStr, out ECurrencyData currency))
                        {
                            data.buildCurrency.Add(new CurrencyData(currency, amount, 1f));
                        }
                    }
                }
            }
            
            if (data.buildCurrency.Count == 0)
            {
                SetBuildCurrencyForItem(data);
            }

            return data;
        }

        private static void SetBuildCurrencyForItem(CH3_LevelData data)
        {
            if (string.IsNullOrEmpty(data.id)) return;

            switch (data.id)
            {
                case "endingPortal":
                    data.buildCurrency.Add(new CurrencyData(ECurrencyData.Stone, 12));
                    break;
                case "lvFactoryS":
                    data.buildCurrency.Add(new CurrencyData(ECurrencyData.Stone, 10));
                    data.buildCurrency.Add(new CurrencyData(ECurrencyData.Tree, 10));
                    break;
                case "lvFactoryL":
                    data.buildCurrency.Add(new CurrencyData(ECurrencyData.Stone, 20));
                    data.buildCurrency.Add(new CurrencyData(ECurrencyData.Tree, 20));
                    data.buildCurrency.Add(new CurrencyData(ECurrencyData.Coin, 20));
                    break;
                case "factoryTimber":
                    data.buildCurrency.Add(new CurrencyData(ECurrencyData.ResourceA, 5));
                    data.buildCurrency.Add(new CurrencyData(ECurrencyData.ResourceDefault, 5));
                    break;
                case "factoryStone":
                    data.buildCurrency.Add(new CurrencyData(ECurrencyData.ResourceB, 5));
                    data.buildCurrency.Add(new CurrencyData(ECurrencyData.ResourceDefault, 5));
                    break;
                case "factoryCoin":
                    data.buildCurrency.Add(new CurrencyData(ECurrencyData.ResourceC, 5));
                    data.buildCurrency.Add(new CurrencyData(ECurrencyData.ResourceDefault, 5));
                    break;
                case "skillResetTicket":
                    data.buildCurrency.Add(new CurrencyData(ECurrencyData.Coin, 50));
                    break;
            }
        }

        private static int GetHeaderIndex(string[] headers, string headerName)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i] == headerName) return i;
            }
            return -1;
        }

        private static string GetValue(string[] values, int index)
        {
            if (index < 0 || index >= values.Length) return "";
            return values[index];
        }

        private static int GetIntValue(string[] values, int index, int defaultValue = 0)
        {
            if (index < 0 || index >= values.Length) return defaultValue;
            if (int.TryParse(values[index], out int result)) return result;
            return defaultValue;
        }

        private static float GetFloatValue(string[] values, int index, float defaultValue = 0f)
        {
            if (index < 0 || index >= values.Length) return defaultValue;
            if (float.TryParse(values[index], out float result)) return result;
            return defaultValue;
        }

        private static bool GetBoolValue(string[] values, int index, bool defaultValue = false)
        {
            if (index < 0 || index >= values.Length) return defaultValue;
            string value = values[index];
            if (string.IsNullOrEmpty(value)) return defaultValue;
            return value.ToUpper() == "TRUE";
        }

        private static bool TryParseCurrency(string currencyStr, out ECurrencyData currency)
        {
            currency = ECurrencyData.ResourceDefault;
            if (string.IsNullOrEmpty(currencyStr)) return false;

            string normalized = currencyStr.Trim().ToLower();

            switch (normalized)
            {
                case "tree":
                    currency = ECurrencyData.Tree;
                    return true;
                case "stone":
                    currency = ECurrencyData.Stone;
                    return true;
                case "resoucedefault":
                case "resourcedefault":
                    currency = ECurrencyData.ResourceDefault;
                    return true;
                case "resourcea":
                    currency = ECurrencyData.ResourceA;
                    return true;
                case "resourceb":
                    currency = ECurrencyData.ResourceB;
                    return true;
                case "resourcec":
                    currency = ECurrencyData.ResourceC;
                    return true;
                case "coin":
                    currency = ECurrencyData.Coin;
                    return true;
                default:
                    return false;
            }
        }
    }
}

