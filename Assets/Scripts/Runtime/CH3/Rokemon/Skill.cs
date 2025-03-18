using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime.CH3.Rokemon
{
    public class Skill : MonoBehaviour
    {
        public int Idx;
        public string Type;
        public string SkillName;
        [TextArea(3, 5)]
        public string Desc;
        public int CurLv;
        public int MaxLv;
        [SerializeField] private TextMeshProUGUI _typeTxt;
        [SerializeField] private TextMeshProUGUI _nameTxt;
        [SerializeField] private TextMeshProUGUI _lvTxt;
        private Image _img;

        private void Start()
        {
            _img = GetComponent<Image>();
            _typeTxt.text = Type;
            _nameTxt.text = SkillName;
        }

        public void SkillSelected(bool isSelected)
        {
            _img.color = isSelected ? new Color(1f, 0.8f, 0.8f, 1f) : new Color(1f, 1f, 1f, 1f); // 빨 : 흰
        }

        public void SetLvTxt()
        {
            _lvTxt.text = $"{CurLv} / {MaxLv}";
        }
    }
}