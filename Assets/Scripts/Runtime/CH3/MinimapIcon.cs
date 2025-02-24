using UnityEngine;

namespace Runtime.CH3
{
    public class MinimapIcon : MonoBehaviour
    {
        public Transform player; // 플레이어의 Transform
        public RectTransform icon; // 미니맵 아이콘의 RectTransform
        public float minimapScale = 0.1f; // 미니맵 스케일 조정

        private void Update()
        {
            // 플레이어의 월드 위치를 미니맵 아이콘의 위치로 변환
            Vector3 playerPosition = player.position;
            icon.anchoredPosition = new Vector2(playerPosition.x * minimapScale, playerPosition.z * minimapScale);
        }
    }
}
