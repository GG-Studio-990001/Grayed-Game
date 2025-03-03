using TMPro;
using UnityEngine;

namespace Runtime.CH3.Rokemon
{
    public class Skill : MonoBehaviour
    {
        public string Type;
        public string Name;
        [TextArea(3, 5)]
        public string Desc;
        public int CurRp;
        public int MaxRp;
        [SerializeField] private TextMeshProUGUI _rpTxt;

        public void SetRpTxt()
        {
            _rpTxt.text = $"{CurRp} / {MaxRp}";
        }
    }
}