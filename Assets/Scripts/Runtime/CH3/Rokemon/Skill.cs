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
        public int CurLv;
        public int MaxLv;
        [SerializeField] private TextMeshProUGUI _typeTxt;
        [SerializeField] private TextMeshProUGUI _nameTxt;
        [SerializeField] private TextMeshProUGUI _lvTxt;

        private void Start()
        {
            _typeTxt.text = Type;
            _nameTxt.text = Name;
        }

        public void SetRpTxt()
        {
            _lvTxt.text = $"{CurLv} / {MaxLv}";
        }
    }
}