using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Runtime.CH3.Main
{
    public class MinimapManager : MonoBehaviour
    {
        [Header("Minimap Camera Settings")]
        [SerializeField] private Camera minimapCamera;
        //[SerializeField] private float minimapSize = 10f;
        [SerializeField] private LayerMask minimapLayers;
        [SerializeField] private float cameraHeight = 10f;

        [Header("UI Settings")]
        [SerializeField] private RawImage minimapDisplay;
        [SerializeField] private float zoom = 5f;

        [Header("Icon Settings")]
        [SerializeField] private GameObject playerIconPrefab;
        [SerializeField] private GameObject gridObjectIconPrefab;
        [SerializeField] private GameObject npcIconPrefab;
    
        [Header("Icon Colors")]
        [SerializeField] private Color playerColor = Color.blue;
        [SerializeField] private Color gridObjectColor = Color.yellow;
        [SerializeField] private Color npcColor = Color.red;

        private Transform playerTransform;
        private RenderTexture minimapTexture;
        private Dictionary<Transform, GameObject> minimapIcons = new Dictionary<Transform, GameObject>();
        private RectTransform minimapRectTransform;

        private void Awake()
        {
            minimapRectTransform = minimapDisplay.GetComponent<RectTransform>();
        }

        private void Start()
        {
            InitializeMinimapCamera();
            FindPlayer();
        }

        private void InitializeMinimapCamera()
        {
            if (minimapCamera == null || minimapDisplay == null) return;

            // RenderTexture 생성
            minimapTexture = new RenderTexture(256, 256, 16, RenderTextureFormat.ARGB32);
            minimapCamera.targetTexture = minimapTexture;
            minimapDisplay.texture = minimapTexture;

            // 카메라 설정
            minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            minimapCamera.orthographic = true;
            minimapCamera.orthographicSize = zoom;
            minimapCamera.cullingMask = minimapLayers;
            minimapCamera.clearFlags = CameraClearFlags.SolidColor;
            minimapCamera.backgroundColor = new Color(0, 0, 0, 0);

            // 플레이어 위치로 초기 위치 설정
            if (playerTransform != null)
            {
                Vector3 startPos = playerTransform.position;
                startPos.y = cameraHeight;
                minimapCamera.transform.position = startPos;
            }
        }

        private void LateUpdate()
        {
            if (playerTransform != null && minimapCamera != null)
            {
                // 미니맵 카메라가 항상 플레이어 위에 위치하도록 업데이트
                Vector3 newPos = playerTransform.position;
                newPos.y = cameraHeight;
                minimapCamera.transform.position = newPos;
            }
        }

        private void FindPlayer()
        {
            playerTransform = FindObjectOfType<PlayerGridObject>()?.transform;
            if (playerTransform != null)
            {
                CreateMinimapIcon(playerTransform, playerIconPrefab, playerColor);
            }
        }

        public void CreateMinimapIcon(Transform target, GameObject iconPrefab , Color iconColor)
        {
            if (target == null || iconPrefab == null || minimapDisplay == null) return;

            if (minimapIcons.ContainsKey(target))
            {
                Destroy(minimapIcons[target]);
                minimapIcons.Remove(target);
            }

            // 미니맵 UI 내에 아이콘 생성
            GameObject icon = Instantiate(iconPrefab, minimapDisplay.transform);
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            if (iconRect != null)
            {
                // UI 설정
                iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.sizeDelta = new Vector2(20f, 20f); // 아이콘 크기
            }

            MinimapIcon minimapIcon = icon.GetComponent<MinimapIcon>();
            if (minimapIcon == null)
            {
                minimapIcon = icon.AddComponent<MinimapIcon>();
            }
            minimapIcon.Initialize(iconColor, target, minimapCamera, minimapRectTransform);
            
            minimapIcons[target] = icon;
        }
        
        public void CreateMinimapIcon(Transform target)
        {
            var iconPrefab = playerIconPrefab;
            var iconColor = gridObjectColor;
            if (target == null || iconPrefab == null || minimapDisplay == null) return;

            if (minimapIcons.ContainsKey(target))
            {
                Destroy(minimapIcons[target]);
                minimapIcons.Remove(target);
            }

            // 미니맵 UI 내에 아이콘 생성
            GameObject icon = Instantiate(iconPrefab, minimapDisplay.transform);
            RectTransform iconRect = icon.GetComponent<RectTransform>();
            if (iconRect != null)
            {
                // UI 설정
                iconRect.anchorMin = new Vector2(0.5f, 0.5f);
                iconRect.anchorMax = new Vector2(0.5f, 0.5f);
                iconRect.pivot = new Vector2(0.5f, 0.5f);
                iconRect.sizeDelta = new Vector2(20f, 20f); // 아이콘 크기
            }

            MinimapIcon minimapIcon = icon.GetComponent<MinimapIcon>();
            if (minimapIcon == null)
            {
                minimapIcon = icon.AddComponent<MinimapIcon>();
            }
            minimapIcon.Initialize(iconColor, target, minimapCamera, minimapRectTransform);
            
            minimapIcons[target] = icon;
        }

        public void RemoveMinimapIcon(Transform target)
        {
            if (minimapIcons.ContainsKey(target))
            {
                Destroy(minimapIcons[target]);
                minimapIcons.Remove(target);
            }
        }

        private void OnDestroy()
        {
            foreach (var icon in minimapIcons.Values)
            {
                if (icon != null) Destroy(icon);
            }
            minimapIcons.Clear();

            if (minimapTexture != null)
            {
                minimapTexture.Release();
                Destroy(minimapTexture);
            }
        }

        // 공용 접근자
        public GameObject PlayerIconPrefab => playerIconPrefab;
        public GameObject GridObjectIconPrefab => gridObjectIconPrefab;
        public GameObject NPCIconPrefab => npcIconPrefab;
        public Color PlayerColor => playerColor;
        public Color GridObjectColor => gridObjectColor;
        public Color NPCColor => npcColor;
    }
}