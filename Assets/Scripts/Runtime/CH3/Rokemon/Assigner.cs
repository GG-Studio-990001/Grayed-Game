using TMPro;
using UnityEngine;

namespace Runtime.CH3.Rokemon
{
    public class Assigner : MonoBehaviour
    {
        [Header("==스크립트==")]
        [SerializeField] private RMDialogue _rMDialogue;
        [Header("==스킬==")]
        [SerializeField] private TextMeshProUGUI _typeTxt;
        [SerializeField] private TextMeshProUGUI _nameTxt;
        [SerializeField] private TextMeshProUGUI _descTxt;
        [Header("==할당==")]
        [SerializeField] private TextMeshProUGUI _curLvTxt; // 기존 Lv
        [SerializeField] private TextMeshProUGUI _tmpLvTxt; // 변경중인 임시 Lv
        [Header("==Lv==")]
        [SerializeField] private TextMeshProUGUI _leftLvTxt;
        [SerializeField] private TextMeshProUGUI _leftMaxLvTxt;
        [SerializeField] private int _leftLv = 10;
        [SerializeField] int _totalUsedLv = 0; // 지금까지 사용한 Lv 합 (습득 조건 위함)
        private Skill _curSkill;
        private int _curLv;
        private int _tmpLv;
        private int _maxLv;
        private int _usedLv = 0;
        private readonly int _totalMaxLv = 120; // 라플리가 가질 수 있는 Lv 한도

        private void Start()
        {
            UpdateLeftLvTxt();
        }

        private void UpdateLeftLvTxt()
        {
            int leftMaxLv = _totalMaxLv - 30 - _totalUsedLv;
            _leftLv = Mathf.Clamp(_leftLv, 0, leftMaxLv);

            _leftLvTxt.text = _leftLv.ToString();
            _leftMaxLvTxt.text = "/ " + leftMaxLv.ToString(); // 30 = 초기 할당 값
        }

        public void UpdateAssignPage(Skill skill)
        {
            // _skillIdx = idx;
            _curSkill = skill;
            _typeTxt.text = _curSkill.Type;
            _nameTxt.text = _curSkill.Name;
            _descTxt.text = _curSkill.Desc;
            _curLvTxt.text = _curSkill.CurLv.ToString();

            _curLv = _curSkill.CurLv;
            _tmpLv = _curSkill.CurLv;
            _maxLv = _curSkill.MaxLv;
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
            _leftLvTxt.text = (_leftLv - _tmpLv + _curLv).ToString();
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
            int lastLv = _curSkill.CurLv;
            _curSkill.CurLv = _tmpLv;
            _curSkill.SetLvTxt();

            UpdateTotalUsedLv(_usedLv);

            // 원래 마력 30 찍는 게 조건인데 매력 스킬과 동시에 습득 가능성이 있음, 임시 변형
            //if (_curSkill.idx == 3 && lastLv < 70 && _curSkill.CurLv >= 70) // 마력 스킬 습득
            //{
            //    _rMDialogue.NewSkillDialogue(5);
            //}
        }
        #endregion

        private void UpdateTotalUsedLv(int used)
        {
            int lastUsedLv = _totalUsedLv;
            _totalUsedLv += used;
            Debug.Log($"lastUsedLv = {lastUsedLv}");
            Debug.Log($"_totalUsedLv = {_totalUsedLv}");

            if (lastUsedLv < 30 && _totalUsedLv >= 30) // 매력 스킬 습득
            {
                _rMDialogue.NewSkillDialogue(4);
            }
            else if (lastUsedLv < 45 && _totalUsedLv >= 45) // 마력 스킬 습득
            {
                _rMDialogue.NewSkillDialogue(5);
            }
            else if (lastUsedLv < 60 && _totalUsedLv >= 60) // 행운 스킬 습득
            {
                _rMDialogue.NewSkillDialogue(6);
            }

            UpdateLeftLvTxt();
        }

        public void RevertLeftLv()
        {
            Debug.Log("복구햇");
            _leftLvTxt.text = _leftLv.ToString();
        }
    }
}