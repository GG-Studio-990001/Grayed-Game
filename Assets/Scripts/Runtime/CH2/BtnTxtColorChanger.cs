using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class BtnTxtColorChanger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    private TextMeshProUGUI btnTxt;

    void Start()
    {
        btnTxt = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        btnTxt.color = Color.black;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        btnTxt.color = Color.white;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        btnTxt.color = Color.black;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        btnTxt.color = Color.white;
    }
}
