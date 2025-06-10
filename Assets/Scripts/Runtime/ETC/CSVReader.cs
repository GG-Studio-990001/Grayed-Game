using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;

namespace Runtime.ETC
{
    public class CSVReader
    {
        private const string GAME_CONFIG_SPREADSHEET_ID = "1Hz-mzUoUFhEyNFAiaK8IZQEUE5-Z2Ru1o57Kgn6l5RY";
        private const string WAVE_DATA_SPREADSHEET_ID = "16suUSR-_8VXJIS4F3QmxMD3clafiQp6WkoH1-qyGOTc";
        private const string API_KEY = "YOUR_API_KEY"; // 구글 클라우드 콘솔에서 발급받은 API 키를 입력하세요

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

        public static List<Dictionary<string, object>> ReadFromExcel(string filePath, string sheetName = null)
        {
            var list = new List<Dictionary<string, object>>();
            
            try
            {
                // Excel 파일을 CSV로 변환
                string csvContent = ConvertExcelToCSV(filePath, sheetName);
                
                // CSV 내용을 라인으로 분할
                var lines = Regex.Split(csvContent, LINE_SPLIT_RE);
                
                if (lines.Length <= 1) return list;

                // 헤더 처리
                var header = Regex.Split(lines[0], SPLIT_RE);
                
                // 데이터 처리
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
            }
            catch (Exception e)
            {
                Debug.LogError($"Excel 파일 읽기 실패: {e.Message}");
            }

            return list;
        }

        private static string ConvertExcelToCSV(string excelFilePath, string sheetName = null)
        {
            try
            {
                // Excel 파일을 CSV로 변환하는 프로세스 시작
                System.Diagnostics.Process process = new System.Diagnostics.Process();
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.Verb = "runas"; // 관리자 권한으로 실행

                // PowerShell 스크립트 작성
                string script = $@"
                    Set-ExecutionPolicy Bypass -Scope Process -Force;
                    $excel = New-Object -ComObject Excel.Application
                    $excel.Visible = $false
                    $workbook = $excel.Workbooks.Open('{excelFilePath}')
                    $worksheet = $workbook.Worksheets.Item(1)
                    $csv = ''
                    
                    $rowCount = $worksheet.UsedRange.Rows.Count
                    $colCount = $worksheet.UsedRange.Columns.Count
                    
                    for ($row = 1; $row -le $rowCount; $row++) {{
                        $rowData = @()
                        for ($col = 1; $col -le $colCount; $col++) {{
                            $cell = $worksheet.Cells.Item($row, $col).Text
                            if ($cell -match ',') {{
                                $cell = '""' + $cell + '""'
                            }}
                            $rowData += $cell
                        }}
                        $csv += ($rowData -join ',') + [Environment]::NewLine
                    }}
                    
                    $workbook.Close($false)
                    $excel.Quit()
                    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($worksheet) | Out-Null
                    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($workbook) | Out-Null
                    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($excel) | Out-Null
                    [System.GC]::Collect()
                    [System.GC]::WaitForPendingFinalizers()
                    
                    $csv
                ";

                process.StartInfo.Arguments = $"-Command \"{script}\"";
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    Debug.LogError($"PowerShell Error: {error}");
                }

                // CSV 파일로 저장
                string csvPath = Path.ChangeExtension(excelFilePath, ".csv");
                File.WriteAllText(csvPath, output);

                return output;
            }
            catch (Exception e)
            {
                Debug.LogError($"Excel을 CSV로 변환 실패: {e.Message}");
                return string.Empty;
            }
        }

        public static T[] ReadAndConvertFromExcel<T>(string filePath, string sheetName = null) where T : new()
        {
            var data = ReadFromExcel(filePath, sheetName);
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

        public static async Task<List<Dictionary<string, object>>> ReadFromGoogleSheet(string range = "Sheet1!A1:Z1000", bool isGameConfig = true)
        {
            var list = new List<Dictionary<string, object>>();
            
            try
            {
                string spreadsheetId = isGameConfig ? GAME_CONFIG_SPREADSHEET_ID : WAVE_DATA_SPREADSHEET_ID;
                string url = $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/gviz/tq?tqx=out:csv&range={range}";
                
                using (UnityWebRequest request = UnityWebRequest.Get(url))
                {
                    var operation = request.SendWebRequest();
                    while (!operation.isDone)
                        await Task.Yield();

                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        string csvContent = request.downloadHandler.text;
                        var lines = Regex.Split(csvContent, LINE_SPLIT_RE);

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
                    }
                    else
                    {
                        Debug.LogError($"Error: {request.error}");
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"구글 스프레드시트 읽기 실패: {e.Message}");
            }

            return list;
        }

        public static async Task<T[]> ReadAndConvertFromGoogleSheet<T>(string range = "Sheet1!A1:Z1000", bool isGameConfig = true) where T : new()
        {
            var data = await ReadFromGoogleSheet(range, isGameConfig);
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
    }
}