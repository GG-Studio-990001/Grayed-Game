using UnityEngine;
using UnityEngine.UI;

public class BtnClickController : MonoBehaviour
{
    private Button _btn;

    private void Awake()
    {
        _btn = GetComponent<Button>();
    }

    public void BtnClicked()
    {
        _btn.enabled = false;
        _btn.enabled = true;
    }
}
