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
        [SerializeField] private Sprite[] _sprs;
        private Image _img;

        private void Start()
        {
            _img = GetComponent<Image>();
            _typeTxt.text = Type;
            _nameTxt.text = SkillName;
        }

        public void SkillSelected(bool isSelected)
        {
            _img.sprite = isSelected ? _sprs[1] : _sprs[0];
        }

        public void SetLvTxt()
        {
            _lvTxt.text = $"{CurLv} / {MaxLv}";
        }
    }
}