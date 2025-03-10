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
        [SerializeField] private TextMeshProUGUI _curLvTxt;
        [SerializeField] private TextMeshProUGUI _newLvTxt;
        [SerializeField] private TextMeshProUGUI _leftLvTxt;
        private int _skillIdx;
        private int _curLv;
        private int _newLv;
        private int _maxLv;
        private int _leftLv = 40;
        private int _usedLv = 0;

        public void UpdateAssignPage(int idx)
        {
            _skillIdx = idx;
            _typeTxt.text = _skills[_skillIdx].Type;
            _nameTxt.text = _skills[_skillIdx].Name;
            _descTxt.text = _skills[_skillIdx].Desc;
            _curLvTxt.text = _skills[_skillIdx].CurLv.ToString();

            _curLv = _skills[_skillIdx].CurLv;
            _newLv = _skills[_skillIdx].CurLv;
            _maxLv = _skills[_skillIdx].MaxLv;
            UpdateNewLvTxt();

            _usedLv = 0;
        }

        #region Lv 할당 확인 및 텍스트 업데이트
        public bool HasChanged()
        {
            return _newLv != _curLv;
        }

        private void UpdateNewLvTxt()
        {
            _newLvTxt.text = _newLv.ToString();
        }
        #endregion

        #region Lv 할당(조절) 버튼
        public void MinBtn()
        {
            _newLv = _curLv;
            _usedLv = 0;
            UpdateNewLvTxt();
        }

        public void MaxBtn()
        {
            int availableRp = _leftLv - _usedLv; // 사용 가능한 남은 Lv
            int newLv = Mathf.Clamp(_newLv + availableRp, _curLv, _maxLv);

            _usedLv += newLv - _newLv; // 실제 사용한 값만큼 업데이트
            _newLv = newLv;

            UpdateNewLvTxt();
        }

        public void UpBtn(int val)
        {
            int availableRp = _leftLv - _usedLv;
            if (val > availableRp)
                val = availableRp;

            int newLv = Mathf.Clamp(_newLv + val, _curLv, _maxLv);
            _usedLv += newLv - _newLv;
            _newLv = newLv;
            UpdateNewLvTxt();
        }

        public void DownBtn(int val)
        {
            val = Mathf.Clamp(val, 0, _usedLv); // 반환할 수 있는 범위를 초과하지 않도록 보정

            int newLv = Mathf.Clamp(_newLv - val, _curLv, _maxLv); // 최소 _curLv 유지
            _usedLv -= _newLv - newLv; // 감소한 값만큼 _usedLv 복구
            _newLv = newLv;

            UpdateNewLvTxt();
        }
        #endregion

        #region Lv 저장
        public void SaveLv()
        {
            _skills[_skillIdx].CurLv = _newLv;
            _skills[_skillIdx].SetLvTxt();

            _leftLv -= _usedLv;
            UpdateLeftLv();
        }
        #endregion

        #region 잔여 Lv 업데이트
        private void UpdateLeftLv()
        {
            _leftLvTxt.text = $"{_leftLv} / 70";
        }
        #endregion
    }
}