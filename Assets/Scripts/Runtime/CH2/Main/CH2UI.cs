using TMPro;
using UnityEngine;

public class CH2UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _locationTxt;
    
    public void SetLocationTxt(string text)
    {
        _locationTxt.text = text;
    }
}
