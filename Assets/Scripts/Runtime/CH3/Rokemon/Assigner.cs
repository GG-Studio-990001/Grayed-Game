using TMPro;
using UnityEngine;

namespace Runtime.CH3.Rokemon
{
    public class Assigner : MonoBehaviour
    {
        [SerializeField] private Skill[] _skills;
        [SerializeField] private TextMeshProUGUI _typeTxt;
        [SerializeField] private TextMeshProUGUI _nameTxt;
        [SerializeField] private TextMeshProUGUI _descTxt;
        [SerializeField] private TextMeshProUGUI _curRpTxt;
        [SerializeField] private TextMeshProUGUI _newRpTxt;
        [SerializeField] private TextMeshProUGUI _leftRpTxt;
        private int _skillIdx;
        private int _curRp;
        private int _newRp;
        private int _maxRp;
        private int _leftRp = 40;
        private int _usedRp = 0;

        public void UpdateAssignPage(int idx)
        {
            _skillIdx = idx;
            _typeTxt.text = _skills[_skillIdx].Type;
            _nameTxt.text = _skills[_skillIdx].Name;
            _descTxt.text = _skills[_skillIdx].Desc;
            _curRpTxt.text = _skills[_skillIdx].CurLv.ToString();

            _curRp = _skills[_skillIdx].CurLv;
            _newRp = _skills[_skillIdx].CurLv;
            _maxRp = _skills[_skillIdx].MaxLv;
            UpdateNewRpTxt();

            _usedRp = 0;
        }

        public bool HasChanged()
        {
            return _newRp != _curRp;
        }

        private void UpdateNewRpTxt()
        {
            _newRpTxt.text = _newRp.ToString();
        }

        public void MinBtn()
        {
            _newRp = _curRp;
            _usedRp = 0;
            UpdateNewRpTxt();
        }

        public void MaxBtn()
        {
            int availableRp = _leftRp - _usedRp; // 사용 가능한 남은 RP
            int newRp = Mathf.Clamp(_newRp + availableRp, _curRp, _maxRp);

            _usedRp += newRp - _newRp; // 실제 사용한 값만큼 업데이트
            _newRp = newRp;

            UpdateNewRpTxt();
        }

        public void UpBtn(int val)
        {
            int availableRp = _leftRp - _usedRp;
            if (val > availableRp)
                val = availableRp;

            int newRp = Mathf.Clamp(_newRp + val, _curRp, _maxRp);
            _usedRp += newRp - _newRp;
            _newRp = newRp;
            UpdateNewRpTxt();
        }

        public void DownBtn(int val)
        {
            val = Mathf.Clamp(val, 0, _usedRp); // 반환할 수 있는 범위를 초과하지 않도록 보정

            int newRp = Mathf.Clamp(_newRp - val, _curRp, _maxRp); // 최소 _curRp 유지
            _usedRp -= _newRp - newRp; // 감소한 값만큼 _usedRp 복구
            _newRp = newRp;

            UpdateNewRpTxt();
        }

        public void SaveRp()
        {
            _skills[_skillIdx].CurLv = _newRp;
            _skills[_skillIdx].SetRpTxt();

            _leftRp -= _usedRp;
            UpdateLeftRp();
        }

        private void UpdateLeftRp()
        {
            _leftRpTxt.text = $"{_leftRp} / 70";
        }
    }
}