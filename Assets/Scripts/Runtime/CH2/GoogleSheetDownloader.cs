using UnityEditor;
using UnityEngine;
using System.Net;
using System.IO;
using System.Text;

#if UNITY_EDITOR
public class GoogleSheetDownloader : EditorWindow
{
    private string googleSheetUrl = "https://docs.google.com/spreadsheets/d/1mGlB8XE1-GcuMlUllwEH7tyCogMa6JkliMVS2Uw6AWI/export?format=csv&gid=0";

    [MenuItem("Tools/Download CSV")]
    public static void ShowWindow()
    {
        GetWindow<GoogleSheetDownloader>("Download CSV");
    }

    private void OnGUI()
    {
        GUILayout.Label("Google Sheet URL", EditorStyles.boldLabel);
        googleSheetUrl = EditorGUILayout.TextField(googleSheetUrl);

        if (GUILayout.Button("Download CSV"))
        {
            DownloadCSV();
        }
    }

    private void DownloadCSV()
    {
        using (WebClient webClient = new WebClient())
        {
            try
            {
                // CSV 데이터를 다운로드
                byte[] data = webClient.DownloadData(googleSheetUrl);
                string csvData = Encoding.UTF8.GetString(data);

                // 파일 경로 설정
                string filePath = Path.Combine(Application.dataPath, "Resources", "CH2Branch.csv");

                // UTF-8로 CSV 파일 저장
                using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
                {
                    writer.Write(csvData);
                }

                AssetDatabase.Refresh();
                Debug.Log("CSV downloaded and saved to: " + filePath);
            }
            catch (WebException e)
            {
                Debug.LogError("Failed to download CSV: " + e.Message);
            }
        }
    }
}
#endif