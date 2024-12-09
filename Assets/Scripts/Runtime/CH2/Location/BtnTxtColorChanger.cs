using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Runtime.CH2.Location
{
    public class BtnTxtColorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        private TextMeshProUGUI btnTxt;
        private Button btn; // 버튼 참조

        void Start()
        {
            btnTxt = GetComponentInChildren<TextMeshProUGUI>();
            btn = GetComponent<Button>(); // Button 컴포넌트 가져오기
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsButtonInteractable()) return;
            btnTxt.color = Color.black;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsButtonInteractable()) return;
            btnTxt.color = Color.white;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsButtonInteractable()) return;
            btnTxt.color = Color.black;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!IsButtonInteractable()) return;
            btnTxt.color = Color.white;
        }

        private bool IsButtonInteractable()
        {
            // 버튼이 비활성화 상태라면 false 반환
            return btn != null && btn.interactable;
        }
    }
}