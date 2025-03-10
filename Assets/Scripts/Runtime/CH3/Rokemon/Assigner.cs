using TMPro;
using UnityEngine;

namespace Runtime.CH3.Rokemon
{
    public class Assigner : MonoBehaviour
    {
        [Header("==스킬==")]
        [SerializeField] private Skill[] _skills;
        [SerializeField] private TextMeshProUGUI _typeTxt;
        [SerializeField] private TextMeshProUGUI _nameTxt;
        [SerializeField] private TextMeshProUGUI _descTxt;
        [Header("==할당==")]
        [SerializeField] private TextMeshProUGUI _curLvTxt; // 기존 Lv
        [SerializeField] private TextMeshProUGUI _tmpLvTxt; // 변경중인 임시 Lv
        [Header("==잔여==")]
        [SerializeField] private TextMeshProUGUI _leftLvTxt; // 잔여 Lv
        private int _skillIdx;
        private int _curLv;
        private int _tmpLv;
        private int _maxLv;
        private int _leftLv = 40;
        private int _usedLv = 0;
        private int _totalUsedLv = 30; // 지금까지 사용한 Lv 합 (습득 조건 위함)

        public void UpdateAssignPage(int idx)
        {
            _skillIdx = idx;
            _typeTxt.text = _skills[_skillIdx].Type;
            _nameTxt.text = _skills[_skillIdx].Name;
            _descTxt.text = _skills[_skillIdx].Desc;
            _curLvTxt.text = _skills[_skillIdx].CurLv.ToString();

            _curLv = _skills[_skillIdx].CurLv;
            _tmpLv = _skills[_skillIdx].CurLv;
            _maxLv = _skills[_skillIdx].MaxLv;
            UpdateTmpLvTxt();

            _usedLv = 0;
        }

        #region Lv 할당 확인 및 텍스트 업데이트
        public bool HasChanged()
        {
            return _tmpLv != _curLv;
        }

        private void UpdateTmpLvTxt() // 임시로 할당한 Lv 값 (아직 저장 X)
        {
            _tmpLvTxt.text = _tmpLv.ToString();
        }
        #endregion

        #region Lv 할당(조절) 버튼
        public void MinBtn()
        {
            _tmpLv = _curLv;
            _usedLv = 0;
            UpdateTmpLvTxt();
        }

        public void MaxBtn()
        {
            int availableRp = _leftLv - _usedLv; // 사용 가능한 남은 Lv
            int newLv = Mathf.Clamp(_tmpLv + availableRp, _curLv, _maxLv);

            _usedLv += newLv - _tmpLv; // 실제 사용한 값만큼 업데이트
            _tmpLv = newLv;

            UpdateTmpLvTxt();
        }

        public void UpBtn(int val)
        {
            int availableRp = _leftLv - _usedLv;
            if (val > availableRp)
                val = availableRp;

            int newLv = Mathf.Clamp(_tmpLv + val, _curLv, _maxLv);
            _usedLv += newLv - _tmpLv;
            _tmpLv = newLv;
            UpdateTmpLvTxt();
        }

        public void DownBtn(int val)
        {
            val = Mathf.Clamp(val, 0, _usedLv); // 반환할 수 있는 범위를 초과하지 않도록 보정

            int newLv = Mathf.Clamp(_tmpLv - val, _curLv, _maxLv); // 최소 _curLv 유지
            _usedLv -= _tmpLv - newLv; // 감소한 값만큼 _usedLv 복구
            _tmpLv = newLv;

            UpdateTmpLvTxt();
        }
        #endregion

        #region Lv 저장
        public void SaveLv()
        {
            _skills[_skillIdx].CurLv = _tmpLv;
            _skills[_skillIdx].SetLvTxt();

            UpdateTotalUsedLv(_usedLv);
            UpdateLeftLv(_usedLv);
        }
        #endregion

        private void UpdateTotalUsedLv(int used)
        {
            int lastUsedLv = _totalUsedLv;
            _totalUsedLv += used;

            if (lastUsedLv < 30 && _totalUsedLv >= 30)
            {
                // 매력 스킬 습득 조건 달성
            }
        }

        private void UpdateLeftLv(int used)
        {
            _leftLv -= used;
            _leftLvTxt.text = $"{_leftLv} / 70";
        }

        private void RevertLeftLv()
        {
            // TODO: 잔여를 실시간으로 변경, 저장하지 않으면 복구 가능하도록 짜야함
        }
    }
}