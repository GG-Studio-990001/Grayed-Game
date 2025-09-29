using UnityEngine;

namespace Runtime.CH4
{
    [System.Serializable]
    public struct PlatformRow
    {
        public PlatformInfo[] cols; // 한 줄의 발판 배열
    }

    public class MapLoader : MonoBehaviour
    {
        [SerializeField] private string path;
        [SerializeField] private PlatformRow[] platforms;

        private void Start()
        {
            LoadMap(path);
        }

        private void LoadMap(string csvPath)
        {
            string[][] data = CSVMapReader.ReadRaw(csvPath);
            if (data == null || data.Length == 0)
            {
                Debug.LogError("맵 데이터를 불러오지 못했습니다.");
                return;
            }

            // 첫 줄 : 맵 정보
            string mapName = data[0][0];
            int width = int.Parse(data[0][1]);
            int height = int.Parse(data[0][2]);

            Debug.Log($"맵 불러오기: {mapName} ({width}x{height})");

            // 모든 발판 비활성화
            int idx = 0; // 인덱스 붙여주기
            for (int y = 0; y < platforms.Length; y++)
            {
                for (int x = 0; x < platforms[y].cols.Length; x++)
                {
                    if (platforms[y].cols[x] != null)
                    {
                        platforms[y].cols[x].gameObject.SetActive(false);
                        platforms[y].cols[x].Idx = idx;
                        idx++;
                    }
                }
            }

            // CSV 데이터 기준으로 발판 활성화
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    string cell = data[y + 1][x]; // +1: 첫 줄은 맵 정보
                    if (string.IsNullOrEmpty(cell)) continue;

                    if (int.TryParse(cell, out int type))
                    {
                        if (type >= 1) // 숫자 있는 발판만 활성화
                        {
                            // CSV (왼쪽 위 0,0) → Unity (왼쪽 아래 0,0) 변환
                            int row = (height - 1) - y;
                            int col = x;

                            if (row < platforms.Length && col < platforms[row].cols.Length)
                            {
                                PlatformInfo p = platforms[row].cols[col];
                                if (p != null)
                                {
                                    p.gameObject.SetActive(true);
                                    p.TargetLocation = (Ch4Ch2Locations)type;
                                    p.Txt.text = p.TargetLocation.GetName(); // 확장 메서드로 대체
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
